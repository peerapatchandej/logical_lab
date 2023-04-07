using UnityEngine;

public class FreeZone : MonoBehaviour
{
    [SerializeField] private CheckFrontCol CheckFrontCol;
    [SerializeField] private CheckBodyCol CheckBodyCol;
    [SerializeField] private BoxCollider FrontCol;
    [SerializeField] private BoxCollider BodyCol;
    [SerializeField] private GameFinish gameFinish;
    [SerializeField] private TimeController timeControl;

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            CheckFrontCol.isEnter = false;
            BodyCol.enabled = false;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            if (CheckFrontCol.isEnter == false)
            {
                GetComponent<BoxCollider>().enabled = false;
                timeControl.CalTimeSpent();
                gameFinish.LevelFinish("Lose", "Robot out of area");
            }
            else
            {
                FrontCol.enabled = true;
                BodyCol.enabled = true;
            }
        }
    }
}
