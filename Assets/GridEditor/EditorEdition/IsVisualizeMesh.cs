#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class IsVisualizeMesh : MonoBehaviour
{
    //�I�𔻕ʗp�t���O
    public bool select_flg;

    private EditorGridField editorGrid;
    private Object prefab;

    private void Start()
    {
        //�t���O�̏�����
        select_flg = false;
    }

    private void Update()
    {
    }

    private void OnRenderObject()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;

        if (GridEditorWindow.gridObject == null)
        {
            return;
        }

        if (editorGrid == null)
        {
            editorGrid = ((GameObject)GridEditorWindow.gridObject).GetComponent<EditorGridField>();
        }

        //if (!editorGrid.isPlaced[this.gameObject.GetComponent<GridRelatedInfo>().gridIndex - 1])
        //{
        //    DestroyImmediate(prefab);
        //}

        //�t���O��true�̏ꍇ
        if (select_flg)
        {
            //�I������O�ꂽ�Ƃ��p
            select_flg = false;
            //�I�u�W�F�N�g�̉���
            gameObject.GetComponent<MeshRenderer>().enabled = true;
            //if (GridEditorWindow.obj != null && !editorGrid.isPlaced[this.gameObject.GetComponent<GridRelatedInfo>().gridIndex - 1])
            //{
            //    prefab = PrefabUtility.InstantiatePrefab(GridEditorWindow.obj);
            //    ((GameObject)prefab).transform.position = editorGrid.gridPosFromIndex[this.gameObject.GetComponent<GridRelatedInfo>().gridIndex - 1];
            //    ((GameObject)prefab).transform.parent = ((GameObject)GridEditorWindow.gridObject).transform;
            //    ((GameObject)prefab).hideFlags = HideFlags.HideInHierarchy;
            //}


        }

        if (!Application.isPlaying)
        {
            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();
        }
    }
}
#endif