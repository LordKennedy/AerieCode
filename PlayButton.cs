using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayButton : MonoBehaviour {

    [SerializeField] private GameObject player;

	void Start () {
		Button btn = GetComponent<Button>();
		btn.onClick.AddListener(StartGame);
	}

	void StartGame(){
		// First, hide the menu
        GameObject.Find("PauseMenu").SetActive(false);

        // Unlock/unfreeze player motion
        player.GetComponent<FlyingCharacterController>().Unfreeze();

        // Unlock/unfreeze camera rotation
        Camera.main.GetComponent<maxCamera>().Unfreeze();

        // Lock the cursor
        Cursor.lockState = CursorLockMode.Locked;
	}
}