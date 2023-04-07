using UnityEngine;

public class CheckPlayerBodyCol : MonoBehaviour
{
    [SerializeField] private GameFinish gameFinish;
    [SerializeField] private TimeController timeControl;
    private SoundControl PlaySound;

    private void Start()
    {
        PlaySound = GameObject.Find("SoundSFX").GetComponent<SoundControl>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            GetComponent<BoxCollider>().enabled = false;
            PlaySound.PlayCrash();
            timeControl.CalTimeSpent();
            gameFinish.LevelFinish("Lose", "The enemy smashes your robot");
        }
    }
}
