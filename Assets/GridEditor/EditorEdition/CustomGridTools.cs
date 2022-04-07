using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

// Tagging a class with the EditorTool attribute and no target type registers a global tool. Global tools are valid for any selection, and are accessible through the top left toolbar in the editor.
[EditorTool("Grid Tool")]
internal class CustomGridTools : EditorTool
{
    // Serialize this value to set a default value in the Inspector.
    [SerializeField]
    private Texture2D m_ToolIcon;
    private GUIContent m_IconContent;

    private void OnEnable()
    {
        m_IconContent = new GUIContent()
        {
            image = m_ToolIcon,
            text = "グリッドツール",
            tooltip = "グリッドツール"
        };
    }

    public override GUIContent toolbarIcon => m_IconContent;

    // This is called for each window that your tool is active in. Put the functionality of your tool here.
    public override void OnToolGUI(EditorWindow window)
    {

    }
}