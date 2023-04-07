using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Commands;
using System.Collections.Generic;

public class OpenAnswer : MonoBehaviour
{
    [SerializeField] private Transform CommandBox;
    [SerializeField] private GameObject OpenWindowObj;
    [SerializeField] private OnLoadLevel LoadLevel;
    [SerializeField] private Compiler compiler;
    [SerializeField] private MenuButton menuButton;
    [SerializeField] private CommandManager CmdManager;
    [SerializeField] private GameObject RespondDialog;
    [SerializeField] private Text RespondTxt;
    [SerializeField] private Transform UserList;
    [SerializeField] private Dropdown filterDropdown;
    [SerializeField] private InputField fitlerInput;
    [SerializeField] private Button filterSearch;
    [SerializeField] private GameObject ErrorDialog;
    [SerializeField] private Text ErrorTxt;
    [SerializeField] private Button SubmitError;
    [SerializeField] private GameFinish gameFinish;
    [SerializeField] private Button SuccessChecked;
    [SerializeField] private Button FailChecked;

    [Header("Tween")]
    [SerializeField] private EasyTween TweenOpen;
    [SerializeField] private EasyTween TweenRespond;
    [SerializeField] private EasyTween TweenError;

    [Header("URL")]
    [SerializeField] private string GetSession;
    [SerializeField] private string OpenAnswers;

    private bool ErrorAnswer = false;
    private bool ErrorSession = false;
    private bool ErrorConnect = false;
    private SoundControl PlaySound;

    void Start ()
    {
        PlaySound = GameObject.Find("SoundSFX").GetComponent<SoundControl>();
        //GetComponent<Button>().onClick.AddListener(Open);
        SubmitError.onClick.AddListener(()=>StartCoroutine(ExitErrorDialog()));
    }

    public void Open(GameObject userlist)
    {
        PlaySound.PlayClick();
        /*menuButton.OnOffParameter(false);
        ActiveUserList(false);
        ActiveFilter(false);

        SetPreWidthBlank(336);
        menuButton.ExitOpen.GetComponent<Button>().interactable = false;
        OpenWindowObj.SetActive(false);*/
        StartCoroutine(CheckSession(userlist));
    }

    public IEnumerator CheckSession(GameObject userlist)
    {
        menuButton.OnOffParameter(false);
        ActiveUserList(false);
        ActiveFilter(false);

        SetPreWidthBlank(336);
        menuButton.ExitOpen.GetComponent<Button>().interactable = false;
        //OpenWindowObj.SetActive(false);

        TweenOpen.OpenCloseObjectAnimation();
        yield return new WaitForEndOfFrame();
        TweenRespond.OpenCloseObjectAnimation();

        RespondTxt.text = "Checking session...";
        //RespondDialog.SetActive(true);

        WWW www = new WWW(GetSession);
        yield return www;

        if (www.text != "")
        {
            if (www.text == "Have Session.")
            {
                StartCoroutine(SendDatatoWeb(userlist));
            }
            else if (www.text == "Signed in overlapping.")
            {
                yield return new WaitForSeconds(0.5f);
                TweenRespond.OpenCloseObjectAnimation();

                yield return new WaitForEndOfFrame();
                PlaySound.PlayError();
                TweenError.OpenCloseObjectAnimation();

                ErrorSession = true;
                ErrorTxt.text = "Someone else has signed in with the account you want to sign in to. You can check your email for a new password reset.";
                //RespondDialog.SetActive(false);
                //ErrorDialog.SetActive(true);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                TweenRespond.OpenCloseObjectAnimation();

                yield return new WaitForEndOfFrame();
                PlaySound.PlayError();
                TweenError.OpenCloseObjectAnimation();

                ErrorConnect = true;
                ErrorTxt.text = "Sorry, an error occurred while connecting to the database. Please open the answer again.";
                //RespondDialog.SetActive(false);
                //ErrorDialog.SetActive(true);
            }
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            TweenRespond.OpenCloseObjectAnimation();

            yield return new WaitForEndOfFrame();
            PlaySound.PlayError();
            TweenError.OpenCloseObjectAnimation();

            ErrorConnect = true;
            ErrorTxt.text = "Sorry, an error occurred while connecting to the database. Please open the answer again.";
            //RespondDialog.SetActive(false);
            //ErrorDialog.SetActive(true);
        }
    }

