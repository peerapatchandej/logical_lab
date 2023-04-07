using UnityEngine;

public class CheckFrontCol : MonoBehaviour {

    [SerializeField] private MenuButton menuButton;
    public bool isEnter;

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Line"))
        {
            isEnter = true;
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Line"))
        {
            isEnter = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Line"))
        {
            isEnter = false;
        }
    }
}
