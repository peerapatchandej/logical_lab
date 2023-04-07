using UnityEngine;

public class CheckFinishZone : MonoBehaviour
{
    [SerializeField] private GameFinish gameFinish;
    [SerializeField] private TimeController timeControl;

    private void Start()
    {
        gameFinish.ScoreCount = 100;
    }

    private void OnTriggerEnter(Collider col)
    {
        GetComponent<BoxCollider>().enabled = false;
        timeControl.CalTimeSpent();
        gameFinish.LevelFinish("Win","Excellent!!");
    }
}
