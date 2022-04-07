using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

public class CustomGridToolsAttribute : EditorTool
{
    private struct TransformAndPositions
    {
        public Transform transform;
        public Vector3[] positions;
    }

    private IEnumerable<TransformAndPositions> m_Vertices;
    private GUIContent m_ToolbarIcon;

    public override GUIContent toolbarIcon
    {
        get
        {
            if (m_ToolbarIcon == null)
            {
                m_ToolbarIcon = new GUIContent(
                    AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/GridEditor/EditorTexture/ClickDown.png"),
                    "Grid Visual recognition priority");
            }

            return m_ToolbarIcon;
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
