using System.Collections;
using UnityEngine;

public class ActiveTweenStart : MonoBehaviour {

	public EasyTween[] TweenObject;
    
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.4f);
        TweenObject[0].OpenCloseObjectAnimation();

        yield return new WaitForSeconds(0.7f);
        TweenObject[1].OpenCloseObjectAnimation();
    }
}
