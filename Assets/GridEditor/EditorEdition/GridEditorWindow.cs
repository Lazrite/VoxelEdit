#if UNITY_EDITOR

using System.Diagnostics;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridEditorWindow : EditorWindow
{
    // �u�������I�u�W�F�N�g
    public static Object obj;
    // �ΏۃO���b�h�I�u�W�F�N�g
    public static Object gridObject;
    // �摜�A�C�R��
    private Texture2D texture;
    // �Z���N�^
    private EditorGridSelections editorGridSelections;
    // �ΏۃO���b�h�I�u�W�F�N�g�̃o�b�t�@
    private Vector3 transformBuffer;

    // ���{��̃y�[�W�ɂȂ�悤��URL�쐬
    private string url = "https://github.com/Lazrite/VoxelEdit/tree/master";

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
        // �E�B���h�E���A�N�e�B�u�ɂȂ�����Z���N�^�̃f���Q�[�g�o�^
        editorGridSelections = new EditorGridSelections();

        SceneView.duringSceneGui -= editorGridSelections.OnScene;
        SceneView.duringSceneGui += editorGridSelections.OnScene;
        editorGridSelections.StartSelectProcess();
    }

    private void OnDisable()
    {
        // �E�B���h�E����A�N�e�B�u�ɂȂ�����f���Q�[�g����
        SceneView.duringSceneGui -= editorGridSelections.OnScene;
    }


    /// <summary>
    /// �E�B���h�E���J���Ă���Ƃ��ɉ�ʂɕ\���������v�f�������ɏ���
    /// </summary>
    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();

        using (new EditorGUILayout.HorizontalScope())
        {

            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField("�I�����[�h", GUILayout.Width(64));

                if (editorGridSelections.selectMode != OperationMode.OperationClick)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/ClickDown.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�N���b�N���[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OperationClick;
                        Visualizer.DisableRenderer();
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/ClickDown_selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�N���b�N���[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OperationClick;
                        Visualizer.DisableRenderer();
                    }
                }

                if (editorGridSelections.selectMode != OperationMode.OperationDragFree)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/DragDown.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�t���[�h���b�O���[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OperationDragFree;
                        Visualizer.DisableRenderer();
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/DragDown_selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�t���[�h���b�O���[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OperationDragFree;
                        Visualizer.DisableRenderer();
                    }
                }

                if (editorGridSelections.selectMode != OperationMode.OperationRange)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/RangeSelection.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�͈̓h���b�O���[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OperationRange;
                        Visualizer.DisableRenderer();
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/RangeSelection_selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�͈̓h���b�O���[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.selectMode = OperationMode.OperationRange;
                        Visualizer.DisableRenderer();
                    }
                }
            }

            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField("�c�[�����[�h", GUILayout.Width(64));

                if (editorGridSelections.toolMode != ToolMode.ToolPlace)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/WriteMode.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�ݒu���[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.toolMode = ToolMode.ToolPlace;
                        Visualizer.DisableRenderer();
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/WriteMode_Selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�ݒu���[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.toolMode = ToolMode.ToolPlace;
                        Visualizer.DisableRenderer();
                    }
                }

                if (editorGridSelections.toolMode != ToolMode.ToolErase)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/EraseMode.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�������[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.toolMode = ToolMode.ToolErase;
                        Visualizer.DisableRenderer();
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/EraseMode_Selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�������[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.toolMode = ToolMode.ToolErase;
                        Visualizer.DisableRenderer();
                    }
                }

                if (editorGridSelections.toolMode != ToolMode.ToolReplace)
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/ReplacementMode.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�u�����[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.toolMode = ToolMode.ToolReplace;
                        Visualizer.DisableRenderer();
                    }
                }
                else
                {
                    texture = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorTexture/ReplacementMode_Selected.png",
                        typeof(Texture2D)) as Texture2D;
                    if (GUILayout.Button(new GUIContent(texture, "�u�����[�h"), GUILayout.Width(64), GUILayout.Height(64)))
                    {
                        editorGridSelections.toolMode = ToolMode.ToolReplace;
                        Visualizer.DisableRenderer();
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

                    if (((GameObject)gridObject).GetComponent<EditorGridField>().lineSize <= 0)
                    {
                        ((GameObject)gridObject).GetComponent<EditorGridField>().lineSize = 0.01f;
                    }

                    // �O���b�h���S�����{�^��
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

                    if (GUILayout.Button(new GUIContent("�O���b�h���S����"), style))
                    {
                        if (EditorUtility.DisplayDialog("�O���b�h�����S�ɏ������悤�Ƃ��Ă��܂��I", "��������s����ƃO���b�h���̃I�u�W�F�N�g�͂��ׂď�������A�����o���܂���I",
                                "���s", "�L�����Z��"))
                        {
                            ((GameObject)gridObject).GetComponent<EditorGridField>().ClearGrid();
                            ((GameObject)gridObject).GetComponent<EditorGridField>().InstantiateGridField();
                        }
                    }
                }
            }
        }

        // ��������{�^��
        if (GUILayout.Button(new GUIContent("������@�ɂ��Ă͂�����")))
        {
            Process.Start("ReadMe.html");
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

    }

    private void OnInspectorUpdate()
    {
        if (((GameObject)gridObject) != null)
        {
            if (transformBuffer != ((GameObject)gridObject).transform.position)
            {
                ((GameObject)gridObject).GetComponent<EditorGridField>().ReCalculationGridPos();
                transformBuffer = ((GameObject)gridObject).transform.position;
            }
        }
    }


}

#endif // UNITY_EDITOR