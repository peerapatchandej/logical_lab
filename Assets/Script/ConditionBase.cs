using UnityEngine;

namespace Commands
{
    public enum ConditionType
    {
        None,
        Touch,
        Color,
        Distance
    }

    public enum ConditionValue
    {
        None,
        Touched,
        UnTouched,
        Black,
        White
    }

    public class ConditionBase : MonoBehaviour
    {
        [HideInInspector] public int ConditionNum = 1;
        [HideInInspector] public bool isTouched = false;
        [HideInInspector] public bool isColor = false;
        [HideInInspector] public bool isDistance = false;
        [HideInInspector] public int Port;
    }
}

