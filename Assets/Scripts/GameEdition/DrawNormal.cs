using UnityEngine;

/// <summary>
/// 法線表示クラス(激重)
/// </summary>
public class DrawNormal : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        var preColor = Gizmos.color;
        var preMatrix = Gizmos.matrix;
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;

        // 法線方向にラインを描画
        var mesh = GetComponent<MeshFilter>().sharedMesh;
        for (int i = 0; i < mesh.normals.Length; i++)
        {
            var from = mesh.vertices[i];
            var to = from + mesh.normals[i];
            Gizmos.DrawLine(from, to);
        }

        Gizmos.color = preColor;
        Gizmos.matrix = preMatrix;
    }
}
