#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

[Flags]
public enum VisualizerType
{
    Surface = 1,
    Prefab = 2,
}

public static class Visualizer
{
    private static GameObject surfaceObj;
    private static GameObject placePrefab;
    public static GameObject selectedSurfaceVisualizer;
    public static GameObject prefabVisualizer;


    public static void CreateVisualizer()
    {
        // オブジェクトのビジュアライザをロード
        surfaceObj =
            (GameObject)AssetDatabase.LoadAssetAtPath("Assets/GridEditor/EditorSource/AblePlacementQuadVisualizer.prefab",
                typeof(GameObject));
        placePrefab = (GameObject)GridEditorWindow.obj;

        // ビジュアライザを生成してレンダラーを消す(レンダラーが無いモデルの場合オブジェクトを非アクティブに)
        selectedSurfaceVisualizer = PrefabUtility.InstantiatePrefab(surfaceObj) as GameObject;
        if (selectedSurfaceVisualizer != null)
        {
            selectedSurfaceVisualizer.GetComponent<MeshRenderer>().enabled = false;
        }


        prefabVisualizer = PrefabUtility.InstantiatePrefab(placePrefab) as GameObject;
        if (prefabVisualizer != null)
        {
            prefabVisualizer.layer = LayerMask.NameToLayer("Ignore Raycast");
            if (prefabVisualizer.GetComponent<Renderer>() != null)
            {
                prefabVisualizer.GetComponent<Renderer>().enabled = false;
            }
            else
            {
                prefabVisualizer.SetActive(false);
            }
        }
    }

    public static void MoveVisualizerSurface(Vector3 pos, Vector3 angles)
    {
        // ビジュアライザのレンダラーが無効の場合オンにする
        if (!selectedSurfaceVisualizer.GetComponent<Renderer>().enabled)
        {
            selectedSurfaceVisualizer.GetComponent<Renderer>().enabled = true;
        }

        selectedSurfaceVisualizer.transform.position = pos;
        selectedSurfaceVisualizer.transform.rotation = Quaternion.LookRotation(angles);
    }

    public static void MoveVisualizerPrefab(Vector3 pos, Vector3 angles = default)
    {
        if (prefabVisualizer != null && prefabVisualizer.GetComponent<Renderer>() != null)
        {
            prefabVisualizer.GetComponent<Renderer>().enabled = true;
        }
        else
        {
            if (prefabVisualizer != null)
            {
                prefabVisualizer.SetActive(true);
            }
        }

        prefabVisualizer.transform.position = pos;
        //角度
    }

    public static void DisableRenderer(VisualizerType type = VisualizerType.Prefab | VisualizerType.Surface)
    {
        if (type.HasFlag(VisualizerType.Surface))
        {
            if (selectedSurfaceVisualizer != null)
            {
                selectedSurfaceVisualizer.GetComponent<MeshRenderer>().enabled = false;
            }
        }

        if (type.HasFlag(VisualizerType.Prefab))
        {
            if (prefabVisualizer != null && prefabVisualizer.GetComponent<Renderer>() != null)
            {
                prefabVisualizer.GetComponent<Renderer>().enabled = false;
            }
            else
            {
                if (prefabVisualizer != null)
                {
                    prefabVisualizer.SetActive(false);
                }
            }
        }
    }

    public static void DestroyVisualizer(VisualizerType type = VisualizerType.Prefab | VisualizerType.Surface)
    {
        if (type.HasFlag(VisualizerType.Surface))
        {

        }

        if (type.HasFlag(VisualizerType.Prefab))
        {

        }
    }
}

#endif // UNITY_EDITOR