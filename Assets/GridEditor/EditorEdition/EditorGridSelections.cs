#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using Object = UnityEngine.Object;

[ExecuteAlways]
public class EditorGridSelections
{
    private EditorGridField gridManager;
    private EditorGenerateAblePlacement placementArea = default;
    public OperationMode selectMode;
    public ToolMode toolMode;

    // instantiate���ꂽ�I�u�W�F�N�g�̎Q�ƃo�b�t�@
    private Object instantiateBuffer = default;

    // �O���b�h�T�C�Y
    private Vector3Int gridSize;

    // �C���f�b�N�X�Q�ƃo�b�t�@
    private int indexBuffer = -1;

    // �������̎Q�ƃ��X�g
    private List<int> indexNumList;

    // �h���b�O���Ă��邩�ǂ����̃t���O
    private bool isDrag = false;

    // �r�W���A���C�U�I�u�W�F�N�g
    private Object visualizeObj;

    // �͈̓h���b�O�̃C���f�b�N�X�n�_
    private int rangeDragStartIndex;
    private Vector3Int startIndexPos;

    // �͈̓h���b�O�̃C���f�b�N�X�I�_
    private int rangeDragEndIndex;
    private Vector3Int endIndexPos;

    // �_�~�[�u���b�N�̃o�b�t�@
    private GameObject dummyBlock;

    // �C���f�b�N�X�̈ꎞ�ޔ�p
    private List<int> indexListTmp;

    // �u���b�N�u���\��p���X�g
    private List<(int, GameObject)> replaceReservation;

