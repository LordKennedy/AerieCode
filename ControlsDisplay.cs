using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsDisplay : MonoBehaviour {

    private bool showing = true;
    // Structure goes:
    // This (controlsDisplay)
    // |-- AdditionalText
    // |-- PersistantText
    private bool fired = false;
    void Update() {
        if (Input.GetAxis("Help") != 0f && !fired) {
            fired = true;
            if (showing) {
                showing = false;
                SetHelpDisplay(false);
            } else {
                showing = true;
                SetHelpDisplay(true);
            }
        } else if (Input.GetAxis("Help") == 0f) {
            fired = false;
        }
    }

    private void SetHelpDisplay(bool stat) {
        transform.GetChild(0).gameObject.SetActive(stat);
    }
}
