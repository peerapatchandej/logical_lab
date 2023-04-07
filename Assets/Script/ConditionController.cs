using UnityEngine;
using UnityEngine.UI;
using Commands;

public class ConditionController : MonoBehaviour {

    [SerializeField] private GameObject DropdownPort;
    [SerializeField] private GameObject DropdownCondition;
    [SerializeField] public GameObject Input;

    private void Update()
    {
        if(DropdownCondition != null)
        {
            int condition = DropdownCondition.GetComponent<Dropdown>().value + 1;

            if (condition >= 1 && condition <= 4) Input.SetActive(false);
            else Input.SetActive(true);
        }
    }

    public int GetPort()
    {
        return DropdownPort.GetComponent<Dropdown>().value + 1;
    }

    public void GetCondition(ref ConditionType Type, ref ConditionValue Value)
    {
        int condition = DropdownCondition.GetComponent<Dropdown>().value + 1;

        if (condition == 1)
        {
            Type = ConditionType.Touch;
            Value = ConditionValue.Touched;
        }
        else if (condition == 2)
        {
            Type = ConditionType.Touch;
            Value = ConditionValue.UnTouched;
        }
        else if (condition == 3)
        {
            Type = ConditionType.Color;
            Value = ConditionValue.Black;
        }
        else if (condition == 4)
        {
            Type = ConditionType.Color;
            Value = ConditionValue.White;
        }
        else if (condition == 5)
        {
            Type = ConditionType.Distance;
            Value = ConditionValue.None;
        }
    }

    public int GetRange()
    {
        return int.Parse(Input.transform.GetChild(1).GetComponent<InputField>().text);
    }
}
