#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using Object = UnityEngine.Object;

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

    // インデックス参照バッファ
    private int indexBuffer = -1;

    // 消去時の参照リスト
    private List<int> indexNumList;

    // ドラッグしているかどうかのフラグ
    private bool isDrag = false;

    // ビジュアライザオブジェクト
    private Object visualizeObj;

    // 範囲ドラッグのインデックス始点
    private int rangeDragStartIndex;
    private Vector3Int startIndexPos;

    // 範囲ドラッグのインデックス終点
    private int rangeDragEndIndex;
    private Vector3Int endIndexPos;

    // ダミーブロックのバッファ
    private GameObject dummyBlock;

    // インデックスの一時退避用
    private List<int> indexListTmp;

    public EditorGridSelections()
    {
        dummyBlock = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorSource/BlockDummy.prefab", typeof(GameObject));
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

        int index = default;
        RaycastHit hit = new RaycastHit();
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        int mask = 1 << 6;

        var hits = Physics.RaycastAll(ray);
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        foreach (var hitObj in hits)
        {
            if (hitObj.collider.name == "AblePlacementAround" || hitObj.collider.name == "BlockDummy")
            {
                hit = hitObj;
                Debug.Log(hit);
                break;
            }
        }

        if (hit.collider == null)
        {
            return;
        }

        // フォーカスがグリッドオブジェクト外なら終了
        if (GridEditorWindow.gridObject != hit.transform.root.gameObject)
        {
            return;
        }

        // 内積を利用して手前側のオブジェクトのフォーカスを回避
        if (Vector3.Dot(hit.normal, ray.direction) < 0)
        {
            if (hit.collider.gameObject.GetComponent<IsVisualizeMesh>() != null)
            {

                hit.collider.gameObject.GetComponent<IsVisualizeMesh>().select_flg = true;

                // ビジュアライザが生成されてないなら生成
                if (visualizeObj == null && GridEditorWindow.obj != null && toolMode == ToolMode.TOOL_PLACE)
                {
                    if (hit.collider.name != "AblePlacementAround" && gridManager.IsCheckGridEdgeFromHit(hit))
                    {
                        return;
                    }

                    if (hit.collider.gameObject.GetComponent<GridRelatedInfo>().gridIndex <= 0)
                    {
                        Object.DestroyImmediate(visualizeObj);
                        return;
                    }

                    if (hit.collider.gameObject.GetComponent<GridRelatedInfo>().gridIndex < 0 ||
                        hit.collider.gameObject.GetComponent<GridRelatedInfo>().gridIndex >
                        gridManager.size.x * gridManager.size.y * gridManager.size.z)
                    {
                        return;
                    }

                    visualizeObj = Object.Instantiate(GridEditorWindow.obj,
                        gridManager.gridPosFromIndex[
                            hit.collider.gameObject.GetComponent<GridRelatedInfo>().gridIndex - 1],
                        Quaternion.identity);

                    visualizeObj.name = "visualizeObj";
                }

                if (toolMode != ToolMode.TOOL_PLACE || (hit.collider.name != "AblePlacementAround" && gridManager.IsCheckGridEdgeFromHit(hit)))
                {
                    GameObject.DestroyImmediate(visualizeObj);
                }

                // ビジュアライザが生成されていればフォーカスされている位置にビジュアライザを移動
                if (visualizeObj != null &&
                    !(hit.collider.gameObject.GetComponent<GridRelatedInfo>().gridIndex <= 0 ||
                      hit.collider.gameObject.GetComponent<GridRelatedInfo>().gridIndex >
                      gridManager.size.x * gridManager.size.y * gridManager.size.z))
                {
                    if (((GameObject)visualizeObj).transform.position != gridManager.gridPosFromIndex[
                            hit.collider.gameObject.GetComponent<GridRelatedInfo>().gridIndex - 1])
                    {
                        ((GameObject)visualizeObj).transform.position = gridManager.gridPosFromIndex[
                            hit.collider.gameObject.GetComponent<GridRelatedInfo>().gridIndex - 1];
                    }
                }
            }
        }
        else
        {
            // フォーカスが外れているならビジュアライザを消去
            Object.DestroyImmediate(visualizeObj);
        }

        switch (selectMode)
        {
            case OperationMode.OPERATION_CLICK: // クリックモード
                //左クリックされたら:設置モード
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && toolMode == ToolMode.TOOL_PLACE)
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<GridRelatedInfo>() != null && hit.collider.name == "AblePlacementAround")
                        {
                            index = hit.collider.GetComponent<GridRelatedInfo>().gridIndex;
                        }
                        else if (hit.collider.GetComponent<GridRelatedInfo>() != null && hit.collider.name == "BlockDummy")
                        {
                            index = gridManager.ConjecturePlacementObjIndex(hit);
                        }

                        if (index == default || index <= 0 || index > gridManager.size.x * gridManager.size.y * gridManager.size.z)
                        {
                            return;
                        }

                        if (GridEditorWindow.obj == null)
                        {
                            return;
                        }

                        if (gridManager.IsCheckGridEdgeFromHit(hit))
                        {
                            return;
                        }

                        ToolManager.SetActiveTool<CustomGridTools>();

                        var dummy = PrefabUtility.InstantiatePrefab(dummyBlock);
                        ((GameObject)dummy).transform.parent = ((GameObject)GridEditorWindow.gridObject).transform;
                        ((GameObject)dummy).transform.position = new Vector3(gridManager.gridPosFromIndex[index - 1].x, gridManager.gridPosFromIndex[index - 1].y, gridManager.gridPosFromIndex[index - 1].z);
                        ((GameObject)dummy).GetComponent<GridRelatedInfo>().gridIndex = index;
                        //((GameObject)dummy).hideFlags = HideFlags.HideInHierarchy;
                        ((GameObject)dummy).GetComponent<MeshRenderer>().enabled = false;
                        Undo.RegisterCreatedObjectUndo(dummy, "placed prefab");

                        instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                        ((GameObject)instantiateBuffer).transform.parent = ((GameObject)GridEditorWindow.gridObject).transform;
                        ((GameObject)instantiateBuffer).transform.position = new Vector3(gridManager.gridPosFromIndex[index - 1].x, gridManager.gridPosFromIndex[index - 1].y, gridManager.gridPosFromIndex[index - 1].z);

                        Undo.RecordObject(gridManager, "isPlaced Check");
                        gridManager.isPlaced[index - 1] = true;
                        Undo.RecordObject(gridManager, "Record PlacedObj");
                        gridManager.placedObjects[index - 1] = (GameObject)instantiateBuffer;
                        Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");

                    }
                }

                // 左クリックされたら:消去モード
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && toolMode == ToolMode.TOOL_ERASE)
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<GridRelatedInfo>() != null)
                        {
                            index = gridManager.ConjecturePlacementObjIndex(hit);
                        }

                        if (index < 1)
                        {
                            return;
                        }

                        if (hit.collider.transform.hideFlags == HideFlags.HideInHierarchy)
                        {
                            return;
                        }

                        Undo.RecordObject(gridManager, "isDelete Check");
                        gridManager.isPlaced[hit.collider.GetComponent<GridRelatedInfo>().gridIndex - 1] = false;
                        Undo.DestroyObjectImmediate(gridManager.placedObjects[hit.collider.GetComponent<GridRelatedInfo>().gridIndex - 1]);
                        Undo.RecordObject(gridManager, "isDelete Check");
                        gridManager.placedObjects[hit.collider.GetComponent<GridRelatedInfo>().gridIndex - 1] = null;
                        Undo.DestroyObjectImmediate(hit.collider.transform.gameObject);
                    }
                }
                break;

            case OperationMode.OPERATION_DRAG_FREE: //ドラッグモード
                // マウスボタンが押されたら
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                {
                    isDrag = true;
                    indexBuffer = -1;
                    indexListTmp = new List<int>();
                    // マウスクリックのホットコントロールを固定
                    GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                    Event.current.Use();
                }

                // マウスがドラッグされている場合は以下の処理を実行(マウスクリックダウンのイベントは当該フレームでしかアクティブにならないのでフラグで遷移)
                // 設置モード
                if (isDrag && toolMode == ToolMode.TOOL_PLACE)
                {
                    // マウスが離された場合
                    if (currentEvent.type == EventType.MouseUp ||
                        currentEvent.type == EventType.MouseLeaveWindow && currentEvent.button == 0)
                    {
                        isDrag = false;

                        foreach (var tmp in indexListTmp)
                        {
                            var dummy = PrefabUtility.InstantiatePrefab(dummyBlock);
                            ((GameObject)dummy).transform.parent = ((GameObject)GridEditorWindow.gridObject).transform;
                            ((GameObject)dummy).transform.position = new Vector3(gridManager.gridPosFromIndex[tmp].x, gridManager.gridPosFromIndex[tmp].y, gridManager.gridPosFromIndex[tmp].z);
                            ((GameObject)dummy).GetComponent<GridRelatedInfo>().gridIndex = tmp + 1;
                            //((GameObject)dummy).hideFlags = HideFlags.HideInHierarchy;
                            ((GameObject)dummy).GetComponent<MeshRenderer>().enabled = false;
                            Undo.RegisterCreatedObjectUndo(dummy, "placed prefab");
                        }

                        return;
                    }

                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<GridRelatedInfo>() != null && hit.collider.name == "AblePlacementAround")
                        {
                            index = hit.collider.GetComponent<GridRelatedInfo>().gridIndex;
                        }
                        else if (hit.collider.GetComponent<GridRelatedInfo>() != null && hit.collider.name == "BlockDummy")
                        {
                            index = gridManager.ConjecturePlacementObjIndex(hit);
                        }

                        if (index == default || index <= 0 || index > gridManager.size.x * gridManager.size.y * gridManager.size.z)
                        {
                            return;
                        }

                        if (gridManager.isPlaced[index - 1])
                        {
                            return;
                        }

                        if (GridEditorWindow.obj == null)
                        {
                            return;
                        }

                        if (gridManager.IsCheckGridEdgeFromHit(hit))
                        {
                            return;
                        }

                        if (index != indexBuffer)
                        {
                            ToolManager.SetActiveTool<CustomGridTools>();

                            instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                            ((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
                            ((GameObject)instantiateBuffer).transform.position = new Vector3(gridManager.gridPosFromIndex[index - 1].x, gridManager.gridPosFromIndex[index - 1].y, gridManager.gridPosFromIndex[index - 1].z);
                            Undo.RecordObject(gridManager, "isPlaced Check");
                            gridManager.isPlaced[index - 1] = true;
                            indexListTmp.Add(index - 1);
                            gridManager.placedObjects[index - 1] = (GameObject)instantiateBuffer;
                            Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");
                            indexBuffer = index;
                        }
                    }
                }

                // 消去モード
                if (isDrag && toolMode == ToolMode.TOOL_ERASE)
                {
                    if (currentEvent.type == EventType.MouseUp ||
                        currentEvent.type == EventType.MouseLeaveWindow && currentEvent.button == 0)
                    {
                        isDrag = false;

                        indexNumList.Clear();
                    }

                    // リストがnullなら新しく生成
                    indexNumList ??= new List<int>();

                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<GridRelatedInfo>() != null)
                        {
                            index = gridManager.ConjecturePlacementObjIndex(hit);
                        }

                        if (index <= 0 || index > gridManager.size.x * gridManager.size.y * gridManager.size.z)
                        {
                            return;
                        }

                        if (hit.collider.transform.parent.name == "AblePlacement")
                        {
                            return;
                        }

                        if (hit.collider.transform.hideFlags == HideFlags.HideInHierarchy)
                        {
                            return;
                        }

                        Undo.RecordObject(gridManager, "isDelete Check");
                        gridManager.isPlaced[hit.collider.GetComponent<GridRelatedInfo>().gridIndex - 1] = false;
                        Undo.DestroyObjectImmediate(gridManager.placedObjects[hit.collider.GetComponent<GridRelatedInfo>().gridIndex - 1]);
                        Undo.RecordObject(gridManager, "isDelete Check");
                        gridManager.placedObjects[hit.collider.GetComponent<GridRelatedInfo>().gridIndex - 1] = null;
                        Undo.DestroyObjectImmediate(hit.collider.transform.gameObject);
                    }
                }

                break;
            case OperationMode.OPERATION_RANGE: // 範囲選択モード
                // マウスボタンが押されたら
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                {
                    rangeDragEndIndex = -1;

                    // ドラッグ開始の始点インデックスを取得
                    if (hit.collider != null)
                    {

                        if (toolMode == ToolMode.TOOL_PLACE && (hit.collider.GetComponent<GridRelatedInfo>().gridIndex > 0 && hit.collider.GetComponent<GridRelatedInfo>().gridIndex <= gridManager.size.x * gridManager.size.y * gridManager.size.z))
                        {
                            rangeDragStartIndex = hit.collider.GetComponent<GridRelatedInfo>().gridIndex;
                        }
                        else if (toolMode == ToolMode.TOOL_ERASE)
                        {
                            rangeDragStartIndex = gridManager.ConjecturePlacementObjIndex(hit);
                        }

                        startIndexPos = gridManager.ReturnGridSquarePoint(rangeDragStartIndex);
                    }

                    isDrag = true;

                    // マウスクリックのホットコントロールを固定
                    GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                    Event.current.Use();
                }

                // マウスがドラッグされている場合は以下の処理を実行(マウスクリックダウンのイベントは当該フレームでしかアクティブにならないのでフラグで遷移)
                // 設置モード
                if (isDrag && toolMode == ToolMode.TOOL_PLACE)
                {
                    // マウスが離された場合
                    if (currentEvent.type == EventType.MouseUp ||
                        currentEvent.type == EventType.MouseLeaveWindow && currentEvent.button == 0)
                    {
                        ClearVisualizer();
                        if (hit.collider != null)
                        {
                            rangeDragEndIndex = hit.collider.GetComponent<GridRelatedInfo>().gridIndex;
                        }

                        if (rangeDragEndIndex == -1)
                        {
                            isDrag = false;
                            return;
                        }

                        endIndexPos = gridManager.ReturnGridSquarePoint(rangeDragEndIndex);
                        CreateBoxPlacement(startIndexPos, endIndexPos);
                        isDrag = false;
                        AddPlacementAndDeleteConjecture(startIndexPos, endIndexPos);
                        return;
                    }

                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<GridRelatedInfo>() != null)
                        {
                            index = hit.collider.GetComponent<GridRelatedInfo>().gridIndex;
                        }

                        if (index == default || index <= 0 || index > gridManager.size.x * gridManager.size.y * gridManager.size.z)
                        {
                            return;
                        }

                        if (gridManager.isPlaced[index - 1])
                        {
                            return;
                        }

                        if (GridEditorWindow.obj == null)
                        {
                            return;
                        }

                        if (index != indexBuffer)
                        {
                            ClearVisualizer();
                            CreateVisualizerBox(startIndexPos, gridManager.ReturnGridSquarePoint(hit.collider.GetComponent<GridRelatedInfo>().gridIndex));
                            indexBuffer = index;
                        }
                    }
                }

                // 消去モード
                if (isDrag && toolMode == ToolMode.TOOL_ERASE)
                {
                    // マウスが離された場合
                    if (currentEvent.type == EventType.MouseUp ||
                        currentEvent.type == EventType.MouseLeaveWindow && currentEvent.button == 0)
                    {
                        ClearVisualizer();



                        if (hit.collider != null)
                        {
                            rangeDragEndIndex = gridManager.ConjecturePlacementObjIndex(hit);
                            endIndexPos = gridManager.ReturnGridSquarePoint(rangeDragEndIndex);
                        }

                        if (rangeDragEndIndex == -1)
                        {
                            isDrag = false;
                            return;
                        }

                        if (endIndexPos == new Vector3Int(-1, -1, -1))
                        {
                            isDrag = false;
                            return;
                        }

                        ConfirmDeleteObj(startIndexPos, endIndexPos);
                        isDrag = false;
                        foreach (var indexList in indexNumList)
                        {
                            placementArea.AddAdJoinPlacement(indexList);

                            if (System.Array.IndexOf(gridManager.ablePLacementSurround.index, indexList) != -1)
                            {
                                placementArea.AddSurroundPlacement(indexList);
                            }
                        }

                        indexNumList.Clear();
                        return;
                    }

                    // リストがnullなら新しく生成
                    indexNumList ??= new List<int>();

                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<GridRelatedInfo>() != null)
                        {
                            index = gridManager.ConjecturePlacementObjIndex(hit);
                        }

                        if (index == default)
                        {
                            return;
                        }

                        if (index < 1 || index > gridManager.size.x * gridManager.size.y * gridManager.size.z)
                        {
                            return;
                        }

                        if (!gridManager.isPlaced[index - 1])
                        {
                            return;
                        }

                        if (GridEditorWindow.obj == null)
                        {
                            return;
                        }

                        if (index != indexBuffer)
                        {
                            CancelReservationObj();
                            ReservationDeleteObj(startIndexPos, gridManager.ReturnGridSquarePoint(gridManager.ConjecturePlacementObjIndex(hit)));
                            indexBuffer = index;
                        }
                    }
                }

                break;
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

    /// <summary>
    /// 範囲選択モードの時に選択されていた範囲にビジュアライザを表示するメソッド
    /// </summary>
    private void CreateVisualizerBox(Vector3Int rangeStartIndex, Vector3Int rangeEndIndex)
    {
        if (rangeStartIndex.x > rangeEndIndex.x)
        {
            (rangeStartIndex.x, rangeEndIndex.x) = (rangeEndIndex.x, rangeStartIndex.x);
        }

        if (rangeStartIndex.y > rangeEndIndex.y)
        {
            (rangeStartIndex.y, rangeEndIndex.y) = (rangeEndIndex.y, rangeStartIndex.y);
        }

        if (rangeStartIndex.z > rangeEndIndex.z)
        {
            (rangeStartIndex.z, rangeEndIndex.z) = (rangeEndIndex.z, rangeStartIndex.z);
        }

        for (int i = 0; i < rangeEndIndex.z - rangeStartIndex.z + 1; i++)
        {
            for (int j = 0; j < rangeEndIndex.x - rangeStartIndex.x + 1; j++)
            {
                for (int k = 0; k < rangeEndIndex.y - rangeStartIndex.y + 1; k++)
                {
                    instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                    ((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
                    ((GameObject)instantiateBuffer).transform.position = gridManager.gridPosFromIndexMultiple[rangeStartIndex.x, rangeStartIndex.y, rangeStartIndex.z] + new Vector3((j) * 1.0f, (k) * 1.0f, (i) * 1.0f);
                    ((GameObject)instantiateBuffer).name = "visualizeObj";
                }
            }
        }
    }

    /// <summary>
    /// 範囲選択モードの時に選択されていた範囲にプレハブを設置するメソッド
    /// </summary>
    private void CreateBoxPlacement(Vector3Int rangeStartIndex, Vector3Int rangeEndIndex)
    {
        Vector3Int size = gridManager.size;

        if (rangeStartIndex.x > rangeEndIndex.x)
        {
            (rangeStartIndex.x, rangeEndIndex.x) = (rangeEndIndex.x, rangeStartIndex.x);
        }

        if (rangeStartIndex.y > rangeEndIndex.y)
        {
            (rangeStartIndex.y, rangeEndIndex.y) = (rangeEndIndex.y, rangeStartIndex.y);
        }

        if (rangeStartIndex.z > rangeEndIndex.z)
        {
            (rangeStartIndex.z, rangeEndIndex.z) = (rangeEndIndex.z, rangeStartIndex.z);
        }

        for (int i = 0; i < rangeEndIndex.z - rangeStartIndex.z + 1; i++)
        {
            for (int j = 0; j < rangeEndIndex.x - rangeStartIndex.x + 1; j++)
            {
                for (int k = 0; k < rangeEndIndex.y - rangeStartIndex.y + 1; k++)
                {

                    instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                    ((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
                    ((GameObject)instantiateBuffer).transform.position = gridManager.gridPosFromIndexMultiple[rangeStartIndex.x, rangeStartIndex.y, rangeStartIndex.z] + new Vector3((j) * 1.0f, (k) * 1.0f, (i) * 1.0f);
                    Undo.RecordObject(gridManager, "isPlaced Check");
                    gridManager.isPlaced[(rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y + (i * size.x * size.y) + (j * size.y) + k] = true;
                    gridManager.placedObjects[(rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y + (i * size.x * size.y) + (j * size.y) + k] = (GameObject)instantiateBuffer;
                    Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");
                }
            }
        }
    }

    /// <summary>
    /// 範囲選択モードで設置したオブジェクトに設置可能判定を付与し、不要な設置可能判定を取り除くメソッド
    /// </summary>
    /// <param name="rangeStartIndex"> 選択範囲始点 </param>
    /// <param name="rangeEndIndex"> 選択範囲終点 </param>
    private void AddPlacementAndDeleteConjecture(Vector3Int rangeStartIndex, Vector3Int rangeEndIndex)
    {
        Vector3Int size = gridManager.size;

        if (rangeStartIndex.x > rangeEndIndex.x)
        {
            (rangeStartIndex.x, rangeEndIndex.x) = (rangeEndIndex.x, rangeStartIndex.x);
        }

        if (rangeStartIndex.y > rangeEndIndex.y)
        {
            (rangeStartIndex.y, rangeEndIndex.y) = (rangeEndIndex.y, rangeStartIndex.y);
        }

        if (rangeStartIndex.z > rangeEndIndex.z)
        {
            (rangeStartIndex.z, rangeEndIndex.z) = (rangeEndIndex.z, rangeStartIndex.z);
        }

        for (int i = 0; i < rangeEndIndex.z - rangeStartIndex.z + 1; i++)
        {
            for (int j = 0; j < rangeEndIndex.x - rangeStartIndex.x + 1; j++)
            {
                for (int k = 0; k < rangeEndIndex.y - rangeStartIndex.y + 1; k++)
                {
                    if (i > 0 && i < rangeEndIndex.z - rangeStartIndex.z && j > 0 &&
                        j < rangeEndIndex.x - rangeStartIndex.x && k > 0 && k < rangeEndIndex.y - rangeStartIndex.y)
                    {
                        continue;
                    }

                    placementArea.AddPlaceMentArea((rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y + (i * size.x * size.y) + (j * size.y) + k + 1);
                }
            }
        }

        // 
        for (int i = 0; i < rangeEndIndex.z - rangeStartIndex.z + 1; i++)
        {
            for (int j = 0; j < rangeEndIndex.x - rangeStartIndex.x + 1; j++)
            {
                for (int k = 0; k < rangeEndIndex.y - rangeStartIndex.y + 1; k++)
                {
                    placementArea.DeletePlacementArea((rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y + (i * size.x * size.y) + (j * size.y) + k + 1);
                }
            }
        }
    }

    /// <summary>
    /// 範囲選択時消去予定のオブジェクトのメッシュレンダラーを消すメソッド
    /// </summary>
    private void ReservationDeleteObj(Vector3Int rangeStartIndex, Vector3Int rangeEndIndex)
    {
        Vector3Int size = gridManager.size;

        if (rangeStartIndex.x > rangeEndIndex.x)
        {
            (rangeStartIndex.x, rangeEndIndex.x) = (rangeEndIndex.x, rangeStartIndex.x);
        }

        if (rangeStartIndex.y > rangeEndIndex.y)
        {
            (rangeStartIndex.y, rangeEndIndex.y) = (rangeEndIndex.y, rangeStartIndex.y);
        }

        if (rangeStartIndex.z > rangeEndIndex.z)
        {
            (rangeStartIndex.z, rangeEndIndex.z) = (rangeEndIndex.z, rangeStartIndex.z);
        }

        for (int i = 0; i < rangeEndIndex.z - rangeStartIndex.z + 1; i++)
        {
            for (int j = 0; j < rangeEndIndex.x - rangeStartIndex.x + 1; j++)
            {
                for (int k = 0; k < rangeEndIndex.y - rangeStartIndex.y + 1; k++)
                {
                    if (gridManager.placedObjects[
                            (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                            (i * size.x * size.y) + (j * size.y) + k - 1] == null || ((rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                        (i * size.x * size.y) + (j * size.y) + k - 1 < 0 && (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                        (i * size.x * size.y) + (j * size.y) + k - 1 >= size.x * size.y * size.z))
                    {
                        continue;
                    }

                    if (gridManager.placedObjects[(rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y + (i * size.x * size.y) + (j * size.y) + k - 1].transform.parent == null)
                    {
                        continue;
                    }

                    Undo.RecordObject(gridManager.placedObjects[
                            (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                            (i * size.x * size.y) + (j * size.y) + k].gameObject.transform
                        .GetComponent<MeshRenderer>(), "invisible mesh");

                    gridManager.placedObjects[
                            (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y + (i * size.x * size.y) + (j * size.y) + k].gameObject.transform
                        .GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }
    }

    /// <summary>
    /// 範囲選択から外れたオブジェクトのメッシュレンダラーをenableにするメソッド
    /// </summary>
    private void CancelReservationObj()
    {
        foreach (var placedObject in gridManager.placedObjects)
        {
            if (placedObject == null)
            {
                continue;
            }

            if (!placedObject.GetComponent<MeshRenderer>().enabled)
            {
                placedObject.GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }

    /// <summary>
    /// 消去確定の場合にオブジェクトを消去するメソッド
    /// </summary>
    private void ConfirmDeleteObj(Vector3Int rangeStartIndex, Vector3Int rangeEndIndex)
    {
        Vector3Int size = gridManager.size;

        if (rangeStartIndex.x > rangeEndIndex.x)
        {
            (rangeStartIndex.x, rangeEndIndex.x) = (rangeEndIndex.x, rangeStartIndex.x);
        }

        if (rangeStartIndex.y > rangeEndIndex.y)
        {
            (rangeStartIndex.y, rangeEndIndex.y) = (rangeEndIndex.y, rangeStartIndex.y);
        }

        if (rangeStartIndex.z > rangeEndIndex.z)
        {
            (rangeStartIndex.z, rangeEndIndex.z) = (rangeEndIndex.z, rangeStartIndex.z);
        }

        for (int i = 0; i < rangeEndIndex.z - rangeStartIndex.z + 1; i++)
        {
            for (int j = 0; j < rangeEndIndex.x - rangeStartIndex.x + 1; j++)
            {
                for (int k = 0; k < rangeEndIndex.y - rangeStartIndex.y + 1; k++)
                {
                    if (gridManager.placedObjects[(rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y + (i * size.x * size.y) + (j * size.y) + k] == null)
                    {
                        continue;
                    }

                    Undo.DestroyObjectImmediate(gridManager.placedObjects[(rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y + (i * size.x * size.y) + (j * size.y) + k]);
                    Undo.RecordObject(gridManager, "Delete PlacedFlag");
                    gridManager.isPlaced[(rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y + (i * size.x * size.y) + (j * size.y) + k] = false;
                    Undo.RecordObject(gridManager, "Delete PlacedFlag");
                    gridManager.placedObjects[(rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y + (i * size.x * size.y) + (j * size.y) + k] = null;
                    indexNumList.Add(((rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y + (i * size.x * size.y) + (j * size.y) + k));
                }
            }
        }
    }
}

#endif // UNITY_EDITOR