using UnityEngine;

public class DrawNormal : MonoBehaviour
{
    private void OnDrawGizmos()
    {

        var preColor = Gizmos.color;
        var preMatrix = Gizmos.matrix;
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;

        // –@ü•ûŒü‚Éƒ‰ƒCƒ“‚ğ•`‰æ
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
