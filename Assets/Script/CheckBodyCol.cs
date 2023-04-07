using UnityEngine;

public class CheckBodyCol : MonoBehaviour {

    [SerializeField] private CheckFrontCol checkFront;
    [SerializeField] private GameFinish gameFinish;
    [SerializeField] private TimeController timeControl;
    [SerializeField] private MenuButton menuButton;

    private void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Line"))
        {
            if (checkFront.isEnter != true)
            {
                GetComponent<BoxCollider>().enabled = false;
                timeControl.CalTimeSpent();
                gameFinish.LevelFinish("Lose", "Robot out of area");
            }
        }
    }
}
