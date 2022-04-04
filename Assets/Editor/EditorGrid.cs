using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(EditorGrid))]
public class EditorGrid : Editor
{
    //グリッドの幅
    public float GRID = 0.32f;
    public Vector3Int gridSize = new Vector3Int(5, 5, 5);

    void OnSceneGUI()
    {
        // 選択中のオブジェクト位置
        Vector3 targetPos = Selection.activeGameObject.transform.position;

        //グリッドの色
        Color color = Color.cyan * 0.7f;

        //グリッドの中心座標
        Vector3 orig = Vector3.zero;

        int num = 10;
        float size = GRID * num;

        //グリッド描画
        // 横棒
        Debug.DrawLine(Vector3.zero + targetPos, new Vector3(gridSize.x + targetPos.x, 0 + targetPos.y, 0 + targetPos.z));
        Debug.DrawLine(new Vector3(0 + targetPos.x, gridSize.y + targetPos.y, 0 + targetPos.z), new Vector3(gridSize.x + targetPos.x, gridSize.y + targetPos.y, 0 + targetPos.z));
        Debug.DrawLine(new Vector3(0 + targetPos.x, 0 + targetPos.y, gridSize.z + targetPos.z), new Vector3(gridSize.x + targetPos.x, 0 + targetPos.y, gridSize.z + targetPos.z));
        Debug.DrawLine(new Vector3(0 + targetPos.x, gridSize.y + targetPos.y, gridSize.z + targetPos.z), new Vector3(gridSize.x + targetPos.x, gridSize.y + targetPos.y, gridSize.z + targetPos.z));
        // 縦棒
        Debug.DrawLine(Vector3.zero + targetPos, new Vector3(0 + targetPos.x, gridSize.y + targetPos.y, 0 + targetPos.z));
        Debug.DrawLine(new Vector3(gridSize.x + targetPos.x, 0 + targetPos.y, 0 + targetPos.z), new Vector3(gridSize.x + targetPos.x, gridSize.y + targetPos.y, 0 + targetPos.z));
        Debug.DrawLine(new Vector3(0 + targetPos.x, 0 + targetPos.y, gridSize.z + targetPos.z), new Vector3(0 + targetPos.x, gridSize.y + targetPos.y, gridSize.z + targetPos.z));
        Debug.DrawLine(new Vector3(gridSize.x + targetPos.x, 0 + targetPos.y, gridSize.z + targetPos.z), new Vector3(gridSize.x + targetPos.x, gridSize.y + targetPos.y, gridSize.z + targetPos.z));
        // 奥行棒
        Debug.DrawLine(Vector3.zero + targetPos, new Vector3(0 + targetPos.x, 0 + targetPos.y, gridSize.z + targetPos.z));
        Debug.DrawLine(new Vector3(gridSize.x + targetPos.x, 0 + targetPos.y, 0 + targetPos.z), new Vector3(gridSize.x + targetPos.x, 0 + targetPos.y, gridSize.z + targetPos.z));
        Debug.DrawLine(new Vector3(0 + targetPos.x, gridSize.y + targetPos.y, 0 + targetPos.z), new Vector3(0 + targetPos.x, gridSize.y + targetPos.y, gridSize.z + targetPos.z));
        Debug.DrawLine(new Vector3(gridSize.x + targetPos.x, gridSize.y + targetPos.y, 0 + targetPos.z), new Vector3(gridSize.x + targetPos.x, gridSize.y + targetPos.y, gridSize.z + targetPos.z));

        //Sceneビュー更新
        EditorUtility.SetDirty(target);
    }

    //フォーカスが外れたときに実行
    void OnDisable()
    {
        //Sceneビュー更新
        EditorUtility.SetDirty(target);
    }
}
#endif //UNITY_EDITOR