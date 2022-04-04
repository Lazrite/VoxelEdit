using UnityEngine;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class GridView : MonoBehaviour
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
        triangles = new int[((size.x + 1) * (size.z + 1) * 6) + ((size.y + 1) * (size.z + 1) * 6) + ((size.y + 1) * (size.x + 1) * 6)];
        //���_�̍��W��size���m��
        verts = new Vector3[((size.x + 1) * (size.z + 1) * 6) + ((size.y + 1) * (size.z + 1) * 6) + ((size.y + 1) * (size.x + 1) * 6)];

        //���_�ԍ������蓖��
        for (int i = 0; i < triangles.Length; i++)
        {
            triangles[i] = i;
        }

        //����for��������������J�E���g������
        int x = 0, y = 0, z = 0;

        for (int j = 0; j < size.z + 1; j++)
        {
            //�c��
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
            //�c��
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
            //�c��
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
        if (size.x < 1) size.x = 1;
        if (size.y < 1) size.y = 1;
        if (size.z < 1) size.z = 1;
    }
}