using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CheckedAnswer : MonoBehaviour
{
    [SerializeField] private GameObject FinishDialog;
    [SerializeField] private GameFinish gameFinish;
    [SerializeField] private GameObject RespondDialog;
    [SerializeField] private Text RespondTxt;
    [SerializeField] private GameObject ErrorDialog;
    [SerializeField] private Text ErrorTxt;
    [SerializeField] private Button SubmitError;

    [Header("Tween")]
    [SerializeField] private EasyTween TweenSuccess;
    [SerializeField] private EasyTween TweenFail;
    [SerializeField] private EasyTween TweenRespond;
    [SerializeField] private EasyTween TweenError;

    [Header("URL")]
    [SerializeField] private string GetSession;
    [SerializeField] private string CheckAnswer;

    private bool ErrorChecked = false;
    private bool ErrorSession = false;
    private bool ErrorConnect = false;
    private SoundControl PlaySound;

    private void Start()
    {
        PlaySound = GameObject.Find("SoundSFX").GetComponent<SoundControl>();
        SubmitError.onClick.AddListener(() => StartCoroutine(ExitErrorDialog()));
    }

    public void CheckedAnswers()
    {
        /*if (finish.isSuccess)
        {
            finish.SuccessTryagin.interactable = false;
            finish.NextLevel.GetComponent<Button>().interactable = false;
            finish.SuccessChecked.GetComponent<Button>().interactable = false;
        }
        else
        {
            finish.AdminLevelSelect.GetComponent<Button>().interactable = false;
            finish.FailTryagin.GetComponent<Button>().interactable = false;
            finish.FailChecked.GetComponent<Button>().interactable = false;
        }*/

        StartCoroutine(CheckSession());
    }

    private IEnumerator CheckSession()
    {
        PlaySound.PlayClick();

        if (gameFinish.isSuccess)
        {
            TweenSuccess.OpenCloseObjectAnimation();
            yield return new WaitForEndOfFrame();
        }
        else
        {
            TweenFail.OpenCloseObjectAnimation();
            yield return new WaitForEndOfFrame();
        }

        TweenRespond.OpenCloseObjectAnimation();
        //FinishDialog.SetActive(false);
        RespondTxt.text = "Checking session...";
        //RespondDialog.SetActive(true);

        WWW www = new WWW(GetSession);
        yield return www;

        if (www.text != "")
        {
            if (www.text == "Have Session.")
            {
                StartCoroutine(SendDatatoWeb());
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
                ErrorTxt.text = "Sorry, an error occurred while connecting to the database. Please check the answer again.";
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
            ErrorTxt.text = "Sorry, an error occurred while connecting to the database. Please check the answer again.";
            //RespondDialog.SetActive(false);
            //ErrorDialog.SetActive(true);
        }
    }

    private IEnumerator SendDatatoWeb()
    {
        RespondTxt.text = "Checking answer...";

        WWWForm form = new WWWForm();
        form.AddField("user_id", gameFinish.user_id);
        form.AddField("send_time", gameFinish.send_time);

        WWW www = new WWW(CheckAnswer, form);
        yield return www;

        if (www.text == "Checked success.")
        {
            StartCoroutine(CheckedComplete());
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            TweenRespond.OpenCloseObjectAnimation();

            yield return new WaitForEndOfFrame();
            PlaySound.PlayError();
            TweenError.OpenCloseObjectAnimation();

            ErrorChecked = true;
            //RespondDialog.SetActive(false);
            //ErrorDialog.SetActive(true);
            ErrorTxt.text = "Sorry, an error occurred while connecting to the database. Please check the answer again.";
        }
    }

    private IEnumerator CheckedComplete()
    {
        PlaySound.PlaySuccess();
        RespondTxt.text = "Check answer successfully.";

        yield return new WaitForSeconds(2);

        /*if (finish.isSuccess)
        {
            finish.SuccessTryagin.interactable = true;
            finish.NextLevel.GetComponent<Button>().interactable = true;
            finish.SuccessChecked.GetComponent<Button>().interactable = true;
        }
        else
        {
            finish.AdminLevelSelect.GetComponent<Button>().interactable = true;
            finish.FailTryagin.GetComponent<Button>().interactable = true;
            finish.FailChecked.GetComponent<Button>().interactable = true;
        }*/

        TweenRespond.OpenCloseObjectAnimation();
        yield return new WaitForEndOfFrame();

        if (gameFinish.isSuccess)
        {
            gameFinish.SuccessChecked.GetComponent<Button>().interactable = false;
            TweenSuccess.OpenCloseObjectAnimation();
        }
        else
        {
            gameFinish.FailChecked.GetComponent<Button>().interactable = false;
            TweenFail.OpenCloseObjectAnimation();
        }

        //RespondDialog.SetActive(false);
        //FinishDialog.SetActive(true);
    }

    private IEnumerator ExitErrorDialog()
    {
        if (ErrorChecked)
        {
            ErrorChecked = false;
            //LevelSelect.interactable = true;
            //Tryagin.interactable = true;
            //Checked.interactable = true;
            //finish.NextLevel.GetComponent<Button>().interactable = true;

            TweenError.OpenCloseObjectAnimation();
            yield return new WaitForEndOfFrame();

            if (gameFinish.isSuccess) TweenSuccess.OpenCloseObjectAnimation();
            else TweenFail.OpenCloseObjectAnimation();

            //ErrorDialog.SetActive(false);
            //FinishDialog.SetActive(true);
        }
        else if (ErrorSession) Application.OpenURL("SendMailReset.php");
        else if (ErrorConnect)
        {
            ErrorConnect = false;
            //LevelSelect.interactable = true;
            //Tryagin.interactable = true;
            //Checked.interactable = true;
            //finish.NextLevel.GetComponent<Button>().interactable = true;

            TweenError.OpenCloseObjectAnimation();
            yield return new WaitForEndOfFrame();

            if (gameFinish.isSuccess) TweenSuccess.OpenCloseObjectAnimation();
            else TweenFail.OpenCloseObjectAnimation();

            //ErrorDialog.SetActive(false);
            //FinishDialog.SetActive(true);
        }
    }
}
