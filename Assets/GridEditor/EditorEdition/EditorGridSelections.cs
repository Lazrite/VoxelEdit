#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 選択モード用列挙隊
/// </summary>
public enum OperationMode
{
    OperationClick = 0,
    OperationDragFree,
    OperationRange,
}

/// <summary>
/// ツールモード用列挙体
/// </summary>
public enum ToolMode
{
    ToolPlace = 0,
    ToolErase,
    ToolReplace,
}

[ExecuteAlways]
public class EditorGridSelections
{
    private EditorGridField gridManager;
    private EditorGenerateAblePlacement placementArea = default;
    public OperationMode selectMode;
    public ToolMode toolMode;

    // instantiateされたオブジェクトの参照バッファ
    private Object instantiateBuffer = default;

    // グリッドサイズ
    private Vector3Int gridSize;

    // ドラッグしているかどうかのフラグ
    private bool isDrag = false;

    // ビジュアライザオブジェクト
    private Object visualizeObj;

    // 範囲ドラッグの始点
    private Vector3 startRangeDrag;

    // 範囲ドラッグの終点
    private Vector3 endRangeDrag;

    // ダミーブロックのバッファ
    private GameObject dummyBlock;

    // ドラッグ時生成オブジェクト格納リスト
    private List<GameObject> placedList;

    // ドラッグ開始時のクリックで触ったオブジェクトの法線を取得
    private Vector3 clickedHitNormal;

    // ドラッグ時多重生成防止オブジェクト座標格納リスト
    private List<Vector3> deniedLMultiPlacedList;

    // ドラッグ時消去防止オブジェクト座標格納リスト
    private List<Vector3> deniedDestroyObjectList;

    // 選択座標バッファ
    private Vector3 focusPosBuffer;

    // 消去予約リスト
    private List<GameObject> deleteReservationList;

