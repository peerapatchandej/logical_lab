using UnityEngine;

public class CheckColWall : MonoBehaviour {

    private SoundControl PlaySound;

    private void Start()
    {
        PlaySound = GameObject.Find("SoundSFX").GetComponent<SoundControl>();
    }

    private void OnTriggerEnter(Collider col)
    {
        PlaySound.PlayCrash();
    }
}
