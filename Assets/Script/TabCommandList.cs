using UnityEngine;
using UnityEngine.UI;

public class TabCommandList : MonoBehaviour{

    public Button Movement;
    public Button Condition;
    public Button Loop;
    Button TabMovement;
    Button TabCondition;
    Button TabLoop;
    GameObject Change;

    public Vector3 InitCmdPosX;
    private SoundControl PlaySound;

    public void Start()
    {
        PlaySound = GameObject.Find("SoundSFX").GetComponent<SoundControl>();

        TabMovement = Movement.GetComponent<Button>();
        TabMovement.onClick.AddListener(ChangeMovement);

        TabCondition = Condition.GetComponent<Button>();
        TabCondition.onClick.AddListener(ChangeCondition);

        TabLoop = Loop.GetComponent<Button>();
        TabLoop.onClick.AddListener(ChangeLoop);
    }
    public void ChangeMovement()
    {
        PlaySound.PlayClick();
        Change = GameObject.Find("Movement Body").transform.Find("List Box").gameObject;
        Change.gameObject.SetActive(true);
        Change = GameObject.Find("Condition Body").transform.Find("List Box").gameObject;
        Change.gameObject.SetActive(false);
        Change = GameObject.Find("Loop Body").transform.Find("List Box").gameObject;
        Change.gameObject.SetActive(false);
    }

    public void ChangeCondition()
    {
        PlaySound.PlayClick();
        Change = GameObject.Find("Movement Body").transform.Find("List Box").gameObject;
        Change.gameObject.SetActive(false);
        Change = GameObject.Find("Condition Body").transform.Find("List Box").gameObject;
        Change.gameObject.SetActive(true);
        Change = GameObject.Find("Loop Body").transform.Find("List Box").gameObject;
        Change.gameObject.SetActive(false);
    }

    public void ChangeLoop()
    {
        PlaySound.PlayClick();
        Change = GameObject.Find("Movement Body").transform.Find("List Box").gameObject;
        Change.gameObject.SetActive(false);
        Change = GameObject.Find("Condition Body").transform.Find("List Box").gameObject;
        Change.gameObject.SetActive(false);
        Change = GameObject.Find("Loop Body").transform.Find("List Box").gameObject;
        Change.gameObject.SetActive(true);
    }
}
