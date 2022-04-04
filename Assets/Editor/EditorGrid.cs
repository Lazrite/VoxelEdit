using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(EditorGrid))]
public class EditorGrid : Editor
{
    //�O���b�h�̕�
    public float GRID = 0.32f;
    public Vector3Int gridSize = new Vector3Int(5, 5, 5);

    void OnSceneGUI()
    {
        // �I�𒆂̃I�u�W�F�N�g�ʒu
        Vector3 targetPos = Selection.activeGameObject.transform.position;

        //�O���b�h�̐F
        Color color = Color.cyan * 0.7f;

        //�O���b�h�̒��S���W
        Vector3 orig = Vector3.zero;

        int num = 10;
        float size = GRID * num;

        //�O���b�h�`��
        // ���_
        Debug.DrawLine(Vector3.zero + targetPos, new Vector3(gridSize.x + targetPos.x, 0 + targetPos.y, 0 + targetPos.z));
        Debug.DrawLine(new Vector3(0 + targetPos.x, gridSize.y + targetPos.y, 0 + targetPos.z), new Vector3(gridSize.x + targetPos.x, gridSize.y + targetPos.y, 0 + targetPos.z));
        Debug.DrawLine(new Vector3(0 + targetPos.x, 0 + targetPos.y, gridSize.z + targetPos.z), new Vector3(gridSize.x + targetPos.x, 0 + targetPos.y, gridSize.z + targetPos.z));
        Debug.DrawLine(new Vector3(0 + targetPos.x, gridSize.y + targetPos.y, gridSize.z + targetPos.z), new Vector3(gridSize.x + targetPos.x, gridSize.y + targetPos.y, gridSize.z + targetPos.z));
        // �c�_
        Debug.DrawLine(Vector3.zero + targetPos, new Vector3(0 + targetPos.x, gridSize.y + targetPos.y, 0 + targetPos.z));
        Debug.DrawLine(new Vector3(gridSize.x + targetPos.x, 0 + targetPos.y, 0 + targetPos.z), new Vector3(gridSize.x + targetPos.x, gridSize.y + targetPos.y, 0 + targetPos.z));
        Debug.DrawLine(new Vector3(0 + targetPos.x, 0 + targetPos.y, gridSize.z + targetPos.z), new Vector3(0 + targetPos.x, gridSize.y + targetPos.y, gridSize.z + targetPos.z));
        Debug.DrawLine(new Vector3(gridSize.x + targetPos.x, 0 + targetPos.y, gridSize.z + targetPos.z), new Vector3(gridSize.x + targetPos.x, gridSize.y + targetPos.y, gridSize.z + targetPos.z));
        // ���s�_
        Debug.DrawLine(Vector3.zero + targetPos, new Vector3(0 + targetPos.x, 0 + targetPos.y, gridSize.z + targetPos.z));
        Debug.DrawLine(new Vector3(gridSize.x + targetPos.x, 0 + targetPos.y, 0 + targetPos.z), new Vector3(gridSize.x + targetPos.x, 0 + targetPos.y, gridSize.z + targetPos.z));
        Debug.DrawLine(new Vector3(0 + targetPos.x, gridSize.y + targetPos.y, 0 + targetPos.z), new Vector3(0 + targetPos.x, gridSize.y + targetPos.y, gridSize.z + targetPos.z));
        Debug.DrawLine(new Vector3(gridSize.x + targetPos.x, gridSize.y + targetPos.y, 0 + targetPos.z), new Vector3(gridSize.x + targetPos.x, gridSize.y + targetPos.y, gridSize.z + targetPos.z));

        //Scene�r���[�X�V
        EditorUtility.SetDirty(target);
    }

    //�t�H�[�J�X���O�ꂽ�Ƃ��Ɏ��s
    void OnDisable()
    {
        //Scene�r���[�X�V
        EditorUtility.SetDirty(target);
    }
}
#endif //UNITY_EDITOR