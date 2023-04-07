using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Home : MonoBehaviour {

    public Button StartGameBtn;

	void Start () {
        StartGameBtn.onClick.AddListener(StartGame);
	}
	
	void StartGame()
    {
        SceneManager.LoadScene("Scene 1");
    }
}
