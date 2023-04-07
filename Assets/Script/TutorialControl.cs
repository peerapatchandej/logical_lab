using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialControl : MonoBehaviour
{
    [SerializeField] private Button Tutorial;
    [SerializeField] private Button Previous;
    [SerializeField] private Button Next;
    [SerializeField] private Button Exit;
    [SerializeField] private GameObject TutorialWindow;
    [SerializeField] private GameObject UserTutorial;
    [SerializeField] private GameObject AdminTutorial;
    [SerializeField] private ActiveTweenLevelSelect StartLevelSelect;
    [SerializeField] private SoundControl PlaySound;

    [Header("User Tutorial")]
    [SerializeField] private GameObject UserPage1;
    [SerializeField] private GameObject UserPage2;
    [SerializeField] private GameObject UserPage3;

    [Header("Admin Tutorial")]
    [SerializeField] private GameObject AdminPage1;
    [SerializeField] private GameObject AdminPage2;
    [SerializeField] private GameObject AdminPage3;

    [Header("Level Button")]
    [SerializeField] private Button Level1;
    [SerializeField] private Button Level2;
    [SerializeField] private Button Level3;

    [Header("Tween")]
    [SerializeField] private EasyTween TweenTutorial;
    [SerializeField] private EasyTween TweenRespond;
    [SerializeField] private EasyTween TweenError;

    //private bool isAdmin;
    
	// Use this for initialization
	void Start ()
    {
        //isAdmin = GameObject.Find("UserControl").GetComponent<StartGame>().isAdmin;

        //if (isAdmin) AdminTutorial.SetActive(true);
        //else UserTutorial.SetActive(true);

        Previous.interactable = false;

        Tutorial.onClick.AddListener(OpenTutorial);
        Previous.onClick.AddListener(PreviousInfo);
        Next.onClick.AddListener(NextInfo);
        Exit.onClick.AddListener(ExitTutorial);
    }

    private void OpenTutorial()
    {
        if(StartLevelSelect.isAdmin) AdminTutorial.SetActive(true);
        else UserTutorial.SetActive(true);

        PlaySound.PlayClick();

        Tutorial.interactable = false;
        Level1.interactable = false;
        Level2.interactable = false;
        Level3.interactable = false;
        //TutorialWindow.SetActive(true);

        TweenTutorial.OpenCloseObjectAnimation();
        //yield return new WaitForSeconds(0.1f);
    }

    private void PreviousInfo()
    {
        PlaySound.PlayClick();
        if (StartLevelSelect.isAdmin)
        {
            if (AdminPage3.activeSelf)
            {
                AdminPage3.SetActive(false);
                AdminPage2.SetActive(true);
                Next.interactable = true;
            }
            else if (AdminPage2.activeSelf)
            {
                AdminPage2.SetActive(false);
                AdminPage1.SetActive(true);
                Previous.interactable = false;
            }
        }
        else
        {
            if (UserPage3.activeSelf)
            {
                UserPage3.SetActive(false);
                UserPage2.SetActive(true);
                Next.interactable = true;
            }
            else if (UserPage2.activeSelf)
            {
                UserPage2.SetActive(false);
                UserPage1.SetActive(true);
                Previous.interactable = false;
            }
        }
    }

    private void NextInfo()
    {
        PlaySound.PlayClick();
        if (StartLevelSelect.isAdmin)
        {
            if (AdminPage1.activeSelf)
            {
                AdminPage1.SetActive(false);
                AdminPage2.SetActive(true);
                Previous.interactable = true;
            }
            else if (AdminPage2.activeSelf)
            {
                AdminPage2.SetActive(false);
                AdminPage3.SetActive(true);
                Next.interactable = false;
            }
        }
        else
        {
            if (UserPage1.activeSelf)
            {
                UserPage1.SetActive(false);
                UserPage2.SetActive(true);
                Previous.interactable = true;
            }
            else if (UserPage2.activeSelf)
            {
                UserPage2.SetActive(false);
                UserPage3.SetActive(true);
                Next.interactable = false;
            }
        }
    }

    private void ExitTutorial()
    {
        PlaySound.PlayClick();
        Level1.interactable = true;
        Level2.interactable = true;
        Level3.interactable = true;
        //TutorialWindow.SetActive(false);
        Tutorial.interactable = true;
        TweenTutorial.OpenCloseObjectAnimation();
    }
}
