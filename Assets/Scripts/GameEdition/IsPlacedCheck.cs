using UnityEngine;

public enum CheckMode
{
    CHECK_NONE = 0,

    CHECK_ISPLACED,
    CHECK_SEARCH_PLACEDOBJECT,
}


public class IsPlacedCheck : MonoBehaviour
{
    [SerializeField] private GridManager gridManager = default;
    [SerializeField] private GenerateAblePlacementArea placementArea = default;
    [SerializeField] public CheckMode checkMode = default;

    // Start is called before the first frame update
    private void Start()
    {
        checkMode = new CheckMode();
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        switch (checkMode)
        {
            case CheckMode.CHECK_NONE:
                break;
            case CheckMode.CHECK_ISPLACED:
                gridManager.isPlaced[placementArea.currentIndex] = true;
                break;
            case CheckMode.CHECK_SEARCH_PLACEDOBJECT:
                //placementArea.serchedObject = other.gameObject;
                break;
        }

    }
}
