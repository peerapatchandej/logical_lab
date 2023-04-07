using UnityEngine;
using UnityEngine.UI;
using Commands;

public class MovementController : MonoBehaviour
{
    [SerializeField] private GameObject DropdownSpeed;
    [SerializeField] private InputField Input;

    public int GetSleep()
    {
        return int.Parse(Input.text);
    }

    public int GetSpeedMove()
    {
        //var Struct = GetComponent<StructCommand>();
        var speed = DropdownSpeed.GetComponent<Dropdown>().value + 1;

        if (speed == 1) return speed = 360000;
        else if (speed == 2) return speed = 480000;
        else if (speed == 3) return speed = 760000;
        else if (speed == 4) return speed = 1000000;
        else return speed = 1500000;
    }

    public int GetSpeedRotate()
    {
        var speed = DropdownSpeed.GetComponent<Dropdown>().value + 1;

        if (speed == 1) return speed = 1;
        else if (speed == 2) return speed = 2;
        else if (speed == 3) return speed = 3;
        else if (speed == 4) return speed = 4;
        else return speed = 5;
    }
}
