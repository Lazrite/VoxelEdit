using UnityEngine;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class GridView : MonoBehaviour
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
        triangles = new int[((size.x + 1) * (size.z + 1) * 6) + ((size.y + 1) * (size.z + 1) * 6) + ((size.y + 1) * (size.x + 1) * 6)];
        //頂点の座標をsize分確保
        verts = new Vector3[((size.x + 1) * (size.z + 1) * 6) + ((size.y + 1) * (size.z + 1) * 6) + ((size.y + 1) * (size.x + 1) * 6)];

        //頂点番号を割り当て
        for (int i = 0; i < triangles.Length; i++)
        {
            triangles[i] = i;
        }

        //何回for分が回ったかをカウントさせる
        int x = 0, y = 0, z = 0;

        for (int j = 0; j < size.z + 1; j++)
        {
            //縦線
            for (int i = 0; i < (size.x + 1); i++)
            {
                verts[arrayBuffer] = new Vector3(x, 0, j);
                verts[arrayBuffer + 1] = new Vector3(x, size.y, j);
                verts[arrayBuffer + 2] = new Vector3(lineSize + x, size.y, j);
                verts[arrayBuffer + 3] = new Vector3(lineSize + x, size.y, j);
                verts[arrayBuffer + 4] = new Vector3(lineSize + x, 0, j);
                verts[arrayBuffer + 5] = new Vector3(x, 0, j);
                x++;
                arrayBuffer += 6;
            }
            x = 0;
        }

        for (int j = 0; j < size.z + 1; j++)
        {
            //縦線
            for (int i = 0; i < (size.y + 1); i++)
            {
                verts[arrayBuffer] = new Vector3(0, y, j);
                verts[arrayBuffer + 1] = new Vector3(size.x + lineSize, y, j);
                verts[arrayBuffer + 2] = new Vector3(0, y - lineSize, j);
                verts[arrayBuffer + 3] = new Vector3(size.x + lineSize, y, j);
                verts[arrayBuffer + 4] = new Vector3(size.x + lineSize, y - lineSize, j);
                verts[arrayBuffer + 5] = new Vector3(0, y - lineSize, j);
                y++;
                arrayBuffer += 6;
            }
            y = 0;
        }

        for (int k = 0; k < (size.x + 1); k++)
        {
            //縦線
            for (int i = 0; i < (size.y + 1); i++)
            {
                verts[arrayBuffer] = new Vector3(x, y, 0);
                verts[arrayBuffer + 1] = new Vector3(x, y, size.z);
                verts[arrayBuffer + 2] = new Vector3(x, y - lineSize, 0);
                verts[arrayBuffer + 3] = new Vector3(x, y, size.z);
                verts[arrayBuffer + 4] = new Vector3(x, y - lineSize, size.z);
                verts[arrayBuffer + 5] = new Vector3(x, y - lineSize, 0);
                y++;
                arrayBuffer += 6;
            }
            x++;
            y = 0;
        }
        x = 0;

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
        if (size.x < 1) size.x = 1;
        if (size.y < 1) size.y = 1;
        if (size.z < 1) size.z = 1;
    }
}