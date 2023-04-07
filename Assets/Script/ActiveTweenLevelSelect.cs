using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Text;

public class ActiveTweenLevelSelect : MonoBehaviour {

    [SerializeField] private EasyTween[] TweenObject;
    [SerializeField] private Animator anim;
    [SerializeField] private Image Black;
    [SerializeField] private Text RespondTxt;
    [SerializeField] private Text ErrorTxt;
    [SerializeField] private SoundControl PlaySound;
    [SerializeField] private Button SubmitError;

    [Header("URL")]
    [SerializeField] private string GetSession;

    private bool ErrorSession = false;
    private bool ErrorConnect = false;
    public bool isAdmin = false;

    private IEnumerator Start()
    {
        SubmitError.onClick.AddListener(() => StartCoroutine(SendEmail()));

        TweenObject[5].OpenCloseObjectAnimation();
        RespondTxt.text = "Loading...";

        /*Dictionary<string, string> headers = new Dictionary<string, string>();
        headers["Authorization"] = "Basic " + System.Convert.ToBase64String(Encoding.ASCII.GetBytes("u352742427:logicallab"));*/

        WWW www = new WWW(GetSession);
        yield return www;
            
        if (www.text != "")
        {
            if (www.text == "Have Session.")
            {
                yield return new WaitForSeconds(2);
                TweenObject[5].OpenCloseObjectAnimation();
                yield return new WaitForEndOfFrame();

                isAdmin = true;
                anim.Play("FadeIn");
                yield return new WaitUntil(() => Black.color.a == 0);
                Black.enabled = false;
            }
            else if (www.text == "No have session.")
            {
                yield return new WaitForSeconds(2);
                TweenObject[5].OpenCloseObjectAnimation();
                yield return new WaitForEndOfFrame();

                isAdmin = false;
                anim.Play("FadeIn");
                yield return new WaitUntil(() => Black.color.a == 0);
                Black.enabled = false;
            }
            else if (www.text == "Signed in overlapping.")
            {
                yield return new WaitForSeconds(0.5f);
                TweenObject[5].OpenCloseObjectAnimation();

                yield return new WaitForEndOfFrame();
                PlaySound.PlayError();
                TweenObject[6].OpenCloseObjectAnimation();

                ErrorSession = true;
                ErrorTxt.text = "Someone else has signed in with the account you want to sign in to. You can check your email for a new password reset.";
            }
            else
            {

                yield return new WaitForSeconds(0.5f);
                TweenObject[5].OpenCloseObjectAnimation();

                yield return new WaitForEndOfFrame();
                PlaySound.PlayError();
                TweenObject[6].OpenCloseObjectAnimation();

                ErrorConnect = true;
                ErrorTxt.text = "Sorry, an error occurred while connecting to the database.";
            }
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            TweenObject[5].OpenCloseObjectAnimation();

            yield return new WaitForEndOfFrame();
            PlaySound.PlayError();
            TweenObject[6].OpenCloseObjectAnimation();

            ErrorConnect = true;
            ErrorTxt.text = "Sorry, an error occurred while connecting to the database.";
        }

        yield return new WaitForSeconds(0.4f);
        TweenObject[0].OpenCloseObjectAnimation();

        yield return new WaitForSeconds(0.7f);
        TweenObject[1].OpenCloseObjectAnimation();

        yield return new WaitForSeconds(0.3f);
        TweenObject[2].OpenCloseObjectAnimation();

        yield return new WaitForSeconds(0.3f);
        TweenObject[3].OpenCloseObjectAnimation();

        yield return new WaitForSeconds(0.3f);
        TweenObject[4].OpenCloseObjectAnimation();
    }

    private IEnumerator SendEmail()
    {
        PlaySound.PlayClick();
        if (ErrorSession) Application.OpenURL("SendMailReset.php");
        else if (ErrorConnect)
        {
            TweenObject[6].OpenCloseObjectAnimation();
            yield return new WaitForEndOfFrame();

            ErrorConnect = false;
            DestroyObject(GameObject.Find("SoundBGM"));
            SceneManager.LoadSceneAsync("Home");
        }
    }
}
