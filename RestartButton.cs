using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour {

	void Start () {
		Button btn = GetComponent<Button>();
		btn.onClick.AddListener(Restart);
	}

	void Restart(){
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
	}
}