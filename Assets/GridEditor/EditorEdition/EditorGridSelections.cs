#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// �I�����[�h�p�񋓑�
/// </summary>
public enum OperationMode
{
    OperationClick = 0,
    OperationDragFree,
    OperationRange,
}

/// <summary>
/// �c�[�����[�h�p�񋓑�
/// </summary>
public enum ToolMode
{
    ToolPlace = 0,
    ToolErase,
    ToolReplace,
}

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

    // �h���b�O���Ă��邩�ǂ����̃t���O
    private bool isDrag = false;

    // �r�W���A���C�U�I�u�W�F�N�g
    private Object visualizeObj;

    // �͈̓h���b�O�̎n�_
    private Vector3 startRangeDrag;

    // �͈̓h���b�O�̏I�_
    private Vector3 endRangeDrag;

    // �_�~�[�u���b�N�̃o�b�t�@
    private GameObject dummyBlock;

    // �h���b�O�������I�u�W�F�N�g�i�[���X�g
    private List<GameObject> placedList;

    // �h���b�O�J�n���̃N���b�N�ŐG�����I�u�W�F�N�g�̖@�����擾
    private Vector3 clickedHitNormal;

    // �h���b�O�����d�����h�~�I�u�W�F�N�g���W�i�[���X�g
    private List<Vector3> deniedLMultiPlacedList;

    // �h���b�O�������h�~�I�u�W�F�N�g���W�i�[���X�g
    private List<Vector3> deniedDestroyObjectList;

    // �I�����W�o�b�t�@
    private Vector3 focusPosBuffer;

    // �����\�񃊃X�g
    private List<GameObject> deleteReservationList;

    /// <summary>
    /// �R���X�g���N�^ �A�Z�b�g�̏������������Ă�
    /// </summary>
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
        // �O���b�h�I�u�W�F�N�g���Ȃ��Ȃ�return
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

        RaycastHit hit = new RaycastHit();
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        var hits = Physics.RaycastAll(ray);
        System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        foreach (var hitObj in hits)
        {
            if (hitObj.collider.name == "AblePlacementAround" || hitObj.collider.name == "BlockDummy")
            {
                hit = hitObj;
                break;
            }
        }

        // �Ⴊ���ɂ��������ĂȂ�������return
        if (hit.collider == null)
        {
            Visualizer.DisableRenderer();
            return;
        }

        // �t�H�[�J�X���O���b�h�I�u�W�F�N�g�O�A�����͋N�������I�u�W�F�N�g��null�Ȃ�I��
        if (GridEditorWindow.gridObject != hit.transform.root.gameObject && GridEditorWindow.obj == null)
        {
            return;
        }

        // �ݒu���[�h�ŋN�������I�u�W�F�N�g��null�Ȃ�return
        if ((toolMode == ToolMode.ToolPlace || toolMode == ToolMode.ToolReplace) && GridEditorWindow.obj == null)
        {
            return;
        }

        // �͈͊O�Ȃ�ݒu���Ȃ�
        if (hit.collider.name != "AblePlacementAround" && toolMode == ToolMode.ToolPlace)
        {
            if (hit.collider.gameObject.transform.position.x < gridManager.gridScale / 2 || hit.collider.gameObject.transform.position.x > (gridManager.gridScale * gridManager.size.x) - gridManager.gridScale / 2 ||
                hit.collider.gameObject.transform.position.y < gridManager.gridScale / 2 || hit.collider.gameObject.transform.position.y > (gridManager.gridScale * gridManager.size.y) - gridManager.gridScale / 2 ||
                hit.collider.gameObject.transform.position.z < gridManager.gridScale / 2 || hit.collider.gameObject.transform.position.z > (gridManager.gridScale * gridManager.size.z) - gridManager.gridScale / 2)
            {
                return;
            }
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

            // �ݒu���[�h���̓I�u�W�F�N�g�̃r�W���A���C�U�𐶐�
            if (toolMode == ToolMode.ToolPlace)
            {
                if (Visualizer.prefabVisualizer != null)
                {
                    if (hit.collider.name == "AblePlacementAround")
                    {
                        Visualizer.MoveVisualizerPrefab(
                            hit.transform.position + (hit.normal * gridManager.gridScale / 2), Vector3.zero);
                    }
                    else if (hit.collider.name == "BlockDummy")
                    {
                        Visualizer.MoveVisualizerPrefab(hit.transform.position + (hit.normal * gridManager.gridScale),
                            Vector3.zero);
                    }
                }
            }

            // �������[�h�̎��͖ʂɐڂ��������ɃT�[�t�F�X�r�W���A���C�U�𐶐�
            if (toolMode == ToolMode.ToolErase)
            {
                if (hit.collider.name == "AblePlacementAround")
                {
                    Visualizer.MoveVisualizerSurface(hit.transform.position, -hit.normal);
                }
                else if (hit.collider.name == "BlockDummy")
                {
                    Visualizer.MoveVisualizerSurface((hit.transform.position + hit.normal * gridManager.gridScale / 2),
                        -hit.normal);
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
                    if (hit.collider != null && GridEditorWindow.obj != null)
                    {

                        Vector3 addPos;

                        // �ݒu�\�I�u�W�F�N�g�ɐG��ĂȂ��ꍇ��return
                        if (hit.collider.name != "AblePlacementAround" && hit.collider.name != "BlockDummy")
                        {
                            return;
                        }
                        else
                        {
                            if (hit.collider.name == "AblePlacementAround")
                            {
                                addPos = hit.collider.transform.position + (hit.normal * gridManager.gridScale) / 2;
                            }
                            else
                            {
                                addPos = hit.collider.transform.position + (hit.normal * gridManager.gridScale);
                            }
                        }

                        // �A�N�e�B�u�ȑ���c�[����ύX
                        ToolManager.SetActiveTool<CustomGridTools>();

                        instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                        ((GameObject)instantiateBuffer).transform.parent =
                            ((GameObject)GridEditorWindow.gridObject).transform;

                        ((GameObject)instantiateBuffer).transform.position = Vector3.zero;
                        ((GameObject)instantiateBuffer).transform.position += addPos;

                        Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");

                        Undo.RecordObject(gridManager, "isPlaced Check");
                        gridManager.inGridObjects.Add(((GameObject)instantiateBuffer));

                        var dummy = PrefabUtility.InstantiatePrefab(dummyBlock);
                        ((GameObject)dummy).transform.parent = ((GameObject)GridEditorWindow.gridObject).transform;
                        ((GameObject)dummy).transform.position += addPos;
                        ((GameObject)dummy).hideFlags = HideFlags.HideInHierarchy;
                        ((GameObject)dummy).GetComponent<MeshRenderer>().enabled = false;
                        ((GameObject)dummy).transform.parent = ((GameObject)instantiateBuffer).transform;
                        Undo.RegisterCreatedObjectUndo(dummy, "placed prefab");
                    }

                    return;
                }

                // ���N���b�N���ꂽ��:�������[�h
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 &&
                    toolMode == ToolMode.ToolErase)
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.name == "AblePlacementAround")
                        {
                            return;
                        }

                        // �A�N�e�B�u�ȑ���c�[����ύX
                        ToolManager.SetActiveTool<CustomGridTools>();

                        Undo.RecordObject(gridManager, "isRemoved Check");
                        gridManager.inGridObjects.Remove(hit.collider.transform.gameObject);

                        foreach (Transform child in hit.collider.transform)
                        {
                            if (child.name == "BlockDummy")
                            {
                                Undo.DestroyObjectImmediate(hit.collider.transform.gameObject);
                            }
                        }

                        if (hit.collider.name == "BlockDummy")
                        {
                            Undo.DestroyObjectImmediate(hit.collider.transform.parent.gameObject);
                        }
                    }
                }

                // �u�����[�h
                if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0 &&
                    toolMode == ToolMode.ToolReplace)
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.name == "AblePlacementAround")
                        {
                            return;
                        }

                        // �A�N�e�B�u�ȑ���c�[����ύX
                        ToolManager.SetActiveTool<CustomGridTools>();

                        Vector3 currentPos = hit.collider.transform.position;

                        Undo.RecordObject(gridManager, "isRemoved Check");
                        gridManager.inGridObjects.Remove(hit.collider.transform.gameObject);

                        // �u���O�I�u�W�F�N�g�̏���
                        foreach (Transform child in hit.collider.transform)
                        {
                            if (child.name == "BlockDummy")
                            {
                                Undo.DestroyObjectImmediate(hit.collider.transform.gameObject);
                            }
                        }

                        if (hit.collider.name == "BlockDummy")
                        {
                            Undo.DestroyObjectImmediate(hit.collider.transform.parent.gameObject);
                        }

                        // �u����I�u�W�F�N�g�̐���
                        instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                        ((GameObject)instantiateBuffer).transform.parent =
                            ((GameObject)GridEditorWindow.gridObject).transform;

                        ((GameObject)instantiateBuffer).transform.position = currentPos;

                        Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");

                        Undo.RecordObject(gridManager, "isPlaced Check");
                        gridManager.inGridObjects.Add(((GameObject)instantiateBuffer));

                        var dummy = PrefabUtility.InstantiatePrefab(dummyBlock);
                        ((GameObject)dummy).transform.parent = ((GameObject)GridEditorWindow.gridObject).transform;
                        ((GameObject)dummy).transform.position = currentPos;
                        ((GameObject)dummy).hideFlags = HideFlags.HideInHierarchy;
                        ((GameObject)dummy).GetComponent<MeshRenderer>().enabled = false;
                        ((GameObject)dummy).transform.parent = ((GameObject)instantiateBuffer).transform;
                        Undo.RegisterCreatedObjectUndo(dummy, "placed prefab");
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
                    deniedDestroyObjectList ??= new List<Vector3>();
                    deniedLMultiPlacedList ??= new List<Vector3>();
                    placedList ??= new List<GameObject>();
                    clickedHitNormal = hit.normal;

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

                        foreach (var tmp in placedList)
                        {
                            var dummy = PrefabUtility.InstantiatePrefab(dummyBlock);
                            ((GameObject)dummy).transform.parent = ((GameObject)GridEditorWindow.gridObject).transform;
                            ((GameObject)dummy).transform.position = tmp.transform.position;
                            ((GameObject)dummy).hideFlags = HideFlags.HideInHierarchy;
                            ((GameObject)dummy).GetComponent<MeshRenderer>().enabled = false;
                            ((GameObject)dummy).transform.parent = tmp.transform;
                            Undo.RegisterCreatedObjectUndo(dummy, "placed prefab");
                        }

                        placedList.Clear();
                        deniedLMultiPlacedList.Clear();
                        return;
                    }

                    if (hit.collider != null)
                    {
                        Vector3 addPos;

                        // �ݒu�\�I�u�W�F�N�g�ɐG��ĂȂ��ꍇ��return
                        if (hit.collider.name != "AblePlacementAround" && hit.collider.name != "BlockDummy")
                        {
                            return;
                        }
                        else
                        {
                            if (hit.collider.name == "AblePlacementAround")
                            {
                                addPos = hit.collider.transform.position + (hit.normal * gridManager.gridScale / 2);
                            }
                            else
                            {
                                addPos = hit.collider.transform.position + (hit.normal * gridManager.gridScale);
                            }
                        }

                        // ���ɐ�������Ă�����return
                        foreach (var elem in deniedLMultiPlacedList)
                        {
                            if (addPos == elem)
                            {
                                return;
                            }
                        }

                        // �O��Ɠ����I�u�W�F�N�g�łȂ���ΐݒu����
                        if (hit.collider.gameObject != instantiateBuffer)
                        {
                            ToolManager.SetActiveTool<CustomGridTools>();

                            instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                            ((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
                            ((GameObject)instantiateBuffer).transform.position = Vector3.zero;
                            ((GameObject)instantiateBuffer).transform.position += addPos;
                            deniedLMultiPlacedList.Add(((GameObject)instantiateBuffer).transform.position);
                            placedList.Add((GameObject)instantiateBuffer);
                            Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");

                            Undo.RecordObject(gridManager, "isPlaced Check");
                            gridManager.inGridObjects.Add(((GameObject)instantiateBuffer));
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
                        isDrag = false;
                        deniedDestroyObjectList.Clear();
                    }


                    if (hit.collider != null)
                    {
                        if (hit.collider.name == "AblePlacementAround")
                        {
                            return;
                        }

                        // �@���̌������Ⴄ�ꍇ��return
                        if (clickedHitNormal != hit.normal)
                        {
                            return;
                        }

                        // �����h�~����Ă�����W�̃I�u�W�F�N�g���������Ƃ��Ă���ꍇ��return
                        foreach (var elem in deniedDestroyObjectList)
                        {
                            if (hit.collider.transform.position + (hit.normal * gridManager.gridScale) == elem)
                            {
                                return;
                            }
                        }

                        foreach (Transform child in hit.collider.transform)
                        {
                            if (child.name == "BlockDummy")
                            {
                                Undo.RecordObject(gridManager, "isRemoved Check");
                                gridManager.inGridObjects.Remove(hit.collider.transform.gameObject);

                                deniedDestroyObjectList.Add(hit.collider.transform.position +
                                                            (hit.normal * gridManager.gridScale));
                                Undo.DestroyObjectImmediate(hit.collider.transform.gameObject);
                            }
                        }

                        if (hit.collider.name == "BlockDummy")
                        {
                            Undo.RecordObject(gridManager, "isRemoved Check");
                            gridManager.inGridObjects.Remove(hit.collider.transform.gameObject);
                            Undo.DestroyObjectImmediate(hit.collider.transform.parent.gameObject);
                        }
                    }
                }

                // �u�����[�h
                if (isDrag && toolMode == ToolMode.ToolReplace)
                {
                    // �}�E�X�������ꂽ��
                    if (currentEvent.type == EventType.MouseUp ||
                        currentEvent.type == EventType.MouseLeaveWindow && currentEvent.button == 0)
                    {
                        isDrag = false;
                    }

                    if (hit.collider != null)
                    {
                        if (hit.collider.name == "AblePlacementAround")
                        {
                            return;
                        }

                        if (hit.collider == GridEditorWindow.obj)
                        {
                            return;
                        }

                        // �O��Ɠ����I�u�W�F�N�g�łȂ���ΐݒu����
                        if (hit.collider.gameObject != instantiateBuffer)
                        {
                            // �u���O�I�u�W�F�N�g�̏���
                            foreach (Transform child in hit.collider.transform)
                            {
                                if (child.name == "BlockDummy")
                                {
                                    Undo.RecordObject(gridManager, "isRemoved Check");
                                    gridManager.inGridObjects.Remove(hit.collider.transform.gameObject);
                                    Undo.DestroyObjectImmediate(hit.collider.transform.gameObject);
                                }
                            }

                            if (hit.collider.name == "BlockDummy")
                            {
                                Undo.RecordObject(gridManager, "isRemoved Check");
                                gridManager.inGridObjects.Remove(hit.collider.transform.gameObject);
                                Undo.DestroyObjectImmediate(hit.collider.transform.parent.gameObject);
                            }

                            Vector3 currentPos = hit.collider.transform.position;

                            if (hit.collider.gameObject != instantiateBuffer)
                            {
                                // �u����I�u�W�F�N�g�̐���
                                instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                                ((GameObject)instantiateBuffer).transform.parent =
                                    ((GameObject)GridEditorWindow.gridObject).transform;

                                ((GameObject)instantiateBuffer).transform.position = currentPos;

                                Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");

                                Undo.RecordObject(gridManager, "isPlaced Check");
                                gridManager.inGridObjects.Add(((GameObject)instantiateBuffer));

                                var dummy = PrefabUtility.InstantiatePrefab(dummyBlock);
                                ((GameObject)dummy).transform.parent =
                                    ((GameObject)GridEditorWindow.gridObject).transform;
                                ((GameObject)dummy).transform.position = currentPos;
                                ((GameObject)dummy).hideFlags = HideFlags.HideInHierarchy;
                                ((GameObject)dummy).GetComponent<MeshRenderer>().enabled = false;
                                ((GameObject)dummy).transform.parent = ((GameObject)instantiateBuffer).transform;
                                Undo.RegisterCreatedObjectUndo(dummy, "placed prefab");
                            }
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
                    // �h���b�O�J�n�̎n�_�C���f�b�N�X���擾
                    if (hit.collider != null)
                    {
                        // �ݒu���[�h�Ȃ�u���b�N�ݒu�\����W
                        if (toolMode == ToolMode.ToolPlace || toolMode == ToolMode.ToolReplace)
                        {
                            if (hit.collider.name == "AblePlacementAround")
                            {
                                startRangeDrag = hit.collider.transform.position +
                                                 (hit.normal * gridManager.gridScale) / 2;
                            }
                            else
                            {
                                startRangeDrag = hit.collider.transform.position + (hit.normal * gridManager.gridScale);
                            }
                        }
                        else if (toolMode == ToolMode.ToolErase) // �������[�h�Ȃ�����u���b�N�̎n�_
                        {
                            startRangeDrag = hit.collider.transform.position;
                        }
                    }

                    deleteReservationList = new List<GameObject>();

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

                        if (hit.collider.name == "AblePlacementAround")
                        {
                            endRangeDrag = hit.collider.transform.position + (hit.normal * gridManager.gridScale) / 2;
                        }
                        else
                        {
                            endRangeDrag = hit.collider.transform.position + (hit.normal * gridManager.gridScale);
                        }

                        CreateBoxPlacement(startRangeDrag,
                            hit.collider.gameObject.transform.position + (hit.normal * (gridManager.gridScale / 2)));
                        focusPosBuffer = hit.collider.gameObject.transform.position +
                                         (hit.normal * (gridManager.gridScale / 2));

                        isDrag = false;
                        return;
                    }

                    if (hit.collider != null)
                    {
                        if (GridEditorWindow.obj == null)
                        {
                            return;
                        }

                        if (hit.collider.transform.position + (hit.normal * gridManager.gridScale) != focusPosBuffer)
                        {
                            ClearVisualizer();
                            CreteBoxVisualizer(startRangeDrag,
                                hit.collider.gameObject.transform.position +
                                (hit.normal * (gridManager.gridScale / 2)));
                            focusPosBuffer = hit.collider.gameObject.transform.position +
                                             (hit.normal * (gridManager.gridScale / 2));
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


                        ConfirmDelete();
                        isDrag = false;

                        return;
                    }

                    ResetReservation();

                    if (hit.collider != null)
                    {
                        if (hit.collider.transform.position + (hit.normal * gridManager.gridScale) != focusPosBuffer)
                        {
                            
                            Vector3 endRangeGuessPoint;

                            if (hit.collider.name == "AblePlacementAround")
                            {
                                endRangeGuessPoint = hit.collider.transform.position +
                                                     (hit.normal * gridManager.gridScale) / 2;
                            }
                            else
                            {
                                endRangeGuessPoint = hit.collider.transform.position;
                            }

                            ReservationDeleteObj(startRangeDrag, endRangeGuessPoint);
                            focusPosBuffer = hit.collider.gameObject.transform.position +
                                             (hit.normal * (gridManager.gridScale / 2));
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

                        ConfirmReplace();
                        isDrag = false;

                        return;
                    }

                    ResetReservation();

                    if (hit.collider != null)
                    {

                        if (hit.collider.transform.position + (hit.normal * gridManager.gridScale) != focusPosBuffer)
                        {
                            Vector3 endRangeGuessPoint;

                            if (hit.collider.name == "AblePlacementAround")
                            {
                                endRangeGuessPoint = hit.collider.transform.position +
                                                     (hit.normal * gridManager.gridScale) / 2;
                            }
                            else
                            {
                                endRangeGuessPoint = hit.collider.transform.position;
                            }

                            ReservationDeleteObj(startRangeDrag, endRangeGuessPoint);
                            focusPosBuffer = hit.collider.gameObject.transform.position +
                                             (hit.normal * (gridManager.gridScale / 2));
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

    private void CreteBoxVisualizer(Vector3 rangeStart, Vector3 rangeEnd)
    {
        int signx = 1;
        int signy = 1;
        int signz = 1;

        if (rangeStart.x > rangeEnd.x)
        {
            (rangeStart.x, rangeEnd.x) = (rangeEnd.x, rangeStart.x);
            signx = -1;
        }

        if (rangeStart.y > rangeEnd.y)
        {
            (rangeStart.y, rangeEnd.y) = (rangeEnd.y, rangeStart.y);
            signy = -1;
        }

        if (rangeStart.z > rangeEnd.z)
        {
            (rangeStart.z, rangeEnd.z) = (rangeEnd.z, rangeStart.z);
            signz = -1;
        }

        for (float i = 0; i < rangeEnd.x - rangeStart.x + gridManager.gridScale; i++)
        {
            for (float j = 0; j < rangeEnd.y - rangeStart.y + gridManager.gridScale; j++)
            {
                for (float k = 0; k < rangeEnd.z - rangeStart.z + gridManager.gridScale; k++)
                {
                    instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                    ((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
                    ((GameObject)instantiateBuffer).transform.position =
                        new Vector3(signx * i, signy * j, signz * k) + startRangeDrag;
                    ((GameObject)instantiateBuffer).transform.name = "visualizeObj";
                    Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");
                }
            }
        }
    }

    private void CreateBoxPlacement(Vector3 rangeStart, Vector3 rangeEnd)
    {
        int signx = 1;
        int signy = 1;
        int signz = 1;

        if (rangeStart.x > rangeEnd.x)
        {
            (rangeStart.x, rangeEnd.x) = (rangeEnd.x, rangeStart.x);
            signx = -1;
        }

        if (rangeStart.y > rangeEnd.y)
        {
            (rangeStart.y, rangeEnd.y) = (rangeEnd.y, rangeStart.y);
            signy = -1;
        }

        if (rangeStart.z > rangeEnd.z)
        {
            (rangeStart.z, rangeEnd.z) = (rangeEnd.z, rangeStart.z);
            signz = -1;
        }

        for (float i = 0; i < rangeEnd.x - rangeStart.x + gridManager.gridScale; i++)
        {
            for (float j = 0; j < rangeEnd.y - rangeStart.y + gridManager.gridScale; j++)
            {
                for (float k = 0; k < rangeEnd.z - rangeStart.z + gridManager.gridScale; k++)
                {
                    foreach (GameObject gameObject in gridManager.inGridObjects)
                    {
                        if (gameObject.transform.position == new Vector3(signx * i, signy * j, signz * k) + startRangeDrag)
                        {
                            return;
                        }

                    }

                    instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
                    ((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
                    ((GameObject)instantiateBuffer).transform.position =
                        new Vector3(signx * i, signy * j, signz * k) + startRangeDrag;
                    Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");

                    Undo.RecordObject(gridManager, "isPlaced Check");
                    gridManager.inGridObjects.Add(((GameObject)instantiateBuffer));

                    var dummy = PrefabUtility.InstantiatePrefab(dummyBlock);
                    ((GameObject)dummy).hideFlags = HideFlags.HideInHierarchy;
                    ((GameObject)dummy).transform.position =
                        ((GameObject)instantiateBuffer).transform.position;
                    ((GameObject)dummy).transform.parent = ((GameObject)instantiateBuffer).transform;
                    ((GameObject)dummy).GetComponent<MeshRenderer>().enabled = false;
                    Undo.RegisterCreatedObjectUndo(dummy, "placed prefab");
                }
            }
        }
    }

    private void ReservationDeleteObj(Vector3 rangeStart, Vector3 rangeEnd)
    {
        int signx = 1;
        int signy = 1;
        int signz = 1;

        if (rangeStart.x > rangeEnd.x)
        {
            (rangeStart.x, rangeEnd.x) = (rangeEnd.x, rangeStart.x);
            signx = -1;
        }

        if (rangeStart.y > rangeEnd.y)
        {
            (rangeStart.y, rangeEnd.y) = (rangeEnd.y, rangeStart.y);
            signy = -1;
        }

        if (rangeStart.z > rangeEnd.z)
        {
            (rangeStart.z, rangeEnd.z) = (rangeEnd.z, rangeStart.z);
            signz = -1;
        }

        foreach (var o in gridManager.inGridObjects)
        {
            Vector3 objPos = o.transform.position;

            if (objPos.x >= rangeStart.x - gridManager.gridScale / 2 && objPos.x <= rangeEnd.x + gridManager.gridScale / 2 &&
                objPos.y >= rangeStart.y - gridManager.gridScale / 2 && objPos.y <= rangeEnd.y + gridManager.gridScale / 2 &&
                objPos.z >= rangeStart.z - gridManager.gridScale / 2 && objPos.z <= rangeEnd.z + gridManager.gridScale / 2)
            {
                Undo.RecordObject(o, "DisVisible");
                if (o.gameObject.GetComponent<MeshRenderer>() != null)
                {
                    o.gameObject.GetComponent<MeshRenderer>().enabled = false;
                }
                else
                {
                    o.gameObject.SetActive(false);
                }

                deleteReservationList.Add(o);
            }
        }
    }

    private void ResetReservation()
    {
        foreach (var o in deleteReservationList)
        {
            if (o.name == "AblePlacementAround")
            {
                continue;
            }

            if (o.gameObject.GetComponent<MeshRenderer>() != null)
            {
                o.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
            else
            {
                o.gameObject.SetActive(true);
            }
        }

        deleteReservationList.Clear();
    }

    private void ConfirmDelete()
    {
        for (int i = deleteReservationList.Count; i > 0; i--)
        {
            gridManager.inGridObjects.Remove(deleteReservationList[i - 1]);
            Undo.DestroyObjectImmediate(deleteReservationList[i - 1]);
        }

        deleteReservationList.Clear();
    }

    private void ConfirmReplace()
    {
        for (int i = deleteReservationList.Count; i > 0; i--)
        {
            instantiateBuffer = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
            ((GameObject)instantiateBuffer).transform.parent = gridManager.gameObject.transform;
            ((GameObject)instantiateBuffer).transform.position = deleteReservationList[i- 1].transform.position;
                Undo.RegisterCreatedObjectUndo(instantiateBuffer, "placed prefab");

            Undo.RecordObject(gridManager, "isPlaced Check");
            gridManager.inGridObjects.Add(((GameObject)instantiateBuffer));

            var dummy = PrefabUtility.InstantiatePrefab(dummyBlock);
            ((GameObject)dummy).hideFlags = HideFlags.HideInHierarchy;
            ((GameObject)dummy).transform.position =
                ((GameObject)instantiateBuffer).transform.position;
            ((GameObject)dummy).transform.parent = ((GameObject)instantiateBuffer).transform;
            ((GameObject)dummy).GetComponent<MeshRenderer>().enabled = false;
            Undo.RegisterCreatedObjectUndo(dummy, "placed prefab");

            gridManager.inGridObjects.Remove(deleteReservationList[i - 1]);
            Undo.DestroyObjectImmediate(deleteReservationList[i - 1]);
        }

        deleteReservationList.Clear();
    }
}

#endif // UNITY_EDITOR