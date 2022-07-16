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

}

#endif // UNITY_EDITOR