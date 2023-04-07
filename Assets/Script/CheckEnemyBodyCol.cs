using UnityEngine;

public class CheckEnemyBodyCol : MonoBehaviour
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
        if (col.CompareTag("Enemy"))
        {
            GetComponent<BoxCollider>().enabled = false;
            PlaySound.PlayCrash();
            timeControl.CalTimeSpent();
            gameFinish.ScoreCount = 100;
            gameFinish.LevelFinish("Win", "Excellent!!");
        }
    }
}
