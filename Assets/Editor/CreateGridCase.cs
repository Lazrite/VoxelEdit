using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateGridCase : MonoBehaviour
{
    [MenuItem("GameObject/Grid Object", false, 10)]
    static void CreateCustomGameObject(MenuCommand menuCommand)
    {
        // Create a custom game object
        GameObject go = new GameObject("GridObject");

        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.AddComponent<EditorGridField>();
        go.AddComponent<EditorGenerateAblePlacement>();

        go.GetComponent<EditorGridField>().material = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Line.mat");

        go.GetComponent<EditorGridField>().ClearGrid();
        go.GetComponent<EditorGridField>().InstantiateGridField();

        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
}
