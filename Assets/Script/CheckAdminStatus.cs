using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class CheckAdminStatus : MonoBehaviour
{
    [Header("User")]
    [SerializeField] private GameObject Send;
    [SerializeField] private GameObject SuccessSend;
    [SerializeField] private GameObject UserFail;

    [Header("Admin")]
    [SerializeField] private GameObject Open;
    [SerializeField] private GameObject SuccessCheck;
    [SerializeField] private GameObject AdminFail;
    [SerializeField] private GameObject Lock;

    [Header("Dialog")]
    [SerializeField] private Text RespondTxt;
    [SerializeField] private Text ErrorTxt;
    [SerializeField] private Button SubmitError;

    [Header("Tween")]
    [SerializeField] private EasyTween TweenRespond;
    [SerializeField] private EasyTween TweenError;
    [SerializeField] private EasyTween TweenInfo;

    [Header("ETC")]
    [SerializeField] private SoundControl PlaySound;
    [SerializeField] private Animator anim;
    [SerializeField] private Image Black;
    [SerializeField] private MenuButton menuButton;

    [Header("URL")]
    [SerializeField] private string GetSession;

    private bool ErrorSession = false;
    private bool ErrorConnect = false;
    public bool isAdmin = false;

    // Use this for initialization
    private IEnumerator Start()
    {
        SubmitError.onClick.AddListener(() => StartCoroutine(SendEmail()));

        TweenRespond.OpenCloseObjectAnimation();
        RespondTxt.text = "Loading...";

        WWW www = new WWW(GetSession);
        yield return www;

        if (www.text != "")
        {
            if (www.text == "Have Session.")
            {
                StartCoroutine(SetData(true));
            }
            else if (www.text == "No have session.")
            {
                //Debug.Log(www.text);
                StartCoroutine(SetData(false));
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
            }
            else
            {

                yield return new WaitForSeconds(0.5f);
                TweenRespond.OpenCloseObjectAnimation();

                yield return new WaitForEndOfFrame();
                PlaySound.PlayError();
                TweenError.OpenCloseObjectAnimation();

                ErrorConnect = true;
                ErrorTxt.text = "Sorry, an error occurred while connecting to the database.";
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
            ErrorTxt.text = "Sorry, an error occurred while connecting to the database.";
        }
	}

    private IEnumerator SendEmail()
    {
        PlaySound.PlayClick();
        if (ErrorSession) Application.OpenURL("SendMailReset.php");
        else if (ErrorConnect)
        {
            TweenError.OpenCloseObjectAnimation();
            yield return new WaitForEndOfFrame();

            ErrorConnect = false;
            DestroyObject(GameObject.Find("SoundBGM"));
            SceneManager.LoadSceneAsync("Home");
        }
    }

    private IEnumerator SetData(bool istrue)
    {
        yield return new WaitForSeconds(2);
        TweenRespond.OpenCloseObjectAnimation();
        yield return new WaitForEndOfFrame();

        isAdmin = istrue; 
        Send.SetActive(!istrue);
        SuccessSend.SetActive(!istrue);
        UserFail.SetActive(!istrue);
        Open.SetActive(istrue);
        SuccessCheck.SetActive(istrue);
        AdminFail.SetActive(istrue);
        Lock.SetActive(istrue);

        menuButton.isClick = true;
        menuButton.MenuOffAll();
        menuButton.Play.GetComponent<Button>().interactable = false;

        anim.Play("FadeIn");
        yield return new WaitUntil(() => Black.color.a == 0);
        Black.enabled = false;
        TweenInfo.OpenCloseObjectAnimation();
    }
}
