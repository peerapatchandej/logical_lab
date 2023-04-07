using UnityEngine;

public class CheckBoxCol : MonoBehaviour {

    [SerializeField] private GameFinish gameFinish;
    [SerializeField] private TimeController timeControl;
    private SoundControl PlaySound;

    private void Start()
    {
        PlaySound = GameObject.Find("SoundSFX").GetComponent<SoundControl>();
        gameFinish.ScoreCount = 100;
    }

    private void OnTriggerEnter(Collider col)
    {
        PlaySound.PlayCrash();
        if (gameFinish.ScoreCount > 0)
        {
            if (gameFinish.ScoreCount - 15 > 0) gameFinish.ScoreCount -= 15;
            else
            {
                GetComponent<BoxCollider>().enabled = false;
                gameFinish.ScoreCount = 0;
                timeControl.CalTimeSpent();
                gameFinish.LevelFinish("Lose", "Your score is zero");
            }
        }
    }
}
