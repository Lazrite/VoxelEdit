using UnityEngine;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class GridFlameView : MonoBehaviour
{
    private Vector3[] verts;    //ポリゴンの頂点を入れる
    private int[] triangles;    //三角形を描く際に、頂点の描画順を指定する
    private GridManager gridManager;

    private Material material;
    private Vector3Int size;
    private float lineSize;

    // 配列のバッファ
    private int arrayBuffer = 0;

    private void Start()
    {
        gridManager = transform.root.GetComponent<GridManager>();
        material = gridManager.material;
        size = gridManager.size;
        lineSize = gridManager.lineSize;
    }

    // Update is called once per frame
    private void Update()
    {
        if (gridManager.OnChangeGridInfo())
        {
            CreateGlid();
        }
    }

    private void CreateGlid()
    {
        material = gridManager.material;
        size = gridManager.size;
        lineSize = gridManager.lineSize;

        //新しいMeshを作成
        Mesh mesh = new Mesh();

        //頂点の番号をsize分確保、縦横の線が一本ずつなくなるので+2を入れる、一本の線は頂点6つで表示させるので*6
        triangles = new int[12 * 6];
        //頂点の座標をsize分確保
        verts = new Vector3[12 * 6];

        //頂点番号を割り当て
        for (int i = 0; i < triangles.Length; i++)
        {
            triangles[i] = i;
        }

        //何回for分が回ったかをカウントさせる
        int x = 0, y = 0, z = 0;

        // 横線

        verts[arrayBuffer] = new Vector3(x, 0, 0);
        verts[arrayBuffer + 1] = new Vector3(x, size.y, 0);
        verts[arrayBuffer + 2] = new Vector3(lineSize + x, size.y, 0);
        verts[arrayBuffer + 3] = new Vector3(lineSize + x, size.y, 0);
        verts[arrayBuffer + 4] = new Vector3(lineSize + x, 0, 0);
        verts[arrayBuffer + 5] = new Vector3(x, 0, 0);

        verts[arrayBuffer + 6] = new Vector3(size.x, 0, 0);
        verts[arrayBuffer + 7] = new Vector3(size.x, size.y, 0);
        verts[arrayBuffer + 8] = new Vector3(lineSize + size.x, size.y, 0);
        verts[arrayBuffer + 9] = new Vector3(lineSize + size.x, size.y, 0);
        verts[arrayBuffer + 10] = new Vector3(lineSize + size.x, 0, 0);
        verts[arrayBuffer + 11] = new Vector3(size.x, 0, 0);

        verts[arrayBuffer + 12] = new Vector3(x, 0, size.z);
        verts[arrayBuffer + 13] = new Vector3(x, size.y, size.z);
        verts[arrayBuffer + 14] = new Vector3(lineSize + x, size.y, size.z);
        verts[arrayBuffer + 15] = new Vector3(lineSize + x, size.y, size.z);
        verts[arrayBuffer + 16] = new Vector3(lineSize + x, 0, size.z);
        verts[arrayBuffer + 17] = new Vector3(x, 0, size.z);

        verts[arrayBuffer + 18] = new Vector3(size.x, 0, size.z);
        verts[arrayBuffer + 19] = new Vector3(size.x, size.y, size.z);
        verts[arrayBuffer + 20] = new Vector3(lineSize + size.x, size.y, size.z);
        verts[arrayBuffer + 21] = new Vector3(lineSize + size.x, size.y, size.z);
        verts[arrayBuffer + 22] = new Vector3(lineSize + size.x, 0, size.z);
        verts[arrayBuffer + 23] = new Vector3(size.x, 0, size.z);

        arrayBuffer += 24;

        //縦線

        verts[arrayBuffer] = new Vector3(0, y, 0);
        verts[arrayBuffer + 1] = new Vector3(size.x + lineSize, y, 0);
        verts[arrayBuffer + 2] = new Vector3(0, y - lineSize, 0);
        verts[arrayBuffer + 3] = new Vector3(size.x + lineSize, y, 0);
        verts[arrayBuffer + 4] = new Vector3(size.x + lineSize, y - lineSize, 0);
        verts[arrayBuffer + 5] = new Vector3(0, y - lineSize, 0);

        verts[arrayBuffer + 6] = new Vector3(0, size.y, 0);
        verts[arrayBuffer + 7] = new Vector3(size.x + lineSize, size.y, 0);
        verts[arrayBuffer + 8] = new Vector3(0, size.y - lineSize, 0);
        verts[arrayBuffer + 9] = new Vector3(size.x + lineSize, size.y, 0);
        verts[arrayBuffer + 10] = new Vector3(size.x + lineSize, size.y - lineSize, 0);
        verts[arrayBuffer + 11] = new Vector3(0, size.y - lineSize, 0);

        verts[arrayBuffer + 12] = new Vector3(0, y, size.z);
        verts[arrayBuffer + 13] = new Vector3(size.x + lineSize, y, size.z);
        verts[arrayBuffer + 14] = new Vector3(0, y - lineSize, size.z);
        verts[arrayBuffer + 15] = new Vector3(size.x + lineSize, y, size.z);
        verts[arrayBuffer + 16] = new Vector3(size.x + lineSize, y - lineSize, size.z);
        verts[arrayBuffer + 17] = new Vector3(0, y - lineSize, size.z);

        verts[arrayBuffer + 18] = new Vector3(0, size.y, size.z);
        verts[arrayBuffer + 19] = new Vector3(size.x + lineSize, size.y, size.z);
        verts[arrayBuffer + 20] = new Vector3(0, size.y - lineSize, size.z);
        verts[arrayBuffer + 21] = new Vector3(size.x + lineSize, size.y, size.z);
        verts[arrayBuffer + 22] = new Vector3(size.x + lineSize, size.y - lineSize, size.z);
        verts[arrayBuffer + 23] = new Vector3(0, size.y - lineSize, size.z);

        arrayBuffer += 24;

        verts[arrayBuffer] = new Vector3(x, y, 0);
        verts[arrayBuffer + 1] = new Vector3(x, y, size.z);
        verts[arrayBuffer + 2] = new Vector3(x, y - lineSize, 0);
        verts[arrayBuffer + 3] = new Vector3(x, y, size.z);
        verts[arrayBuffer + 4] = new Vector3(x, y - lineSize, size.z);
        verts[arrayBuffer + 5] = new Vector3(x, y - lineSize, 0);

        verts[arrayBuffer + 6] = new Vector3(size.x, size.y, 0);
        verts[arrayBuffer + 7] = new Vector3(size.x, size.y, size.z);
        verts[arrayBuffer + 8] = new Vector3(size.x, size.y - lineSize, 0);
        verts[arrayBuffer + 9] = new Vector3(size.x, size.y, size.z);
        verts[arrayBuffer + 10] = new Vector3(size.x, size.y - lineSize, size.z);
        verts[arrayBuffer + 11] = new Vector3(size.x, size.y - lineSize, 0);

        verts[arrayBuffer + 12] = new Vector3(x, size.y, 0);
        verts[arrayBuffer + 13] = new Vector3(x, size.y, size.z);
        verts[arrayBuffer + 14] = new Vector3(x, size.y - lineSize, 0);
        verts[arrayBuffer + 15] = new Vector3(x, size.y, size.z);
        verts[arrayBuffer + 16] = new Vector3(x, size.y - lineSize, size.z);
        verts[arrayBuffer + 17] = new Vector3(x, size.y - lineSize, 0);

        verts[arrayBuffer + 18] = new Vector3(size.x, y, 0);
        verts[arrayBuffer + 19] = new Vector3(size.x, y, size.z);
        verts[arrayBuffer + 20] = new Vector3(size.x, y - lineSize, 0);
        verts[arrayBuffer + 21] = new Vector3(size.x, y, size.z);
        verts[arrayBuffer + 22] = new Vector3(size.x, y - lineSize, size.z);
        verts[arrayBuffer + 23] = new Vector3(size.x, y - lineSize, 0);

        arrayBuffer += 24;

        //作った頂点番号、座標データを作成したmeshに追加
        mesh.vertices = verts;
        mesh.triangles = triangles;
        arrayBuffer = 0;

        //再計算()
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        //再計算後に完成したMeshを追加
        GetComponent<MeshFilter>().mesh = mesh;
        //設定したMaterialを反映
        GetComponent<MeshRenderer>().material = material;
    }

    private void OnValidate()
    {
        if (size.x < 1)
        {
            size.x = 1;
        }

        if (size.y < 1)
        {
            size.y = 1;
        }

        if (size.z < 1)
        {
            size.z = 1;
        }
    }
}