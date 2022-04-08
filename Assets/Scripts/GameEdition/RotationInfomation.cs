using UnityEngine;

public class RotationInfomation : MonoBehaviour
{
    [SerializeField] private GridManager gridManager = default;
    [SerializeField] private GameObject emptyObj = default;
    [SerializeField] private Material[] materials = default;

    private GameObject[] InfoObjects;


    private Vector3[] verts;    //ポリゴンの頂点を入れる
    private int[] triangles;    //三角形を描く際に、頂点の描画順を指定する



    private Material material;
    private Vector3Int size;
    private float lineSize;

    // 配列のバッファ
    private int arrayBuffer = 0;

    // Start is called before the first frame update
    private void Start()
    {
        InfoObjects = new GameObject[12];

        InfoObjects[0] = Instantiate(emptyObj, new Vector3(gameObject.transform.position.x - 0.5f, gameObject.transform.position.y - 0.5f, gameObject.transform.position.z), Quaternion.identity, gameObject.transform);
        InfoObjects[1] = Instantiate(emptyObj, new Vector3(gameObject.transform.position.x + 0.5f, gameObject.transform.position.y - 0.5f, gameObject.transform.position.z), Quaternion.identity, gameObject.transform);
        InfoObjects[2] = Instantiate(emptyObj, new Vector3(gameObject.transform.position.x - 0.5f, gameObject.transform.position.y + 0.5f, gameObject.transform.position.z), Quaternion.identity, gameObject.transform);
        InfoObjects[3] = Instantiate(emptyObj, new Vector3(gameObject.transform.position.x + 0.5f, gameObject.transform.position.y + 0.5f, gameObject.transform.position.z), Quaternion.identity, gameObject.transform);
        InfoObjects[4] = Instantiate(emptyObj, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.5f, gameObject.transform.position.z - 0.5f), Quaternion.Euler(0, 90, 0), gameObject.transform);
        InfoObjects[5] = Instantiate(emptyObj, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.5f, gameObject.transform.position.z - 0.5f), Quaternion.Euler(0, 90, 0), gameObject.transform);
        InfoObjects[6] = Instantiate(emptyObj, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.5f, gameObject.transform.position.z + 0.5f), Quaternion.Euler(0, 90, 0), gameObject.transform);
        InfoObjects[7] = Instantiate(emptyObj, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.5f, gameObject.transform.position.z + 0.5f), Quaternion.Euler(0, 90, 0), gameObject.transform);
        InfoObjects[8] = Instantiate(emptyObj, new Vector3(gameObject.transform.position.x - 0.5f, gameObject.transform.position.y, gameObject.transform.position.z - 0.5f), Quaternion.Euler(90, 0, 0), gameObject.transform);
        InfoObjects[9] = Instantiate(emptyObj, new Vector3(gameObject.transform.position.x - 0.5f, gameObject.transform.position.y, gameObject.transform.position.z + 0.5f), Quaternion.Euler(90, 0, 0), gameObject.transform);
        InfoObjects[10] = Instantiate(emptyObj, new Vector3(gameObject.transform.position.x + 0.5f, gameObject.transform.position.y, gameObject.transform.position.z - 0.5f), Quaternion.Euler(90, 0, 0), gameObject.transform);
        InfoObjects[11] = Instantiate(emptyObj, new Vector3(gameObject.transform.position.x + 0.5f, gameObject.transform.position.y, gameObject.transform.position.z + 0.5f), Quaternion.Euler(90, 0, 0), gameObject.transform);

        InfoObjects[0].GetComponent<MeshRenderer>().material = materials[0];
        InfoObjects[4].GetComponent<MeshRenderer>().material = materials[1];
        InfoObjects[8].GetComponent<MeshRenderer>().material = materials[2];

        for (int i = 0; i < InfoObjects.Length; i++)
        {
            InfoObjects[i].layer = 7;
        }

    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void CreateInfo()
    {
        material = gridManager.material;
        size = new Vector3Int(1, 1, 1);
        lineSize = gridManager.lineSize;

        for (int i = 0; i < InfoObjects.Length; i++)
        {
            //新しいMeshを作成
            Mesh mesh = new Mesh();

            //頂点の番号をsize分確保、縦横の線が一本ずつなくなるので+2を入れる、一本の線は頂点6つで表示させるので*6
            triangles = new int[12 * 6];
            //頂点の座標をsize分確保
            verts = new Vector3[12 * 6];

            //頂点番号を割り当て
            for (int j = 0; j < triangles.Length; j++)
            {
                triangles[j] = j;
            }

            //何回for分が回ったかをカウントさせる
            int x = 0, y = 0;

            switch (i)
            {
                // 横線
                case 0:
                    verts[arrayBuffer] = new Vector3(x, 0, 0);
                    verts[arrayBuffer + 1] = new Vector3(x, size.y, 0);
                    verts[arrayBuffer + 2] = new Vector3(lineSize + x, size.y, 0);
                    verts[arrayBuffer + 3] = new Vector3(lineSize + x, size.y, 0);
                    verts[arrayBuffer + 4] = new Vector3(lineSize + x, 0, 0);
                    verts[arrayBuffer + 5] = new Vector3(x, 0, 0);
                    break;
                case 1:
                    verts[arrayBuffer + 6] = new Vector3(size.x, 0, 0);
                    verts[arrayBuffer + 7] = new Vector3(size.x, size.y, 0);
                    verts[arrayBuffer + 8] = new Vector3(lineSize + size.x, size.y, 0);
                    verts[arrayBuffer + 9] = new Vector3(lineSize + size.x, size.y, 0);
                    verts[arrayBuffer + 10] = new Vector3(lineSize + size.x, 0, 0);
                    verts[arrayBuffer + 11] = new Vector3(size.x, 0, 0);
                    break;
                case 2:
                    verts[arrayBuffer + 12] = new Vector3(x, 0, size.z);
                    verts[arrayBuffer + 13] = new Vector3(x, size.y, size.z);
                    verts[arrayBuffer + 14] = new Vector3(lineSize + x, size.y, size.z);
                    verts[arrayBuffer + 15] = new Vector3(lineSize + x, size.y, size.z);
                    verts[arrayBuffer + 16] = new Vector3(lineSize + x, 0, size.z);
                    verts[arrayBuffer + 17] = new Vector3(x, 0, size.z);
                    break;
                case 3:
                    verts[arrayBuffer + 18] = new Vector3(size.x, 0, size.z);
                    verts[arrayBuffer + 19] = new Vector3(size.x, size.y, size.z);
                    verts[arrayBuffer + 20] = new Vector3(lineSize + size.x, size.y, size.z);
                    verts[arrayBuffer + 21] = new Vector3(lineSize + size.x, size.y, size.z);
                    verts[arrayBuffer + 22] = new Vector3(lineSize + size.x, 0, size.z);
                    verts[arrayBuffer + 23] = new Vector3(size.x, 0, size.z);
                    break;
                case 4:
                    verts[arrayBuffer] = new Vector3(0, y, 0);
                    verts[arrayBuffer + 1] = new Vector3(size.x + lineSize, y, 0);
                    verts[arrayBuffer + 2] = new Vector3(0, y - lineSize, 0);
                    verts[arrayBuffer + 3] = new Vector3(size.x + lineSize, y, 0);
                    verts[arrayBuffer + 4] = new Vector3(size.x + lineSize, y - lineSize, 0);
                    verts[arrayBuffer + 5] = new Vector3(0, y - lineSize, 0);
                    break;
                case 5:
                    verts[arrayBuffer + 6] = new Vector3(0, size.y, 0);
                    verts[arrayBuffer + 7] = new Vector3(size.x + lineSize, size.y, 0);
                    verts[arrayBuffer + 8] = new Vector3(0, size.y - lineSize, 0);
                    verts[arrayBuffer + 9] = new Vector3(size.x + lineSize, size.y, 0);
                    verts[arrayBuffer + 10] = new Vector3(size.x + lineSize, size.y - lineSize, 0);
                    verts[arrayBuffer + 11] = new Vector3(0, size.y - lineSize, 0);
                    break;
                case 6:
                    verts[arrayBuffer + 12] = new Vector3(0, y, size.z);
                    verts[arrayBuffer + 13] = new Vector3(size.x + lineSize, y, size.z);
                    verts[arrayBuffer + 14] = new Vector3(0, y - lineSize, size.z);
                    verts[arrayBuffer + 15] = new Vector3(size.x + lineSize, y, size.z);
                    verts[arrayBuffer + 16] = new Vector3(size.x + lineSize, y - lineSize, size.z);
                    verts[arrayBuffer + 17] = new Vector3(0, y - lineSize, size.z);
                    break;
                case 7:
                    verts[arrayBuffer + 18] = new Vector3(0, size.y, size.z);
                    verts[arrayBuffer + 19] = new Vector3(size.x + lineSize, size.y, size.z);
                    verts[arrayBuffer + 20] = new Vector3(0, size.y - lineSize, size.z);
                    verts[arrayBuffer + 21] = new Vector3(size.x + lineSize, size.y, size.z);
                    verts[arrayBuffer + 22] = new Vector3(size.x + lineSize, size.y - lineSize, size.z);
                    verts[arrayBuffer + 23] = new Vector3(0, size.y - lineSize, size.z);
                    break;
                case 8:
                    verts[arrayBuffer] = new Vector3(x, y, 0);
                    verts[arrayBuffer + 1] = new Vector3(x, y, size.z);
                    verts[arrayBuffer + 2] = new Vector3(x, y - lineSize, 0);
                    verts[arrayBuffer + 3] = new Vector3(x, y, size.z);
                    verts[arrayBuffer + 4] = new Vector3(x, y - lineSize, size.z);
                    verts[arrayBuffer + 5] = new Vector3(x, y - lineSize, 0);
                    break;
                case 9:
                    verts[arrayBuffer + 6] = new Vector3(size.x, size.y, 0);
                    verts[arrayBuffer + 7] = new Vector3(size.x, size.y, size.z);
                    verts[arrayBuffer + 8] = new Vector3(size.x, size.y - lineSize, 0);
                    verts[arrayBuffer + 9] = new Vector3(size.x, size.y, size.z);
                    verts[arrayBuffer + 10] = new Vector3(size.x, size.y - lineSize, size.z);
                    verts[arrayBuffer + 11] = new Vector3(size.x, size.y - lineSize, 0);
                    break;
                case 10:
                    verts[arrayBuffer + 12] = new Vector3(x, size.y, 0);
                    verts[arrayBuffer + 13] = new Vector3(x, size.y, size.z);
                    verts[arrayBuffer + 14] = new Vector3(x, size.y - lineSize, 0);
                    verts[arrayBuffer + 15] = new Vector3(x, size.y, size.z);
                    verts[arrayBuffer + 16] = new Vector3(x, size.y - lineSize, size.z);
                    verts[arrayBuffer + 17] = new Vector3(x, size.y - lineSize, 0);
                    break;
                case 11:
                    verts[arrayBuffer + 18] = new Vector3(size.x, y, 0);
                    verts[arrayBuffer + 19] = new Vector3(size.x, y, size.z);
                    verts[arrayBuffer + 20] = new Vector3(size.x, y - lineSize, 0);
                    verts[arrayBuffer + 21] = new Vector3(size.x, y, size.z);
                    verts[arrayBuffer + 22] = new Vector3(size.x, y - lineSize, size.z);
                    verts[arrayBuffer + 23] = new Vector3(size.x, y - lineSize, 0);
                    break;
            }

            arrayBuffer += 6;

            //作った頂点番号、座標データを作成したmeshに追加
            mesh.vertices = verts;
            mesh.triangles = triangles;
            arrayBuffer = 0;

            //再計算()
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            //再計算後に完成したMeshを追加
            InfoObjects[i].GetComponent<MeshFilter>().mesh = mesh;

            //設定したMaterialを反映

            switch (i)
            {
                case 0:
                    InfoObjects[i].GetComponent<MeshRenderer>().material = materials[0];
                    break;
                case 4:
                    InfoObjects[i].GetComponent<MeshRenderer>().material = materials[1];
                    break;
                case 8:
                    InfoObjects[i].GetComponent<MeshRenderer>().material = materials[2];
                    break;
                default:
                    InfoObjects[i].GetComponent<MeshRenderer>().material = material;
                    break;
            }

        }
    }
}
