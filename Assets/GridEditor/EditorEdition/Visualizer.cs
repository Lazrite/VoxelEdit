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
    /// �r�W���A���C�U�𐶐����郁�\�b�h
    /// </summary>
    /// <param name="type"> ��������r�W���A���C�U�̃^�C�v </param>
    public static void CreateVisualizer(VisualizerType type = VisualizerType.Prefab | VisualizerType.Surface)
    {
        if (type.HasFlag(VisualizerType.Surface))
        {
            // �T�[�t�F�X�I�u�W�F�N�g�̃r�W���A���C�U�����[�h
            surfaceObj =
                (GameObject)AssetDatabase.LoadAssetAtPath(
                    "Assets/GridEditor/EditorSource/AblePlacementQuadVisualizer.prefab",
                    typeof(GameObject));

            // �r�W���A���C�U�𐶐����ă����_���[������(�����_���[���������f���̏ꍇ�I�u�W�F�N�g���A�N�e�B�u��)
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
    /// �T�[�t�F�X�r�W���A���C�U���ړ������郁�\�b�h
    /// </summary>
    /// <param name="pos"> �T�[�t�F�X�r�W���A���C�U�̈ړ�����W </param>
    /// <param name="angles"> �T�[�t�F�X�r�W���A���C�U�̊p�x </param>
    public static void MoveVisualizerSurface(Vector3 pos, Vector3 angles)
    {
        // �r�W���A���C�U�̃����_���[�������̏ꍇ�I���ɂ���
        if (!selectedSurfaceVisualizer.GetComponent<Renderer>().enabled)
        {
            selectedSurfaceVisualizer.GetComponent<Renderer>().enabled = true;
        }

        selectedSurfaceVisualizer.transform.position = pos;
        selectedSurfaceVisualizer.transform.rotation = Quaternion.LookRotation(angles);
    }

    /// <summary>
    /// �v���n�u�̃r�W���A���C�U���ړ������郁�\�b�h�B�r�W���A���C�U�����F�s�̏ꍇ�͉�������
    /// </summary>
    /// <param name="pos"> �v���n�u�̈ړ�����W </param>
    /// <param name="angles"> �v���n�u�̊p�x </param>
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
        // TODO �p�x��ݒ�ł���悤�ɂ���Ƃ��Ƀr�W���A���C�U��Ή�������
    }

    /// <summary>
    /// �r�W���A���C�U�̃����_�����I�t�ɂ��郁�\�b�h�B ���f���v���n�u�Ń����_���[���Ȃ��ꍇ�͔�A�N�e�B�u�ɂ���
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
    /// �r�W���A���C�U���������郁�\�b�h
    /// </summary>
    /// <param name="type"> ��������r�W���A���C�U�̃^�C�v(�f�t�H���g�ł͑S�ď���) </param>
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
    /// �A�Z���u�������[�h����O�Ƀr�W���A���C�U���������郁�\�b�h
    /// </summary>
    [InitializeOnLoadMethod]
    public static void PreDestroyVisualizer()
    {
        // �A�Z���u���Ƀf���Q�[�g�o�^
        AssemblyReloadEvents.beforeAssemblyReload += () =>
        {
            DestroyVisualizer();
        };
    }
}

#endif // UNITY_EDITOR