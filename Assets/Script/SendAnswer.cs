using UnityEngine;
using Commands;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class SendAnswer : MonoBehaviour
{
    [SerializeField] private OnLoadLevel LoadLevel;
    [SerializeField] private GameObject CommandBox;
    [SerializeField] private MenuButton menuButton;
    [SerializeField] private GameObject SendDialog;
    [SerializeField] private InputField Name;
    [SerializeField] private InputField Surname;
    [SerializeField] private InputField Email;
    [SerializeField] private Button SendEmailBtn;
    [SerializeField] private Button Exit;
    [SerializeField] private GameObject SubmitEmailDialog;
    [SerializeField] private Button YesBtn;
    [SerializeField] private Button NoBtn;
    [SerializeField] private Compiler compile;
    [SerializeField] private Text NameWarningTxt;
    [SerializeField] private Text SurnameWarningTxt;
    [SerializeField] private Text EmailWarningTxt;
    [SerializeField] private GameObject ErrorDialog;
    [SerializeField] private Text ErrorTxt;
    [SerializeField] private Button SubmitError;
    [SerializeField] private GameObject ResponseDialog;
    [SerializeField] private Text ResponseTxt;
    [SerializeField] private GameFinish gameFinish;
    [SerializeField] private GameObject FinishDialog;

    [Header("Tween")]
    [SerializeField] private EasyTween TweenSend;
    [SerializeField] private EasyTween TweenConfirm;
    [SerializeField] private EasyTween TweenRespond;
    [SerializeField] private EasyTween TweenError;
    [SerializeField] private EasyTween TweenSuccess;
    [SerializeField] private EasyTween TweenFail;

    [Header("URL")]
    [SerializeField] private string CheckUserDatas;
    [SerializeField] private string InsertUserDatas;
    [SerializeField] private string UpdateUserDatas;
    [SerializeField] private string SendAnswers;

    private bool ErrorSend = false;
    private SoundControl PlaySound;

    private string StringCmd = "";

    private void Start()
    {
        PlaySound = GameObject.Find("SoundSFX").GetComponent<SoundControl>();

        SendEmailBtn.onClick.AddListener(Send);
        YesBtn.onClick.AddListener(() => StartCoroutine(YesSelected()));
        NoBtn.onClick.AddListener(() => StartCoroutine(NoSelected()));
        Exit.onClick.AddListener(()=> StartCoroutine(ExitDialog()));
        SubmitError.onClick.AddListener(() => StartCoroutine(ExitErrorDialog()));
    }

    public IEnumerator ShowSubmitDialog()
    {
        if (CommandBox.transform.childCount > 1)
        {
            menuButton.OnOffParameter(false);
            compile.Compile();

            if (compile.ListOptimize.Count > 0)
            {
                ResetText();
                if (gameFinish.isFinish)
                {
                    //FinishDialog.SetActive(false);
                    TweenSuccess.OpenCloseObjectAnimation();
                    yield return new WaitForEndOfFrame();
                }
                //SendDialog.SetActive(true);
                TweenSend.OpenCloseObjectAnimation();
            }
        }
    }

    private IEnumerator ExitDialog()
    {
        PlaySound.PlayClick();
        if (gameFinish.isFinish)
        {
            compile.ListOptimize = new List<StructCommand>();
            gameFinish.SuccessTryagin.interactable = true;
            gameFinish.SuccessSend.GetComponent<Button>().interactable = true;
            gameFinish.NextLevel.GetComponent<Button>().interactable = true;
            //SendDialog.SetActive(false);
            TweenSend.OpenCloseObjectAnimation();
            yield return new WaitForEndOfFrame();
            TweenSuccess.OpenCloseObjectAnimation();
            //FinishDialog.SetActive(true);
        }
        else
        {
            menuButton.OnOffParameter(true);
            compile.ListOptimize = new List<StructCommand>();
            //SendDialog.SetActive(false);
            TweenSend.OpenCloseObjectAnimation();
            EnableButton();
        }
    }

    public void Send()
    {
        PlaySound.PlayClick();
        InteractableDialog(false);

        if (AlphabetsValidate(Name) && AlphabetsValidate(Surname) && EmailValidate())
        {
            StringCmd = "";
            NameWarningTxt.text = "";
            SurnameWarningTxt.text = "";
            EmailWarningTxt.text = "";
            StartCoroutine(CheckUserData());
        }
        else
        {
            InteractableDialog(true);

            if (!AlphabetsValidate(Name))
            {
                if (Name.text != "") NameWarningTxt.text = "Please enter your English name.";
                else NameWarningTxt.text = "Please enter your name.";
            }
            else NameWarningTxt.text = "";

            if (!AlphabetsValidate(Surname))
            {
                if (Surname.text != "") SurnameWarningTxt.text = "Please enter your English last name.";
                else SurnameWarningTxt.text = "Please enter your last name.";
            }
            else SurnameWarningTxt.text = "";

            if (!EmailValidate())
            {
                if(Email.text != "") EmailWarningTxt.text = "Email must be in the format: your@email.com.";
                else EmailWarningTxt.text = "Please enter your email address.";
            }
            else EmailWarningTxt.text = "";
        }
    }

    private bool EmailValidate()
    {
        Regex mailValidator = new Regex(@"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$");
        return mailValidator.IsMatch(Email.text);
    }

    private bool AlphabetsValidate(InputField input)
    {
        Regex nameValidator = new Regex("^[A-Za-z]+$");
        return nameValidator.IsMatch(input.text);
    }

    private void CreateStringCommand()
    {
        for (int i = 0; i < CommandBox.transform.childCount - 1; i++)
        {
            GameObject Cmd = CommandBox.transform.GetChild(i).gameObject;
            StructCommand Struct = Cmd.GetComponent<StructCommand>();

            Cmd.name = SetNameCmd(Cmd.name);

            StringCmd += Cmd.name + ";" +
                         Struct.Id.ToString() + ";" +
                         Struct.Type.ToString() + ";" +
                         Struct.Sleep.ToString() + ";" +
                         Struct.Speed.ToString() + ";" +
                         Struct.ConditionId.ToString() + ";" +
                         Struct.InnerId.ToString() + ";" +
                         Struct.Port.ToString() + ";" +
                         Struct.conditionType.ToString() + ";" +
                         Struct.conditionValue.ToString() + ";" +
                         Struct.Range.ToString() + ";" +
                         Struct.LoopId.ToString() + ";" +
                         Struct.LoopType.ToString() + ";" +
                         Struct.LoopCount.ToString() + ";" +
                         Struct.TmpLoopCount.ToString() + ";" +
                         Struct.endType.ToString() + ";" +
                         Struct.Source.ToString() + ";" +
                         Struct.Destination.ToString() + ";" +
                         Struct.EndIfDestination.ToString() + "\n";
        }
    }

    private string SetNameCmd(string name)
    {
        if (name.IndexOf("(Clone)") != -1) return name.Remove(name.IndexOf("(Clone)"));
        else return name;
    }

    private IEnumerator CheckUserData()
    {
        TweenSend.OpenCloseObjectAnimation();
        yield return new WaitForEndOfFrame();
        TweenRespond.OpenCloseObjectAnimation();

        //SendDialog.SetActive(false);
        //ResponseDialog.SetActive(true);
        ResponseTxt.text = "Checking user data...";

        WWWForm form = new WWWForm();
        form.AddField("email", Email.text);
        form.AddField("level", LoadLevel.Level.ToString());

        WWW www = new WWW(CheckUserDatas, form);
        yield return www;

        if(www.text == "Found user data in database.")
        {
            yield return new WaitForSeconds(0.5f);
            TweenRespond.OpenCloseObjectAnimation();
            yield return new WaitForEndOfFrame();
            //ResponseDialog.SetActive(false);
            InteractableSubmitEmailDialog(true);
            //SubmitEmailDialog.SetActive(true);
            TweenConfirm.OpenCloseObjectAnimation();
        }
        else if(www.text == "Update user data in database.")
        {
            StartCoroutine(UpdateUserData());
        }
        else if(www.text == "Not found user in database.")
        {
            StartCoroutine(InsertUserData());
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            TweenRespond.OpenCloseObjectAnimation();

            yield return new WaitForEndOfFrame();
            PlaySound.PlayError();
            TweenError.OpenCloseObjectAnimation();

            ErrorSend = true;
            //ResponseDialog.SetActive(false);
            //ErrorDialog.SetActive(true);
            ErrorTxt.text = "Sorry, an error occurred while connecting to the database. Please send the answer again.";
        }
    }

    private IEnumerator InsertUserData()
    {
        //TweenRespond.OpenCloseObjectAnimation();
        ResponseTxt.text = "Adding user data...";

        WWWForm form = new WWWForm();
        form.AddField("name", Name.text);
        form.AddField("surname", Surname.text);
        form.AddField("email", Email.text);
        form.AddField("level", LoadLevel.Level.ToString());

        WWW www = new WWW(InsertUserDatas, form);
        yield return www;

        if(www.text == "Insert user data complete.")
        {
            CreateStringCommand();
            StartCoroutine(SendAnswerData(StringCmd));
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            TweenRespond.OpenCloseObjectAnimation();

            yield return new WaitForEndOfFrame();
            PlaySound.PlayError();
            TweenError.OpenCloseObjectAnimation();

            ErrorSend = true;
            //ResponseDialog.SetActive(false);
            //ErrorDialog.SetActive(true);
            ErrorTxt.text = "Sorry, an error occurred while connecting to the database. Please send the answer again.";
        }
    }

    private IEnumerator UpdateUserData()
    {
        //ResponseDialog.SetActive(true);
        ResponseTxt.text = "Editing user data...";

        WWWForm form = new WWWForm();
        form.AddField("name", Name.text);
        form.AddField("surname", Surname.text);
        form.AddField("email", Email.text);
        form.AddField("level", LoadLevel.Level.ToString());

        WWW www = new WWW(UpdateUserDatas, form);
        yield return www;

        if (www.text == "Update user data complete.")
        {
            CreateStringCommand();
            StartCoroutine(SendAnswerData(StringCmd));
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            TweenRespond.OpenCloseObjectAnimation();

            yield return new WaitForEndOfFrame();
            PlaySound.PlayError();
            TweenError.OpenCloseObjectAnimation();

            ErrorSend = true;
            //ResponseDialog.SetActive(false);
            //ErrorDialog.SetActive(true);
            ErrorTxt.text = "Sorry, an error occurred while connecting to the database. Please send the answer again.";
        }
    }

    private IEnumerator SendAnswerData(string cmd)
    {
        ResponseTxt.text = "Sending answer and email...";

        WWWForm form = new WWWForm();
        form.AddField("level", LoadLevel.Level.ToString());
        form.AddField("cmd", cmd);
        form.AddField("name", Name.text);
        form.AddField("surname", Surname.text);
        form.AddField("email", Email.text);

        WWW www = new WWW(SendAnswers, form);
        yield return www;

        if(www.text == "Send mail complete." || www.text == "Send answer complete.")
        {
            StartCoroutine(SendAnswerComplete());
        }
        else if(www.text == "Send answer incomplete.")
        {
            yield return new WaitForSeconds(0.5f);
            TweenRespond.OpenCloseObjectAnimation();

            yield return new WaitForEndOfFrame();
            PlaySound.PlayError();
            TweenError.OpenCloseObjectAnimation();

            ErrorSend = true;
            //ResponseDialog.SetActive(false);
            //ErrorDialog.SetActive(true);
            ErrorTxt.text = "Sorry, an error occurred while sending answer. Please send the answer again.";
        }
        else if(www.text == "Send mail incomplete.")
        {
            yield return new WaitForSeconds(0.5f);
            TweenRespond.OpenCloseObjectAnimation();

            yield return new WaitForEndOfFrame();
            PlaySound.PlayError();
            TweenError.OpenCloseObjectAnimation();

            ErrorSend = true;
            //ResponseDialog.SetActive(false);
            //ErrorDialog.SetActive(true);
            ErrorTxt.text = "Sorry, an error occurred while sending email. Please send the answer again.";
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            TweenRespond.OpenCloseObjectAnimation();

            yield return new WaitForEndOfFrame();
            PlaySound.PlayError();
            TweenError.OpenCloseObjectAnimation();

            ErrorSend = true;
            //ResponseDialog.SetActive(false);
            //ErrorDialog.SetActive(true);
            ErrorTxt.text = "Sorry, an error occurred while connecting to the database. Please send the answer again.";
        }
    }

    private IEnumerator SendAnswerComplete()
    {
        PlaySound.PlaySuccess();
        ClearCommand();
        ResponseTxt.text = "Send the answer successfully.";

        yield return new WaitForSeconds(2);

        menuButton.OnOffParameter(true);
        TweenRespond.OpenCloseObjectAnimation();
        //ResponseDialog.SetActive(false);
        InteractableDialog(true);
        InteractableSubmitEmailDialog(true);
        //SubmitEmailDialog.SetActive(false);
        //SendDialog.SetActive(false);

        if (gameFinish.isFinish)
        {
            gameFinish.SuccessTryagin.interactable = true;
            //gameFinish.SuccessSend.GetComponent<Button>().interactable = false;
            gameFinish.NextLevel.GetComponent<Button>().interactable = true;
            //FinishDialog.SetActive(true);
            TweenSuccess.OpenCloseObjectAnimation();
        }
        else EnableButton();
    }

    private IEnumerator YesSelected()
    {
        PlaySound.PlayClick();
        InteractableSubmitEmailDialog(false);
        //SubmitEmailDialog.SetActive(false);
        TweenConfirm.OpenCloseObjectAnimation();
        yield return new WaitForEndOfFrame();
        TweenRespond.OpenCloseObjectAnimation();
        StartCoroutine(UpdateUserData());
    }

    private IEnumerator NoSelected()
    {
        PlaySound.PlayClick();
        NameWarningTxt.text = "";
        SurnameWarningTxt.text = "";
        EmailWarningTxt.text = "";
        //SubmitEmailDialog.SetActive(false);
        InteractableDialog(true);
        //SendDialog.SetActive(true);
        TweenConfirm.OpenCloseObjectAnimation();
        yield return new WaitForEndOfFrame();
        TweenSend.OpenCloseObjectAnimation();
    }

    private void EnableButton()
    {
        menuButton.MenuOnAll();
        menuButton.isClick = false;
    }

    private void ClearCommand()
    {
        for (int i = CommandBox.transform.childCount - 2; i >= 0; i--)
        {
            DestroyImmediate(CommandBox.transform.GetChild(i).gameObject);
        }

        var obj = CommandBox.transform.GetChild(CommandBox.transform.childCount - 1);
        obj.GetComponent<LayoutElement>().preferredWidth = 336;

        compile.ListOptimize = new List<StructCommand>();
    }

    private void ResetText()
    {
        StringCmd = "";
        Name.text = "";
        Surname.text = "";
        Email.text = "";

        NameWarningTxt.text = "";
        SurnameWarningTxt.text = "";
        EmailWarningTxt.text = "";
    }

    private void InteractableDialog(bool isbool)
    {
        Name.interactable = isbool;
        Surname.interactable = isbool;
        Email.interactable = isbool;
        SendEmailBtn.interactable = isbool;
        Exit.interactable = isbool;
    }
    private void InteractableSubmitEmailDialog(bool isbool)
    {
        YesBtn.interactable = isbool;
        NoBtn.interactable = isbool;
    }

    private IEnumerator ExitErrorDialog()
    {
        PlaySound.PlayClick();
        if (ErrorSend)
        {
            if (SubmitEmailDialog.activeSelf == true)
            {
                InteractableSubmitEmailDialog(true);
                SubmitEmailDialog.SetActive(false);
            }
            ErrorSend = false;
            InteractableDialog(true);
            compile.ListOptimize = new List<StructCommand>();
            //ErrorDialog.SetActive(false);
            //SendDialog.SetActive(true);

            TweenError.OpenCloseObjectAnimation();
            yield return new WaitForEndOfFrame();
            TweenSend.OpenCloseObjectAnimation();
        }
    }
}
