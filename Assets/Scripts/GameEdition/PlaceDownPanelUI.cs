using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum OperationMode
{
    OPERATION_CLICK = 0,
    OPERATION_DRAG_FREE,
}

public enum ToolMode
{
    TOOL_PLACE,
    TOOL_ERASE,
}

public class PlaceDownPanelUI : MonoBehaviour
{
    [SerializeField] List<Button> modeButton = default;

    public OperationMode selectMode;
    

    // Start is called before the first frame update
    void Start()
    {
        selectMode = OperationMode.OPERATION_CLICK;
        foreach (var button in modeButton)
        {
            button.interactable = true;

            if (button.name == "ClickDown")
            {
                button.interactable = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelectClickDown()
    {
        selectMode = OperationMode.OPERATION_CLICK;
        foreach(var button in modeButton)
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
        selectMode = OperationMode.OPERATION_DRAG_FREE;
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
