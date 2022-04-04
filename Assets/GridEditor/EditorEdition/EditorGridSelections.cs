using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class EditorGridSelections : MonoBehaviour
{
    private EditorGridField gridManager;
    private EditorGenerateAblePlacement placementArea = default;
    public OperationMode selectMode;
    public ToolMode toolMode;

    // instanciateされたオブジェクトの参照バッファ
    private Object instantiateBuffer = default;

    // グリッドサイズ
    private Vector3Int gridSize;

    // インデックス参照バッファ
    private int indexBuffer = -1;

    // 消去時の参照リスト
    private List<int> indexNumList;

    string str;
    int state;

    private bool isDrag = false;

    // Start is called before the first frame update
    private void Start()
    {
        if (((GameObject)GridEditorWindow.gridObject) != null)
        {
            gridManager = ((GameObject)GridEditorWindow.gridObject).GetComponent<EditorGridField>();
            placementArea = ((GameObject)GridEditorWindow.gridObject).GetComponent<EditorGenerateAblePlacement>();
        }
    }

    private void OnDrawGizmos()
    {

    }

    public void OnScene(SceneView sceneView)
    {
        if (((GameObject)GridEditorWindow.gridObject) != null)
        {
            gridManager = ((GameObject)GridEditorWindow.gridObject).GetComponent<EditorGridField>();
            placementArea = ((GameObject)GridEditorWindow.gridObject).GetComponent<EditorGenerateAblePlacement>();
        }

        // Get mouse events for object placement when in the Scene View
        Event currentEvent = Event.current;

        if (GridEditorWindow.gridObject == null)
        {
            return;
        }

        int index = default;
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        int mask = 1 << 6;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            if (GridEditorWindow.gridObject != hit.transform.root.gameObject)
            {
                return;
            }

            if (Vector3.Dot(hit.normal, ray.direction) < 0)
            {
                if (hit.collider.gameObject.GetComponent<IsVisualizeMesh>() != null)
                {
                    hit.collider.gameObject.GetComponent<IsVisualizeMesh>().select_flg = true;
                }
            }
        }

        switch (selectMode)
        {
            case OperationMode.OPERATION_CLICK: // クリックモード
                //左クリックされたら:設置モード
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && toolMode == ToolMode.TOOL_PLACE)
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

                        if (GridEditorWindow.obj == null)
                        {
                            return;
                        }

                        instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                        ((GameObject)instantiateBuffer).transform.parent = ((GameObject)GridEditorWindow.gridObject).transform;
                        ((GameObject)instantiateBuffer).transform.position = ((GameObject)GridEditorWindow.gridObject).transform.position + new Vector3(gridManager.gridPosFromIndex[index - 1].x, gridManager.gridPosFromIndex[index - 1].y, gridManager.gridPosFromIndex[index - 1].z);

                        Undo.RecordObject(gridManager, "isPlaced Check");
                        gridManager.isPlaced[index - 1] = true;
                        Undo.RecordObject(gridManager, "Record PlacedObj");
                        gridManager.placedObjects[index - 1] = (GameObject)instantiateBuffer;
                        Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");
                        placementArea.AddPlaceMentArea(index);
                        placementArea.DeletePlacementArea();
                    }
                }

                // 左クリックされたら:消去モード
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 && toolMode == ToolMode.TOOL_ERASE)
                {

                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<GridRelatedInfo>() != null)
                        {
                            index = ConjecturePlacementObjIndex(hit);
                        }

                        if (hit.collider.transform.hideFlags == HideFlags.HideInHierarchy)
                        {
                            return;
                        }

                        Undo.DestroyObjectImmediate(hit.collider.transform.parent.gameObject);
                        gridManager.isPlaced[index - 1] = false;
                        gridManager.placedObjects[index - 1] = null;
                        placementArea.AddAdJoinPlacement(index);

                        if (System.Array.IndexOf(gridManager.ablePLacementSurround.index, index) != -1)
                        {
                            placementArea.AddSurroundPlacement(index);
                        }
                    }
                }
                break;

            case OperationMode.OPERATION_DRAG_FREE: //ドラッグモード
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                {
                    isDrag = true;
                    // マウスクリックのホットコントロールを固定
                    GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                    Event.current.Use();
                }

                if(isDrag)
                {
                    if(currentEvent.type == EventType.MouseUp || currentEvent.type == EventType.MouseLeaveWindow && currentEvent.button == 0)
                    {
                        isDrag = false;
                        Undo.IncrementCurrentGroup();
                        placementArea.AddCheckedPlacementArea();
                        Undo.IncrementCurrentGroup();
                        placementArea.DeletePlacementArea();
                        return;
                    }

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

                        if (gridManager.isPlaced[index - 1])
                        {
                            return;
                        }

                        if (GridEditorWindow.obj == null)
                        {
                            return;
                        }

                        

                        if (index != indexBuffer)
                        {
                            instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                            ((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
                            ((GameObject)instantiateBuffer).transform.position = new Vector3(gridManager.gridPosFromIndex[index - 1].x, gridManager.gridPosFromIndex[index - 1].y, gridManager.gridPosFromIndex[index - 1].z);
                            Undo.RecordObject(gridManager, "isPlaced Check");
                            gridManager.isPlaced[index - 1] = true;
                            Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");
                            gridManager.placedObjects[index - 1] = (GameObject)instantiateBuffer;
                            Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");
                            indexBuffer = index;
                        }
                    }
                }

                if (Input.GetMouseButton(1))
                {
                    if (indexNumList == null)
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

    private void OnRenderObject()
    {
        

        if (!Application.isPlaying)
        {
            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();
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