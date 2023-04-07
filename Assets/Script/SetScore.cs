using UnityEngine;
using UnityEngine.UI;

public class SetScore : MonoBehaviour
{
    [SerializeField] private GameFinish gameFinish;
    private Text ScoreTxt;

    private void Start()
    {
        ScoreTxt = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update ()
    {
        ScoreTxt.text = gameFinish.ScoreCount.ToString();

    }
}
