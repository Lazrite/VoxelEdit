using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum OperationMode
{
    OperationClick = 0,
    OperationDragFree,
    OperationRange,
}

public enum ToolMode
{
    ToolPlace = 0,
    ToolErase,
    ToolReplace,
}

public class PlaceDownPanelUI : MonoBehaviour
{
    [SerializeField] private List<Button> modeButton = default;

    public OperationMode selectMode;


    // Start is called before the first frame update
    private void Start()
    {
        selectMode = OperationMode.OperationClick;
        foreach (var button in modeButton)
        {
            button.interactable = true;

            if (button.name == "ClickDown")
            {
                button.interactable = false;
            }
        }
    }

    public void OnSelectClickDown()
    {
        selectMode = OperationMode.OperationClick;
        foreach (var button in modeButton)
        {
            button.interactable = true;

            if (button.name == "ClickDown")
            {
                button.interactable = false;
            }
        }
    }

    public void OnSelectDragDown()
    {
        selectMode = OperationMode.OperationDragFree;
        foreach (var button in modeButton)
        {
            button.interactable = true;

            if (button.name == "DragDown")
            {
                button.interactable = false;
            }
        }
    }
}
