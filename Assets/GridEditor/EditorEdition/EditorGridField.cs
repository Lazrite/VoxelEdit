#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class EditorGridField : MonoBehaviour
{
    private Vector3[] verts;    //�|���S���̒��_������
    private int[] triangles;    //�O�p�`��`���ۂɁA���_�̕`�揇���w�肷��

    [SerializeField] public Material material;
    [HideInInspector] public Vector3Int size;
    [HideInInspector] public float lineSize;
    [HideInInspector] public GameObject areaGameObject;

    private GameObject instantiateBuffer;

    [HideInInspector] public Vector3[,,] gridPosFromIndexMultiple;
    public Vector3[] gridPosFromIndex;
    public bool[] isPlaced;
    public GameObject[] VisualizedBuffer;
    public GameObject[] placedObjects;
    public (GameObject[] obj, float[] index) ablePLacementSurround;

    // �z��̃o�b�t�@
    private int arrayBuffer = 0;

    public void InstantiateGridField()
    {
        isPlaced = new bool[size.x * size.y * size.z];
        gridPosFromIndexMultiple = new Vector3[size.x, size.y, size.z];
        gridPosFromIndex = new Vector3[size.x * size.y * size.z];
        VisualizedBuffer = new GameObject[size.x * size.y * size.z];
        placedObjects = new GameObject[size.x * size.y * size.z];
        ablePLacementSurround.obj = new GameObject[(size.x * size.y) * 2 + (size.x * size.z) * 2 + (size.y * size.z) * 2];
        ablePLacementSurround.index = new float[(size.x * size.y) * 2 + (size.x * size.z) * 2 + (size.y * size.z) * 2];
        areaGameObject = AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorSource/AblePlacementQuad.prefab", typeof(GameObject)) as GameObject;

        for (int i = 0; i < size.z; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                for (int k = 0; k < size.y; k++)
                {
                    gridPosFromIndexMultiple[j, k, i] = new Vector3(this.transform.position.x + 0.5f + j, this.transform.position.y + 0.5f + k, this.transform.position.z + 0.5f + i);
                }
            }
        }

        for (int i = 0; i < size.z; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                for (int k = 0; k < size.y; k++)
                {
                    gridPosFromIndex[(i * size.x * size.y) + (j * size.y) + k] = gridPosFromIndexMultiple[j, k, i];
                }
            }
        }

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
        int x = 0, y = 0;

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


        int surroundBuffer = 0;

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (isPlaced[(j + 1) + (i * size.y) - 1])
                {
                    continue;
                }

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(i + 0.5f, j + 0.5f, 0), Quaternion.Euler(0, -180, 0), gameObject.transform);
                instantiateBuffer.GetComponent<GridRelatedInfo>().gridIndex = (j + 1) + (i * size.y);
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                EditorUtility.SetDirty(instantiateBuffer);
                ablePLacementSurround.index[surroundBuffer] = (j + 1) + (i * size.y);
                ablePLacementSurround.obj[surroundBuffer] = instantiateBuffer;
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (isPlaced[(j + 1) + (i * size.y) + (size.x * size.y * (size.z - 1)) - 1])
                {
                    continue;
                }

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(i + 0.5f, j + 0.5f, size.z), Quaternion.identity, gameObject.transform);
                instantiateBuffer.GetComponent<GridRelatedInfo>().gridIndex = (j + 1) + (i * size.y) + (size.x * size.y * (size.z - 1));
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                EditorUtility.SetDirty(instantiateBuffer);
                ablePLacementSurround.index[surroundBuffer] = (j + 1) + (i * size.y) + (size.x * size.y * (size.z - 1));
                ablePLacementSurround.obj[surroundBuffer] = instantiateBuffer;
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                if (isPlaced[(((j * size.x * size.y) + 1) + i) - 1])
                {
                    continue;
                }

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(0, i + 0.5f, j + 0.5f), Quaternion.Euler(0, -90, 0), gameObject.transform);
                instantiateBuffer.GetComponent<GridRelatedInfo>().gridIndex = ((j * size.x * size.y) + 1) + i;
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                EditorUtility.SetDirty(instantiateBuffer);
                ablePLacementSurround.index[surroundBuffer] = ((j * size.x * size.y) + 1) + i;
                ablePLacementSurround.obj[surroundBuffer] = instantiateBuffer;
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                if (isPlaced[((j * size.x * size.y) + 1) + i + (size.y * (size.x - 1)) - 1])
                {
                    continue;
                }

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(size.x, i + 0.5f, j + 0.5f), Quaternion.Euler(0, 90, 0), gameObject.transform);
                instantiateBuffer.GetComponent<GridRelatedInfo>().gridIndex = ((j * size.x * size.y) + 1) + i + (size.y * (size.x - 1));
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                EditorUtility.SetDirty(instantiateBuffer);
                ablePLacementSurround.index[surroundBuffer] = ((j * size.x * size.y) + 1) + i + (size.y * (size.x - 1));
                ablePLacementSurround.obj[surroundBuffer] = instantiateBuffer;
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                if (isPlaced[(((j * (size.x * size.y)) + 1) + (i * size.y)) - 1])
                {
                    continue;
                }

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(i + 0.5f, 0, j + 0.5f), Quaternion.Euler(90, 0, 0), gameObject.transform);
                instantiateBuffer.GetComponent<GridRelatedInfo>().gridIndex = (((j * (size.x * size.y)) + 1) + (i * size.y));
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                EditorUtility.SetDirty(instantiateBuffer);
                ablePLacementSurround.index[surroundBuffer] = (((j * (size.x * size.y)) + 1) + (i * size.y));
                ablePLacementSurround.obj[surroundBuffer] = instantiateBuffer;
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                if (isPlaced[(((j * (size.x * size.y)) + 1) + (i * size.y)) + (size.y - 1) - 1])
                {
                    continue;
                }

                instantiateBuffer = Instantiate(areaGameObject, transform.position + new Vector3(i + 0.5f, size.y, j + 0.5f), Quaternion.Euler(-90, 0, 0), gameObject.transform);
                instantiateBuffer.GetComponent<GridRelatedInfo>().gridIndex = (((j * (size.x * size.y)) + 1) + (i * size.y)) + (size.y - 1);
                instantiateBuffer.hideFlags = HideFlags.HideInHierarchy;
                EditorUtility.SetDirty(instantiateBuffer);
                ablePLacementSurround.index[surroundBuffer] = (((j * (size.x * size.y)) + 1) + (i * size.y)) + (size.y - 1);
                ablePLacementSurround.obj[surroundBuffer] = instantiateBuffer;
                surroundBuffer++;
            }
        }
    }

    public void ClearGrid()
    {
        for (int i = transform.childCount; i > 0; --i)
        {
            GameObject.DestroyImmediate(gameObject.transform.GetChild(0).gameObject);
            Debug.Log(gameObject.transform.childCount);
        }
    }
}

#endif // UNITY_EDITOR