using UnityEngine;

public class CheckBall : MonoBehaviour
{
    [SerializeField] private GameFinish gameFinish;
    [SerializeField] private TimeController timeControl;
    [SerializeField] private CheckSensor Touch;
    private SoundControl PlaySound;

    private void Start()
    {
        PlaySound = GameObject.Find("SoundSFX").GetComponent<SoundControl>();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Ball"))
        {
            PlaySound.PlayBroken();
            Touch.isTouched = false;
            gameFinish.CountBall--;
            gameFinish.ScoreCount += 15;
            col.gameObject.SetActive(false);

            if (gameFinish.CountBall == 0)
            {
                timeControl.CalTimeSpent();
                gameFinish.LevelFinish("Win","Excellent!!");
            }
        }
    }
}
