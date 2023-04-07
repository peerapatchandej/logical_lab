using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Logout : MonoBehaviour
{
    //private string Token;
    [SerializeField] private MenuButton menuButton;

    void Start ()
    {
        //Token = GameObject.Find("UserControl").GetComponent<Login>().Token;
    }

    public void LogoutUser()
    {
        StartCoroutine(SendDataToWeb());
    }

    IEnumerator SendDataToWeb()
    {
        //WWWForm form = new WWWForm();
        //form.AddField("tokenId", Token);

        WWW www = new WWW("https://cooplogicallab.000webhostapp.com/Logout.php");
        yield return www;

        menuButton.MenuOnAll();
        menuButton.isClick = false;

        SceneManager.LoadSceneAsync("Home");
    }
}
