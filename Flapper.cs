using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flapper : MonoBehaviour {
    public void Flap() {
        GetComponent<AudioSource>().Play();
    }
}
