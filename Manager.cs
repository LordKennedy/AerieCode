using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject Player;

    void Start() {
        PauseMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetAxis("Cancel") != 0f) {
            // Show the menu
            PauseMenu.SetActive(true);
            // Freeze Player
            Player.GetComponent<FlyingCharacterController>().Freeze();
            // Freeze Camera
            Camera.main.GetComponent<maxCamera>().Freeze();
            // Unlock the cursor
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
