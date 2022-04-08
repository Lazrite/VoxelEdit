#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

public class CustomGridToolsAttribute : EditorTool
{
    private GUIContent ToolbarIcon;

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

    private void OnEnable()
    {
        ToolManager.activeToolChanged += ActiveToolDidChange;
    }

    private void OnDisable()
    {
        ToolManager.activeToolChanged -= ActiveToolDidChange;
    }

    private void ActiveToolDidChange()
    {
        if (!ToolManager.IsActiveTool(this))
        {
            return;
        }
    }

    public override void OnToolGUI(EditorWindow window)
    {

    }
}

#endif // UNITY_EDITOR