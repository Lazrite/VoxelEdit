#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EditorGridField))]
public class CreateGridOBJ : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGridField ed = target as EditorGridField;

        if (GUILayout.Button("グリッド生成"))
        {
            if (ed != null)
            {
                Undo.RecordObject(ed, "Create Grid");
                ed.ClearGrid();
                ed.InstantiateGridField();
            }
        }

        EditorGUI.BeginChangeCheck();

        ed.size = EditorGUILayout.Vector3IntField("サイズ", ed.size);
        ed.lineSize = EditorGUILayout.FloatField("線の太さ", ed.lineSize);

        // 不正値入力防止
        if (ed.size.x < 1)
        {
            ed.size.x = 1;
        }

        if (ed.size.y < 1)
        {
            ed.size.y = 1;
        }

        if (ed.size.z < 1)
        {
            ed.size.z = 1;
        }

        if (EditorGUI.EndChangeCheck())
        {
            // 変更が行われたらグリッドを完全消去
            // TODO: 保持するように！！
            Undo.RecordObject(ed, "Clear Grid");
            ed.ClearGrid();
            ed.InstantiateGridField();
        }

        EditorGUI.BeginChangeCheck();

        ed.transform.position = EditorGUILayout.Vector3Field("オブジェクト位置", ed.transform.position);

    }

    private void OnDisable()
    {
        EditorUtility.SetDirty(target);
    }
}

#endif // UNITY EDITOR