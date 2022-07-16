#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEngine;

public class CustomGridToolsAttribute : EditorTool
{
    private GUIContent ToolbarIcon; // アイコン

    /// <summary>
    /// ツールバーのアイコン設定
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
    /// 有効になった時
    /// </summary>
    private void OnEnable()
    {
        ToolManager.activeToolChanged += ActiveToolDidChange;
    }

    /// <summary>
    /// 無効になった時
    /// </summary>
    private void OnDisable()
    {
        ToolManager.activeToolChanged -= ActiveToolDidChange;
    }

    /// <summary>
    /// ツールがアクティブになった時
    /// </summary>
    private void ActiveToolDidChange()
    {
        if (!ToolManager.IsActiveTool(this))
        {
            return;
        }
    }

    /// <summary>
    /// ツールがアクティブな場合の挙動
    /// </summary>
    /// <param name="window"> 現在のウィンドウ </param>
    public override void OnToolGUI(EditorWindow window)
    {

    }
}

#endif // UNITY_EDITOR