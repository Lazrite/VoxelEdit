using UnityEngine;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class GridFlameView : MonoBehaviour
{
    private Vector3[] verts;    //�|���S���̒��_������
    private int[] triangles;    //�O�p�`��`���ۂɁA���_�̕`�揇���w�肷��
    private GridManager gridManager;

    private Material material;
    private Vector3Int size;
    private float lineSize;

    // �z��̃o�b�t�@
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

        //�V����Mesh���쐬
        Mesh mesh = new Mesh();

        //���_�̔ԍ���size���m�ہA�c���̐�����{���Ȃ��Ȃ�̂�+2������A��{�̐��͒��_6�ŕ\��������̂�*6
        triangles = new int[12 * 6];
        //���_�̍��W��size���m��
        verts = new Vector3[12 * 6];

        //���_�ԍ������蓖��
        for (int i = 0; i < triangles.Length; i++)
        {
            triangles[i] = i;
        }

        //����for��������������J�E���g������
        int x = 0, y = 0, z = 0;

        // ����

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

        //�c��

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

        //��������_�ԍ��A���W�f�[�^���쐬����mesh�ɒǉ�
        mesh.vertices = verts;
        mesh.triangles = triangles;
        arrayBuffer = 0;

        //�Čv�Z()
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        //�Čv�Z��Ɋ�������Mesh��ǉ�
        GetComponent<MeshFilter>().mesh = mesh;
        //�ݒ肵��Material�𔽉f
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