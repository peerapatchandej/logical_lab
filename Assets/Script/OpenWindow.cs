using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OpenWindow : MonoBehaviour
{
    [SerializeField] private GameObject User;
    [SerializeField] private Transform Parent;
    [SerializeField] private GameObject OpenWindowObj;
    [SerializeField] private OnLoadLevel LoadLevel;
    [SerializeField] private MenuButton menuButton;
    [SerializeField] private GameObject RespondDialog;
    [SerializeField] private Text RespondTxt;
    [SerializeField] private GameObject ErrorDialog;
    [SerializeField] private Text ErrorTxt;
    [SerializeField] private Button SubmitError;

    [Header("Tween")]
    [SerializeField] private EasyTween TweenOpen;
    [SerializeField] private EasyTween TweenRespond;
    [SerializeField] private EasyTween TweenError;

    [Header("URL")]
    [SerializeField] private string GetSession;
    [SerializeField] private string UserList;

    [HideInInspector] public List<string> listName;
    [HideInInspector] public List<string> listDate;
    private bool ErrorWindow = false;
    private bool ErrorSession = false;
    private bool ErrorConnect = false;
    private SoundControl PlaySound;

    private void Start()
    {
        PlaySound = GameObject.Find("SoundSFX").GetComponent<SoundControl>();
        SubmitError.onClick.AddListener(ExitErrorDialog);
    }

    public void Open()
    {
        menuButton.OnOffParameter(false);

        for (int i = Parent.transform.childCount - 1; i >= 1; i--)
        {
            var user = Parent.transform.GetChild(i).gameObject;
            DestroyImmediate(user);
        }

        StartCoroutine(CheckSession());
    }

    private IEnumerator CheckSession()
    {
        PlaySound.PlayClick();
        TweenRespond.OpenCloseObjectAnimation();

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

    private IEnumerator SendDatatoWeb()
    {
        RespondTxt.text = "Searching for answers list...";

        WWWForm form = new WWWForm();
        form.AddField("level", LoadLevel.Level.ToString());

        WWW www = new WWW(UserList, form);
        yield return www;
        if (www.text != "")
        {
            if (www.text.IndexOf("Answer") != -1)
            {
                var answer = www.text.Remove(0, 6);   //Remove "Answer" text
                StartCoroutine(CreateUser(answer));
            }
            else if (www.text == "No data.") StartCoroutine(NotFoundData());
            else
            {
                yield return new WaitForSeconds(0.5f);
                TweenRespond.OpenCloseObjectAnimation();

                yield return new WaitForEndOfFrame();
                PlaySound.PlayError();
                TweenError.OpenCloseObjectAnimation();

                ErrorWindow = true;
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

            ErrorWindow = true;
            //RespondDialog.SetActive(false);
            //ErrorDialog.SetActive(true);
            ErrorTxt.text = "Sorry, an error occurred while connecting to the database. Please open the answer again.";
        }
    }

    private IEnumerator CreateUser(string text)
    {
        string list = text;
        string[] ArrayList;
        string[] AttrList;

        if (list != "")
        {
            ArrayList = list.Split('\n');

            for (int i = 0; i < ArrayList.Length - 1; i++)
            {
                AttrList = ArrayList[i].Split(';');
                User.SetActive(true);
                var user = Instantiate(User, Parent) as GameObject;
                user.name = AttrList[0];

                user.transform.GetChild(0).GetComponent<Text>().text = AttrList[1]+" "+AttrList[2];
                user.transform.GetChild(1).GetComponent<Text>().text = AttrList[3];

                if (AttrList[4] == "1") user.transform.GetChild(2).gameObject.SetActive(true);
                else user.transform.GetChild(3).gameObject.SetActive(true);

                User.SetActive(false);

                //Set List Data
                listName.Add(AttrList[1] + " " + AttrList[2]);
                listDate.Add(AttrList[3]);
            }
        }

        yield return new WaitForSeconds(0.5f);
        TweenRespond.OpenCloseObjectAnimation();

        yield return new WaitForEndOfFrame();
        TweenOpen.OpenCloseObjectAnimation();

        //OpenWindowObj.SetActive(true);
        //RespondDialog.SetActive(false);
    }

    private IEnumerator NotFoundData()
    {
        RespondTxt.text = "No answer list found.";
        
        yield return new WaitForSeconds(2);
        TweenRespond.OpenCloseObjectAnimation();
        //RespondDialog.SetActive(false);
        menuButton.OnOffParameter(true);
        menuButton.MenuOnAll();
        menuButton.isClick = false;
        menuButton.isDrop = true;
        //menuButton.ExitOpen.GetComponent<Button>().interactable = true;
    }

    private void ExitErrorDialog()
    {
        PlaySound.PlayClick();
        if (ErrorWindow)
        {
            ErrorWindow = false;
            //RespondDialog.SetActive(false);
            menuButton.OnOffParameter(true);
            menuButton.MenuOnAll();
            menuButton.isClick = false;
            menuButton.isDrop = true;
            //ErrorDialog.SetActive(false);
            TweenError.OpenCloseObjectAnimation();
        }
        else if (ErrorSession) Application.OpenURL("SendMailReset.php");
        else if (ErrorConnect)
        {
            ErrorConnect = false;
            //RespondDialog.SetActive(false);
            menuButton.OnOffParameter(true);
            menuButton.MenuOnAll();
            menuButton.isClick = false;
            menuButton.isDrop = true;
            //ErrorDialog.SetActive(false);
            TweenError.OpenCloseObjectAnimation();
        }
    }
}
