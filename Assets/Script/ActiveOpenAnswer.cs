using UnityEngine;
using UnityEngine.UI;
using System;

public class ActiveOpenAnswer : MonoBehaviour {

    [SerializeField] private OpenAnswer openAnswer;
    //public event Action<int> OpenAnswerData = delegate { };

	// Use this for initialization
	void Start ()
    {
        GetComponent<Button>().onClick.AddListener(() => openAnswer.Open(gameObject));
    }
}
