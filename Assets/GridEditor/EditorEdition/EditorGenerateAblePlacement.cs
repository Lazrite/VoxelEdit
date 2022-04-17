#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class EditorGenerateAblePlacement : MonoBehaviour
{
    private GameObject areaGameObject;

    private EditorGridField gridManager;
    private Vector3Int size;

    private Object instantiateBuffer;

    private int currentIndex;

    public EditorGenerateAblePlacement(GameObject areaGameObject)
    {
        this.areaGameObject = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorSource/AblePlacementQuad.prefab", typeof(GameObject));
    }

    // Start is called before the first frame update
    private void Start()
    {
        gridManager = transform.GetComponent<EditorGridField>();
        size = gridManager.size;
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void CreateAblePlacement()
    {
        gridManager = transform.GetComponent<EditorGridField>();
        size = gridManager.size;

        areaGameObject =
            (GameObject)AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorSource/AblePlacementQuad.prefab",
                typeof(GameObject));

        int surroundBuffer = 0;

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (gridManager.isPlaced[(j + 1) + (i * size.y) - 1])
                {
                    continue;
                }

                instantiateBuffer = PrefabUtility.InstantiatePrefab(areaGameObject);
                ((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
                ((GameObject)instantiateBuffer).transform.position = new Vector3(i + 0.5f, j + 0.5f, 0);
                ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = (j + 1) + (i * size.y);
                gridManager.ablePLacementSurround.index[surroundBuffer] = (j + 1) + (i * size.y);
                gridManager.ablePLacementSurround.obj[surroundBuffer] = (GameObject)instantiateBuffer;
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                if (gridManager.isPlaced[(j + 1) + (i * size.y) + (size.x * size.y * (size.z - 1)) - 1])
                {
                    continue;
                }

                instantiateBuffer = PrefabUtility.InstantiatePrefab(areaGameObject);
                ((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
                ((GameObject)instantiateBuffer).transform.position = new Vector3(i + 0.5f, j + 0.5f, size.z);
                ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = (j + 1) + (i * size.y);
                gridManager.ablePLacementSurround.index[surroundBuffer] = (j + 1) + (i * size.y);
                gridManager.ablePLacementSurround.obj[surroundBuffer] = (GameObject)instantiateBuffer;
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                if (gridManager.isPlaced[(((j * size.x * size.y) + 1) + i) - 1])
                {
                    continue;
                }

                instantiateBuffer = PrefabUtility.InstantiatePrefab(areaGameObject);
                ((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
                ((GameObject)instantiateBuffer).transform.position = new Vector3(0, i + 0.5f, j + 0.5f);
                ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = (j + 1) + (i * size.y);
                gridManager.ablePLacementSurround.index[surroundBuffer] = (j + 1) + (i * size.y);
                gridManager.ablePLacementSurround.obj[surroundBuffer] = (GameObject)instantiateBuffer;
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                if (gridManager.isPlaced[((j * size.x * size.y) + 1) + i + (size.y * (size.x - 1)) - 1])
                {
                    continue;
                }

                instantiateBuffer = PrefabUtility.InstantiatePrefab(areaGameObject);
                ((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
                ((GameObject)instantiateBuffer).transform.position = new Vector3(size.x, i + 0.5f, j + 0.5f);
                ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = (j + 1) + (i * size.y);
                gridManager.ablePLacementSurround.index[surroundBuffer] = (j + 1) + (i * size.y);
                gridManager.ablePLacementSurround.obj[surroundBuffer] = (GameObject)instantiateBuffer;
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                if (gridManager.isPlaced[(((j * (size.x * size.y)) + 1) + (i * size.y)) - 1])
                {
                    continue;
                }

                instantiateBuffer = PrefabUtility.InstantiatePrefab(areaGameObject);
                ((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
                ((GameObject)instantiateBuffer).transform.position = new Vector3(i + 0.5f, 0, j + 0.5f);
                ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = (j + 1) + (i * size.y);
                gridManager.ablePLacementSurround.index[surroundBuffer] = (j + 1) + (i * size.y);
                gridManager.ablePLacementSurround.obj[surroundBuffer] = (GameObject)instantiateBuffer;
                surroundBuffer++;
            }
        }

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.z; j++)
            {
                if (gridManager.isPlaced[(((j * (size.x * size.y)) + 1) + (i * size.y)) + (size.y - 1) - 1])
                {
                    continue;
                }

                instantiateBuffer = PrefabUtility.InstantiatePrefab(areaGameObject);
                ((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
                ((GameObject)instantiateBuffer).transform.position = new Vector3(i + 0.5f, size.y, j + 0.5f);
                ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = (j + 1) + (i * size.y);
                gridManager.ablePLacementSurround.index[surroundBuffer] = (j + 1) + (i * size.y);
                gridManager.ablePLacementSurround.obj[surroundBuffer] = (GameObject)instantiateBuffer;
                surroundBuffer++;
            }
        }
    }

    public void AddPlaceMentArea(int index)
    {
        gridManager = transform.GetComponent<EditorGridField>();
        size = gridManager.size;

        areaGameObject =
            (GameObject)AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorSource/AblePlacementQuad.prefab",
                typeof(GameObject));


        if (index >= 1 && index <= size.x * size.y * size.z)
        {
            // Z��O
            instantiateBuffer = Instantiate(areaGameObject,
                new Vector3(gridManager.placedObjects[index - 1].transform.position.x,
                    gridManager.placedObjects[index - 1].transform.position.y,
                    gridManager.placedObjects[index - 1].transform.position.z - 0.5f), Quaternion.Euler(0, 0, 0),
                gridManager.placedObjects[index - 1].transform);
            ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index - (size.x * size.y);

            // Z������
            instantiateBuffer = Instantiate(areaGameObject,
                new Vector3(gridManager.placedObjects[index - 1].transform.position.x,
                    gridManager.placedObjects[index - 1].transform.position.y,
                    gridManager.placedObjects[index - 1].transform.position.z + 0.5f), Quaternion.Euler(0, 180, 0), gridManager.placedObjects[index - 1].transform);
            ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index + (size.x * size.y);

            // X������
            instantiateBuffer = Instantiate(areaGameObject,
                new Vector3(gridManager.placedObjects[index - 1].transform.position.x + 0.5f,
                    gridManager.placedObjects[index - 1].transform.position.y,
                    gridManager.placedObjects[index - 1].transform.position.z), Quaternion.Euler(0, 270, 0), gridManager.placedObjects[index - 1].transform);
            ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index + size.y;

            // X������
            instantiateBuffer = Instantiate(areaGameObject,
                new Vector3(gridManager.placedObjects[index - 1].transform.position.x - 0.5f,
                    gridManager.placedObjects[index - 1].transform.position.y,
                    gridManager.placedObjects[index - 1].transform.position.z), Quaternion.Euler(0, 90, 0), gridManager.placedObjects[index - 1].transform);
            ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index - size.y;

            // Y������
            instantiateBuffer = Instantiate(areaGameObject,
                new Vector3(gridManager.placedObjects[index - 1].transform.position.x,
                    gridManager.placedObjects[index - 1].transform.position.y + 0.5f,
                    gridManager.placedObjects[index - 1].transform.position.z), Quaternion.Euler(90, 0, 0), gridManager.placedObjects[index - 1].transform);
            ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index + 1;

            // Y������
            instantiateBuffer = Instantiate(areaGameObject,
                new Vector3(gridManager.placedObjects[index - 1].transform.position.x,
                    gridManager.placedObjects[index - 1].transform.position.y - 0.5f,
                    gridManager.placedObjects[index - 1].transform.position.z), Quaternion.Euler(270, 0, 0), gridManager.placedObjects[index - 1].transform);
            ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index - 1;
        }
    }

    public void AddAdJoinPlacement(int index)
    {
        areaGameObject =
            (GameObject)AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorSource/AblePlacementQuad.prefab",
                typeof(GameObject));

        // Z��O�̃u���b�N���m�F
        if (index - (size.x * size.y) - 1 >= 0 && index - (size.x * size.y) <= size.x * size.y * size.z)
        {
            if (gridManager.placedObjects[index - (size.x * size.y) - 1] != null)
            {
                bool Alreadyinstantiate = false;

                foreach (Transform child in gridManager.placedObjects[index - (size.x * size.y) - 1].transform)
                {
                    if (child.transform.eulerAngles == new Vector3(0, 180, 0) && child.tag == "AblePlacement")
                    {
                        Alreadyinstantiate = true;
                        break;
                    }
                }

                if (!Alreadyinstantiate)
                {
                    instantiateBuffer = Instantiate(areaGameObject,
                    new Vector3(gridManager.gridPosFromIndex[index - (size.x * size.y) - 1].x,
                                gridManager.gridPosFromIndex[index - (size.x * size.y) - 1].y,
                                gridManager.gridPosFromIndex[index - (size.x * size.y) - 1].z + 0.5f), Quaternion.Euler(0, -180, 0), gridManager.placedObjects[index - (size.x * size.y) - 1].transform);
                    ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index;
                }
            }
        }

        // Z�����̃u���b�N���m�F
        if (index + (size.x * size.y) - 1 >= 0 && index + (size.x * size.y) <= size.x * size.y * size.z)
        {
            if (gridManager.placedObjects[index + (size.x * size.y) - 1] != null)
            {
                bool Alreadyinstantiate = false;

                foreach (Transform child in gridManager.placedObjects[index + (size.x * size.y) - 1].transform)
                {
                    if (child.transform.eulerAngles == new Vector3(0, 0, 0) && child.tag == "AblePlacement")
                    {
                        Alreadyinstantiate = true;
                        break;
                    }
                }

                if (!Alreadyinstantiate)
                {
                    instantiateBuffer = Instantiate(areaGameObject,
                        new Vector3(gridManager.gridPosFromIndex[index + (size.x * size.y) - 1].x,
                                gridManager.gridPosFromIndex[index + (size.x * size.y) - 1].y,
                                gridManager.gridPosFromIndex[index + (size.x * size.y) - 1].z - 0.5f), Quaternion.Euler(0, 0, 0), gridManager.placedObjects[index + (size.x * size.y) - 1].transform);
                    ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index;
                }
            }
        }

        // X�������̃u���b�N���m�F
        if (index + size.y - 1 >= 0 && index + size.y <= size.x * size.y * size.z)
        {
            if (gridManager.placedObjects[index + size.y - 1] != null)
            {
                bool Alreadyinstantiate = false;

                foreach (Transform child in gridManager.placedObjects[index + size.y - 1].transform)
                {
                    if (child.transform.eulerAngles == new Vector3(0, 90, 0) && child.tag == "AblePlacement")
                    {
                        Alreadyinstantiate = true;
                        break;
                    }
                }

                if (!Alreadyinstantiate)
                {
                    instantiateBuffer = Instantiate(areaGameObject,
                        new Vector3(gridManager.gridPosFromIndex[index + size.y - 1].x - 0.5f,
                                gridManager.gridPosFromIndex[index + size.y - 1].y,
                                gridManager.gridPosFromIndex[index + size.y - 1].z), Quaternion.Euler(0, 90, 0), gridManager.placedObjects[index + size.y - 1].transform);
                    ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index;
                }
            }
        }

        // X�������̃u���b�N���m�F
        if (index - size.y - 1 >= 0 && index - size.y <= size.x * size.y * size.z)
        {
            if (gridManager.placedObjects[index - size.y - 1] != null)
            {
                bool Alreadyinstantiate = false;

                foreach (Transform child in gridManager.placedObjects[index - size.y - 1].transform)
                {
                    if (child.transform.eulerAngles == new Vector3(0, 270, 0) && child.tag == "AblePlacement")
                    {
                        Alreadyinstantiate = true;
                        break;
                    }
                }

                if (!Alreadyinstantiate)
                {
                    instantiateBuffer = Instantiate(areaGameObject,
                        new Vector3(gridManager.gridPosFromIndex[index - size.y - 1].x + 0.5f,
                                gridManager.gridPosFromIndex[index - size.y - 1].y,
                                gridManager.gridPosFromIndex[index - size.y - 1].z), Quaternion.Euler(0, -90, 0), gridManager.placedObjects[index - size.y - 1].transform);
                    ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index;
                }
            }
        }

        // Y�������̃u���b�N���m�F
        if (index + 1 - 1 >= 0 && index + 1 <= size.x * size.y * size.z)
        {
            if (gridManager.placedObjects[index + 1 - 1] != null)
            {
                bool Alreadyinstantiate = false;

                foreach (Transform child in gridManager.placedObjects[index + 1 - 1].transform)
                {
                    if (child.transform.eulerAngles == new Vector3(-90, 0, 0) && child.tag == "AblePlacement")
                    {
                        Alreadyinstantiate = true;
                        break;
                    }
                }

                if (!Alreadyinstantiate)
                {
                    instantiateBuffer = Instantiate(areaGameObject,
                        new Vector3(gridManager.gridPosFromIndex[index + 1 - 1].x,
                                gridManager.gridPosFromIndex[index + 1 - 1].y - 0.5f,
                                gridManager.gridPosFromIndex[index + 1 - 1].z), Quaternion.Euler(-90, 0, 0), gridManager.placedObjects[index + 1 - 1].transform);
                    ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index;
                }
            }
        }

        // Y�������̃u���b�N���m�F
        if (index - 1 - 1 >= 0 && index - 1 - 1 <= size.x * size.y * size.z)
        {
            if (gridManager.placedObjects[index - 1 - 1] != null)
            {
                bool Alreadyinstantiate = false;

                foreach (Transform child in gridManager.placedObjects[index - 1 - 1].transform)
                {
                    if (child.transform.eulerAngles == new Vector3(90, 0, 0) && child.tag == "AblePlacement")
                    {
                        Alreadyinstantiate = true;
                        break;
                    }
                }

                if (!Alreadyinstantiate)
                {
                    instantiateBuffer = Instantiate(areaGameObject,
                        new Vector3(gridManager.gridPosFromIndex[index - 1 - 1].x,
                                gridManager.gridPosFromIndex[index - 1 - 1].y + 0.5f,
                                gridManager.gridPosFromIndex[index - 1 - 1].z), Quaternion.Euler(90, 0, 0), gridManager.placedObjects[index - 1 - 1].transform);
                    ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index;
                }
            }
        }
    }

    public void AddSurroundPlacement(int index)
    {
        gridManager = transform.GetComponent<EditorGridField>();
        size = gridManager.size;

        int buf = 0;
        areaGameObject =
            (GameObject)AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorSource/AblePlacementQuad.prefab",
                typeof(GameObject));

        // ���̓I�u�W�F�N�gZ����
        for (int i = 0; i < size.x * size.y; buf++, i++)
        {
            if (gridManager.ablePLacementSurround.index[buf] == index && gridManager.ablePLacementSurround.obj[buf] == null)
            {
                instantiateBuffer = Instantiate(areaGameObject,
                        new Vector3(gridManager.gridPosFromIndex[index - 1].x,
                                    gridManager.gridPosFromIndex[index - 1].y,
                                    gridManager.gridPosFromIndex[index - 1].z - 0.5f), Quaternion.Euler(0, -180, 0), gameObject.transform);
                ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index;
                gridManager.ablePLacementSurround.obj[buf] = ((GameObject)instantiateBuffer);
            }
        }

        // ���̓I�u�W�F�N�gZ����
        for (int i = 0; i < size.x * size.y; buf++, i++)
        {
            if (gridManager.ablePLacementSurround.index[buf] == index && gridManager.ablePLacementSurround.obj[buf] == null)
            {
                instantiateBuffer = Instantiate(areaGameObject,
                        new Vector3(gridManager.gridPosFromIndex[index - 1].x,
                                    gridManager.gridPosFromIndex[index - 1].y,
                                    gridManager.gridPosFromIndex[index - 1].z + 0.5f), Quaternion.identity, gameObject.transform);
                ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index;
                gridManager.ablePLacementSurround.obj[buf] = ((GameObject)instantiateBuffer);
            }
        }

        // ���̓I�u�W�F�N�gX����
        for (int i = 0; i < size.z * size.y; buf++, i++)
        {
            if (gridManager.ablePLacementSurround.index[buf] == index && gridManager.ablePLacementSurround.obj[buf] == null)
            {
                instantiateBuffer = Instantiate(areaGameObject,
                        new Vector3(gridManager.gridPosFromIndex[index - 1].x - 0.5f,
                                    gridManager.gridPosFromIndex[index - 1].y,
                                    gridManager.gridPosFromIndex[index - 1].z), Quaternion.Euler(0, -90, 0), gameObject.transform);
                ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index;
                gridManager.ablePLacementSurround.obj[buf] = ((GameObject)instantiateBuffer);
            }
        }

        // ���̓I�u�W�F�N�gX����
        for (int i = 0; i < size.z * size.y; buf++, i++)
        {
            if (gridManager.ablePLacementSurround.index[buf] == index && gridManager.ablePLacementSurround.obj[buf] == null)
            {
                instantiateBuffer = Instantiate(areaGameObject,
                        new Vector3(gridManager.gridPosFromIndex[index - 1].x + 0.5f,
                                    gridManager.gridPosFromIndex[index - 1].y,
                                    gridManager.gridPosFromIndex[index - 1].z), Quaternion.Euler(0, 90, 0), gameObject.transform);
                ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index;
                gridManager.ablePLacementSurround.obj[buf] = ((GameObject)instantiateBuffer);
            }
        }

        // ���̓I�u�W�F�N�gY����
        for (int i = 0; i < size.x * size.z; buf++, i++)
        {
            if (gridManager.ablePLacementSurround.index[buf] == index && gridManager.ablePLacementSurround.obj[buf] == null)
            {
                instantiateBuffer = Instantiate(areaGameObject,
                        new Vector3(gridManager.gridPosFromIndex[index - 1].x,
                                    gridManager.gridPosFromIndex[index - 1].y - 0.5f,
                                    gridManager.gridPosFromIndex[index - 1].z), Quaternion.Euler(90, 0, 0), gameObject.transform);
                ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index;
                gridManager.ablePLacementSurround.obj[buf] = ((GameObject)instantiateBuffer);
            }
        }

        // ���̓I�u�W�F�N�gY�㑤
        for (int i = 0; i < size.x * size.z; buf++, i++)
        {
            if (gridManager.ablePLacementSurround.index[buf] == index && gridManager.ablePLacementSurround.obj[buf] == null)
            {
                instantiateBuffer = Instantiate(areaGameObject,
                        new Vector3(gridManager.gridPosFromIndex[index - 1].x,
                                    gridManager.gridPosFromIndex[index - 1].y + 0.5f,
                                    gridManager.gridPosFromIndex[index - 1].z), Quaternion.Euler(-90, 0, 0), gameObject.transform);
                ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index;
                gridManager.ablePLacementSurround.obj[buf] = ((GameObject)instantiateBuffer);
            }
        }
    }

    public void AddCheckedPlacementArea()
    {
        gridManager = transform.GetComponent<EditorGridField>();
        size = gridManager.size;

        for (int i = 0; i < size.z; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                for (int k = 0; k < size.y; k++)
                {
                    if (gridManager.isPlaced[(i * size.x * size.y) + (j * size.y) + k])
                    {
                        AddPlaceMentArea((i * size.x * size.y) + (j * size.y) + k + 1);
                    }
                }
            }
        }
    }

    public void DeletePlacementArea(int index)
    {
        gridManager = transform.GetComponent<EditorGridField>();
        size = gridManager.size;


        if (!gridManager.placedObjects[index - 1])
        {
            return;
        }

        // Z�������̒[�ɐڂ��ĂȂ��ꍇ
        if (!gridManager.isCheckGridEdgeFromIndex(index).HasFlag(EdgeOrientation.NegativeZ))
        {
            // Z��O�̃u���b�N���m�F
            if (index - (size.x * size.y) > 0 && index - (size.x * size.y) <= size.x * size.y * size.z)
            {
                if (gridManager.placedObjects[index - (size.x * size.y) - 1] != null)
                {
                    foreach (Transform child in gridManager.placedObjects[index - 1].transform)
                    {
                        if (child.transform.eulerAngles != new Vector3(0, 0, 0) || child.tag != "AblePlacement")
                        {
                            continue;
                        }

                        Undo.DestroyObjectImmediate(child.gameObject);
                        break;
                    }
                }
            }
        }

        // Z�������̒[�ɐڂ��ĂȂ��ꍇ
        if (!gridManager.isCheckGridEdgeFromIndex(index).HasFlag(EdgeOrientation.PositiveZ))
        {
            // Z�����̃u���b�N���m�F
            if (index + (size.x * size.y) > 0 && index + (size.x * size.y) <= size.x * size.y * size.z)
            {
                if (gridManager.placedObjects[index + (size.x * size.y) - 1] != null)
                {
                    foreach (Transform child in gridManager.placedObjects[index - 1].transform)
                    {
                        if (child.transform.eulerAngles != new Vector3(0, 180, 0) || child.tag != "AblePlacement")
                        {
                            continue;
                        }

                        DestroyImmediate(child.gameObject);
                        break;
                    }
                }
            }
        }

        // X�������̒[�ɐڂ��ĂȂ��ꍇ
        if (!gridManager.isCheckGridEdgeFromIndex(index).HasFlag(EdgeOrientation.PositiveX))
        {
            // X�������̃u���b�N���m�F
            if (index + size.y > 0 && index + size.y <= size.x * size.y * size.z)
            {
                if (gridManager.placedObjects[index + size.y - 1] != null)
                {
                    foreach (Transform child in gridManager.placedObjects[index - 1].transform)
                    {
                        if (child.transform.eulerAngles != new Vector3(0, 270, 0) || child.tag != "AblePlacement")
                        {
                            continue;
                        }

                        DestroyImmediate(child.gameObject);
                        break;
                    }
                }
            }
        }

        // X�������̒[�ɐڂ��ĂȂ��ꍇ
        if (!gridManager.isCheckGridEdgeFromIndex(index).HasFlag(EdgeOrientation.NegativeX))
        {
            // X�������̃u���b�N���m�F
            if (index - size.y > 0 && index - size.y <= size.x * size.y * size.z)
            {
                if (gridManager.placedObjects[index - size.y - 1] != null)
                {
                    foreach (Transform child in gridManager.placedObjects[index - 1].transform)
                    {
                        if (child.transform.eulerAngles != new Vector3(0, 90, 0) || child.tag != "AblePlacement")
                        {
                            continue;
                        }

                        DestroyImmediate(child.gameObject);
                        break;
                    }
                }
            }
        }

        // Y�������̒[�ɐڂ��ĂȂ��ꍇ
        if (!gridManager.isCheckGridEdgeFromIndex(index).HasFlag(EdgeOrientation.PositiveY))
        {
            // Y�������̃u���b�N���m�F
            if (index + 1 > 0 && index + 1 <= size.x * size.y * size.z)
            {
                if (gridManager.placedObjects[index - 1 + 1] != null)
                {
                    foreach (Transform child in gridManager.placedObjects[index - 1].transform)
                    {
                        if (child.transform.eulerAngles != new Vector3(90, 0, 0) || child.tag != "AblePlacement")
                        {
                            continue;
                        }

                        DestroyImmediate(child.gameObject);
                        break;
                    }
                }
            }
        }

        // Y�������̒[�ɐڂ��ĂȂ��ꍇ
        if (!gridManager.isCheckGridEdgeFromIndex(index).HasFlag(EdgeOrientation.NegativeY))
        {
            // Y�������̃u���b�N���m�F
            if (index - 1 > 0 && index - 1 <= size.x * size.y * size.z)
            {
                if (gridManager.placedObjects[index - 1 - 1] != null)
                {
                    foreach (Transform child in gridManager.placedObjects[index - 1].transform)
                    {
                        if (child.transform.eulerAngles != new Vector3(270, 0, 0) || child.tag != "AblePlacement")
                        {
                            continue;
                        }

                        DestroyImmediate(child.gameObject);
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// �������w�肵�Đڑ��ʂ𐶐����郁�\�b�h�B�͈͑I�����[�h�ŗ��p
    /// </summary>
    /// <param name="edge"> �ǂ̕����֐������邩(����Enum�͒[���ǂ����Ƃ����{���̎g�����ŗ��p���ĂȂ��̂Œ���) </param>
    /// <param name="index"> �Ώۃv���n�u�̃O���b�h�ʒu </param>
    public void AddPlacementDir(EdgeOrientation edge, int index)
    {
        EdgeOrientation whereFromEdge = gridManager.isCheckGridEdgeFromIndex(index);

        if (index + size.y > 0 && index + size.y <= size.x * size.y * size.z)
        {
            if (edge.HasFlag(EdgeOrientation.PositiveX) && !gridManager.isPlaced[index + size.y - 1])
            {
                instantiateBuffer = Instantiate(areaGameObject,
                    new Vector3(gridManager.placedObjects[index - 1].transform.position.x + 0.5f,
                        gridManager.placedObjects[index - 1].transform.position.y,
                        gridManager.placedObjects[index - 1].transform.position.z), Quaternion.Euler(0, 270, 0),
                    gridManager.placedObjects[index - 1].transform);
                ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index + size.y;
            }
        }

        if (index - size.y > 0 && index - size.y <= size.x * size.y * size.z)
        {
            if (edge.HasFlag(EdgeOrientation.NegativeX) && !gridManager.isPlaced[index - size.y - 1])
            {
                instantiateBuffer = Instantiate(areaGameObject,
                    new Vector3(gridManager.placedObjects[index - 1].transform.position.x - 0.5f,
                        gridManager.placedObjects[index - 1].transform.position.y,
                        gridManager.placedObjects[index - 1].transform.position.z), Quaternion.Euler(0, 90, 0),
                    gridManager.placedObjects[index - 1].transform);
                ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index - size.y;
            }
        }

        if (index > 0 && index <= size.x * size.y * size.z)
        {
            if (index - 1 - 1 < 0)
            {
                instantiateBuffer = Instantiate(areaGameObject,
                    new Vector3(gridManager.placedObjects[index - 1].transform.position.x,
                        gridManager.placedObjects[index - 1].transform.position.y - 0.5f,
                        gridManager.placedObjects[index - 1].transform.position.z), Quaternion.Euler(270, 0, 0),
                    gridManager.placedObjects[index - 1].transform);
                ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index - 1;
            }
            else if (edge.HasFlag(EdgeOrientation.NegativeY) && !gridManager.isPlaced[index - 1 - 1])
            {
                instantiateBuffer = Instantiate(areaGameObject,
                    new Vector3(gridManager.placedObjects[index - 1].transform.position.x,
                        gridManager.placedObjects[index - 1].transform.position.y - 0.5f,
                        gridManager.placedObjects[index - 1].transform.position.z), Quaternion.Euler(270, 0, 0),
                    gridManager.placedObjects[index - 1].transform);
                ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index - 1;
            }
        }

        if (index > 0 && index <= size.x * size.y * size.z)
        {
            if (index - 1 + 1 >= size.x * size.y * size.z)
            {
                instantiateBuffer = Instantiate(areaGameObject,
                    new Vector3(gridManager.placedObjects[index - 1].transform.position.x,
                        gridManager.placedObjects[index - 1].transform.position.y + 0.5f,
                        gridManager.placedObjects[index - 1].transform.position.z), Quaternion.Euler(90, 0, 0),
                    gridManager.placedObjects[index - 1].transform);
                ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index + 1;
            }
            else if (edge.HasFlag(EdgeOrientation.PositiveY) && !gridManager.isPlaced[index - 1 + 1])
            {
                instantiateBuffer = Instantiate(areaGameObject,
                    new Vector3(gridManager.placedObjects[index - 1].transform.position.x,
                        gridManager.placedObjects[index - 1].transform.position.y + 0.5f,
                        gridManager.placedObjects[index - 1].transform.position.z), Quaternion.Euler(90, 0, 0),
                    gridManager.placedObjects[index - 1].transform);
                ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex = index + 1;
            }
        }

        if (index - (size.x * size.y) > 0 && index - (size.x * size.y) <= size.x * size.y * size.z)
        {
            if (edge.HasFlag(EdgeOrientation.NegativeZ) && !gridManager.isPlaced[index - (size.x * size.y) - 1])
            {
                instantiateBuffer = Instantiate(areaGameObject,
                    new Vector3(gridManager.placedObjects[index - 1].transform.position.x,
                        gridManager.placedObjects[index - 1].transform.position.y,
                        gridManager.placedObjects[index - 1].transform.position.z - 0.5f),
                    Quaternion.Euler(0, 0, 0),
                    gridManager.placedObjects[index - 1].transform);
                ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex =
                    index - (size.x * size.y);
            }
        }

        if (index + (size.x * size.y) > 0 && index + (size.x * size.y) <= size.x * size.y * size.z)
        {
            if (edge.HasFlag(EdgeOrientation.PositiveZ) && !gridManager.isPlaced[index + (size.x * size.y) - 1])
            {
                instantiateBuffer = Instantiate(areaGameObject,
                    new Vector3(gridManager.placedObjects[index - 1].transform.position.x,
                        gridManager.placedObjects[index - 1].transform.position.y,
                        gridManager.placedObjects[index - 1].transform.position.z + 0.5f),
                    Quaternion.Euler(0, 180, 0),
                    gridManager.placedObjects[index - 1].transform);
                ((GameObject)instantiateBuffer).GetComponent<GridRelatedInfo>().gridIndex =
                    index + (size.x * size.y);
            }
        }

    }
}

#endif // UNITY_EDITOR