    private IEnumerator SendDatatoWeb(GameObject userlist)
    {
        RespondTxt.text = "Opening answer...";

        WWWForm form = new WWWForm();
        form.AddField("user_id", int.Parse(userlist.name));
        form.AddField("level", LoadLevel.Level.ToString());

        WWW www = new WWW(OpenAnswers, form);
        yield return www;

        if (www.text != "")
        {
            if (www.text.IndexOf("Answer") != -1)
            {
                var answer = www.text.Remove(0, 6);   //Remove "Answer" text
                CrateCmd(answer);

                gameFinish.user_id = userlist.name;
                gameFinish.send_time = userlist.transform.GetChild(1).GetComponent<Text>().text;

                TweenRespond.OpenCloseObjectAnimation();
                //RespondDialog.SetActive(false);
                ActiveUserList(true);
                ActiveFilter(true);
                menuButton.OnOffParameter(true);
                menuButton.MenuOnAll();
                menuButton.isClick = false;
                menuButton.isDrop = true;
                menuButton.ExitOpen.GetComponent<Button>().interactable = true;

                if (userlist.transform.GetChild(2).gameObject.activeSelf)
                {
                    SuccessChecked.interactable = false;
                    FailChecked.interactable = false;
                }
                else
                {
                    SuccessChecked.interactable = true;
                    FailChecked.interactable = true;
                }
            }
            else if (www.text == "No data.") StartCoroutine(NotFoundData());
            else
            {
                yield return new WaitForSeconds(0.5f);
                TweenRespond.OpenCloseObjectAnimation();

                yield return new WaitForEndOfFrame();
                PlaySound.PlayError();
                TweenError.OpenCloseObjectAnimation();

                ErrorAnswer = true;
                //RespondDialog.SetActive(false);
                //ErrorDialog.SetActive(true);
                ErrorTxt.text = "Sorry, an error occurred while connecting to the database. Please open the answer again.";
            }

        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            TweenRespond.OpenCloseObjectAnimation();

            yield return new WaitForEndOfFrame();
            PlaySound.PlayError();
            TweenError.OpenCloseObjectAnimation();

            ErrorAnswer = true;
            //RespondDialog.SetActive(false);
            //ErrorDialog.SetActive(true);
            ErrorTxt.text = "Sorry, an error occurred while connecting to the database. Please open the answer again.";
        }
    }

    private  void CrateCmd(string text)
    {
        string Cmd = text;
        string[] ArrayCmd;
        string[] AttrCmd;
        int CountIndex = 0;

        if (Cmd != "")
        {
            DestroyCmd();
            ArrayCmd = Cmd.Split('\n');

            for (int i = 0; i < ArrayCmd.Length - 1; i++)
            {
                AttrCmd = ArrayCmd[i].Split(';');

                var cmd = Instantiate(Resources.Load("Prefab/"+ AttrCmd[0]), CommandBox) as GameObject;
                var Struct = cmd.GetComponent<StructCommand>();

                SetValueCmd(AttrCmd, Struct);
                SetInputValue(cmd, Struct);
                cmd.name = SetNameCmd(cmd.name);
                ShowFullCmd(cmd);
                SortOrderCmd(cmd, ref CountIndex);
                TabCmd(cmd, Struct);
                CmdManager.SetLineNumber();
            }
            //OpenWindowObj.SetActive(false);
        }
    }

    private void SetValueCmd(string[] AttrCmd, StructCommand Struct)
    {
        Struct.Id = (CommandsId)Enum.Parse(typeof(CommandsId), AttrCmd[1]);
        Struct.Type = (CommandsType)Enum.Parse(typeof(CommandsType), AttrCmd[2]);
        Struct.Sleep = int.Parse(AttrCmd[3]);
        Struct.Speed = int.Parse(AttrCmd[4]);
        Struct.ConditionId = int.Parse(AttrCmd[5]);
        Struct.InnerId = int.Parse(AttrCmd[6]);
        Struct.Port = int.Parse(AttrCmd[7]);
        Struct.conditionType = (ConditionType)Enum.Parse(typeof(ConditionType), AttrCmd[8]);
        Struct.conditionValue = (ConditionValue)Enum.Parse(typeof(ConditionValue), AttrCmd[9]);
        Struct.Range = int.Parse(AttrCmd[10]);
        Struct.LoopId = int.Parse(AttrCmd[11]);
        Struct.LoopType = (LoopType)Enum.Parse(typeof(LoopType), AttrCmd[12]);
        Struct.LoopCount = int.Parse(AttrCmd[13]);
        Struct.TmpLoopCount = int.Parse(AttrCmd[14]);
        Struct.endType = (EndType)Enum.Parse(typeof(EndType), AttrCmd[15]);
        Struct.Source = int.Parse(AttrCmd[16]);
        Struct.Destination = int.Parse(AttrCmd[17]);
        Struct.EndIfDestination = int.Parse(AttrCmd[18]);
    }

