using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GetSession : MonoBehaviour
{
    [SerializeField] private Button startBtn;
    public bool isAdmin = false;

    void Awake()
    {
        StartCoroutine(SendDataToWeb());
    }

    IEnumerator SendDataToWeb()
    {
        WWW www = new WWW("https://cooplogicallab.000webhostapp.com/GetSession.php");
        yield return www;

        if (www.text != "")
        {
            if (www.text == "Have Session.")
            {
                isAdmin = true;
            }
            else if (www.text == "No have session.")
            {
                isAdmin = false;
            }
        }
        else
        {
            Application.ExternalEval("gotoIndex()");
        }
    }
}
