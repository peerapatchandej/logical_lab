using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OnLoadLevel : MonoBehaviour
{
    [SerializeField] private GameObject MenuList;
    [SerializeField] private GameObject OpenBtn;
    public int Level = 1;

    /*void Awake ()
    {
        if (GameObject.Find("SessionControl") == null) SceneManager.LoadSceneAsync("Level Select");
        else
        {
            if (GameObject.Find("SessionControl").GetComponent<GetSession>().isAdmin)
            {
                //OpenBtn.interactable = true;
                MenuList.GetComponent<RectTransform>().sizeDelta = new Vector2(310f, 53f);
                OpenBtn.SetActive(true);
            }
            else
            {
                //OpenBtn.interactable = false;
                MenuList.GetComponent<RectTransform>().sizeDelta = new Vector2(243f, 53f);
                OpenBtn.SetActive(false);
            }
        }
    }*/
}
