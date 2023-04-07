using UnityEngine;

public class SetFrameRate : MonoBehaviour
{
    [SerializeField] private int FrameRate = 60;
	
	void Start () {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = FrameRate;
	}

}
