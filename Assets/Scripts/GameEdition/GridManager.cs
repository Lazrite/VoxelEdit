using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] public Material material;
    [SerializeField] public Vector3Int size;
    [SerializeField] public float lineSize;

    private Material oldMaterial;
    private Vector3Int oldSize;
    private float oldLineSize;

    [HideInInspector] public Vector3[,,] gridPosFromIndexMultiple;
    public Vector3[] gridPosFromIndex;
    public bool[] isPlaced;
    public GameObject[] placedObjects;
    public (GameObject[] obj, float[] index) ablePLacementSurround;

    private void Start()
    {
        isPlaced = new bool[size.x * size.y * size.z];
        gridPosFromIndexMultiple = new Vector3[size.x, size.y, size.z];
        gridPosFromIndex = new Vector3[size.x * size.y * size.z];
        placedObjects = new GameObject[size.x * size.y * size.z];
        ablePLacementSurround.obj = new GameObject[(size.x * size.y) * 2 + (size.x * size.z) * 2 + (size.y * size.z) * 2];
        ablePLacementSurround.index = new float[(size.x * size.y) * 2 + (size.x * size.z) * 2 + (size.y * size.z) * 2];
    }

    private void Update()
    {
        if (OnChangeGridInfo())
        {
            isPlaced = new bool[size.x * size.y * size.z];
            gridPosFromIndexMultiple = new Vector3[size.x, size.y, size.z];
            gridPosFromIndex = new Vector3[size.x * size.y * size.z];
            placedObjects = new GameObject[size.x * size.y * size.z];

            for (int i = 0; i < size.z; i++)
            {
                for(int j = 0; j < size.x; j++)
                {
                    for(int k = 0; k < size.y; k++)
                    {
                        gridPosFromIndexMultiple[j, k, i] = new Vector3(0.5f + j, 0.5f + k, 0.5f + i);
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
        }
    }

    private void LateUpdate()
    {
        if(OnChangeGridInfo())
        {
            oldLineSize = lineSize;
            oldMaterial = material;
            oldSize = size;
        }
    }

    public bool OnChangeGridInfo()
    {
        if(material != oldMaterial || size != oldSize || lineSize != oldLineSize)
        {
            return true;
        }

        return false;
    }

    private void OnValidate()
    {
        if (size.x < 1) size.x = 1;
        if (size.y < 1) size.y = 1;
        if (size.z < 1) size.z = 1;
        if (lineSize < 0.01f) lineSize = 0.01f;
    }

}
