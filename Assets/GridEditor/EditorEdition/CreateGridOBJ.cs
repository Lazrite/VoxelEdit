using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EditorGridField))]
public class CreateGridOBJ : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGridField ed = target as EditorGridField;

        if (GUILayout.Button("�O���b�h����"))
        {
            ed.ClearGrid();
            ed.InstantiateGridField();

        }

        EditorGUI.BeginChangeCheck();

        ed.size = EditorGUILayout.Vector3IntField("�T�C�Y", ed.size);
        ed.lineSize = EditorGUILayout.FloatField("���̑���", ed.lineSize);
        ed.areaGameObject = EditorGUILayout.ObjectField("�ݒu�\�I�u�W�F�N�g", ed.areaGameObject, typeof(GameObject), true) as GameObject;

        if (ed.size.x < 1) ed.size.x = 1;
        if (ed.size.y < 1) ed.size.y = 1;
        if (ed.size.z < 1) ed.size.z = 1;

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(ed, "Clear Grid");
            ed.ClearGrid();
            ed.InstantiateGridField();
        }

    }

    private void OnDisable()
    {
        EditorUtility.SetDirty(target);
    }
}