#if UNITY_EDITOR

using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ウィンドウ情報クラス
/// </summary>
public class GridEditorWindow : EditorWindow
{
    // 置きたいオブジェクト
    public static Object obj;
    // 対象グリッドオブジェクト
    public static Object gridObject;
    // 画像アイコン
    private Texture2D texture;
    // セレクタ
    private EditorGridSelections editorGridSelections;
    // 対象グリッドオブジェクトのバッファ
    private Vector3 transformBuffer;

    // 日本語のページになるようにURL作成
    private string url = "https://github.com/Lazrite/VoxelEdit/tree/master";

    /// <Summary>
    /// ウィンドウを表示します。
    /// </Summary>
    [MenuItem("Window/GridWindow")]
    private static void Open()
    {
        var window = GetWindow<GridEditorWindow>();
        window.titleContent = new GUIContent("Grid");
    }

    private void OnEnable()
    {
        // ウィンドウがアクティブになったらセレクタのデリゲート登録
        editorGridSelections = new EditorGridSelections();

        SceneView.duringSceneGui -= editorGridSelections.OnScene;
        SceneView.duringSceneGui += editorGridSelections.OnScene;
        editorGridSelections.StartSelectProcess();
    }

    private void OnDisable()
    {
        // ウィンドウが非アクティブになったらデリゲート解除
        SceneView.duringSceneGui -= editorGridSelections.OnScene;
    }


    /// <summary>
    /// ウィンドウが開いているときに画面に表示したい要素をここに書く
    /// </summary>
    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();

