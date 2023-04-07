using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextLevel : MonoBehaviour {

    [Header("ETC")]
    [SerializeField] private OnLoadLevel loadLevel;
    [SerializeField] private SoundControl PlaySound;
    [SerializeField] private Animator anim;
    [SerializeField] private Image Black;

    [Header("Dialog")]
    [SerializeField] private GameObject FinishDialog;
    //[SerializeField] Text RespondTxt;
    //[SerializeField] GameObject RespondDialog;

    [Header("Tween")]
    //[SerializeField] private EasyTween TweenRespond;
    [SerializeField] private EasyTween TweenSuccess;

    //private Fading fade;

    public void Start()
    {
        //fade = GameObject.Find("UserControl").GetComponent<Fading>();
    }

    public void LoadLevel()
    {
        PlaySound.PlayClick();
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        TweenSuccess.OpenCloseObjectAnimation();
        yield return new WaitForEndOfFrame();
        //TweenRespond.OpenCloseObjectAnimation();
        //FinishDialog.SetActive(false);
        //RespondDialog.SetActive(true);
        //RespondTxt.text = "Loading...";
        //yield return new WaitForSeconds(2);

        //TweenRespond.OpenCloseObjectAnimation();

        /*var fadetime = fade.BeginFade(1);
        yield return new WaitForSeconds(fadetime);*/

        Black.enabled = true;
        anim.Play("FadeOut");
        yield return new WaitUntil(() => Black.color.a == 1);

        if (loadLevel.Level != 3) SceneManager.LoadSceneAsync("Level " + (loadLevel.Level + 1));
        else SceneManager.LoadSceneAsync("Level Select");
    }
}
