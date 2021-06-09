using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Collectable represents the actual items we can pick up
// This will be attached to flowers, twigs, etc.

public class Collectable : MonoBehaviour {
    // This is the name of the item as it will appear in the pickup message
    // and highlight display
    [SerializeField] public string CollectableName;

    // This is what the placed collectable will look like if you do not break it down
    // [SerializeField] public GameObject StandardModel;
    // Not necessary because it is attached to the gameobject

    // This is what the placed collectable will look like if you DO break if down
    // (Not used for now)
    // [SerializeField] public GameObject BrokenModel;

    // An image for the HUD
    [SerializeField] public Sprite HUDSprite;

    // And placement sound
    [SerializeField] public AudioClip PlacementSound;

    // and pickup sound
    [SerializeField] public AudioClip PickupSound;

    [SerializeField] public Material HoloMaterial;
    private bool isHolo = false;
    private bool canSwitch = true;
    private Material OriginalMaterial;

    void Start() {
        OriginalMaterial = GetComponent<MeshRenderer>().materials[0];
        if (HoloMaterial == null) canSwitch = false;
    }

    public void SwitchToHologramMaterial() {
        if (!canSwitch) return;
        if (isHolo) return;

        var mats = GetComponent<MeshRenderer>().materials;
        mats[0] = HoloMaterial;
        GetComponent<MeshRenderer>().materials = mats;

        GetComponent<Collider>().enabled = false;
        isHolo = true;
    }

    public void SwitchToStandardMaterial() {
        if (!canSwitch) return;
        if (!isHolo) return;

        var mats = GetComponent<MeshRenderer>().materials;
        mats[0] = OriginalMaterial;
        GetComponent<MeshRenderer>().materials = mats;

        GetComponent<Collider>().enabled = true;
        isHolo = false;
    }
}