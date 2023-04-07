using System.Collections;
using UnityEngine;

public class ActiveTweenInGame : MonoBehaviour {

    [SerializeField] private EasyTween TweenInfo;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.3f);
        TweenInfo.OpenCloseObjectAnimation();
    }
}
