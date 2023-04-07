using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Commands;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private GameObject Stop,Tash,InfoRuleBtn,ExitInfo,TabInfo,TabRobot,TabCommand,Send,Open,Compile,LevelSelect,LogOutBtn;
    [SerializeField] private OnLoadLevel LoadLevel;
    [SerializeField] private GameObject CommandBox;
    [SerializeField] private GameObject Robot;
    [SerializeField] private GameObject InfoRule;
    [SerializeField] private GameObject CommandList;
    [SerializeField] private GameObject ChangeInfo,ChangeRobot,ChangeCommand;
    [SerializeField] private RunCommand RunCmd;
    [SerializeField] private Compiler compiler;
    [SerializeField] private GameObject Ball;
    [SerializeField] private GameObject OpenWindow,UserList;
    [SerializeField] private GameFinish gameFinish;
    [SerializeField] private CommandsController CmdController;
    [SerializeField] private GameObject Blade;
    [SerializeField] private GameObject MenuList;
    [SerializeField] private CheckFrontCol checkFront;
    [SerializeField] private BoxCollider FrontCol;
    [SerializeField] private BoxCollider BodyCol;
    [SerializeField] private SoundControl PlaySound;
    [SerializeField] private Animator anim;
    [SerializeField] private Image Black;
    [SerializeField] private GameObject ResonseDialog;
    [SerializeField] private Text ResponseTxt;

    [Header("Tween")]
    [SerializeField] private EasyTween TweenRespond;
    [SerializeField] private EasyTween TweenInfo;
    [SerializeField] private EasyTween TweenOpen;

    [HideInInspector] public bool isPlay = false;
    [HideInInspector] public bool isStop = false;
    [HideInInspector] public bool isCompile = false;
    [HideInInspector] public bool isAIPlay = false;
    [HideInInspector] public bool isDrop = false;
    [HideInInspector] public bool isClick = false;
    [HideInInspector] public bool lastCmd = false;
    [HideInInspector] public GameObject CameraOnPlay;
    public GameObject ExitOpen;
    public GameObject Play;
    private StructCommand Struct;
    
    //private Fading fade;

    public void Start()
    {
        Play.GetComponent<Button>().onClick.AddListener(Playgame);
        Stop.GetComponent<Button>().onClick.AddListener(Stopgame);
        Tash.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(TashCommand()));
        InfoRuleBtn.GetComponent<Button>().onClick.AddListener(ShowInfoRule);
        ExitInfo.GetComponent<Button>().onClick.AddListener(HideInfoRule);
        TabInfo.GetComponent<Button>().onClick.AddListener(ChangeTabInfo);
        TabRobot.GetComponent<Button>().onClick.AddListener(ChangeTabRobot);
        TabCommand.GetComponent<Button>().onClick.AddListener(ChangeTabCommand);
        Send.GetComponent<Button>().onClick.AddListener(SendData);
        Open.GetComponent<Button>().onClick.AddListener(OpenData);
        ExitOpen.GetComponent<Button>().onClick.AddListener(ExitOpenWindow);
        Compile.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(Compiler()));
        LevelSelect.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(BacktoLevelSelect()));
        LogOutBtn.GetComponent<Button>().onClick.AddListener(UserLogout);
    }

    private void Update()
    {
        if (CommandBox.transform.childCount > 1 && isClick == false && isDrop == true)
        {
            Tash.GetComponent<Button>().interactable = true;
            Send.GetComponent<Button>().interactable = true;
            Compile.GetComponent<Button>().interactable = true;
        }
        else
        {
            Tash.GetComponent<Button>().interactable = false;
            Send.GetComponent<Button>().interactable = false;
            Compile.GetComponent<Button>().interactable = false;
        }
    }

    private IEnumerator BacktoLevelSelect()
    {
        PlaySound.PlayClick();
        Black.enabled = true;
        anim.Play("FadeOut");
        yield return new WaitUntil(() => Black.color.a == 1);

        SceneManager.LoadSceneAsync("Level Select");
    }

    public IEnumerator Compiler()
    {
        PlaySound.PlayClick();

        TweenRespond.OpenCloseObjectAnimation();
        yield return new WaitForSeconds(0.1f);

        if (CommandBox.transform.childCount > 1)
        {
            ResponseTxt.text = "Compiling command...";
            ResonseDialog.SetActive(true);

            isClick = true;
            isCompile = true;
            isStop = false;
            lastCmd = false;

            MenuOffAll();
            Play.GetComponent<Button>().interactable = false;
            OnOffParameter(false);
            Compile.GetComponent<Compiler>().Compile();

            if (LoadLevel.Level == 2) Compile.GetComponent<Compiler>().CompileAI();

            if(ResonseDialog.activeSelf) StartCoroutine(DelayCompile());
        }
    }
    private IEnumerator DelayCompile()
    {
        yield return new WaitForSeconds(1);
        Play.GetComponent<Button>().interactable = true;
        TweenRespond.OpenCloseObjectAnimation();
    }
    public void Playgame()
    {
        PlaySound.PlayClick();
        isCompile = false;
        MenuOffAll();
        Stop.gameObject.SetActive(true);
        Stop.GetComponent<Button>().interactable = true;
        CameraOnPlay.SetActive(true);

        if (LoadLevel.Level != 1)
        {
            FrontCol.enabled = true;
            BodyCol.enabled = true;
        }
        else
        {
            Blade.GetComponent<MeshCollider>().enabled = false;
            Blade.transform.localPosition = new Vector3(Blade.transform.localPosition.x, 0, Blade.transform.localPosition.z);
        }

        StartCoroutine(Delay(0.5f));
    }
    public void Stopgame()
    {
        PlaySound.PlayClick();
        isClick = false;
        isPlay = false;
        isAIPlay = false;
        isStop = true;

        CmdController.TimeSleep = 0;
        CmdController.InteractTime = 1;
        Stop.gameObject.SetActive(false);
        CameraOnPlay.SetActive(false);
        MenuOnAll();
        OnOffParameter(true);

        if(LoadLevel.Level != 3) gameFinish.ScoreCount = 0;
        else gameFinish.ScoreCount = 100;

        if (LoadLevel.Level == 1)
        {
            CmdController.InteractTime = 1;
            Blade.GetComponent<MeshCollider>().enabled = false;
            Blade.transform.localPosition = new Vector3(Blade.transform.localPosition.x, 0, Blade.transform.localPosition.z);
            ResetBall();
        }
        else
        {
            checkFront.isEnter = true;
            FrontCol.enabled = false;
            BodyCol.enabled = false;
        }
    }
    public IEnumerator TashCommand()
    {
        PlaySound.PlayClick();
        TweenRespond.OpenCloseObjectAnimation();
        yield return new WaitForSeconds(0.1f);

        ResponseTxt.text = "Deleting commands...";

        isClick = true;
        MenuOffAll();
        Play.GetComponent<Button>().interactable = false;

        for (int i = CommandBox.transform.childCount - 2; i >= 0 ; i--)
        {
            DestroyImmediate(CommandBox.transform.GetChild(i).gameObject);
        }
        StartCoroutine(DelayTash());
    }

    private IEnumerator DelayTash()
    {
        yield return new WaitForSeconds(1);
        TweenRespond.OpenCloseObjectAnimation();

        var obj = CommandBox.transform.GetChild(CommandBox.transform.childCount - 1);
        obj.GetComponent<LayoutElement>().preferredWidth = 336;

        compiler.ListOptimize = new List<StructCommand>();

        isClick = false;
        MenuOnAll();
        Play.GetComponent<Button>().interactable = false;

    }

    public void ShowInfoRule()
    {
        PlaySound.PlayClick();
        isClick = true;
        TweenInfo.OpenCloseObjectAnimation();
        MenuOffAll();
        Play.GetComponent<Button>().interactable = false;
        
    }
    public void HideInfoRule()
    {
        PlaySound.PlayClick();
        isClick = false;
        TweenInfo.OpenCloseObjectAnimation();
        MenuOnAll();
        
    }
    public void ChangeTabInfo()
    {
        PlaySound.PlayClick();
        ChangeInfo.SetActive(true);
        ChangeRobot.SetActive(false);
        ChangeCommand.SetActive(false);
    }
    public void ChangeTabRobot()
    {
        PlaySound.PlayClick();
        ChangeInfo.SetActive(false);
        ChangeRobot.SetActive(true);
        ChangeCommand.SetActive(false);
    }
    public void ChangeTabCommand()
    {
        PlaySound.PlayClick();
        ChangeInfo.SetActive(false);
        ChangeRobot.SetActive(false);
        ChangeCommand.SetActive(true);
    }
    private void UserLogout()
    {
        isClick = true;
        MenuOffAll();
        LogOutBtn.GetComponent<Logout>().LogoutUser();
    }
    public void SendData()
	{
        PlaySound.PlayClick();
        isClick = true;
        MenuOffAll();
        Play.GetComponent<Button>().interactable = false;
        StartCoroutine(Send.GetComponent<SendAnswer>().ShowSubmitDialog());
    }
    public void OpenData()
    {
        PlaySound.PlayClick();
        isClick = true;
        MenuOffAll();
        Play.GetComponent<Button>().interactable = false;
        Open.GetComponent<OpenWindow>().Open();
    }

    private void ExitOpenWindow()
    {
        PlaySound.PlayClick();
        for (int i = UserList.transform.childCount - 1; i >= 1 ; i--)
        {
            var user = UserList.transform.GetChild(i).gameObject;
            DestroyImmediate(user);
        }

        isClick = false;
        TweenOpen.OpenCloseObjectAnimation();
        MenuOnAll();
        OnOffParameter(true);
    }

    private IEnumerator Delay(float time)
    {
        yield return new WaitForSeconds(time);
        isPlay = true;
        isAIPlay = true;
        isStop = false;
    }

    public void ResetBall()
    {
        for(int i = 0; i < Ball.transform.childCount; i++)
        {
            var ball = Ball.transform.GetChild(i).gameObject;
            if (ball.activeSelf == false) ball.SetActive(true);
        }
        gameFinish.CountBall = Ball.transform.childCount;
    }

    public void OnOffParameter(bool isLock)
    {
        for (int i = 0; i < CommandBox.transform.childCount - 1; i++)
        {
            var Command = CommandBox.transform.GetChild(i);
            Struct = Command.GetComponent<StructCommand>();

            if (Struct.Type == CommandsType.Transform || Struct.Type == CommandsType.Rotation)
            {
                var cmd = Command.transform.GetChild(1).transform;
                cmd.GetChild(1).GetComponent<Dropdown>().interactable = isLock;
                cmd.GetChild(4).GetComponent<InputField>().interactable = isLock;
            }
            else if (Struct.Type == CommandsType.Idle)
            {
                Command.transform.GetChild(1).GetChild(2).GetComponent<InputField>().interactable = isLock;
            }
            else if (Struct.Id == CommandsId.If || Struct.Id == CommandsId.ElseIf)
            {
                var cmd = Command.transform.GetChild(1).transform;
                cmd.GetChild(2).GetComponent<Dropdown>().interactable = isLock;
                cmd.GetChild(3).GetComponent<Dropdown>().interactable = isLock;
            }
            else if (Struct.Id == CommandsId.For)
            {
                Command.transform.GetChild(1).GetChild(1).GetComponent<InputField>().interactable = isLock;
            }
        }
    }

    public void MenuOffAll()
    {
        for(int i = 0; i < MenuList.transform.childCount; i++)
        {
            MenuList.transform.GetChild(i).GetComponent<Button>().interactable = false;
        }
        Compile.GetComponent<Button>().interactable = false;
        Play.GetComponent<Button>().interactable = true;
    }

    public void MenuOnAll()
    {
        for (int i = 0; i < MenuList.transform.childCount; i++)
        {
            MenuList.transform.GetChild(i).GetComponent<Button>().interactable = true;
        }
        Compile.GetComponent<Button>().interactable = true;
        Play.GetComponent<Button>().interactable = false;
    }
}
