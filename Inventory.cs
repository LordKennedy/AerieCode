using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    // The items we have
    // private List<Collectable> CollectableItems = new List<Collectable>();
    private List<GameObject> InventoryList = new List<GameObject>();

    // [SerializeField] private int MaxInventorySize = 5;

    // What do we parent the photos to?
    [SerializeField] private Transform InventoryDisplay;
    [SerializeField] private GameObject TemplateHUDStub;
    // HUDStub Parent
    // |-- Background
    // |-- Image To Edit

    // The camera we can get information from
    private maxCamera MaxCameraInst;
    
    // Audio player for pickup fx
    private AudioSource player;

    void Start() {
        MaxCameraInst = Camera.main.GetComponent<maxCamera>();
        player = GetComponent<AudioSource>();
    }

    private void ShowInventoryImage(Sprite sprite) {
        // First, duplicate the template
        GameObject obj = Instantiate(TemplateHUDStub);
        // Now set the image
        obj.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = sprite; // ForegroundImage, GetChild(1) will be the BackgroundImage
        // And turn it on
        obj.SetActive(true);
        obj.transform.SetParent(InventoryDisplay);
    }

    // For now, delete the child at index 1
    private void DeleteInventoryImage() {
        if (InventoryDisplay.childCount >= 2)
            Destroy(InventoryDisplay.GetChild(InventoryDisplay.childCount - 1).gameObject);
    }


    [SerializeField] private float PickupDistance = 5f;
    private void TryPickup() {
        // First, we shoot a ray out of the center of the camera
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, PickupDistance)) {
            if (hit.collider.gameObject.GetComponent<Collectable>()) {
                Debug.Log("Got collectable " + hit.collider.gameObject.GetComponent<Collectable>().CollectableName);
                // Add the the inventory lst
                InventoryList.Add(hit.collider.gameObject);
                // Disable on pickup
                hit.collider.gameObject.SetActive(false);
                // Show the relevant image
                ShowInventoryImage(hit.collider.gameObject.GetComponent<Collectable>().HUDSprite);
                // Play the sound
                player.PlayOneShot(hit.collider.gameObject.GetComponent<Collectable>().PickupSound);
            }
        }
    }

    private float zOffset = 0f;

    private void PlaceNewestItem() {
        if (InventoryList.Count == 0) return;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, PickupDistance)) {
            GameObject obj = InventoryList[InventoryList.Count - 1];
            obj.transform.position = hit.point + Vector3.up * zOffset;;
            obj.GetComponent<Collectable>().SwitchToStandardMaterial();
            obj.SetActive(true);
            obj.GetComponent<Collider>().enabled = true;
            // Remove the entry from our array
            InventoryList.RemoveAt(InventoryList.Count - 1);
            // Remove the inventory image
            DeleteInventoryImage();
        }
    }

    // Display the hologram version of this item wherever the camera is pointing
    private void PreviewPlacement(float xRot, float yRot, float zTranslation) {
        if (InventoryList.Count == 0) return;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, PickupDistance)) {
            GameObject obj = InventoryList[InventoryList.Count - 1];
            obj.GetComponent<Collider>().enabled = false;
            // Apply offset
            zOffset += zTranslation;
            // Apply rotation relative to WORLD SPACE
            obj.transform.Rotate(xRot, yRot, 0f, Space.World);
            // obj.transform.Rotate(0f, yRot, 0f, Space.World);
            // transform.Rotate(0f, h * LRRotationSpeed, 0f, Space.World);

            obj.transform.position = hit.point + Vector3.up * zOffset;
            obj.GetComponent<Collectable>().SwitchToHologramMaterial();
            obj.SetActive(true);
        } else HidePreview();
    }

    private void HidePreview() {
        if (InventoryList.Count == 0) return;
        GameObject obj = InventoryList[InventoryList.Count - 1];
        obj.SetActive(false);
    }

    bool hasFired = false;
    bool keepOffset = false;
    void Update() {

        bool canAct = MaxCameraInst.isFPS();
        float f1 = Input.GetAxis("Fire1");
        float f2 = Input.GetAxis("Fire2");
        // See if we let go of f1
        if (f1 == 0) hasFired = false;
        if (f2 == 0) keepOffset = false;
        else keepOffset = true;

        // We also want to get the input from the scroll wheel and rotational keys
        // Which will be Tab/Q in the Y dim and E/R in the X dir
        float xRot = Input.GetAxis("RotateX") * Time.deltaTime * 50f;
        float yRot = Input.GetAxis("RotateY") * Time.deltaTime * 50f;
        // And the translational information from the scroll wheel
        float zTrn = Input.GetAxis("Zoom");

        // Pick up items by just left clicking
        if (canAct && !hasFired && f1 != 0f && f2 == 0f) {
            TryPickup();
            hasFired = true;
        } else if (canAct && f2 != 0f) {
            PreviewPlacement(xRot, yRot, zTrn);
        }  else if (canAct && f2 == 0) { // May need to remove canAct
            // Hide the preview
            HidePreview();
        }
        
        if (canAct && !hasFired && Input.GetAxis("Fire2") != 0f && Input.GetAxis("Fire1") != 0f) {
            // Place item if not fired
            hasFired = true;
            PlaceNewestItem();
        }

        if (!keepOffset) {
            zOffset = 0f;
            if (InventoryList.Count > 0) InventoryList[InventoryList.Count - 1].transform.rotation = Quaternion.identity;
        }
    }


}
