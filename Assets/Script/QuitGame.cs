using UnityEngine;

public class QuitGame : MonoBehaviour
{
    void Start ()
	{
		string eval = "";
		eval += "function OnApplicationQuit()";
		eval += "{";
		eval += "   GetUnity().SendMessage('" + gameObject.name + "', 'OnApplicationQuit', '');";
		eval += "   return true;";
		eval += "}";
		eval += "window.onbeforeunload = OnApplicationQuit;";
		Application.ExternalEval(eval);
	}

	public void OnApplicationQuit()
	{
        Application.Quit();

    }
}
