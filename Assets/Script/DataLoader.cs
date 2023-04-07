using System.Collections;
using UnityEngine;

public class DataLoader : MonoBehaviour
{
    private string[] Users;

	IEnumerator Start () {
        WWW UserData = new WWW("http://localhost/LogicalLab/Login.php");
        yield return UserData;
        string UserDataString = UserData.text;
        Debug.Log(UserDataString);
        Users = UserDataString.Split(';');
        Debug.Log(GetDataValue(Users[0], "ID : "));
	}

    string GetDataValue(string data,string index)
    {
        string value = data.Substring(data.IndexOf(index)+index.Length);
        if(value.Contains("|")) value = value.Remove(value.IndexOf("|"));
        return value;
    }
}