    private void SetInputValue(GameObject cmd, StructCommand Struct)
    {
        //Set input value
        if (Struct.Type == CommandsType.Transform)
        {
            var input = cmd.transform.GetChild(1);
            var speed = 0;

            //Set Speed
            if (Struct.Speed == 360000 || Struct.Speed == -360000) speed = 0;
            else if (Struct.Speed == 480000 || Struct.Speed == -480000) speed = 1;
            else if (Struct.Speed == 760000 || Struct.Speed == -760000) speed = 2;
            else if (Struct.Speed == 1000000 || Struct.Speed == -1000000) speed = 3;
            else if (Struct.Speed == 1500000 || Struct.Speed == -1500000) speed = 4;

            input.GetChild(1).GetComponent<Dropdown>().value = speed;
            input.GetChild(4).GetComponent<InputField>().text = Struct.Sleep.ToString();    //Set Sleep
        }
        else if(Struct.Type == CommandsType.Rotation)
        {
            var input = cmd.transform.GetChild(1);
            var speed = 0;

            //Set Speed
            if (Struct.Speed == 1 || Struct.Speed == -1) speed = 0;
            else if (Struct.Speed == 2 || Struct.Speed == -2) speed = 1;
            else if (Struct.Speed == 3 || Struct.Speed == -3) speed = 2;
            else if (Struct.Speed == 4 || Struct.Speed == -4) speed = 3;
            else if (Struct.Speed == 5 || Struct.Speed == -5) speed = 4;

            input.GetChild(1).GetComponent<Dropdown>().value = speed;
            input.GetChild(4).GetComponent<InputField>().text = Struct.Sleep.ToString();    //Set Sleep
        }
        else if(Struct.Type == CommandsType.Idle)
        {
            cmd.transform.GetChild(1).GetChild(2).GetComponent<InputField>().text = Struct.Sleep.ToString();
        }
        else if (Struct.Id == CommandsId.If || Struct.Id == CommandsId.ElseIf)
        {
            var input = cmd.transform.GetChild(1);
            var condition = 0;

            input.GetChild(1).GetChild(1).GetComponent<InputField>().text = Struct.Range.ToString();    //Set Range
            input.GetChild(2).GetComponent<Dropdown>().value = Struct.Port - 1;                         //Set Port

            //Set Condition
            if (Struct.conditionValue == ConditionValue.Touched) condition = 0;
            else if (Struct.conditionValue == ConditionValue.UnTouched) condition = 1;
            else if (Struct.conditionValue == ConditionValue.Black) condition = 2;
            else if (Struct.conditionValue == ConditionValue.White) condition = 3;
            else if (Struct.conditionValue == ConditionValue.None) condition = 4;

            input.GetChild(3).GetComponent<Dropdown>().value = condition;   
        }
        else if(Struct.Id == CommandsId.For)
        {
            var input = cmd.transform.GetChild(1);
            input.GetChild(1).GetComponent<InputField>().text = Struct.LoopCount.ToString();
        }
    }

    private void ShowFullCmd(GameObject cmd)
    {
        if (cmd.transform.name != "EndIFMini" && cmd.transform.name != "EndLoopMini")
        {
            cmd.transform.GetChild(0).gameObject.SetActive(false);
            cmd.transform.GetChild(1).gameObject.SetActive(true);
            cmd.transform.GetChild(1).localScale = new Vector3(1, 1, 1);
        }
    }

    private string SetNameCmd(string name)
    {
        if (name.IndexOf("(Clone)") != -1) return name.Remove(name.IndexOf("(Clone)"));
        else return name;
    }

    private void SortOrderCmd(GameObject cmd, ref int CountIndex)
    {
        cmd.transform.SetSiblingIndex(CommandBox.transform.GetSiblingIndex() + CountIndex);
        CountIndex++;
    }

