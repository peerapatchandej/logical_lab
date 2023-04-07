using UnityEngine;
using Commands;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class ErrorControl : MonoBehaviour
{
    [SerializeField] private List<StructCommand> ListCommand;
    [SerializeField] private GameObject ErrorMessage;
    [SerializeField] private Text Message;
    [SerializeField] private Button Submit;
    [SerializeField] private MenuButton menuButton;
    [SerializeField] private Button Play;
    [SerializeField] private Compiler compile;

    [Header("Tween")]
    [SerializeField] private EasyTween TweenError;

    private SoundControl PlaySound;

    private void Start()
    {
        PlaySound = GameObject.Find("SoundSFX").GetComponent<SoundControl>();
        Submit.onClick.AddListener(CloseDialog);
    }

    public IEnumerator ErrorDialog(GameObject ErrorCmd)
    {
        yield return new WaitForSeconds(1f);
        //TweenDialog[0].OpenCloseObjectAnimation();
        PlaySound.PlayError();
        
        Play.interactable = false;

        var Struct = ErrorCmd.GetComponent<StructCommand>();
        string linenum = "";

        if (Struct.Id == CommandsId.ElseIf)
        {
            linenum = ErrorCmd.transform.GetChild(1).GetChild(4).GetComponent<Text>().text;
        }
        else if (Struct.Id == CommandsId.Break || Struct.Id == CommandsId.Else)
        {
            linenum = ErrorCmd.transform.GetChild(1).GetChild(0).GetComponent<Text>().text;
        }

        Message.text = "Command line " + linenum + " error, please check and compile the command again.";
        //ErrorMessage.SetActive(true);
        TweenError.OpenCloseObjectAnimation();
    }

    private void CloseDialog()
    {
        if (compile.ErrorCompile)
        {
            ListCommand = new List<StructCommand>();

            menuButton.isStop = true;
            menuButton.isClick = false;
            menuButton.isCompile = false;
            menuButton.OnOffParameter(true);
            menuButton.MenuOnAll();
            compile.ErrorCompile = false;

            //ErrorMessage.SetActive(false);
            TweenError.OpenCloseObjectAnimation();
        }
    }
}
