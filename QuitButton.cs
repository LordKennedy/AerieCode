using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class QuitButton : MonoBehaviour {

	void Start () {
		Button btn = GetComponent<Button>();
		btn.onClick.AddListener(Quit);
	}

	void Quit(){
        Application.Quit();
	}
}