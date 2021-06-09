// original: http://www.unifycommunity.com/wiki/index.php?title=MouseOrbitZoom
using UnityEngine;
using System.Collections;

[AddComponentMenu("Camera-Control/3dsMax Camera Style")]
public class maxCamera : MonoBehaviour {
    public Transform target;
    public Vector3 targetOffset;
    public Vector3 fpsOffset;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public int zoomRate = 40;
    public float panSpeed = 0.3f;
    public float zoomDampening = 5.0f;

    public GameObject reticle;
 
    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;

    private enum CameraMode {Follow, Free, Unlocked, FPS};
    private CameraMode CurrentCameraMode = CameraMode.Free;

    public bool isFPS() {
        return CurrentCameraMode == CameraMode.FPS;
    }

    private float TimeSinceLastInput = 0f;
    public float TimeBeforeFollowCam = 1f;

    private Invisifier invis;
    private FlyingCharacterController fcc;

    void Start() { Init(); }
    void OnEnable() { Init(); }

    private bool KeepFreeCam = false;
    public void SetKeepFreeCam(bool cond) {
        KeepFreeCam = cond;
    }

    public void Init() {
        //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
        if (!target) {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);
            target = go.transform;
        }

        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        desiredDistance = distance;

        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;

        xDeg = Vector3.Angle(Vector3.right, transform.right );
        yDeg = Vector3.Angle(Vector3.up, transform.up );

        // Cursor.lockState = CursorLockMode.Locked;
        invis = target.GetComponent<Invisifier>();
        fcc = target.GetComponent<FlyingCharacterController>();
    }

    /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */

    float TimeToZoom = 0.5f;
    float CurrentZoomTime = 0f;
    bool Zooming = false;
    bool lockInput = false;

    private bool frozen = true;
    public void Freeze() {
        frozen = true;
    }
    public void Unfreeze() {
        frozen = false;
    }

    bool blockFPS = false;
    public void BlockFPS() {
        blockFPS = true;
        CurrentCameraMode = CameraMode.Free;
        reticle.SetActive(false);
        invis.SetOn(true);
        Zooming = false;
        CurrentZoomTime = 0f;
    }
    public void UnblockFPS() {
        blockFPS = false;
    }


    void Update() {
        if (frozen) return;
        if (Input.GetAxis("CameraSwitch") != 0){
            if (!lockInput){
                lockInput = true;
                if (CurrentCameraMode != CameraMode.FPS && !blockFPS) {
                    CurrentCameraMode = CameraMode.FPS;
                    reticle.SetActive(true);
                    Zooming = true;
                } else {
                    CurrentCameraMode = CameraMode.Free;
                    reticle.SetActive(false);
                    invis.SetOn(true);
                    Zooming = false;
                }
                CurrentZoomTime = 0f;
            }
        } else lockInput = false;

    // void LateUpdate() {
        
        // Special case for fps camera
        if (CurrentCameraMode == CameraMode.FPS) {
            if (transform.position != target.position + fpsOffset && Zooming) {
                // Interpolate to the target + fpsOffset
                CurrentZoomTime += Time.deltaTime;
                transform.position = Vector3.Lerp(transform.position, target.position + fpsOffset, CurrentZoomTime/TimeToZoom);
            } else {
                Zooming = false;
                invis.SetOn(false);
                // Now handle rotation
                // TODO: Make this controller compatable
                xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);

                desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
                transform.rotation = desiredRotation;
                // TODO: try to make the code much closer to the freecam controls
                // And adjust our position
                transform.position = target.position + fpsOffset;
            }
            return;
        }
        // other camera modes VVVV
        // First, poll to see if we need to switch to Freecam or Followcam
        UpdateCam();

        if (CurrentCameraMode == CameraMode.Free) {
            xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            //Clamp the vertical axis for the orbit
            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
            // set camera rotation 
            desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
            currentRotation = transform.rotation;

            rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
            transform.rotation = rotation;
        }
        if (CurrentCameraMode == CameraMode.Follow) {
            var degs = fcc.GetDegs();
            yDeg = degs.xDeg;
            xDeg = degs.yDeg;
            desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
            currentRotation = transform.rotation;

            rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
            // rotation = desiredRotation;
            transform.rotation = rotation;

            // Set x and y degrees so when the cam resumes it's centered correctly
            // TODO TODO TODO
            // yDeg = desiredRotation.eulerAngles.x;
            // yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
            // xDeg = desiredRotation.eulerAngles.y;
        }

        // affect the desired Zoom distance if we roll the scrollwheel
        desiredDistance -= Input.GetAxis("Zoom") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        //clamp the zoom min/max
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        // For smoothing of the zoom, lerp distance
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);
        // calculate position based on the new currentDistance 
        position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
        // transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 3f);
        // Now, we want to make it so the camera will not actually clip through colliders
        // Shoot a ray from the target pos to the expected pos, which is position
        RaycastHit hit;
        if (Physics.Raycast(target.position + Vector3.up, (position - target.position).normalized, out hit, (position - target.position).magnitude)) {
            transform.position = hit.point;
        }
        else transform.position = position;
    }

    private static float ClampAngle(float angle, float min, float max) {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    private void UpdateCam() {
        if (!KeepFreeCam && Input.GetAxis("Mouse X") == 0f && Input.GetAxis("Mouse Y") == 0f)
            TimeSinceLastInput += Time.deltaTime;
        else {
            TimeSinceLastInput = 0f;
            CurrentCameraMode = CameraMode.Free;
        }
        if (TimeSinceLastInput >= TimeBeforeFollowCam)
            CurrentCameraMode = CameraMode.Follow;
    }
}