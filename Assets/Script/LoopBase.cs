using UnityEngine;

namespace Commands
{
    public enum LoopType
    {
        None,
        Forever,
        For
    }

    public class LoopBase : MonoBehaviour
    {
        [HideInInspector] public int LoopNum = 1;
    }
}

