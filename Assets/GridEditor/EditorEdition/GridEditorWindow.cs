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

    //���{��̃y�[�W�ɂȂ�悤��URL�쐬
    string url = "https://github.com/Lazrite/VoxelEdit/tree/master";

    /// <Summary>
    /// �E�B���h�E��\�����܂��B
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
                EditorGUILayout.LabelField("�I�����[�h", GUILayout.Width(64));

                if (editorGridSelections.selectMode != OperationMode.OPERATION_CLICK)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/ClickDown.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�N���b�N���[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OPERATION_CLICK;
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/ClickDown_selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�N���b�N���[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OPERATION_CLICK;
                    }
                }

                if (editorGridSelections.selectMode != OperationMode.OPERATION_DRAG_FREE)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/DragDown.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�t���[�h���b�O���[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OPERATION_DRAG_FREE;
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/DragDown_selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�t���[�h���b�O���[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OPERATION_DRAG_FREE;
                    }
                }

                if (editorGridSelections.selectMode != OperationMode.OPERATION_RANGE)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/RangeSelection.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�͈̓h���b�O���[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OPERATION_RANGE;
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/RangeSelection_selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�͈̓h���b�O���[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OPERATION_RANGE;
                    }
                }
            }

            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField("�c�[�����[�h", GUILayout.Width(64));

                if (editorGridSelections.toolMode != ToolMode.TOOL_PLACE)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/WriteMode.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�ݒu���[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.toolMode = ToolMode.TOOL_PLACE;
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/WriteMode_Selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�ݒu���[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.toolMode = ToolMode.TOOL_PLACE;
                    }
                }

                if (editorGridSelections.toolMode != ToolMode.TOOL_ERASE)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/EraseMode.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�������[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.toolMode = ToolMode.TOOL_ERASE;
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/EraseMode_Selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�������[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.toolMode = ToolMode.TOOL_ERASE;
                    }
                }

            }

            using (new EditorGUILayout.VerticalScope())
            {
                // �I�u�W�F�N�g�i�V�[�����I�u�W�F�N�g�s�j
                obj = EditorGUILayout.ObjectField("�u�������I�u�W�F�N�g", obj, typeof(GameObject), false);
                
                EditorGUI.BeginChangeCheck();

                gridObject = EditorGUILayout.ObjectField("�O���b�h�I�u�W�F�N�g", gridObject, typeof(GameObject), true) as GameObject;
                
                if (EditorGUI.EndChangeCheck())
                {
                    ((GameObject)gridObject).GetComponent<EditorGridField>().PreLoadGridInfo();
                }

                if (gridObject != null)
                {
                    // �I�u�W�F�N�g�̈ʒu
                    ((GameObject)gridObject).transform.position =
                        EditorGUILayout.Vector3Field("�I�u�W�F�N�g�̈ʒu", ((GameObject)gridObject).transform.position);

                    if(((GameObject)gridObject).transform.position != transformBuffer)
                    {
                        transformBuffer = ((GameObject)gridObject).transform.position;
                        ((GameObject)gridObject).GetComponent<EditorGridField>().ReCalculationGridPos();
                    }

                    EditorGUI.BeginChangeCheck();

                    // �O���b�h�̑傫��
                    ((GameObject)gridObject).GetComponent<EditorGridField>().size =
                        EditorGUILayout.Vector3IntField("�O���b�h�T�C�Y",
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

                    // ���̑����\��
                    ((GameObject)gridObject).GetComponent<EditorGridField>().lineSize =
                        EditorGUILayout.FloatField("���̑���",
                            ((GameObject)gridObject).GetComponent<EditorGridField>().lineSize);

                    if(((GameObject)gridObject).GetComponent<EditorGridField>().lineSize <= 0)
                    {
                        ((GameObject)gridObject).GetComponent<EditorGridField>().lineSize = 0.01f;
                    }

                }
            }

        }

        if (GUILayout.Button(new GUIContent("������@�ɂ��Ă͂�����")))
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