    private void TabCmd(GameObject cmd,StructCommand Struct)
    {
        var PreWidth = cmd.GetComponent<LayoutElement>();
        RectTransform Pos;
        var InitPreWidth = 0;
        var ChildParam = 0;

        if (Struct.Id == CommandsId.If || Struct.Id == CommandsId.ElseIf || Struct.Type == CommandsType.Transform
           || Struct.Type == CommandsType.Rotation)
        {
            InitPreWidth = 336;
            ChildParam = 1;
        }
        else if (Struct.Type == CommandsType.Idle) { InitPreWidth = 206; ChildParam = 1; }
        else if (Struct.Id == CommandsId.Forever) { InitPreWidth = 129; ChildParam = 1; }
        else if (Struct.Id == CommandsId.For) { InitPreWidth = 206; ChildParam = 1; }
        else if (Struct.Id == CommandsId.EndIf) { InitPreWidth = 129; ChildParam = 0; }
        else if (Struct.Id == CommandsId.EndLoop) { InitPreWidth = 129; ChildParam = 0; }
        else if (Struct.Id == CommandsId.Break) { InitPreWidth = 129; ChildParam = 1; }
        else if (Struct.Id == CommandsId.Interact) { InitPreWidth = 129; ChildParam = 1; }
        else if (Struct.Id == CommandsId.Else) { InitPreWidth = 129; ChildParam = 1; }

        Pos = Struct.transform.GetChild(ChildParam).GetComponent<RectTransform>();
        PreWidth.preferredWidth = InitPreWidth + 50 * Struct.InnerId;
        Pos.localPosition = new Vector2(25 * Struct.InnerId, Pos.localPosition.y);

        //Set PreWidth Blank
        float PreWidthMax = 336;

        for (int i = 0; i < CommandBox.transform.childCount; i++)
        {
            var Cmd = CommandBox.transform.GetChild(i);
            var StructCmd = Cmd.GetComponent<StructCommand>();
            var PreWidthCmd = Cmd.GetComponent<LayoutElement>().preferredWidth;

            if (StructCmd == null) continue;
            else if (PreWidthCmd > PreWidthMax) PreWidthMax = PreWidthCmd;
        }
        
        SetPreWidthBlank(PreWidthMax);
    }

    private void DestroyCmd()
    {
        for (int i = CommandBox.transform.childCount - 2; i >= 0; i--)
        {
            DestroyImmediate(CommandBox.transform.GetChild(i).gameObject);
        }
        compiler.ListOptimize = new List<StructCommand>();
    }

    private void SetPreWidthBlank(float PreWidthMax)
    {
        var obj = CommandBox.transform.GetChild(CommandBox.transform.childCount - 1);
        obj.GetComponent<LayoutElement>().preferredWidth = PreWidthMax;
    }

    private IEnumerator NotFoundData()
    {
        RespondTxt.text = "Can not open the answer. Please try again.";

        yield return new WaitForSeconds(2);

        TweenRespond.OpenCloseObjectAnimation();
        //RespondDialog.SetActive(false);
        ActiveUserList(true);
        ActiveFilter(true);
        menuButton.MenuOnAll();
        menuButton.isClick = false;
        menuButton.isDrop = true;
        menuButton.ExitOpen.GetComponent<Button>().interactable = true;
        //OpenWindowObj.SetActive(true);
        TweenOpen.OpenCloseObjectAnimation();
    }

    private void ActiveUserList(bool isActive)
    {
        for(int i = 1;i < UserList.childCount; i++)
        {
            UserList.GetChild(i).GetComponent<Button>().interactable = isActive;
        }
    }

    private void ActiveFilter(bool isActive)
    {
        filterDropdown.interactable = isActive;
        fitlerInput.interactable = isActive;
        filterSearch.interactable = isActive;
    }

    private IEnumerator ExitErrorDialog()
    {
        if (ErrorAnswer)
        {
            ErrorAnswer = false;
            ActiveUserList(true);
            ActiveFilter(true);
            menuButton.OnOffParameter(true);
            menuButton.ExitOpen.GetComponent<Button>().interactable = true;
            //ErrorDialog.SetActive(false);
            //OpenWindowObj.SetActive(true);

            TweenError.OpenCloseObjectAnimation();
            yield return new WaitForEndOfFrame();
            TweenOpen.OpenCloseObjectAnimation();
        }
        else if (ErrorSession) Application.OpenURL("SendMailReset.php");
        else if (ErrorConnect)
        {
            ErrorConnect = false;
            ActiveUserList(true);
            ActiveFilter(true);
            menuButton.OnOffParameter(true);
            menuButton.ExitOpen.GetComponent<Button>().interactable = true;
            //ErrorDialog.SetActive(false);
            //OpenWindowObj.SetActive(true);

            TweenError.OpenCloseObjectAnimation();
            yield return new WaitForEndOfFrame();
            TweenOpen.OpenCloseObjectAnimation();
        }
    }
}
