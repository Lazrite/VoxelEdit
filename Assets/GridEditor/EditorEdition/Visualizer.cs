#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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

    /// <summary>
    /// ビジュアライザを生成するメソッド
    /// </summary>
    /// <param name="type"> 生成するビジュアライザのタイプ </param>
    public static void CreateVisualizer(VisualizerType type = VisualizerType.Prefab | VisualizerType.Surface)
    {
        if (type.HasFlag(VisualizerType.Surface))
        {
            // サーフェスオブジェクトのビジュアライザをロード
            surfaceObj =
                (GameObject)AssetDatabase.LoadAssetAtPath(
                    "Assets/GridEditor/EditorSource/AblePlacementQuadVisualizer.prefab",
                    typeof(GameObject));

            // ビジュアライザを生成してレンダラーを消す(レンダラーが無いモデルの場合オブジェクトを非アクティブに)
            selectedSurfaceVisualizer = PrefabUtility.InstantiatePrefab(surfaceObj) as GameObject;
            if (selectedSurfaceVisualizer != null)
            {
                selectedSurfaceVisualizer.GetComponent<MeshRenderer>().enabled = false;
            }
        }
        if (type.HasFlag(VisualizerType.Prefab))
        {
            placePrefab = (GameObject)GridEditorWindow.obj;

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
    }

    /// <summary>
    /// サーフェスビジュアライザを移動させるメソッド
    /// </summary>
    /// <param name="pos"> サーフェスビジュアライザの移動先座標 </param>
    /// <param name="angles"> サーフェスビジュアライザの角度 </param>
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

    /// <summary>
    /// プレハブのビジュアライザを移動させるメソッド。ビジュアライザが視認不可の場合は可視化する
    /// </summary>
    /// <param name="pos"> プレハブの移動先座標 </param>
    /// <param name="angles"> プレハブの角度 </param>
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
        // TODO 角度を設定できるようにするときにビジュアライザを対応させる
    }

    /// <summary>
    /// ビジュアライザのレンダラをオフにするメソッド。 モデルプレハブでレンダラーがない場合は非アクティブにする
    /// </summary>
    /// <param name="type"></param>
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

    /// <summary>
    /// ビジュアライザを消去するメソッド
    /// </summary>
    /// <param name="type"> 消去するビジュアライザのタイプ(デフォルトでは全て消す) </param>
    public static void DestroyVisualizer(VisualizerType type = VisualizerType.Prefab | VisualizerType.Surface)
    {
        if (type.HasFlag(VisualizerType.Surface) && surfaceObj != null)
        {
            Object.DestroyImmediate(selectedSurfaceVisualizer);
        }

        if (type.HasFlag(VisualizerType.Prefab) && prefabVisualizer != null)
        {
            Object.DestroyImmediate(prefabVisualizer);
        }
    }

    /// <summary>
    /// アセンブリをロードする前にビジュアライザを消去するメソッド
    /// </summary>
    [InitializeOnLoadMethod]
    public static void PreDestroyVisualizer()
    {
        // アセンブリにデリゲート登録
        AssemblyReloadEvents.beforeAssemblyReload += () =>
        {
            DestroyVisualizer();
        };
    }
}

#endif // UNITY_EDITOR