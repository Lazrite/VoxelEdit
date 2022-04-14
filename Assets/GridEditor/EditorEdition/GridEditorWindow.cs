#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class GridEditorWindow : EditorWindow
{
    public static Object obj;
    public static Object gridObject;
    private Texture2D texture;

    private EditorGridSelections editorGridSelections;

    private Vector3 transformBuffer;

    //日本語のページになるようにURL作成
    string url = "https://github.com/Lazrite/VoxelEdit/tree/master";

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
        editorGridSelections = new EditorGridSelections();

        SceneView.duringSceneGui -= editorGridSelections.OnScene;
        SceneView.duringSceneGui += editorGridSelections.OnScene;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= editorGridSelections.OnScene;
    }



    private void OnGUI()
    {

        EditorGUI.BeginChangeCheck();

        using (new EditorGUILayout.HorizontalScope())
        {

            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField("選択モード", GUILayout.Width(64));

                if (editorGridSelections.selectMode != OperationMode.OPERATION_CLICK)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/ClickDown.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "クリックモード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OPERATION_CLICK;
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/ClickDown_selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "クリックモード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OPERATION_CLICK;
                    }
                }

                if (editorGridSelections.selectMode != OperationMode.OPERATION_DRAG_FREE)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/DragDown.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "フリードラッグモード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OPERATION_DRAG_FREE;
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/DragDown_selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "フリードラッグモード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OPERATION_DRAG_FREE;
                    }
                }

                if (editorGridSelections.selectMode != OperationMode.OPERATION_RANGE)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/RangeSelection.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "範囲ドラッグモード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OPERATION_RANGE;
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/RangeSelection_selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "範囲ドラッグモード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OPERATION_RANGE;
                    }
                }
            }

            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField("ツールモード", GUILayout.Width(64));

                if (editorGridSelections.toolMode != ToolMode.TOOL_PLACE)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/WriteMode.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "設置モード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.toolMode = ToolMode.TOOL_PLACE;
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/WriteMode_Selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "設置モード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.toolMode = ToolMode.TOOL_PLACE;
                    }
                }

                if (editorGridSelections.toolMode != ToolMode.TOOL_ERASE)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/EraseMode.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "消去モード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.toolMode = ToolMode.TOOL_ERASE;
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/EraseMode_Selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "消去モード"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.toolMode = ToolMode.TOOL_ERASE;
                    }
                }

            }

            using (new EditorGUILayout.VerticalScope())
            {
                // オブジェクト（シーン内オブジェクト不可）
                obj = EditorGUILayout.ObjectField("置きたいオブジェクト", obj, typeof(GameObject), false);
                
                EditorGUI.BeginChangeCheck();

                gridObject = EditorGUILayout.ObjectField("グリッドオブジェクト", gridObject, typeof(GameObject), true) as GameObject;
                
                if (EditorGUI.EndChangeCheck())
                {
                    ((GameObject)gridObject).GetComponent<EditorGridField>().PreLoadGridInfo();
                }

                if (gridObject != null)
                {
                    // オブジェクトの位置
                    ((GameObject)gridObject).transform.position =
                        EditorGUILayout.Vector3Field("オブジェクトの位置", ((GameObject)gridObject).transform.position);

                    if(((GameObject)gridObject).transform.position != transformBuffer)
                    {
                        transformBuffer = ((GameObject)gridObject).transform.position;
                        ((GameObject)gridObject).GetComponent<EditorGridField>().ReCalculationGridPos();
                    }

                    EditorGUI.BeginChangeCheck();

                    // グリッドの大きさ
                    ((GameObject)gridObject).GetComponent<EditorGridField>().size =
                        EditorGUILayout.Vector3IntField("グリッドサイズ",
                            ((GameObject)gridObject).GetComponent<EditorGridField>().size);

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
                        Undo.RecordObject(((GameObject)gridObject).GetComponent<EditorGridField>(), "Clear Grid");
                        ((GameObject)gridObject).GetComponent<EditorGridField>().ClearGrid();
                        ((GameObject)gridObject).GetComponent<EditorGridField>().InstantiateGridField();
                    }

                    // 線の太さ表示
                    ((GameObject)gridObject).GetComponent<EditorGridField>().lineSize =
                        EditorGUILayout.FloatField("線の太さ",
                            ((GameObject)gridObject).GetComponent<EditorGridField>().lineSize);

                    if(((GameObject)gridObject).GetComponent<EditorGridField>().lineSize <= 0)
                    {
                        ((GameObject)gridObject).GetComponent<EditorGridField>().lineSize = 0.01f;
                    }

                }
            }

        }

        if (GUILayout.Button(new GUIContent("操作方法についてはこちら")))
        {
            Process.Start("ReadMe.html");
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

    }

}

#endif // UNITY_EDITOR