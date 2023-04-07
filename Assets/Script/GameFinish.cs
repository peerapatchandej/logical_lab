using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameFinish : MonoBehaviour
{
    [SerializeField] private GameObject FinishDialog;
    [SerializeField] private GameObject Success;
    [SerializeField] private GameObject Fail;

    [Header("Success")]
    [SerializeField] private Text Level;
    [SerializeField] private Text Score;
    public Button SuccessTryagin;
    public GameObject NextLevel;
    public GameObject SuccessSend;
    public GameObject SuccessChecked;
    [SerializeField] private Text SuccessStatus;
    [SerializeField] private Text SuccessCmdCount;
    [SerializeField] private Text SuccessTimeSpent;

    [Header("Fail User")]
    //[SerializeField] private GameObject UserFail;
    [SerializeField] private GameObject UserLevelSelect;
    [SerializeField] private GameObject FailUserTryagin;

    [Header("Fail Admin")]
    //[SerializeField] private GameObject AdminFail;
    public GameObject AdminLevelSelect;
    public GameObject FailTryagin;
    public GameObject FailChecked;

    [Header("Fail All")]
    [SerializeField] private Text AdminStatus;
    [SerializeField] private Text AdminCmdCount;
    [SerializeField] private Text AdminTimeSpent;

    [Header("ETC")]
    [SerializeField] private MenuButton menuButton;
    [SerializeField] private OnLoadLevel onLoadLevel;
    [SerializeField] private Transform CommandBox;
    [SerializeField] private TimeController timeControl;
    [SerializeField] private GameObject Stop;
    [SerializeField] private CommandsController CmdController;
    [SerializeField] private CheckFrontCol checkFront;
    [SerializeField] private BoxCollider FrontCol;
    [SerializeField] private BoxCollider BodyCol;
    [SerializeField] private SendAnswer sendAnswer;
    [SerializeField] private CheckedAnswer checkedAnswer;
    [SerializeField] private NextLevel nextLevel;
    [SerializeField] private BoxCollider PlayerCol;
    [SerializeField] private BoxCollider AICol;
    [SerializeField] private BoxCollider FinishZone;
    [SerializeField] private BoxCollider[] FreeZoneCol;
    [SerializeField] private BoxCollider[] ObstacleBox;
    [SerializeField] private Text RespondText;
    [SerializeField] private Animator anim;
    [SerializeField] private Image Black;
    [SerializeField] private SoundControl PlaySound;
    [SerializeField] private CheckAdminStatus CheckAdmin;

    [Header("Tween")]
    [SerializeField] private EasyTween TweenSuccess;
    [SerializeField] private EasyTween TweenFail;
    [SerializeField] private EasyTween TweenRespond;

    [HideInInspector] public int ScoreCount = 0;
    [HideInInspector] public string user_id;
    [HideInInspector] public string send_time;
    [HideInInspector] public bool isFinish = false;
    [HideInInspector] public bool isSuccess = false;
    
    //private Fading fade;

    //Level 1
    public int CountBall = 6;

    private void Start()
    {
        //fade = GameObject.Find("UserControl").GetComponent<Fading>();
        //PlaySound = GameObject.Find("SoundSFX").GetComponent<SoundControl>();
        //isAdmin = GameObject.Find("UserControl").GetComponent<StartGame>().isAdmin;

        //Success
        SuccessSend.GetComponent<Button>().onClick.AddListener(SendAns);
        SuccessChecked.GetComponent<Button>().onClick.AddListener(() => checkedAnswer.CheckedAnswers());
        NextLevel.GetComponent<Button>().onClick.AddListener(() => nextLevel.LoadLevel());
        SuccessTryagin.onClick.AddListener(GameTryagin);

        //Fail
        UserLevelSelect.GetComponent<Button>().onClick.AddListener(()=>StartCoroutine(GotoLevelSelect()));
        FailUserTryagin.GetComponent<Button>().onClick.AddListener(GameTryagin);
        AdminLevelSelect.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(GotoLevelSelect()));
        FailTryagin.GetComponent<Button>().onClick.AddListener(GameTryagin);
        FailChecked.GetComponent<Button>().onClick.AddListener(() => checkedAnswer.CheckedAnswers());
    }

    private void SendAns()
    {
        PlaySound.PlayClick();
        SuccessTryagin.interactable = false;
        SuccessSend.GetComponent<Button>().interactable = false;
        NextLevel.GetComponent<Button>().interactable = false;
        StartCoroutine(sendAnswer.ShowSubmitDialog());
    }

    /*private void GotoNextLevel()
    {
        SceneManager.LoadSceneAsync("Level "+ (onLoadLevel.Level + 1));
    }*/

    public void GameTryagin()
    {
        PlaySound.PlayClick();
        isFinish = false;
        menuButton.isStop = true;
        menuButton.isClick = false;
        menuButton.isCompile = false;
        menuButton.OnOffParameter(true);
        menuButton.MenuOnAll();
        menuButton.CameraOnPlay.SetActive(false);

        //Set Collider
        if (onLoadLevel.Level == 2)
        {
            PlayerCol.enabled = true;
            AICol.enabled = true;
        }
        else if (onLoadLevel.Level == 3)
        {
            FinishZone.enabled = true;

            for (int i = 0; i < FreeZoneCol.Length; i++)
            {
                FreeZoneCol[i].enabled = true;
            }
            for (int i = 0; i < ObstacleBox.Length; i++)
            {
                ObstacleBox[i].enabled = true;
            }
        }

        if (onLoadLevel.Level != 1)
        {
            checkFront.isEnter = true;
            FrontCol.enabled = false;
            BodyCol.enabled = false;
        }
        else menuButton.ResetBall();

        if (onLoadLevel.Level != 3) ScoreCount = 0;
        else ScoreCount = 100;

        Stop.SetActive(false);
        Stop.GetComponent<Button>().interactable = true;
        timeControl.ShowDefaultTime();

        //FinishDialog.SetActive(false);
        //Success.SetActive(false);
        //Fail.SetActive(false);

        if(isSuccess) TweenSuccess.OpenCloseObjectAnimation();
        else TweenFail.OpenCloseObjectAnimation();
    }

    private IEnumerator GotoLevelSelect()
    {
        PlaySound.PlayClick();
        TweenFail.OpenCloseObjectAnimation();
        yield return new WaitForEndOfFrame();

        /*TweenRespond.OpenCloseObjectAnimation();
        RespondText.text = "Loading...";
        yield return new WaitForSeconds(2);

        TweenRespond.OpenCloseObjectAnimation();

        var fadetime = fade.BeginFade(1);
        yield return new WaitForSeconds(fadetime);*/

        Black.enabled = true;
        anim.Play("FadeOut");
        yield return new WaitUntil(() => Black.color.a == 1);

        SceneManager.LoadSceneAsync("Level Select");
    }

    public void LevelFinish(string status, string endStatus)
    {
        isFinish = true;
        menuButton.isPlay = false;
        menuButton.isAIPlay = false;
        menuButton.MenuOffAll();
        CmdController.TimeSleep = 0;
        CmdController.InteractTime = 1;
        timeControl.StartTime = false;
        Stop.GetComponent<Button>().interactable = false;
        StartCoroutine(Delay(status, endStatus));
    }

    private IEnumerator Delay(string status, string endStatus)
    {
        yield return new WaitForSeconds(0.5f);
        DialogControl(onLoadLevel.Level, status, endStatus);
    }

    private void DialogControl(int level, string status,string endStatus)
    {
        if (status == "Win")
        {
            PlaySound.PlayWin();
            
            isSuccess = true;
            if (CheckAdmin.isAdmin)
            {
                SuccessChecked.SetActive(true);
            }
            else
            {
                SuccessSend.SetActive(true);
                SuccessSend.GetComponent<Button>().interactable = true;
            }

            Level.text = "Level " + level;
            SuccessStatus.text = endStatus;
            Score.text = ScoreCount.ToString();
            SuccessCmdCount.text = "Command count : " + (CommandBox.childCount - 1);

            if (timeControl.SecUse >= 10) SuccessTimeSpent.text = "Time use : 0" + timeControl.MinuteUse + " : " + timeControl.SecUse;
            else SuccessTimeSpent.text = "Time use : 0" + timeControl.MinuteUse + " : 0" + timeControl.SecUse;

            /*if (onLoadLevel.Level == 3)
            {
                NextLevel.SetActive(false);
            }
            else NextLevel.SetActive(true);*/

            //Success.SetActive(true);
            TweenSuccess.OpenCloseObjectAnimation();
        }
        else
        {
            PlaySound.PlayLose();

            isSuccess = false;
            if (CheckAdmin.isAdmin)
            {
                UserLevelSelect.SetActive(true);
                FailUserTryagin.SetActive(true);
                //AdminFail.SetActive(true);
            }
            else
            {
                AdminLevelSelect.SetActive(true);
                FailTryagin.SetActive(true);
                FailChecked.SetActive(true);
                //UserFail.SetActive(true);
            }

            AdminStatus.text = endStatus;
            AdminCmdCount.text = "Command count : " + (CommandBox.childCount - 1);

            if (timeControl.SecUse >= 10) AdminTimeSpent.text = "Time use : 0" + timeControl.MinuteUse + " : " + timeControl.SecUse;
            else AdminTimeSpent.text = "Time use : 0" + timeControl.MinuteUse + " : 0" + timeControl.SecUse;

            //Fail.SetActive(true);
            TweenFail.OpenCloseObjectAnimation();
        }
        //FinishDialog.SetActive(true);
    }

    
}
