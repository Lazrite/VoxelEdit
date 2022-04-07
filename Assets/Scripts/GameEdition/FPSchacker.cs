using UnityEngine;

public class FPSchacker : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        float fps = 1f / Time.deltaTime;
        Debug.LogFormat("{0}fps", fps);
    }
}
