using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    //[SerializeField] private GameObject RespondDialog;
    [SerializeField] private Text RespondTxt;
    [SerializeField] private Button AnotherLevel1;
    [SerializeField] private Button AnotherLevel2;
    [SerializeField] private Button Tutorial;
    [SerializeField] private Animator anim;
    [SerializeField] private Image Black;
    [SerializeField] private SoundControl PlaySound;

    [Header("Tween")]
    [SerializeField] private EasyTween TweenDialog;

    //[SerializeField] private Fading fade;

    void Start ()
    {
        GetComponent<Button>().onClick.AddListener(LoadLevel);
	}

    private void LoadLevel()
    {
        PlaySound.PlayClick();
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        GetComponent<Button>().interactable = false;
        AnotherLevel1.interactable = false;
        AnotherLevel2.interactable = false;
        Tutorial.interactable = false;

        //TweenDialog.OpenCloseObjectAnimation();

        //RespondDialog.SetActive(true);
        //RespondTxt.text = "Loading...";

        /*yield return new WaitForSeconds(2);
        TweenDialog.OpenCloseObjectAnimation();*/

        /*var fadetime = fade.BeginFade(1);
        yield return new WaitForSeconds(fadetime);*/

        Black.enabled = true;
        anim.Play("FadeOut");
        yield return new WaitUntil(() => Black.color.a == 1);

        SceneManager.LoadSceneAsync(gameObject.name);
    }
}
