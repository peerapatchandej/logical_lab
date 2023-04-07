using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartGame : MonoBehaviour
{
    [SerializeField] private Button StartBtn;
    [SerializeField] private GameObject RespondDialog;
    [SerializeField] private Text RespondTxt;
    [SerializeField] private GameObject ErrorDialog;
    [SerializeField] private Text ErrorTxt;
    [SerializeField] private Button SubmitError;
    [SerializeField] private GameObject SoundBGM;
    [SerializeField] private GameObject SoundSFX;
    [SerializeField] private Animator anim;
    [SerializeField] private Image Black;
    [SerializeField] private Fading fade;
    [SerializeField] private string GetSession;

    [Header("Tween")]
    [SerializeField] private EasyTween[] TweenDialog; 

    [HideInInspector] public bool isAdmin = false;
    private bool ErrorSession = false;
    private bool ErrorConnect = false;
    private SoundControl PlaySound;
    

    private IEnumerator Start()
    {
        anim.Play("FadeIn");
        yield return new WaitUntil(() => Black.color.a == 0);

        Black.enabled = false;
        StartBtn.onClick.AddListener(() => StartCoroutine(Startgame()));
        SubmitError.onClick.AddListener(SendEmail);
        PlaySound = SoundSFX.GetComponent<SoundControl>();
    }

    private IEnumerator Startgame()
    {
        PlaySound.PlayClick();
        StartBtn.interactable = false;
        DontDestroyOnLoad(SoundBGM);

        Black.enabled = true;
        anim.Play("FadeOut");
        yield return new WaitUntil(() => Black.color.a == 1);
        SceneManager.LoadSceneAsync("Level Select");

        //StartCoroutine(SendDataToWeb());
    }

    private void SendEmail()
    {
        PlaySound.PlayClick();
        if (ErrorSession) Application.OpenURL("SendMailReset.php");
        else if(ErrorConnect)
        {
            ErrorConnect = false;
            //ErrorDialog.SetActive(false);
            StartBtn.interactable = true;
            TweenDialog[1].OpenCloseObjectAnimation();
        }
    }

    private IEnumerator SendDataToWeb()
    {
        TweenDialog[0].OpenCloseObjectAnimation();

        RespondTxt.text = "Loading...";
        //RespondDialog.SetActive(true);

        WWW www = new WWW(GetSession);
        yield return www;

        if (www.text != "")
        {
            if (www.text == "Have Session.")
            {
                StartCoroutine(GotoLevelSelect(true));
            }
            else if (www.text == "No have session.")
            {
                StartCoroutine(GotoLevelSelect(false));
            }
            else if(www.text ==  "Signed in overlapping.")
            {
                yield return new WaitForSeconds(0.5f);
                TweenDialog[0].OpenCloseObjectAnimation();

                yield return new WaitForEndOfFrame();
                PlaySound.PlayError();
                TweenDialog[1].OpenCloseObjectAnimation();

                ErrorSession = true;
                ErrorTxt.text = "Someone else has signed in with the account you want to sign in to. You can check your email for a new password reset.";
                //RespondDialog.SetActive(false);
                //ErrorDialog.SetActive(true);
            }
            else
            {

                yield return new WaitForSeconds(0.5f);
                TweenDialog[0].OpenCloseObjectAnimation();

                yield return new WaitForEndOfFrame();
                PlaySound.PlayError();
                TweenDialog[1].OpenCloseObjectAnimation();

                ErrorConnect = true;
                ErrorTxt.text = "Sorry, an error occurred while connecting to the database. Please click button to start game again.";
                //RespondDialog.SetActive(false);
                //ErrorDialog.SetActive(true);
            }
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            TweenDialog[0].OpenCloseObjectAnimation();

            yield return new WaitForEndOfFrame();
            PlaySound.PlayError();
            TweenDialog[1].OpenCloseObjectAnimation();

            ErrorConnect = true;
            ErrorTxt.text = "Sorry, an error occurred while connecting to the database. Please click button to start game again.";

            //RespondDialog.SetActive(false);
            //ErrorDialog.SetActive(true);
        }
    }

    private IEnumerator GotoLevelSelect(bool istrue)
    {
        TweenDialog[0].OpenCloseObjectAnimation();
        yield return new WaitForEndOfFrame();

        isAdmin = istrue;
        //RespondDialog.SetActive(false);
        //DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(SoundBGM);
        //DontDestroyOnLoad(SoundSFX);

        //yield return new WaitForSeconds(0.6f);
        /*var fadetime = fade.BeginFade(1);
        yield return new WaitForSeconds(fadetime);*/
        Black.enabled = true;
        anim.Play("FadeOut");
        yield return new WaitUntil(() => Black.color.a == 1);
        SceneManager.LoadSceneAsync("Level Select");
    }
}
