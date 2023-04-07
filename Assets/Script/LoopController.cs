using UnityEngine;
using UnityEngine.UI;

public class LoopController : MonoBehaviour
{
    [SerializeField] private GameObject Input;

    public int GetCount()
    {
        return int.Parse(Input.GetComponent<InputField>().text);
    }
}
