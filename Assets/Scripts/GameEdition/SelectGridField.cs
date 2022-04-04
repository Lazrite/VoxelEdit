using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SelectGridField : MonoBehaviour
{
    [SerializeField] private GameObject prefab = default;
    [SerializeField] private GridManager gridManager = default;
    [SerializeField] private GenerateAblePlacementArea placementArea = default;
    [SerializeField] private PlaceDownPanelUI placeDownPanelUI = default;

    // instanciateされたオブジェクトの参照バッファ
    private GameObject instantiateBuffer = default;

    // グリッドサイズ
    private Vector3Int gridSize;

    // インデックス参照バッファ
    private int indexBuffer = -1;

    // 消去時の参照リスト
    private List<int> indexNumList;

    // Start is called before the first frame update
    private void Start()
    {
        gridSize = gridManager.size;
    }

    private void Update()
    {
        int index = default;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int mask = 1 << 6;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            Debug.Log(hit.collider.gameObject);

            if (Vector3.Dot(hit.normal, ray.direction) < 0)
            {
                if (hit.collider.gameObject.GetComponent<IsVisualizeMesh>() != null)
                {
                    hit.collider.gameObject.GetComponent<IsVisualizeMesh>().select_flg = true;
                }
            }
        }

        switch (placeDownPanelUI.selectMode)
        {
            case OperationMode.OPERATION_CLICK: // クリックモード
                //左クリックされたら
                if (Input.GetMouseButtonDown(0))
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<GridRelatedInfo>() != null)
                        {
                            index = hit.collider.GetComponent<GridRelatedInfo>().gridIndex;
                        }

                        if (index == default)
                        {
                            return;
                        }

                        instantiateBuffer = Instantiate(prefab, new Vector3(gridManager.gridPosFromIndex[index - 1].x, gridManager.gridPosFromIndex[index - 1].y, gridManager.gridPosFromIndex[index - 1].z), Quaternion.identity);
                        gridManager.isPlaced[index - 1] = true;
                        gridManager.placedObjects[index - 1] = instantiateBuffer;
                        placementArea.AddPlaceMentArea(index);
                        placementArea.DeletePlacementArea();
                        Destroy(hit.collider.gameObject);
                    }
                }

                if (Input.GetMouseButtonDown(1))
                {
                    if (hit.collider != null)
                    {
                        if(hit.collider.GetComponent<GridRelatedInfo>() != null)
                        {
                            index = ConjecturePlacementObjIndex(hit);
                        }

                        if(hit.collider.transform.parent.name == "AblePlacement")
                        {
                            return;
                        }

                        Destroy(hit.collider.transform.parent.gameObject);
                        gridManager.isPlaced[index - 1] = false;
                        gridManager.placedObjects[index - 1] = null;
                        placementArea.AddAdJoinPlacement(index);

                        if(System.Array.IndexOf(gridManager.ablePLacementSurround.index, index) != -1)
                        {
                            placementArea.AddSurroundPlacement(index);
                        }
                    }
                }
                break;

            case OperationMode.OPERATION_DRAG_FREE: //ドラッグモード
                if (Input.GetMouseButton(0))
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<GridRelatedInfo>() != null)
                        {
                            index = hit.collider.GetComponent<GridRelatedInfo>().gridIndex;
                        }

                        if (index == default)
                        {
                            return;
                        }

                        if(gridManager.isPlaced[index - 1])
                        {
                            return;
                        }

                        if (index != indexBuffer)
                        {
                            instantiateBuffer = Instantiate(prefab, new Vector3(gridManager.gridPosFromIndex[index - 1].x, gridManager.gridPosFromIndex[index - 1].y, gridManager.gridPosFromIndex[index - 1].z), Quaternion.identity);
                            gridManager.isPlaced[index - 1] = true;
                            gridManager.placedObjects[index - 1] = instantiateBuffer;
                            indexBuffer = index;
                            Destroy(hit.collider.gameObject);
                        }
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    placementArea.AddCheckedPlacementArea();
                    placementArea.DeletePlacementArea();
                }

                if(Input.GetMouseButton(1))
                {

                    if(indexNumList == null)
                    {
                        indexNumList = new List<int>();
                    }

                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<GridRelatedInfo>() != null)
                        {
                            index = ConjecturePlacementObjIndex(hit);
                        }

                        if (hit.collider.transform.parent.name == "AblePlacement")
                        {
                            return;
                        }

                        Destroy(hit.collider.transform.parent.gameObject);
                        gridManager.isPlaced[index - 1] = false;
                        gridManager.placedObjects[index - 1] = null;
                        indexNumList.Add(index);
                        
                    }
                }

                if (Input.GetMouseButtonUp(1))
                {
                    foreach (var indexList in indexNumList)
                    {
                        placementArea.AddAdJoinPlacement(indexList);

                        if (System.Array.IndexOf(gridManager.ablePLacementSurround.index, indexList) != -1)
                        {
                            placementArea.AddSurroundPlacement(indexList);
                        }
                    }

                    indexNumList.Clear();
                }
                break;
        }
    }

    private int ConjecturePlacementObjIndex(RaycastHit hitObj)
    {
        var hitAngles = hitObj.collider.transform.eulerAngles;
        var hitIndex = hitObj.collider.GetComponent<GridRelatedInfo>().gridIndex;

        if (hitAngles == new Vector3(0, 180, 0))
        {
            return hitIndex - (gridManager.size.x * gridManager.size.y);
        }
        else if (hitAngles == Vector3.zero)
        {
            return hitIndex + (gridManager.size.x * gridManager.size.y);
        }
        else if (hitAngles == new Vector3(0, 90, 0))
        {
            return hitIndex + gridManager.size.y;
        }
        else if (hitAngles == new Vector3(0, 270, 0))
        {
            return hitIndex - gridManager.size.y;
        }
        else if (hitAngles == new Vector3(90, 0, 0))
        {
            return hitIndex - 1;
        }
        else if (hitAngles == new Vector3(270, 0, 0))
        {
            return hitIndex + 1;
        }

        return -1;
    }
}