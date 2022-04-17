#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class IsVisualizeMesh : MonoBehaviour
{
    //選択判別用フラグ
    public bool select_flg;

    private EditorGridField editorGrid;
    private Object prefab;

    private void Start()
    {
        //フラグの初期化
        select_flg = false;
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

        //フラグがtrueの場合
        if (select_flg)
        {
            //選択から外れたとき用
            select_flg = false;
            //オブジェクトの可視化
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        }

        if (!Application.isPlaying)
        {
            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();
        }
    }
}
#endif
