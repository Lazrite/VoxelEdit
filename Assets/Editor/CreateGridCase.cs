using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class CreateGridCase : MonoBehaviour
{
    // MenuItem�̊K�w�w��
    [MenuItem("GameObject/Grid Object", false, 10)]
    private static void CreateCustomGameObject(MenuCommand menuCommand)
    {
        // ���V�[�����̃O���b�h�I�u�W�F�N�g����
        int gridObjInCurrentScene = new int();
        // ���V�[�����̃O���b�h�I�u�W�F�N�g
        Object[] all = Resources.FindObjectsOfTypeAll(typeof(GameObject));

        // �I�u�W�F�N�g��{�����A���O�d�������݂��Ă��邩�𔻒�
        foreach (GameObject objall in all)
        {
            if (objall.activeInHierarchy)
            {
                if (Regex.IsMatch(objall.name, "GridObject (.)") || objall.name == "GridObject")
                {
                    gridObjInCurrentScene++;
                }
            }
        }

        // Create a custom game object
        GameObject go = new GameObject("GridObject " + "(" + gridObjInCurrentScene + ")");

        // �O���b�h�I�u�W�F�N�g���\������R���|�[�l���g��Add
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.AddComponent<EditorGridField>();
        go.AddComponent<EditorGenerateAblePlacement>();

        // �r���h�C���}�e���A���擾
        go.GetComponent<EditorGridField>().material = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Line.mat");

        // �f�t�H���g�̒l���Z�b�g
        go.GetComponent<EditorGridField>().size = new Vector3Int(5, 5, 5);
        go.GetComponent<EditorGridField>().lineSize = 0.02f;
        go.GetComponent<EditorGridField>().ClearGrid();
        go.GetComponent<EditorGridField>().InstantiateGridField();


        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
}
