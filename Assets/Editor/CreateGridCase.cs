using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class CreateGridCase : MonoBehaviour
{
    // MenuItemの階層指定
    [MenuItem("GameObject/Grid Object", false, 10)]
    private static void CreateCustomGameObject(MenuCommand menuCommand)
    {
        // 現シーン内のグリッドオブジェクト総数
        int gridObjInCurrentScene = new int();
        // 現シーン内のグリッドオブジェクト
        Object[] all = Resources.FindObjectsOfTypeAll(typeof(GameObject));

        // オブジェクトを捜査し、名前重複が存在しているかを判定
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

        // グリッドオブジェクトを構成するコンポーネントをAdd
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.AddComponent<EditorGridField>();
        go.AddComponent<EditorGenerateAblePlacement>();

        // ビルドインマテリアル取得
        go.GetComponent<EditorGridField>().material = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Line.mat");

        // デフォルトの値をセット
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
