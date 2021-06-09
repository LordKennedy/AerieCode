using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisifier : MonoBehaviour {

    [SerializeField] private GameObject ObjectToHide;
    bool isOn = true;

    public void SetOn(bool cond) {
        if (cond == isOn) return;
        isOn = cond;
        ObjectToHide.GetComponent<SkinnedMeshRenderer>().enabled = cond;
    }
}