    /// <summary>
    /// コンストラクタ アセットの初期化だけしてる
    /// </summary>
    public EditorGridSelections()
    {
        dummyBlock =
            (GameObject)AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorSource/BlockDummy.prefab",
                typeof(GameObject));
    }

    /// <summary>
    /// エディタ上ではStartが呼ばれない(maybe)のでこのメソッドをデリゲートと一緒に呼んでね?
    /// </summary>
    public void StartSelectProcess()
    {
        if (((GameObject)GridEditorWindow.gridObject) != null)
        {
            gridManager = ((GameObject)GridEditorWindow.gridObject).GetComponent<EditorGridField>();
            placementArea = ((GameObject)GridEditorWindow.gridObject).GetComponent<EditorGenerateAblePlacement>();
        }
    }

    /// <summary>
    /// シーンビューでセレクタを動かすためのメソッド(エディタ上のメインループ)。デリゲートに登録することで利用する。
    /// </summary>
    /// <param name="sceneView"> 現在のシーン(デリゲートするので使わない) </param>
    public void OnScene(SceneView sceneView)
    {
        // グリッドオブジェクトがないならreturn
        if (((GameObject)GridEditorWindow.gridObject) == null)
        {
            return;
        }

        if (((GameObject)GridEditorWindow.gridObject) != null)
        {
            gridManager = ((GameObject)GridEditorWindow.gridObject).GetComponent<EditorGridField>();
            placementArea = ((GameObject)GridEditorWindow.gridObject).GetComponent<EditorGenerateAblePlacement>();
        }

        // Get mouse events for object placement when in the Scene View
        Event currentEvent = Event.current;

        if (GridEditorWindow.gridObject == null)
        {
            return;
        }

        RaycastHit hit = new RaycastHit();
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        var hits = Physics.RaycastAll(ray);
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        foreach (var hitObj in hits)
        {
            if (hitObj.collider.name == "AblePlacementAround" || hitObj.collider.name == "BlockDummy")
            {
                hit = hitObj;
                break;
            }
        }

        // 例が何にもあたってなかったらreturn
        if (hit.collider == null)
        {
            Visualizer.DisableRenderer();
            return;
        }

        // フォーカスがグリッドオブジェクト外、或いは起きたいオブジェクトがnullなら終了
        if (GridEditorWindow.gridObject != hit.transform.root.gameObject && GridEditorWindow.obj == null)
        {
            return;
        }

        // 設置モードで起きたいオブジェクトがnullならreturn
        if ((toolMode == ToolMode.ToolPlace || toolMode == ToolMode.ToolReplace) && GridEditorWindow.obj == null)
        {
            return;
        }

        // 範囲外なら設置しない
        if (hit.collider.name != "AblePlacementAround" && toolMode == ToolMode.ToolPlace)
        {
            if (hit.collider.gameObject.transform.position.x < gridManager.gridScale / 2 || hit.collider.gameObject.transform.position.x > (gridManager.gridScale * gridManager.size.x) - gridManager.gridScale / 2 ||
                hit.collider.gameObject.transform.position.y < gridManager.gridScale / 2 || hit.collider.gameObject.transform.position.y > (gridManager.gridScale * gridManager.size.y) - gridManager.gridScale / 2 ||
                hit.collider.gameObject.transform.position.z < gridManager.gridScale / 2 || hit.collider.gameObject.transform.position.z > (gridManager.gridScale * gridManager.size.z) - gridManager.gridScale / 2)
            {
                return;
            }
        }

        // 内積を利用して手前側のオブジェクトのフォーカスを回避
        if (Vector3.Dot(hit.normal, ray.direction) < 0)
        {
            // サーフェスビジュアライザが生成されてないなら生成
            if (Visualizer.selectedSurfaceVisualizer == null)
            {
                Visualizer.CreateVisualizer(VisualizerType.Surface);
            }

            if (Visualizer.prefabVisualizer == null && GridEditorWindow.obj != null)
            {
                Visualizer.CreateVisualizer(VisualizerType.Prefab);
            }

            // 設置モード時はオブジェクトのビジュアライザを生成
            if (toolMode == ToolMode.ToolPlace)
            {
                if (Visualizer.prefabVisualizer != null)
                {
                    if (hit.collider.name == "AblePlacementAround")
                    {
                        Visualizer.MoveVisualizerPrefab(
                            hit.transform.position + (hit.normal * gridManager.gridScale / 2), Vector3.zero);
                    }
                    else if (hit.collider.name == "BlockDummy")
                    {
                        Visualizer.MoveVisualizerPrefab(hit.transform.position + (hit.normal * gridManager.gridScale),
                            Vector3.zero);
                    }
                }
            }

            // 消去モードの時は面に接した部分にサーフェスビジュアライザを生成
            if (toolMode == ToolMode.ToolErase)
            {
                if (hit.collider.name == "AblePlacementAround")
                {
                    Visualizer.MoveVisualizerSurface(hit.transform.position, -hit.normal);
                }
                else if (hit.collider.name == "BlockDummy")
                {
                    Visualizer.MoveVisualizerSurface((hit.transform.position + hit.normal * gridManager.gridScale / 2),
                        -hit.normal);
                }
            }
        }


        switch (selectMode)
        {
            #region OPERATION_CLICK

            case OperationMode.OperationClick: // クリックモード
                //左クリックされたら:設置モード
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 &&
                    toolMode == ToolMode.ToolPlace)
                {
                    if (hit.collider != null && GridEditorWindow.obj != null)
                    {

                        Vector3 addPos;

                        // 設置可能オブジェクトに触れてない場合はreturn
                        if (hit.collider.name != "AblePlacementAround" && hit.collider.name != "BlockDummy")
                        {
                            return;
                        }
                        else
                        {
                            if (hit.collider.name == "AblePlacementAround")
                            {
                                addPos = hit.collider.transform.position + (hit.normal * gridManager.gridScale) / 2;
                            }
                            else
                            {
                                addPos = hit.collider.transform.position + (hit.normal * gridManager.gridScale);
                            }
                        }

                        // アクティブな操作ツールを変更
                        ToolManager.SetActiveTool<CustomGridTools>();

                        instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                        ((GameObject)instantiateBuffer).transform.parent =
                            ((GameObject)GridEditorWindow.gridObject).transform;

                        ((GameObject)instantiateBuffer).transform.position = Vector3.zero;
                        ((GameObject)instantiateBuffer).transform.position += addPos;

                        Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");

                        Undo.RecordObject(gridManager, "isPlaced Check");
                        gridManager.inGridObjects.Add(((GameObject)instantiateBuffer));

                        var dummy = PrefabUtility.InstantiatePrefab(dummyBlock);
                        ((GameObject)dummy).transform.parent = ((GameObject)GridEditorWindow.gridObject).transform;
                        ((GameObject)dummy).transform.position += addPos;
                        ((GameObject)dummy).hideFlags = HideFlags.HideInHierarchy;
                        ((GameObject)dummy).GetComponent<MeshRenderer>().enabled = false;
                        ((GameObject)dummy).transform.parent = ((GameObject)instantiateBuffer).transform;
                        Undo.RegisterCreatedObjectUndo(dummy, "placed prefab");
                    }

                    return;
                }

                // 左クリックされたら:消去モード
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 &&
                    toolMode == ToolMode.ToolErase)
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.name == "AblePlacementAround")
                        {
                            return;
                        }

                        // アクティブな操作ツールを変更
                        ToolManager.SetActiveTool<CustomGridTools>();

                        Undo.RecordObject(gridManager, "isRemoved Check");
                        gridManager.inGridObjects.Remove(hit.collider.transform.gameObject);

                        foreach (Transform child in hit.collider.transform)
                        {
                            if (child.name == "BlockDummy")
                            {
                                Undo.DestroyObjectImmediate(hit.collider.transform.gameObject);
                            }
                        }

                        if (hit.collider.name == "BlockDummy")
                        {
                            Undo.DestroyObjectImmediate(hit.collider.transform.parent.gameObject);
                        }
                    }
                }

                // 置換モード
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 &&
                    toolMode == ToolMode.ToolReplace)
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.name == "AblePlacementAround")
                        {
                            return;
                        }

                        // アクティブな操作ツールを変更
                        ToolManager.SetActiveTool<CustomGridTools>();

                        Vector3 currentPos = hit.collider.transform.position;

                        Undo.RecordObject(gridManager, "isRemoved Check");
                        gridManager.inGridObjects.Remove(hit.collider.transform.gameObject);

                        // 置換前オブジェクトの消去
                        foreach (Transform child in hit.collider.transform)
                        {
                            if (child.name == "BlockDummy")
                            {
                                Undo.DestroyObjectImmediate(hit.collider.transform.gameObject);
                            }
                        }

                        if (hit.collider.name == "BlockDummy")
                        {
                            Undo.DestroyObjectImmediate(hit.collider.transform.parent.gameObject);
                        }

                        // 置換後オブジェクトの生成
                        instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                        ((GameObject)instantiateBuffer).transform.parent =
                            ((GameObject)GridEditorWindow.gridObject).transform;

                        ((GameObject)instantiateBuffer).transform.position = currentPos;

                        Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");

                        Undo.RecordObject(gridManager, "isPlaced Check");
                        gridManager.inGridObjects.Add(((GameObject)instantiateBuffer));

                        var dummy = PrefabUtility.InstantiatePrefab(dummyBlock);
                        ((GameObject)dummy).transform.parent = ((GameObject)GridEditorWindow.gridObject).transform;
                        ((GameObject)dummy).transform.position = currentPos;
                        ((GameObject)dummy).hideFlags = HideFlags.HideInHierarchy;
                        ((GameObject)dummy).GetComponent<MeshRenderer>().enabled = false;
                        ((GameObject)dummy).transform.parent = ((GameObject)instantiateBuffer).transform;
                        Undo.RegisterCreatedObjectUndo(dummy, "placed prefab");
                    }
                }

                break;

            #endregion

            #region OPERATION_DRAG

            case OperationMode.OperationDragFree: //ドラッグモード
                // マウスボタンが押されたら
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                {
                    isDrag = true;
                    deniedDestroyObjectList ??= new List<Vector3>();
                    deniedLMultiPlacedList ??= new List<Vector3>();
                    placedList ??= new List<GameObject>();
                    clickedHitNormal = hit.normal;

                    // マウスクリックのホットコントロールを固定
                    GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                    Event.current.Use();
                }

                // マウスがドラッグされている場合は以下の処理を実行(マウスクリックダウンのイベントは当該フレームでしかアクティブにならないのでフラグで遷移)
                // 設置モード
                if (isDrag && toolMode == ToolMode.ToolPlace)
                {
                    // マウスが離された場合
                    if (currentEvent.type == EventType.MouseUp ||
                        currentEvent.type == EventType.MouseLeaveWindow && currentEvent.button == 0)
                    {
                        isDrag = false;

                        foreach (var tmp in placedList)
                        {
                            var dummy = PrefabUtility.InstantiatePrefab(dummyBlock);
                            ((GameObject)dummy).transform.parent = ((GameObject)GridEditorWindow.gridObject).transform;
                            ((GameObject)dummy).transform.position = tmp.transform.position;
                            ((GameObject)dummy).hideFlags = HideFlags.HideInHierarchy;
                            ((GameObject)dummy).GetComponent<MeshRenderer>().enabled = false;
                            ((GameObject)dummy).transform.parent = tmp.transform;
                            Undo.RegisterCreatedObjectUndo(dummy, "placed prefab");
                        }

                        placedList.Clear();
                        deniedLMultiPlacedList.Clear();
                        return;
                    }

                    if (hit.collider != null)
                    {
                        Vector3 addPos;

                        // 設置可能オブジェクトに触れてない場合はreturn
                        if (hit.collider.name != "AblePlacementAround" && hit.collider.name != "BlockDummy")
                        {
                            return;
                        }
                        else
                        {
                            if (hit.collider.name == "AblePlacementAround")
                            {
                                addPos = hit.collider.transform.position + (hit.normal * gridManager.gridScale / 2);
                            }
                            else
                            {
                                addPos = hit.collider.transform.position + (hit.normal * gridManager.gridScale);
                            }
                        }

                        // 既に生成されていたらreturn
                        foreach (var elem in deniedLMultiPlacedList)
                        {
                            if (addPos == elem)
                            {
                                return;
                            }
                        }

                        // 前回と同じオブジェクトでなければ設置処理
                        if (hit.collider.gameObject != instantiateBuffer)
                        {
                            ToolManager.SetActiveTool<CustomGridTools>();

                            instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                            ((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
                            ((GameObject)instantiateBuffer).transform.position = Vector3.zero;
                            ((GameObject)instantiateBuffer).transform.position += addPos;
                            deniedLMultiPlacedList.Add(((GameObject)instantiateBuffer).transform.position);
                            placedList.Add((GameObject)instantiateBuffer);
                            Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");

                            Undo.RecordObject(gridManager, "isPlaced Check");
                            gridManager.inGridObjects.Add(((GameObject)instantiateBuffer));
                        }
                    }
                }

                // 消去モード
                if (isDrag && toolMode == ToolMode.ToolErase)
                {
                    // マウスが離された場合
                    if (currentEvent.type == EventType.MouseUp ||
                        currentEvent.type == EventType.MouseLeaveWindow && currentEvent.button == 0)
                    {
                        isDrag = false;
                        deniedDestroyObjectList.Clear();
                    }


                    if (hit.collider != null)
                    {
                        if (hit.collider.name == "AblePlacementAround")
                        {
                            return;
                        }

                        // 法線の向きが違う場合はreturn
                        if (clickedHitNormal != hit.normal)
                        {
                            return;
                        }

                        // 消去防止されている座標のオブジェクトを消そうとしている場合はreturn
                        foreach (var elem in deniedDestroyObjectList)
                        {
                            if (hit.collider.transform.position + (hit.normal * gridManager.gridScale) == elem)
                            {
                                return;
                            }
                        }

                        foreach (Transform child in hit.collider.transform)
                        {
                            if (child.name == "BlockDummy")
                            {
                                Undo.RecordObject(gridManager, "isRemoved Check");
                                gridManager.inGridObjects.Remove(hit.collider.transform.gameObject);

                                deniedDestroyObjectList.Add(hit.collider.transform.position +
                                                            (hit.normal * gridManager.gridScale));
                                Undo.DestroyObjectImmediate(hit.collider.transform.gameObject);
                            }
                        }

                        if (hit.collider.name == "BlockDummy")
                        {
                            Undo.RecordObject(gridManager, "isRemoved Check");
                            gridManager.inGridObjects.Remove(hit.collider.transform.gameObject);
                            Undo.DestroyObjectImmediate(hit.collider.transform.parent.gameObject);
                        }
                    }
                }

                // 置換モード
                if (isDrag && toolMode == ToolMode.ToolReplace)
                {
                    // マウスが離されたら
                    if (currentEvent.type == EventType.MouseUp ||
                        currentEvent.type == EventType.MouseLeaveWindow && currentEvent.button == 0)
                    {
                        isDrag = false;
                    }

                    if (hit.collider != null)
                    {
                        if (hit.collider.name == "AblePlacementAround")
                        {
                            return;
                        }

                        if (hit.collider == GridEditorWindow.obj)
                        {
                            return;
                        }

                        // 前回と同じオブジェクトでなければ設置処理
                        if (hit.collider.gameObject != instantiateBuffer)
                        {
                            // 置換前オブジェクトの消去
                            foreach (Transform child in hit.collider.transform)
                            {
                                if (child.name == "BlockDummy")
                                {
                                    Undo.RecordObject(gridManager, "isRemoved Check");
                                    gridManager.inGridObjects.Remove(hit.collider.transform.gameObject);
                                    Undo.DestroyObjectImmediate(hit.collider.transform.gameObject);
                                }
                            }

                            if (hit.collider.name == "BlockDummy")
                            {
                                Undo.RecordObject(gridManager, "isRemoved Check");
                                gridManager.inGridObjects.Remove(hit.collider.transform.gameObject);
                                Undo.DestroyObjectImmediate(hit.collider.transform.parent.gameObject);
                            }

                            Vector3 currentPos = hit.collider.transform.position;

                            if (hit.collider.gameObject != instantiateBuffer)
                            {
                                // 置換後オブジェクトの生成
                                instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                                ((GameObject)instantiateBuffer).transform.parent =
                                    ((GameObject)GridEditorWindow.gridObject).transform;

                                ((GameObject)instantiateBuffer).transform.position = currentPos;

                                Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");

                                Undo.RecordObject(gridManager, "isPlaced Check");
                                gridManager.inGridObjects.Add(((GameObject)instantiateBuffer));

                                var dummy = PrefabUtility.InstantiatePrefab(dummyBlock);
                                ((GameObject)dummy).transform.parent =
                                    ((GameObject)GridEditorWindow.gridObject).transform;
                                ((GameObject)dummy).transform.position = currentPos;
                                ((GameObject)dummy).hideFlags = HideFlags.HideInHierarchy;
                                ((GameObject)dummy).GetComponent<MeshRenderer>().enabled = false;
                                ((GameObject)dummy).transform.parent = ((GameObject)instantiateBuffer).transform;
                                Undo.RegisterCreatedObjectUndo(dummy, "placed prefab");
                            }
                        }
                    }
                }

                break;

            #endregion

            #region OPERATION_RANGE

            case OperationMode.OperationRange: // 範囲選択モード
                // マウスボタンが押されたら
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                {
                    // ドラッグ開始の始点インデックスを取得
                    if (hit.collider != null)
                    {
                        // 設置モードならブロック設置予定座標
                        if (toolMode == ToolMode.ToolPlace || toolMode == ToolMode.ToolReplace)
                        {
                            if (hit.collider.name == "AblePlacementAround")
                            {
                                startRangeDrag = hit.collider.transform.position +
                                                 (hit.normal * gridManager.gridScale) / 2;
                            }
                            else
                            {
                                startRangeDrag = hit.collider.transform.position + (hit.normal * gridManager.gridScale);
                            }
                        }
                        else if (toolMode == ToolMode.ToolErase) // 消去モードなら消すブロックの始点
                        {
                            startRangeDrag = hit.collider.transform.position;
                        }
                    }

                    deleteReservationList = new List<GameObject>();

                    isDrag = true;

                    // マウスクリックのホットコントロールを固定
                    GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                    Event.current.Use();
                }

                // マウスがドラッグされている場合は以下の処理を実行(マウスクリックダウンのイベントは当該フレームでしかアクティブにならないのでフラグで遷移)
                // 設置モード
                if (isDrag && toolMode == ToolMode.ToolPlace)
                {
                    // マウスが離された場合
                    if (currentEvent.type == EventType.MouseUp ||
                        currentEvent.type == EventType.MouseLeaveWindow && currentEvent.button == 0)
                    {
                        ClearVisualizer();

                        if (hit.collider.name == "AblePlacementAround")
                        {
                            endRangeDrag = hit.collider.transform.position + (hit.normal * gridManager.gridScale) / 2;
                        }
                        else
                        {
                            endRangeDrag = hit.collider.transform.position + (hit.normal * gridManager.gridScale);
                        }

                        CreateBoxPlacement(startRangeDrag,
                            hit.collider.gameObject.transform.position + (hit.normal * (gridManager.gridScale / 2)));
                        focusPosBuffer = hit.collider.gameObject.transform.position +
                                         (hit.normal * (gridManager.gridScale / 2));

                        isDrag = false;
                        return;
                    }

                    if (hit.collider != null)
                    {
                        if (GridEditorWindow.obj == null)
                        {
                            return;
                        }

                        if (hit.collider.transform.position + (hit.normal * gridManager.gridScale) != focusPosBuffer)
                        {
                            ClearVisualizer();
                            CreteBoxVisualizer(startRangeDrag,
                                hit.collider.gameObject.transform.position +
                                (hit.normal * (gridManager.gridScale / 2)));
                            focusPosBuffer = hit.collider.gameObject.transform.position +
                                             (hit.normal * (gridManager.gridScale / 2));
                        }
                    }
                }

                // 消去モード
                if (isDrag && toolMode == ToolMode.ToolErase)
                {
                    // マウスが離された場合
                    if (currentEvent.type == EventType.MouseUp ||
                        currentEvent.type == EventType.MouseLeaveWindow && currentEvent.button == 0)
                    {
                        ClearVisualizer();


                        ConfirmDelete();
                        isDrag = false;

                        return;
                    }

                    ResetReservation();

                    if (hit.collider != null)
                    {
                        if (hit.collider.transform.position + (hit.normal * gridManager.gridScale) != focusPosBuffer)
                        {
                            
                            Vector3 endRangeGuessPoint;

                            if (hit.collider.name == "AblePlacementAround")
                            {
                                endRangeGuessPoint = hit.collider.transform.position +
                                                     (hit.normal * gridManager.gridScale) / 2;
                            }
                            else
                            {
                                endRangeGuessPoint = hit.collider.transform.position;
                            }

                            ReservationDeleteObj(startRangeDrag, endRangeGuessPoint);
                            focusPosBuffer = hit.collider.gameObject.transform.position +
                                             (hit.normal * (gridManager.gridScale / 2));
                        }
                    }
                }

                // 置換モード
                if (isDrag && toolMode == ToolMode.ToolReplace)
                {
                    // マウスが離された場合
                    if (currentEvent.type == EventType.MouseUp ||
                        currentEvent.type == EventType.MouseLeaveWindow && currentEvent.button == 0)
                    {
                        ClearVisualizer();

                        ConfirmReplace();
                        isDrag = false;

                        return;
                    }

                    ResetReservation();

                    if (hit.collider != null)
                    {

                        if (hit.collider.transform.position + (hit.normal * gridManager.gridScale) != focusPosBuffer)
                        {
                            Vector3 endRangeGuessPoint;

                            if (hit.collider.name == "AblePlacementAround")
                            {
                                endRangeGuessPoint = hit.collider.transform.position +
                                                     (hit.normal * gridManager.gridScale) / 2;
                            }
                            else
                            {
                                endRangeGuessPoint = hit.collider.transform.position;
                            }

                            ReservationDeleteObj(startRangeDrag, endRangeGuessPoint);
                            focusPosBuffer = hit.collider.gameObject.transform.position +
                                             (hit.normal * (gridManager.gridScale / 2));
                        }
                    }
                }

                break;

            #endregion OPERATION_RANGE
        }
    }

    /// <summary>
    /// 任意のタイミングで全てのビジュアライザを消去するメソッド
    /// </summary>
    private void ClearVisualizer()
    {
        // Scene内からtypeを元に探してくる。
        // GameObjectを取得したいMonoBehaviorなどに変更すればそれが取れる。
        var gameObjects = (GameObject[])Object.FindObjectsOfType(typeof(GameObject));

        foreach (var obj in gameObjects)
        {
            if (obj.name == "visualizeObj")
            {
                GameObject.DestroyImmediate(obj);
            }
        }
    }

    private void CreteBoxVisualizer(Vector3 rangeStart, Vector3 rangeEnd)
    {
        int signx = 1;
        int signy = 1;
        int signz = 1;

        if (rangeStart.x > rangeEnd.x)
        {
            (rangeStart.x, rangeEnd.x) = (rangeEnd.x, rangeStart.x);
            signx = -1;
        }

        if (rangeStart.y > rangeEnd.y)
        {
            (rangeStart.y, rangeEnd.y) = (rangeEnd.y, rangeStart.y);
            signy = -1;
        }

        if (rangeStart.z > rangeEnd.z)
        {
            (rangeStart.z, rangeEnd.z) = (rangeEnd.z, rangeStart.z);
            signz = -1;
        }

        for (float i = 0; i < rangeEnd.x - rangeStart.x + gridManager.gridScale; i++)
        {
            for (float j = 0; j < rangeEnd.y - rangeStart.y + gridManager.gridScale; j++)
            {
                for (float k = 0; k < rangeEnd.z - rangeStart.z + gridManager.gridScale; k++)
                {
                    instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                    ((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
                    ((GameObject)instantiateBuffer).transform.position =
                        new Vector3(signx * i, signy * j, signz * k) + startRangeDrag;
                    ((GameObject)instantiateBuffer).transform.name = "visualizeObj";
                    Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");
                }
            }
        }
    }

    private void CreateBoxPlacement(Vector3 rangeStart, Vector3 rangeEnd)
    {
        int signx = 1;
        int signy = 1;
        int signz = 1;

        if (rangeStart.x > rangeEnd.x)
        {
            (rangeStart.x, rangeEnd.x) = (rangeEnd.x, rangeStart.x);
            signx = -1;
        }

        if (rangeStart.y > rangeEnd.y)
        {
            (rangeStart.y, rangeEnd.y) = (rangeEnd.y, rangeStart.y);
            signy = -1;
        }

        if (rangeStart.z > rangeEnd.z)
        {
            (rangeStart.z, rangeEnd.z) = (rangeEnd.z, rangeStart.z);
            signz = -1;
        }

        for (float i = 0; i < rangeEnd.x - rangeStart.x + gridManager.gridScale; i++)
        {
            for (float j = 0; j < rangeEnd.y - rangeStart.y + gridManager.gridScale; j++)
            {
                for (float k = 0; k < rangeEnd.z - rangeStart.z + gridManager.gridScale; k++)
                {
                    foreach (GameObject gameObject in gridManager.inGridObjects)
                    {
                        if (gameObject.transform.position == new Vector3(signx * i, signy * j, signz * k) + startRangeDrag)
                        {
                            return;
                        }

                    }

                    instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                    ((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
                    ((GameObject)instantiateBuffer).transform.position =
                        new Vector3(signx * i, signy * j, signz * k) + startRangeDrag;
                    Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");

                    Undo.RecordObject(gridManager, "isPlaced Check");
                    gridManager.inGridObjects.Add(((GameObject)instantiateBuffer));

                    var dummy = PrefabUtility.InstantiatePrefab(dummyBlock);
                    ((GameObject)dummy).hideFlags = HideFlags.HideInHierarchy;
                    ((GameObject)dummy).transform.position =
                        ((GameObject)instantiateBuffer).transform.position;
                    ((GameObject)dummy).transform.parent = ((GameObject)instantiateBuffer).transform;
                    ((GameObject)dummy).GetComponent<MeshRenderer>().enabled = false;
                    Undo.RegisterCreatedObjectUndo(dummy, "placed prefab");
                }
            }
        }
    }

    private void ReservationDeleteObj(Vector3 rangeStart, Vector3 rangeEnd)
    {
        int signx = 1;
        int signy = 1;
        int signz = 1;

        if (rangeStart.x > rangeEnd.x)
        {
            (rangeStart.x, rangeEnd.x) = (rangeEnd.x, rangeStart.x);
            signx = -1;
        }

        if (rangeStart.y > rangeEnd.y)
        {
            (rangeStart.y, rangeEnd.y) = (rangeEnd.y, rangeStart.y);
            signy = -1;
        }

        if (rangeStart.z > rangeEnd.z)
        {
            (rangeStart.z, rangeEnd.z) = (rangeEnd.z, rangeStart.z);
            signz = -1;
        }

        foreach (var o in gridManager.inGridObjects)
        {
            Vector3 objPos = o.transform.position;

            if (objPos.x >= rangeStart.x - gridManager.gridScale / 2 && objPos.x <= rangeEnd.x + gridManager.gridScale / 2 &&
                objPos.y >= rangeStart.y - gridManager.gridScale / 2 && objPos.y <= rangeEnd.y + gridManager.gridScale / 2 &&
                objPos.z >= rangeStart.z - gridManager.gridScale / 2 && objPos.z <= rangeEnd.z + gridManager.gridScale / 2)
            {
                Undo.RecordObject(o, "DisVisible");
                if (o.gameObject.GetComponent<MeshRenderer>() != null)
                {
                    o.gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
                else
                {
                    o.gameObject.SetActive(false);
                }

                deleteReservationList.Add(o);
            }
        }
    }

    private void ResetReservation()
    {
        foreach (var o in deleteReservationList)
        {
            if (o.name == "AblePlacementAround")
            {
                continue;
            }

            if (o.gameObject.GetComponent<MeshRenderer>() != null)
            {
                o.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
            else
            {
                o.gameObject.SetActive(true);
            }
        }

        deleteReservationList.Clear();
    }

    private void ConfirmDelete()
    {
        for (int i = deleteReservationList.Count; i > 0; i--)
        {
            gridManager.inGridObjects.Remove(deleteReservationList[i - 1]);
            Undo.DestroyObjectImmediate(deleteReservationList[i - 1]);
        }

        deleteReservationList.Clear();
    }

    private void ConfirmReplace()
    {
        for (int i = deleteReservationList.Count; i > 0; i--)
        {
            instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
            ((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
            ((GameObject)instantiateBuffer).transform.position = deleteReservationList[i- 1].transform.position;
                Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");

            Undo.RecordObject(gridManager, "isPlaced Check");
            gridManager.inGridObjects.Add(((GameObject)instantiateBuffer));

            var dummy = PrefabUtility.InstantiatePrefab(dummyBlock);
            ((GameObject)dummy).hideFlags = HideFlags.HideInHierarchy;
            ((GameObject)dummy).transform.position =
                ((GameObject)instantiateBuffer).transform.position;
            ((GameObject)dummy).transform.parent = ((GameObject)instantiateBuffer).transform;
            ((GameObject)dummy).GetComponent<MeshRenderer>().enabled = false;
            Undo.RegisterCreatedObjectUndo(dummy, "placed prefab");

            gridManager.inGridObjects.Remove(deleteReservationList[i - 1]);
            Undo.DestroyObjectImmediate(deleteReservationList[i - 1]);
        }

        deleteReservationList.Clear();
    }
}

#endif // UNITY_EDITOR