    public EditorGridSelections()
    {
        dummyBlock =
            (GameObject)AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorSource/BlockDummy.prefab",
                typeof(GameObject));
    }

    /// <summary>
    /// �G�f�B�^��ł�Start���Ă΂�Ȃ�(maybe)�̂ł��̃��\�b�h���f���Q�[�g�ƈꏏ�ɌĂ�ł�?
    /// </summary>
    public void StartSelectProcess()
    {
        if (((GameObject)GridEditorWindow.gridObject) != null)
        {
            gridManager = ((GameObject)GridEditorWindow.gridObject).GetComponent<EditorGridField>();
            placementArea = ((GameObject)GridEditorWindow.gridObject).GetComponent<EditorGenerateAblePlacement>();
        }
    }

    /// <summary>
    /// �V�[���r���[�ŃZ���N�^�𓮂������߂̃��\�b�h(�G�f�B�^��̃��C�����[�v)�B�f���Q�[�g�ɓo�^���邱�Ƃŗ��p����B
    /// </summary>
    /// <param name="sceneView"> ���݂̃V�[��(�f���Q�[�g����̂Ŏg��Ȃ�) </param>
    public void OnScene(SceneView sceneView)
    {
        if (((GameObject)GridEditorWindow.gridObject) == null)
        {
            return;
        }

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
        RaycastHit hit = new RaycastHit();
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        int mask = 1 << 6;

        var hits = Physics.RaycastAll(ray);
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        foreach (var hitObj in hits)
        {
            if (hitObj.collider.name == "AblePlacementAround" || hitObj.collider.name == "BlockDummy")
            {
                hit = hitObj;
                Debug.Log(hit);
                break;
            }
        }

        if (hit.collider == null)
        {
            Visualizer.DisableRenderer();
            return;
        }

        // �t�H�[�J�X���O���b�h�I�u�W�F�N�g�O�Ȃ�I��
        if (GridEditorWindow.gridObject != hit.transform.root.gameObject)
        {
            return;
        }

        // ���ς𗘗p���Ď�O���̃I�u�W�F�N�g�̃t�H�[�J�X�����
        if (Vector3.Dot(hit.normal, ray.direction) < 0)
        {
            // �T�[�t�F�X�r�W���A���C�U����������ĂȂ��Ȃ琶��
            if (Visualizer.selectedSurfaceVisualizer == null)
            {
                Visualizer.CreateVisualizer(VisualizerType.Surface);
            }

            if (Visualizer.prefabVisualizer == null && GridEditorWindow.obj != null)
            {
                Visualizer.CreateVisualizer(VisualizerType.Prefab);
            }

            if (toolMode == ToolMode.ToolPlace)
            {
                if (Visualizer.prefabVisualizer != null)
                {
                    if ((Visualizer.prefabVisualizer).transform.position != gridManager.gridPosFromIndex[
                            hit.collider.gameObject.GetComponent<GridRelatedInfo>().gridIndex - 1])
                    {
                        if (hit.collider.name == "AblePlacementAround")
                        {
                            Visualizer.MoveVisualizerPrefab(hit.transform.position + (hit.normal * 0.5f), Vector3.zero);
                        }
                        else if (hit.collider.name == "BlockDummy")
                        {
                            if (!gridManager.IsCheckGridEdgeFromHit(hit))
                            {
                                Visualizer.MoveVisualizerPrefab(gridManager.gridPosFromIndex[
                                    gridManager.ConjecturePlacementObjIndex(hit) - 1], Vector3.zero);
                            }
                        }
                    }
                }
            }

            // �������[�h�̎��͖ʂɐڂ��������ɃT�[�t�F�X�r�W���A���C�U�𐶐�
            if (toolMode == ToolMode.ToolErase)
            {
                if ((Visualizer.selectedSurfaceVisualizer).transform.position != gridManager.gridPosFromIndex[
                        hit.collider.gameObject.GetComponent<GridRelatedInfo>().gridIndex - 1])
                {
                    if (hit.collider.name == "AblePlacementAround")
                    {
                        Visualizer.MoveVisualizerSurface(hit.transform.position, -hit.normal);
                    }
                    else if (hit.collider.name == "BlockDummy")
                    {
                        Visualizer.MoveVisualizerSurface(hit.transform.position + (hit.normal * 0.5f), -hit.normal);
                    }
                }
            }
        }

        switch (selectMode)
        {
            #region OPERATION_CLICK

            case OperationMode.OperationClick: // �N���b�N���[�h
                //���N���b�N���ꂽ��:�ݒu���[�h
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 &&
                    toolMode == ToolMode.ToolPlace)
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<GridRelatedInfo>() != null &&
                            hit.collider.name == "AblePlacementAround")
                        {
                            index = hit.collider.GetComponent<GridRelatedInfo>().gridIndex;
                        }
                        else if (hit.collider.GetComponent<GridRelatedInfo>() != null &&
                                 hit.collider.name == "BlockDummy")
                        {
                            index = gridManager.ConjecturePlacementObjIndex(hit);
                        }

                        if (index == default || index <= 0 ||
                            index > gridManager.size.x * gridManager.size.y * gridManager.size.z)
                        {
                            return;
                        }

                        if (GridEditorWindow.obj == null)
                        {
                            return;
                        }

                        if (gridManager.IsCheckGridEdgeFromHit(hit) && hit.collider.name == "BlockDummy")
                        {
                            return;
                        }

                        ToolManager.SetActiveTool<CustomGridTools>();

                        var dummy = PrefabUtility.InstantiatePrefab(dummyBlock);
                        ((GameObject)dummy).transform.parent = ((GameObject)GridEditorWindow.gridObject).transform;
                        ((GameObject)dummy).transform.position = new Vector3(gridManager.gridPosFromIndex[index - 1].x,
                            gridManager.gridPosFromIndex[index - 1].y, gridManager.gridPosFromIndex[index - 1].z);
                        ((GameObject)dummy).GetComponent<GridRelatedInfo>().gridIndex = index;
                        //((GameObject)dummy).hideFlags = HideFlags.HideInHierarchy;
                        ((GameObject)dummy).GetComponent<MeshRenderer>().enabled = false;
                        Undo.RegisterCreatedObjectUndo(dummy, "placed prefab");

                        instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                        ((GameObject)instantiateBuffer).transform.parent =
                            ((GameObject)GridEditorWindow.gridObject).transform;
                        ((GameObject)instantiateBuffer).transform.position = new Vector3(
                            gridManager.gridPosFromIndex[index - 1].x, gridManager.gridPosFromIndex[index - 1].y,
                            gridManager.gridPosFromIndex[index - 1].z);

                        Undo.RecordObject(gridManager, "isPlaced Check");
                        gridManager.isPlaced[index - 1] = true;
                        Undo.RecordObject(gridManager, "Record PlacedObj");
                        gridManager.placedObjects[index - 1] = (GameObject)instantiateBuffer;
                        Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");
                    }
                }

                // ���N���b�N���ꂽ��:�������[�h
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 &&
                    toolMode == ToolMode.ToolErase)
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<GridRelatedInfo>() != null)
                        {
                            index = hit.collider.GetComponent<GridRelatedInfo>().gridIndex;
                        }

                        if (index < 1)
                        {
                            return;
                        }

                        if (hit.collider.name == "AblePlacementAround")
                        {
                            return;
                        }

                        Undo.RecordObject(gridManager, "isDelete Check");
                        gridManager.isPlaced[hit.collider.GetComponent<GridRelatedInfo>().gridIndex - 1] = false;
                        Undo.DestroyObjectImmediate(
                            gridManager.placedObjects[hit.collider.GetComponent<GridRelatedInfo>().gridIndex - 1]);
                        Undo.RecordObject(gridManager, "isDelete Check");
                        gridManager.placedObjects[hit.collider.GetComponent<GridRelatedInfo>().gridIndex - 1] = null;
                        Undo.DestroyObjectImmediate(hit.collider.transform.gameObject);
                    }
                }

                // �u�����[�h
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 &&
                    toolMode == ToolMode.ToolReplace)
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<GridRelatedInfo>() != null)
                        {
                            index = hit.collider.GetComponent<GridRelatedInfo>().gridIndex;
                        }

                        if (index < 1)
                        {
                            return;
                        }

                        if (hit.collider.name == "AblePlacementAround")
                        {
                            return;
                        }

                        // �u���O�I�u�W�F�N�g�̏���
                        Undo.DestroyObjectImmediate(
                            gridManager.placedObjects[hit.collider.GetComponent<GridRelatedInfo>().gridIndex - 1]);
                        Undo.RecordObject(gridManager, "isDelete Check");
                        gridManager.placedObjects[hit.collider.GetComponent<GridRelatedInfo>().gridIndex - 1] = null;

                        // �u����I�u�W�F�N�g�̐���
                        instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                        ((GameObject)instantiateBuffer).transform.parent =
                            ((GameObject)GridEditorWindow.gridObject).transform;
                        ((GameObject)instantiateBuffer).transform.position = new Vector3(
                            gridManager.gridPosFromIndex[index - 1].x, gridManager.gridPosFromIndex[index - 1].y,
                            gridManager.gridPosFromIndex[index - 1].z);
                        Undo.RecordObject(gridManager, "Record PlacedObj");
                        gridManager.placedObjects[index - 1] = (GameObject)instantiateBuffer;

                        Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");
                    }
                }

                break;

            #endregion

            #region OPERATION_DRAG

            case OperationMode.OperationDragFree: //�h���b�O���[�h
                // �}�E�X�{�^���������ꂽ��
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                {
                    isDrag = true;
                    indexBuffer = -1;
                    indexListTmp = new List<int>();
                    // �}�E�X�N���b�N�̃z�b�g�R���g���[�����Œ�
                    GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                    Event.current.Use();
                }

                // �}�E�X���h���b�O����Ă���ꍇ�͈ȉ��̏��������s(�}�E�X�N���b�N�_�E���̃C�x���g�͓��Y�t���[���ł����A�N�e�B�u�ɂȂ�Ȃ��̂Ńt���O�őJ��)
                // �ݒu���[�h
                if (isDrag && toolMode == ToolMode.ToolPlace)
                {
                    // �}�E�X�������ꂽ�ꍇ
                    if (currentEvent.type == EventType.MouseUp ||
                        currentEvent.type == EventType.MouseLeaveWindow && currentEvent.button == 0)
                    {
                        isDrag = false;

                        foreach (var tmp in indexListTmp)
                        {
                            var dummy = PrefabUtility.InstantiatePrefab(dummyBlock);
                            ((GameObject)dummy).transform.parent = ((GameObject)GridEditorWindow.gridObject).transform;
                            ((GameObject)dummy).transform.position = new Vector3(gridManager.gridPosFromIndex[tmp].x,
                                gridManager.gridPosFromIndex[tmp].y, gridManager.gridPosFromIndex[tmp].z);
                            ((GameObject)dummy).GetComponent<GridRelatedInfo>().gridIndex = tmp + 1;
                            //((GameObject)dummy).hideFlags = HideFlags.HideInHierarchy;
                            ((GameObject)dummy).GetComponent<MeshRenderer>().enabled = false;
                            Undo.RegisterCreatedObjectUndo(dummy, "placed prefab");
                        }

                        return;
                    }

                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<GridRelatedInfo>() != null &&
                            hit.collider.name == "AblePlacementAround")
                        {
                            index = hit.collider.GetComponent<GridRelatedInfo>().gridIndex;
                        }
                        else if (hit.collider.GetComponent<GridRelatedInfo>() != null &&
                                 hit.collider.name == "BlockDummy")
                        {
                            index = gridManager.ConjecturePlacementObjIndex(hit);
                        }

                        if (index == default || index <= 0 ||
                            index > gridManager.size.x * gridManager.size.y * gridManager.size.z)
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

                        if (gridManager.IsCheckGridEdgeFromHit(hit) && hit.collider.name == "BlockDummy")
                        {
                            return;
                        }

                        if (index != indexBuffer)
                        {
                            ToolManager.SetActiveTool<CustomGridTools>();

                            instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                            ((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
                            ((GameObject)instantiateBuffer).transform.position = new Vector3(
                                gridManager.gridPosFromIndex[index - 1].x, gridManager.gridPosFromIndex[index - 1].y,
                                gridManager.gridPosFromIndex[index - 1].z);
                            Undo.RecordObject(gridManager, "isPlaced Check");
                            gridManager.isPlaced[index - 1] = true;
                            indexListTmp.Add(index - 1);
                            gridManager.placedObjects[index - 1] = (GameObject)instantiateBuffer;
                            Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");
                            indexBuffer = index;
                        }
                    }
                }

                // �������[�h
                if (isDrag && toolMode == ToolMode.ToolErase)
                {
                    if (currentEvent.type == EventType.MouseUp ||
                        currentEvent.type == EventType.MouseLeaveWindow && currentEvent.button == 0)
                    {
                        isDrag = false;

                        for (int i = 0; i < ((GameObject)GridEditorWindow.gridObject).transform.childCount;)
                        {
                            if (((GameObject)GridEditorWindow.gridObject).transform.GetChild(i).name != "BlockDummy")
                            {
                                ++i;
                                continue;
                            }

                            if (indexListTmp.Contains(((GameObject)GridEditorWindow.gridObject).transform.GetChild(i)
                                    .GetComponent<GridRelatedInfo>().gridIndex))
                            {
                                Undo.DestroyObjectImmediate(((GameObject)GridEditorWindow.gridObject).transform
                                    .GetChild(i).gameObject);
                                continue;
                            }

                            ++i;
                        }

                        indexNumList.Clear();
                    }

                    // ���X�g��null�Ȃ�V��������
                    indexNumList ??= new List<int>();

                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<GridRelatedInfo>() != null)
                        {
                            index = hit.collider.GetComponent<GridRelatedInfo>().gridIndex;
                        }

                        if (index <= 0 || index > gridManager.size.x * gridManager.size.y * gridManager.size.z)
                        {
                            return;
                        }

                        if (hit.collider.name == "AblePlacementAround")
                        {
                            return;
                        }

                        if (index != indexBuffer &&
                            gridManager.placedObjects[hit.collider.GetComponent<GridRelatedInfo>().gridIndex - 1] !=
                            null)
                        {
                            Undo.RecordObject(gridManager, "isDelete Check");
                            gridManager.isPlaced[hit.collider.GetComponent<GridRelatedInfo>().gridIndex - 1] = false;
                            Undo.DestroyObjectImmediate(
                                gridManager.placedObjects[hit.collider.GetComponent<GridRelatedInfo>().gridIndex - 1]);
                            Undo.RecordObject(gridManager, "isDelete Check");
                            gridManager.placedObjects[hit.collider.GetComponent<GridRelatedInfo>().gridIndex - 1] =
                                null;
                            indexListTmp.Add(index);
                            indexBuffer = index;
                        }
                    }
                }

                // �u�����[�h
                if (isDrag && toolMode == ToolMode.ToolReplace)
                {
                    if (currentEvent.type == EventType.MouseUp ||
                        currentEvent.type == EventType.MouseLeaveWindow && currentEvent.button == 0)
                    {
                        isDrag = false;

                        indexNumList.Clear();
                    }

                    // ���X�g��null�Ȃ�V��������
                    indexNumList ??= new List<int>();

                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<GridRelatedInfo>() != null)
                        {
                            index = hit.collider.GetComponent<GridRelatedInfo>().gridIndex;
                        }

                        if (index <= 0 || index > gridManager.size.x * gridManager.size.y * gridManager.size.z)
                        {
                            return;
                        }

                        if (hit.collider.name == "AblePlacementAround")
                        {
                            return;
                        }

                        if (hit.collider == GridEditorWindow.obj)
                        {
                            return;
                        }

                        if (index != indexBuffer &&
                            !indexListTmp.Contains(hit.collider.GetComponent<GridRelatedInfo>().gridIndex))
                        {
                            // �u���O�I�u�W�F�N�g�̏���
                            Undo.RecordObject(gridManager, "isDelete Check");
                            Undo.DestroyObjectImmediate(
                                gridManager.placedObjects[hit.collider.GetComponent<GridRelatedInfo>().gridIndex - 1]);
                            Undo.RecordObject(gridManager, "isDelete Check");
                            gridManager.placedObjects[hit.collider.GetComponent<GridRelatedInfo>().gridIndex - 1] =
                                null;

                            // �u����I�u�W�F�N�g�̐���
                            instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                            ((GameObject)instantiateBuffer).transform.parent =
                                ((GameObject)GridEditorWindow.gridObject).transform;
                            ((GameObject)instantiateBuffer).transform.position = new Vector3(
                                gridManager.gridPosFromIndex[index - 1].x, gridManager.gridPosFromIndex[index - 1].y,
                                gridManager.gridPosFromIndex[index - 1].z);
                            Undo.RecordObject(gridManager, "Record PlacedObj");
                            gridManager.placedObjects[hit.collider.GetComponent<GridRelatedInfo>().gridIndex - 1] =
                                (GameObject)instantiateBuffer;
                            Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");

                            indexListTmp.Add(index);

                            indexBuffer = index;
                        }
                    }
                }

                break;

            #endregion

            #region OPERATION_RANGE

            case OperationMode.OperationRange: // �͈͑I�����[�h
                // �}�E�X�{�^���������ꂽ��
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
                {
                    rangeDragEndIndex = -1;

                    // �h���b�O�J�n�̎n�_�C���f�b�N�X���擾
                    if (hit.collider != null)
                    {
                        if (toolMode == ToolMode.ToolPlace &&
                            (hit.collider.GetComponent<GridRelatedInfo>().gridIndex > 0 &&
                             hit.collider.GetComponent<GridRelatedInfo>().gridIndex <=
                             gridManager.size.x * gridManager.size.y * gridManager.size.z))
                        {
                            rangeDragStartIndex = hit.collider.GetComponent<GridRelatedInfo>().gridIndex;
                        }
                        else if (toolMode == ToolMode.ToolErase)
                        {
                            rangeDragStartIndex = hit.collider.GetComponent<GridRelatedInfo>().gridIndex;
                        }
                        else if (toolMode == ToolMode.ToolReplace &&
                                 (hit.collider.GetComponent<GridRelatedInfo>().gridIndex > 0 &&
                                  hit.collider.GetComponent<GridRelatedInfo>().gridIndex <=
                                  gridManager.size.x * gridManager.size.y * gridManager.size.z))
                        {
                            rangeDragStartIndex = hit.collider.GetComponent<GridRelatedInfo>().gridIndex;
                        }

                        startIndexPos = gridManager.ReturnGridSquarePoint(rangeDragStartIndex);
                    }

                    isDrag = true;

                    // �}�E�X�N���b�N�̃z�b�g�R���g���[�����Œ�
                    GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                    Event.current.Use();
                }

                // �}�E�X���h���b�O����Ă���ꍇ�͈ȉ��̏��������s(�}�E�X�N���b�N�_�E���̃C�x���g�͓��Y�t���[���ł����A�N�e�B�u�ɂȂ�Ȃ��̂Ńt���O�őJ��)
                // �ݒu���[�h
                if (isDrag && toolMode == ToolMode.ToolPlace)
                {
                    // �}�E�X�������ꂽ�ꍇ
                    if (currentEvent.type == EventType.MouseUp ||
                        currentEvent.type == EventType.MouseLeaveWindow && currentEvent.button == 0)
                    {
                        ClearVisualizer();
                        if (hit.collider != null)
                        {
                            rangeDragEndIndex = hit.collider.GetComponent<GridRelatedInfo>().gridIndex;
                        }

                        if (rangeDragEndIndex == -1)
                        {
                            isDrag = false;
                            return;
                        }

                        endIndexPos = gridManager.ReturnGridSquarePoint(rangeDragEndIndex);
                        CreateBoxPlacement(startIndexPos, endIndexPos);
                        isDrag = false;
                        return;
                    }

                    if (hit.collider != null)
                    {
                        if (hit.collider.GetComponent<GridRelatedInfo>() != null)
                        {
                            index = hit.collider.GetComponent<GridRelatedInfo>().gridIndex;
                        }

                        if (index == default || index <= 0 ||
                            index > gridManager.size.x * gridManager.size.y * gridManager.size.z)
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
                            ClearVisualizer();
                            CreateVisualizerBox(startIndexPos,
                                gridManager.ReturnGridSquarePoint(
                                    hit.collider.GetComponent<GridRelatedInfo>().gridIndex));
                            indexBuffer = index;
                        }
                    }
                }

                // �������[�h
                if (isDrag && toolMode == ToolMode.ToolErase)
                {
                    // �}�E�X�������ꂽ�ꍇ
                    if (currentEvent.type == EventType.MouseUp ||
                        currentEvent.type == EventType.MouseLeaveWindow && currentEvent.button == 0)
                    {
                        ClearVisualizer();


                        if (hit.collider != null)
                        {
                            rangeDragEndIndex = hit.collider.GetComponent<GridRelatedInfo>().gridIndex;
                            endIndexPos = gridManager.ReturnGridSquarePoint(rangeDragEndIndex);
                        }

                        if (rangeDragEndIndex == -1)
                        {
                            isDrag = false;
                            return;
                        }

                        if (endIndexPos == new Vector3Int(-1, -1, -1))
                        {
                            isDrag = false;
                            return;
                        }

                        ConfirmDeleteObj(startIndexPos, endIndexPos);
                        isDrag = false;

                        indexNumList.Clear();
                        return;
                    }

                    // ���X�g��null�Ȃ�V��������
                    indexNumList ??= new List<int>();

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

                        if (index < 1 || index > gridManager.size.x * gridManager.size.y * gridManager.size.z)
                        {
                            return;
                        }

                        if (!gridManager.isPlaced[index - 1])
                        {
                            return;
                        }

                        if (index != indexBuffer)
                        {
                            CancelReservationObj();
                            ReservationDeleteObj(startIndexPos,
                                gridManager.ReturnGridSquarePoint(
                                    hit.collider.GetComponent<GridRelatedInfo>().gridIndex));
                            indexBuffer = index;
                        }
                    }
                }

                // �u�����[�h
                if (isDrag && toolMode == ToolMode.ToolReplace)
                {
                    // �}�E�X�������ꂽ�ꍇ
                    if (currentEvent.type == EventType.MouseUp ||
                        currentEvent.type == EventType.MouseLeaveWindow && currentEvent.button == 0)
                    {
                        ClearVisualizer();


                        if (hit.collider != null)
                        {
                            rangeDragEndIndex = hit.collider.GetComponent<GridRelatedInfo>().gridIndex;
                            endIndexPos = gridManager.ReturnGridSquarePoint(rangeDragEndIndex);
                        }

                        if (rangeDragEndIndex == -1)
                        {
                            isDrag = false;
                            return;
                        }

                        if (endIndexPos == new Vector3Int(-1, -1, -1))
                        {
                            isDrag = false;
                            return;
                        }

                        ConfirmReplaceObj(startIndexPos, endIndexPos);
                        isDrag = false;

                        indexNumList.Clear();
                        replaceReservation.Clear();
                        return;


                    }

                    // ���X�g��null�Ȃ�V��������
                    indexNumList ??= new List<int>();
                    replaceReservation ??= new List<(int, GameObject)>();

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

                        if (index < 1 || index > gridManager.size.x * gridManager.size.y * gridManager.size.z)
                        {
                            return;
                        }

                        if (!gridManager.isPlaced[index - 1])
                        {
                            return;
                        }

                        if (index != indexBuffer)
                        {
                            CancelReplaceReservationObj();
                            ReplaceReservationObj(startIndexPos,
                                gridManager.ReturnGridSquarePoint(
                                    hit.collider.GetComponent<GridRelatedInfo>().gridIndex));
                            indexBuffer = index;
                        }
                    }
                }

                break;

            #endregion OPERATION_RANGE
        }
    }


    /// <summary>
    /// �C�ӂ̃^�C�~���O�őS�Ẵr�W���A���C�U���������郁�\�b�h
    /// </summary>
    private void ClearVisualizer()
    {
        // Scene������type�����ɒT���Ă���B
        // GameObject���擾������MonoBehavior�ȂǂɕύX����΂��ꂪ����B
        var gameObjects = (GameObject[])Object.FindObjectsOfType(typeof(GameObject));

        foreach (var obj in gameObjects)
        {
            if (obj.name == "visualizeObj")
            {
                GameObject.DestroyImmediate(obj);
            }
        }
    }

    /// <summary>
    /// �͈͑I�����[�h�̎��ɑI������Ă����͈͂Ƀr�W���A���C�U��\�����郁�\�b�h
    /// </summary>
    private void CreateVisualizerBox(Vector3Int rangeStartIndex, Vector3Int rangeEndIndex)
    {
        if (rangeStartIndex.x > rangeEndIndex.x)
        {
            (rangeStartIndex.x, rangeEndIndex.x) = (rangeEndIndex.x, rangeStartIndex.x);
        }

        if (rangeStartIndex.y > rangeEndIndex.y)
        {
            (rangeStartIndex.y, rangeEndIndex.y) = (rangeEndIndex.y, rangeStartIndex.y);
        }

        if (rangeStartIndex.z > rangeEndIndex.z)
        {
            (rangeStartIndex.z, rangeEndIndex.z) = (rangeEndIndex.z, rangeStartIndex.z);
        }

        for (int i = 0; i < rangeEndIndex.z - rangeStartIndex.z + 1; i++)
        {
            for (int j = 0; j < rangeEndIndex.x - rangeStartIndex.x + 1; j++)
            {
                for (int k = 0; k < rangeEndIndex.y - rangeStartIndex.y + 1; k++)
                {
                    instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                    ((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
                    ((GameObject)instantiateBuffer).transform.position =
                        gridManager.gridPosFromIndexMultiple[rangeStartIndex.x, rangeStartIndex.y, rangeStartIndex.z] +
                        new Vector3((j) * 1.0f, (k) * 1.0f, (i) * 1.0f);
                    ((GameObject)instantiateBuffer).name = "visualizeObj";
                }
            }
        }
    }

    /// <summary>
    /// �͈͑I�����[�h�̎��ɑI������Ă����͈͂Ƀv���n�u��ݒu���郁�\�b�h
    /// </summary>
    private void CreateBoxPlacement(Vector3Int rangeStartIndex, Vector3Int rangeEndIndex)
    {
        Vector3Int size = gridManager.size;

        if (rangeStartIndex.x > rangeEndIndex.x)
        {
            (rangeStartIndex.x, rangeEndIndex.x) = (rangeEndIndex.x, rangeStartIndex.x);
        }

        if (rangeStartIndex.y > rangeEndIndex.y)
        {
            (rangeStartIndex.y, rangeEndIndex.y) = (rangeEndIndex.y, rangeStartIndex.y);
        }

        if (rangeStartIndex.z > rangeEndIndex.z)
        {
            (rangeStartIndex.z, rangeEndIndex.z) = (rangeEndIndex.z, rangeStartIndex.z);
        }

        for (int i = 0; i < rangeEndIndex.z - rangeStartIndex.z + 1; i++)
        {
            for (int j = 0; j < rangeEndIndex.x - rangeStartIndex.x + 1; j++)
            {
                for (int k = 0; k < rangeEndIndex.y - rangeStartIndex.y + 1; k++)
                {
                    var dummy = PrefabUtility.InstantiatePrefab(dummyBlock);
                    ((GameObject)dummy).transform.parent = ((GameObject)GridEditorWindow.gridObject).transform;
                    ((GameObject)dummy).transform.position = new Vector3(
                        gridManager.gridPosFromIndex[
                            (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                            (i * size.x * size.y) + (j * size.y) + k].x,
                        gridManager.gridPosFromIndex[
                            (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                            (i * size.x * size.y) + (j * size.y) + k].y,
                        gridManager.gridPosFromIndex[
                            (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                            (i * size.x * size.y) + (j * size.y) + k].z);
                    ((GameObject)dummy).GetComponent<GridRelatedInfo>().gridIndex =
                        (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                        (i * size.x * size.y) + (j * size.y) + k + 1;
                    //((GameObject)dummy).hideFlags = HideFlags.HideInHierarchy;
                    ((GameObject)dummy).GetComponent<MeshRenderer>().enabled = false;
                    Undo.RegisterCreatedObjectUndo(dummy, "placed prefab");

                    instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                    ((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
                    ((GameObject)instantiateBuffer).transform.position =
                        gridManager.gridPosFromIndexMultiple[rangeStartIndex.x, rangeStartIndex.y, rangeStartIndex.z] +
                        new Vector3((j) * 1.0f, (k) * 1.0f, (i) * 1.0f);
                    Undo.RecordObject(gridManager, "isPlaced Check");
                    gridManager.isPlaced[
                        (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                        (i * size.x * size.y) + (j * size.y) + k] = true;
                    gridManager.placedObjects[
                        (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                        (i * size.x * size.y) + (j * size.y) + k] = (GameObject)instantiateBuffer;
                    Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");
                }
            }
        }
    }

    /// <summary>
    /// �͈͑I�����[�h�Őݒu�����I�u�W�F�N�g�ɐݒu�\�����t�^���A�s�v�Ȑݒu�\�������菜�����\�b�h
    /// </summary>
    /// <param name="rangeStartIndex"> �I��͈͎n�_ </param>
    /// <param name="rangeEndIndex"> �I��͈͏I�_ </param>
    private void AddPlacementAndDeleteConjecture(Vector3Int rangeStartIndex, Vector3Int rangeEndIndex)
    {
        Vector3Int size = gridManager.size;

        if (rangeStartIndex.x > rangeEndIndex.x)
        {
            (rangeStartIndex.x, rangeEndIndex.x) = (rangeEndIndex.x, rangeStartIndex.x);
        }

        if (rangeStartIndex.y > rangeEndIndex.y)
        {
            (rangeStartIndex.y, rangeEndIndex.y) = (rangeEndIndex.y, rangeStartIndex.y);
        }

        if (rangeStartIndex.z > rangeEndIndex.z)
        {
            (rangeStartIndex.z, rangeEndIndex.z) = (rangeEndIndex.z, rangeStartIndex.z);
        }

        for (int i = 0; i < rangeEndIndex.z - rangeStartIndex.z + 1; i++)
        {
            for (int j = 0; j < rangeEndIndex.x - rangeStartIndex.x + 1; j++)
            {
                for (int k = 0; k < rangeEndIndex.y - rangeStartIndex.y + 1; k++)
                {
                    if (i > 0 && i < rangeEndIndex.z - rangeStartIndex.z && j > 0 &&
                        j < rangeEndIndex.x - rangeStartIndex.x && k > 0 && k < rangeEndIndex.y - rangeStartIndex.y)
                    {
                        continue;
                    }

                    placementArea.AddPlaceMentArea((rangeStartIndex.z * size.x * size.y) +
                                                   (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                                                   (i * size.x * size.y) + (j * size.y) + k + 1);
                }
            }
        }

        // 
        for (int i = 0; i < rangeEndIndex.z - rangeStartIndex.z + 1; i++)
        {
            for (int j = 0; j < rangeEndIndex.x - rangeStartIndex.x + 1; j++)
            {
                for (int k = 0; k < rangeEndIndex.y - rangeStartIndex.y + 1; k++)
                {
                    placementArea.DeletePlacementArea((rangeStartIndex.z * size.x * size.y) +
                                                      (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                                                      (i * size.x * size.y) + (j * size.y) + k + 1);
                }
            }
        }
    }

    /// <summary>
    /// �͈͑I���������\��̃I�u�W�F�N�g�̃��b�V�������_���[���������\�b�h
    /// </summary>
    private void ReservationDeleteObj(Vector3Int rangeStartIndex, Vector3Int rangeEndIndex)
    {
        Vector3Int size = gridManager.size;

        if (rangeStartIndex.x > rangeEndIndex.x)
        {
            (rangeStartIndex.x, rangeEndIndex.x) = (rangeEndIndex.x, rangeStartIndex.x);
        }

        if (rangeStartIndex.y > rangeEndIndex.y)
        {
            (rangeStartIndex.y, rangeEndIndex.y) = (rangeEndIndex.y, rangeStartIndex.y);
        }

        if (rangeStartIndex.z > rangeEndIndex.z)
        {
            (rangeStartIndex.z, rangeEndIndex.z) = (rangeEndIndex.z, rangeStartIndex.z);
        }

        for (int i = 0; i < rangeEndIndex.z - rangeStartIndex.z + 1; i++)
        {
            for (int j = 0; j < rangeEndIndex.x - rangeStartIndex.x + 1; j++)
            {
                for (int k = 0; k < rangeEndIndex.y - rangeStartIndex.y + 1; k++)
                {
                    if (gridManager.placedObjects[
                            (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                            (i * size.x * size.y) + (j * size.y) + k] == null ||
                        ((rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                            (i * size.x * size.y) + (j * size.y) + k - 1 < 0 && (rangeStartIndex.z * size.x * size.y) +
                            (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                            (i * size.x * size.y) + (j * size.y) + k - 1 >= size.x * size.y * size.z))
                    {
                        continue;
                    }

                    if (gridManager
                            .placedObjects[
                                (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) +
                                rangeStartIndex.y + (i * size.x * size.y) + (j * size.y) + k].transform.parent ==
                        null)
                    {
                        continue;
                    }

                    if (!gridManager
                            .placedObjects[
                                (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) +
                                rangeStartIndex.y + (i * size.x * size.y) + (j * size.y) + k].gameObject.activeSelf)
                    {
                        continue;
                    }

                    if (gridManager.placedObjects[
                                (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) +
                                rangeStartIndex.y +
                                (i * size.x * size.y) + (j * size.y) + k].gameObject.transform
                            .GetComponent<MeshRenderer>() != null)
                    {
                        Undo.RecordObject(gridManager.placedObjects[
                                (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) +
                                rangeStartIndex.y +
                                (i * size.x * size.y) + (j * size.y) + k].gameObject.transform
                            .GetComponent<MeshRenderer>(), "invisible mesh");

                        gridManager.placedObjects[
                                (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) +
                                rangeStartIndex.y + (i * size.x * size.y) + (j * size.y) + k].gameObject.transform
                            .GetComponent<MeshRenderer>().enabled = false;
                    }
                    else
                    {
                        Undo.RecordObject(gridManager.placedObjects[
                            (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) +
                            rangeStartIndex.y +
                            (i * size.x * size.y) + (j * size.y) + k].gameObject, "invisible mesh");

                        gridManager.placedObjects[
                            (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) +
                            rangeStartIndex.y +
                            (i * size.x * size.y) + (j * size.y) + k].SetActive(false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// �͈͑I������O�ꂽ�I�u�W�F�N�g�̃��b�V�������_���[��enable�ɂ��郁�\�b�h
    /// </summary>
    private void CancelReservationObj()
    {
        foreach (var placedObject in gridManager.placedObjects)
        {
            if (placedObject == null)
            {
                continue;
            }

            if (placedObject.GetComponent<Renderer>() != null)
            {
                if (!placedObject.GetComponent<Renderer>().enabled)
                {
                    placedObject.GetComponent<Renderer>().enabled = true;
                }
            }
            else
            {
                placedObject.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// �����m��̏ꍇ�ɃI�u�W�F�N�g���������郁�\�b�h
    /// </summary>
    private void ConfirmDeleteObj(Vector3Int rangeStartIndex, Vector3Int rangeEndIndex)
    {
        Vector3Int size = gridManager.size;
        List<int> deleteIndexList = new List<int>();
        List<GameObject> dummyObj = new List<GameObject>();

        if (rangeStartIndex.x > rangeEndIndex.x)
        {
            (rangeStartIndex.x, rangeEndIndex.x) = (rangeEndIndex.x, rangeStartIndex.x);
        }

        if (rangeStartIndex.y > rangeEndIndex.y)
        {
            (rangeStartIndex.y, rangeEndIndex.y) = (rangeEndIndex.y, rangeStartIndex.y);
        }

        if (rangeStartIndex.z > rangeEndIndex.z)
        {
            (rangeStartIndex.z, rangeEndIndex.z) = (rangeEndIndex.z, rangeStartIndex.z);
        }

        for (int i = 0; i < rangeEndIndex.z - rangeStartIndex.z + 1; i++)
        {
            for (int j = 0; j < rangeEndIndex.x - rangeStartIndex.x + 1; j++)
            {
                for (int k = 0; k < rangeEndIndex.y - rangeStartIndex.y + 1; k++)
                {
                    if (gridManager.placedObjects[
                            (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                            (i * size.x * size.y) + (j * size.y) + k] == null)
                    {
                        continue;
                    }

                    deleteIndexList.Add((rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) +
                                        rangeStartIndex.y + (i * size.x * size.y) + (j * size.y) + k + 1);

                    Undo.DestroyObjectImmediate(gridManager.placedObjects[
                        (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                        (i * size.x * size.y) + (j * size.y) + k]);
                    Undo.RecordObject(gridManager, "Delete PlacedFlag");
                    gridManager.isPlaced[
                        (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                        (i * size.x * size.y) + (j * size.y) + k] = false;
                    Undo.RecordObject(gridManager, "Delete PlacedFlag");
                    gridManager.placedObjects[
                        (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                        (i * size.x * size.y) + (j * size.y) + k] = null;
                    indexNumList.Add(((rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) +
                                      rangeStartIndex.y + (i * size.x * size.y) + (j * size.y) + k));
                }
            }
        }

        for (int i = 0; i < ((GameObject)GridEditorWindow.gridObject).transform.childCount;)
        {
            if (((GameObject)GridEditorWindow.gridObject).transform.GetChild(i).name != "BlockDummy")
            {
                ++i;
                continue;
            }

            if (deleteIndexList.Contains(((GameObject)GridEditorWindow.gridObject).transform.GetChild(i)
                    .GetComponent<GridRelatedInfo>().gridIndex))
            {
                Undo.DestroyObjectImmediate(((GameObject)GridEditorWindow.gridObject).transform.GetChild(i).gameObject);
                continue;
            }

            ++i;
        }
    }

    private void ReplaceReservationObj(Vector3Int rangeStartIndex, Vector3Int rangeEndIndex)
    {
        Vector3Int size = gridManager.size;

        if (rangeStartIndex.x > rangeEndIndex.x)
        {
            (rangeStartIndex.x, rangeEndIndex.x) = (rangeEndIndex.x, rangeStartIndex.x);
        }

        if (rangeStartIndex.y > rangeEndIndex.y)
        {
            (rangeStartIndex.y, rangeEndIndex.y) = (rangeEndIndex.y, rangeStartIndex.y);
        }

        if (rangeStartIndex.z > rangeEndIndex.z)
        {
            (rangeStartIndex.z, rangeEndIndex.z) = (rangeEndIndex.z, rangeStartIndex.z);
        }

        for (int i = 0; i < rangeEndIndex.z - rangeStartIndex.z + 1; i++)
        {
            for (int j = 0; j < rangeEndIndex.x - rangeStartIndex.x + 1; j++)
            {
                for (int k = 0; k < rangeEndIndex.y - rangeStartIndex.y + 1; k++)
                {
                    //instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                    //((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
                    //((GameObject)instantiateBuffer).transform.position =
                    //    gridManager.gridPosFromIndexMultiple[rangeStartIndex.x, rangeStartIndex.y, rangeStartIndex.z] +
                    //    new Vector3((j) * 1.0f, (k) * 1.0f, (i) * 1.0f);
                    //((GameObject)instantiateBuffer).name = "visualizeObj";

                    if (gridManager.placedObjects[
                                (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) +
                                rangeStartIndex.y +
                                (i * size.x * size.y) + (j * size.y) + k].gameObject.transform
                            .GetComponent<MeshRenderer>() != null)
                    {
                        Undo.RecordObject(gridManager.placedObjects[
                                (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) +
                                rangeStartIndex.y +
                                (i * size.x * size.y) + (j * size.y) + k].gameObject.transform
                            .GetComponent<MeshRenderer>(), "invisible mesh");

                        gridManager.placedObjects[
                                (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) +
                                rangeStartIndex.y + (i * size.x * size.y) + (j * size.y) + k].gameObject.transform
                            .GetComponent<MeshRenderer>().enabled = false;

                        var go = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                        ((GameObject)go).gameObject.transform.position = gridManager.placedObjects[
                            (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) +
                            rangeStartIndex.y +
                            (i * size.x * size.y) + (j * size.y) + k].gameObject.transform.position;

                        var a = ((rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) +
                                 rangeStartIndex.y + (i * size.x * size.y) + (j * size.y) + k, (GameObject)go);

                        replaceReservation.Add(a);
                    }
                    else
                    {
                        Undo.RecordObject(gridManager.placedObjects[
                            (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) +
                            rangeStartIndex.y +
                            (i * size.x * size.y) + (j * size.y) + k].gameObject, "invisible mesh");

                        gridManager.placedObjects[
                            (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) +
                            rangeStartIndex.y +
                            (i * size.x * size.y) + (j * size.y) + k].SetActive(false);

                        var go = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                        ((GameObject)go).gameObject.transform.position = gridManager.placedObjects[
                            (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) +
                            rangeStartIndex.y +
                            (i * size.x * size.y) + (j * size.y) + k].gameObject.transform.position;

                        var a = ((rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) +
                                 rangeStartIndex.y + (i * size.x * size.y) + (j * size.y) + k, (GameObject)go);

                        replaceReservation.Add(a);
                    }
                }
            }
        }
    }

    private void CancelReplaceReservationObj()
    {
        foreach (var placedObject in gridManager.placedObjects)
        {
            if (placedObject == null)
            {
                continue;
            }

            if (placedObject.GetComponent<Renderer>() != null)
            {
                if (!placedObject.GetComponent<Renderer>().enabled)
                {
                    placedObject.GetComponent<Renderer>().enabled = true;
                }

            }
            else
            {
                placedObject.gameObject.SetActive(true);
            }
        }

        foreach (var valueTuple in replaceReservation)
        {
            Undo.DestroyObjectImmediate(valueTuple.Item2);
        }
        
        replaceReservation.Clear();
    }

    private void ConfirmReplaceObj(Vector3Int rangeStartIndex, Vector3Int rangeEndIndex)
    {
        Vector3Int size = gridManager.size;
        List<int> deleteIndexList = new List<int>();
        List<GameObject> dummyObj = new List<GameObject>();

        if (rangeStartIndex.x > rangeEndIndex.x)
        {
            (rangeStartIndex.x, rangeEndIndex.x) = (rangeEndIndex.x, rangeStartIndex.x);
        }

        if (rangeStartIndex.y > rangeEndIndex.y)
        {
            (rangeStartIndex.y, rangeEndIndex.y) = (rangeEndIndex.y, rangeStartIndex.y);
        }

        if (rangeStartIndex.z > rangeEndIndex.z)
        {
            (rangeStartIndex.z, rangeEndIndex.z) = (rangeEndIndex.z, rangeStartIndex.z);
        }

        for (int i = 0; i < rangeEndIndex.z - rangeStartIndex.z + 1; i++)
        {
            for (int j = 0; j < rangeEndIndex.x - rangeStartIndex.x + 1; j++)
            {
                for (int k = 0; k < rangeEndIndex.y - rangeStartIndex.y + 1; k++)
                {
                    if (gridManager.placedObjects[
                            (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                            (i * size.x * size.y) + (j * size.y) + k] == null)
                    {
                        continue;
                    }

                    deleteIndexList.Add((rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) +
                                        rangeStartIndex.y + (i * size.x * size.y) + (j * size.y) + k + 1);

                    Undo.DestroyObjectImmediate(gridManager.placedObjects[
                        (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                        (i * size.x * size.y) + (j * size.y) + k]);

                    //Undo.RecordObject(gridManager, "Delete PlacedFlag");
                    //gridManager.isPlaced[
                    //    (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                    //    (i * size.x * size.y) + (j * size.y) + k] = false;
                    //Undo.RecordObject(gridManager, "Delete PlacedFlag");
                    //gridManager.placedObjects[
                    //    (rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) + rangeStartIndex.y +
                    //    (i * size.x * size.y) + (j * size.y) + k] = null;
                    //indexNumList.Add(((rangeStartIndex.z * size.x * size.y) + (rangeStartIndex.x * size.y) +
                    //                  rangeStartIndex.y + (i * size.x * size.y) + (j * size.y) + k));
                }
            }
        }

        foreach (var valueTuple in replaceReservation)
        {
            valueTuple.Item2.transform.parent = ((GameObject)GridEditorWindow.gridObject).transform;

            gridManager.placedObjects[
                valueTuple.Item1] = valueTuple.Item2;
        }
    }
}

#endif // UNITY_EDITOR