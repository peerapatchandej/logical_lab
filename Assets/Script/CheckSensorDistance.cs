using Commands;
using UnityEngine;

public class CheckSensorDistance : MonoBehaviour
{
    [SerializeField] private ConditionBase conditionBase;
    [HideInInspector] public int port;
    public bool isDistance = false;

    public bool CheckDistance(bool isChecked,int Range)
    {
        if (isChecked)
        {
            Vector3 Direct = Vector3.up;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.up, out hit, Range))
            {
                if (hit.collider.CompareTag("Block"))
                {
                    //Debug.DrawLine(transform.position, hit.point, Color.red);
                    isDistance = true;
                    conditionBase.isDistance = isDistance;
                    conditionBase.Port = port;
                    return isDistance;

                }
                else if(hit.collider.CompareTag("Wall"))
                {
                    //Debug.DrawLine(transform.position, hit.point, Color.red);
                    isDistance = true;
                    conditionBase.isDistance = isDistance;
                    conditionBase.Port = port;
                    return isDistance;
                }
            }
        }
        else
        {
            isDistance = false;
            conditionBase.isDistance = isDistance;
            conditionBase.Port = port;
            return isDistance;
        }

        return false;
    }
}