        using (new EditorGUILayout.HorizontalScope())
        {
            // 各ツールモードボタン
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField("選択モード", GUILayout.Width(64));

                if (editorGridSelections.selectMode != OperationMode.OperationClick)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/ClickDown.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "クリックモード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OperationClick;
                        Visualizer.DisableRenderer();
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/ClickDown_selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "クリックモード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OperationClick;
                        Visualizer.DisableRenderer();
                    }
                }

                if (editorGridSelections.selectMode != OperationMode.OperationDragFree)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/DragDown.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "フリードラッグモード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OperationDragFree;
                        Visualizer.DisableRenderer();
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/DragDown_selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "フリードラッグモード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OperationDragFree;
                        Visualizer.DisableRenderer();
                    }
                }

                if (editorGridSelections.selectMode != OperationMode.OperationRange)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/RangeSelection.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "範囲ドラッグモード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OperationRange;
                        Visualizer.DisableRenderer();
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/RangeSelection_selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "範囲ドラッグモード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OperationRange;
                        Visualizer.DisableRenderer();
                    }
                }
            }

            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField("ツールモード", GUILayout.Width(64));

                if (editorGridSelections.toolMode != ToolMode.ToolPlace)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/WriteMode.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "設置モード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.toolMode = ToolMode.ToolPlace;
                        Visualizer.DisableRenderer();
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/WriteMode_Selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "設置モード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.toolMode = ToolMode.ToolPlace;
                        Visualizer.DisableRenderer();
                    }
                }

                if (editorGridSelections.toolMode != ToolMode.ToolErase)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/EraseMode.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "消去モード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.toolMode = ToolMode.ToolErase;
                        Visualizer.DisableRenderer();
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/EraseMode_Selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "消去モード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.toolMode = ToolMode.ToolErase;
                        Visualizer.DisableRenderer();
                    }
                }

                if (editorGridSelections.toolMode != ToolMode.ToolReplace)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/ReplacementMode.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "置換モード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.toolMode = ToolMode.ToolReplace;
                        Visualizer.DisableRenderer();
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/ReplacementMode_Selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "置換モード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.toolMode = ToolMode.ToolReplace;
                        Visualizer.DisableRenderer();
                    }
                }

            }

            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUI.BeginChangeCheck();

                // オブジェクト（シーン内オブジェクト不可）
                obj = EditorGUILayout.ObjectField("置きたいオブジェクト", obj, typeof(GameObject), false);

                if (EditorGUI.EndChangeCheck())
                {
                    Visualizer.DestroyVisualizer();
                }

                EditorGUI.BeginChangeCheck();

                // グリッドオブジェクト(こちらはシーン内のオブジェクトをアタッチする)
                gridObject = EditorGUILayout.ObjectField("グリッドオブジェクト", gridObject, typeof(GameObject), true) as GameObject;

                if (EditorGUI.EndChangeCheck())
                {
                    Visualizer.DestroyVisualizer();
                }

                if (gridObject != null)
                {
                    EditorGUI.BeginChangeCheck();

                    // グリッドの大きさ
                    ((GameObject)gridObject).GetComponent<EditorGridField>().size =
                        EditorGUILayout.Vector3IntField("グリッドサイズ",
                            ((GameObject)gridObject).GetComponent<EditorGridField>().size);

                    // 不正値入力防止
                    if (((GameObject)gridObject).GetComponent<EditorGridField>().size.x <= 0)
                    {
                        ((GameObject)gridObject).GetComponent<EditorGridField>().size.x = 1;
                    }
                    if (((GameObject)gridObject).GetComponent<EditorGridField>().size.y <= 0)
                    {
                        ((GameObject)gridObject).GetComponent<EditorGridField>().size.y = 1;
                    }
                    if (((GameObject)gridObject).GetComponent<EditorGridField>().size.z <= 0)
                    {
                        ((GameObject)gridObject).GetComponent<EditorGridField>().size.z = 1;
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        // 大きさが変わったらグリッド完全消去(現在警告なし)
                        // TODO: ちゃんと情報を保存できるようにしなきゃ！！！！！！
                        Undo.RecordObject(((GameObject)gridObject).GetComponent<EditorGridField>(), "Clear Grid");
                        ((GameObject)gridObject).GetComponent<EditorGridField>().ClearGrid();
                        ((GameObject)gridObject).GetComponent<EditorGridField>().InstantiateGridField();
                    }

                    // 線の太さ表示
                    ((GameObject)gridObject).GetComponent<EditorGridField>().lineSize =
                        EditorGUILayout.FloatField("線の太さ",
                            ((GameObject)gridObject).GetComponent<EditorGridField>().lineSize);

                    if (((GameObject)gridObject).GetComponent<EditorGridField>().lineSize <= 0)
                    {
                        ((GameObject)gridObject).GetComponent<EditorGridField>().lineSize = 0.01f;
                    }

                    // グリッド完全消去ボタン
                    GUIStyle style = new GUIStyle(EditorStyles.miniButton)
                    {
                        normal =
                        {
                            textColor = Color.red
                        },
                        alignment = TextAnchor.MiddleCenter,
                        hover =
                        {
                            textColor = Color.red
                        }
                    };

                    // グリッド完全消去ボタン
                    if (GUILayout.Button(new GUIContent("グリッド完全消去"), style))
                    {
                        // 最終警告
                        if (EditorUtility.DisplayDialog("グリッドを完全に消去しようとしています！", "これを実行するとグリッド内のオブジェクトはすべて消去され、復元出来ません！",
                                "実行", "キャンセル"))
                        {
                            ((GameObject)gridObject).GetComponent<EditorGridField>().ClearGrid();
                            ((GameObject)gridObject).GetComponent<EditorGridField>().InstantiateGridField();
                        }
                    }

                    // JSONコンバートボタン
                    if (GUILayout.Button(new GUIContent("Jsonにコンバート"), style))
                    {
                        List<GameObject> convObj = new List<GameObject>();

                        // 出力用オブジェクトをリストに格納
                        foreach (GameObject convert in ((GameObject)gridObject).GetComponent<EditorGridField>().inGridObjects)
                        {
                            if (convert == null)
                            {
                                continue;
                            }
                            if (convert.name == "BlockDummy" || convert.name == "AblePlacementAround")
                                continue;

                            convObj.Add(convert);
                        }

                        ConvertJson.SaveJson(convObj);
                    }
                }
            }
        }

        // 操作説明ボタン
        if (GUILayout.Button(new GUIContent("操作方法についてはこちら")))
        {
            Process.Start(url);
        }

        // 数値変更確認
        if (EditorGUI.EndChangeCheck())
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

    }
}

#endif // UNITY_EDITOR