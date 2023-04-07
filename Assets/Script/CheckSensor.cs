using UnityEngine;
using Commands;

public class CheckSensor : MonoBehaviour {

    [SerializeField] private ConditionBase conditionBase;
    public int port;
    public bool isTouched = false;
    public bool isColor = false;

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Wall"))
        {
            isTouched = true;
            conditionBase.isTouched = isTouched;
            conditionBase.Port = port;
        }
        else if (col.CompareTag("Line"))
        {
            isColor = true;
            conditionBase.isColor = isColor;
            conditionBase.Port = port;
        }
        else if (col.CompareTag("Ball"))
        {
            isTouched = true;
            conditionBase.isTouched = isTouched;
            conditionBase.Port = port;
        }
    }

    /*private void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Line"))
        {
            isColor = true;
            conditionBase.isColor = isColor;
            conditionBase.Port = port;
        }

    }*/

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Wall"))
        {
            isTouched = false;
            conditionBase.isTouched = isTouched;
            conditionBase.Port = port;
        }
        else if (col.CompareTag("Line"))
        {
            isColor = false;
            conditionBase.isColor = isColor;
            conditionBase.Port = port;
        }
        else if (col.CompareTag("Ball"))
        {
            isTouched = false;
            conditionBase.isTouched = isTouched;
            conditionBase.Port = port;
        }

    }
}
