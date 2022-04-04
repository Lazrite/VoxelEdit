using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomGridToolsAttribute : EditorTool
{
    struct TransformAndPositions
    {
        public Transform transform;
        public Vector3[] positions;
    }

    IEnumerable<TransformAndPositions> m_Vertices;
    GUIContent m_ToolbarIcon;

    public override GUIContent toolbarIcon
    {
        get
        {
            if (m_ToolbarIcon == null)
                m_ToolbarIcon = new GUIContent(
                    AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/GridEditor/EditorTexture/ClickDown.png"),
                    "Grid Visual recognition priority");
            return m_ToolbarIcon;
        }
    }

    void OnEnable()
    {
        ToolManager.activeToolChanged += ActiveToolDidChange;
    }

    void OnDisable()
    {
        ToolManager.activeToolChanged -= ActiveToolDidChange;
    }

    void ActiveToolDidChange()
    {
        if (!ToolManager.IsActiveTool(this))
            return;
    }

    public override void OnToolGUI(EditorWindow window)
    {

    }
}
