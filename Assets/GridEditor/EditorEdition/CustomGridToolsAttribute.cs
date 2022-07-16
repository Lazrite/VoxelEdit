#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

public class CustomGridToolsAttribute : EditorTool
{
    private GUIContent ToolbarIcon; // �A�C�R��

    /// <summary>
    /// �c�[���o�[�̃A�C�R���ݒ�
    /// </summary>
    public override GUIContent toolbarIcon
    {
        get
        {
            if (ToolbarIcon == null)
            {
                ToolbarIcon = new GUIContent(
                    AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/GridEditor/EditorTexture/ClickDown.png"),
                    "Grid Visual recognition priority");
            }

            return ToolbarIcon;
        }
    }

    /// <summary>
    /// �L���ɂȂ�����
    /// </summary>
    private void OnEnable()
    {
        ToolManager.activeToolChanged += ActiveToolDidChange;
    }

    /// <summary>
    /// �����ɂȂ�����
    /// </summary>
    private void OnDisable()
    {
        ToolManager.activeToolChanged -= ActiveToolDidChange;
    }

    /// <summary>
    /// �c�[�����A�N�e�B�u�ɂȂ�����
    /// </summary>
    private void ActiveToolDidChange()
    {
        if (!ToolManager.IsActiveTool(this))
        {
            return;
        }
    }

    /// <summary>
    /// �c�[�����A�N�e�B�u�ȏꍇ�̋���
    /// </summary>
    /// <param name="window"> ���݂̃E�B���h�E </param>
    public override void OnToolGUI(EditorWindow window)
    {

    }
}

#endif // UNITY_EDITOR