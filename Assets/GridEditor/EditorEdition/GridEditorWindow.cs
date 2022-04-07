using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridEditorWindow : EditorWindow
{
    public static Object obj;
    public static Object gridObject;
    private Texture2D texture;

    private EditorGridSelections editorGridSelections;

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
                gridObject = EditorGUILayout.ObjectField("グリッドオブジェクト", gridObject, typeof(GameObject), true) as GameObject;
            }

        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

    }

}
