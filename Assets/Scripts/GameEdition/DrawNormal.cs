using UnityEngine;

public class DrawNormal : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

    }

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
