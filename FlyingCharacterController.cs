using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingCharacterController : MonoBehaviour {

    // Use the ordinal values to dictate ordAnimationState
    private enum FlightMode {Grounded = 0, Flying = 1, Landing = 2, TakingOff = 3};
    private FlightMode CurrentFlightMode = FlightMode.Grounded;

    private bool isWalking = false; // Used alongside FlightMode.Grounded to indicate walking/idle

    private Animator anim;
    private void SetAnimationState(FlightMode fmode) {
        SetWalking(false);
        CurrentFlightMode = fmode;
        if (fmode == FlightMode.Flying) {
            anim.SetTrigger("ToFly");
        }
        else if (fmode == FlightMode.Landing) {
            MainCamera.GetComponent<maxCamera>().UnblockFPS();
            anim.SetTrigger("ToLand");

        }
        else if (fmode == FlightMode.TakingOff) {
            MainCamera.GetComponent<maxCamera>().BlockFPS();
            anim.SetTrigger("ToTakeoff");
        }
    }
    private void SetWalking(bool cond) {
        isWalking = cond;
        anim.SetBool("IsWalking", cond);
    }

    private float UDRotationSpeed = 2.5f;
    private float LRRotationSpeed = 2.5f;
    private float leanSpeed = 75f;
    private float maxLeanAngle = 80f;

    [SerializeField] private Transform NestPosition;
    [SerializeField] private Vector3 NestOffset;

    private float landingDistance = 8f;
    private float maxMoveSpeed = 0.35f;
    private float minMoveSpeed = 0.05f;
    private float moveSpeed = 3f;
    private float accSpeed = 0.5f;

    private float gravity = -1f;

    private float currentSpeed = 0f;
    private CharacterController cc;
    [SerializeField] private GameObject Body;
    [SerializeField] private AnimationClip landingAnim;

    private Vector3 landingPoint;
    private Camera MainCamera;

    // We are switching over from a standard Transform.Rotate() model to a xDeg yDeg model
    // This makes the camera more reliable and easier to reset
    private float CharXDeg = 0f;
    private float CharYDeg = 0f;

    public (float xDeg, float yDeg) GetDegs() {
        return (CharXDeg, CharYDeg);
    }

    public void IncDegs(float xDeg, float yDeg) {
        
    }

    void Start() {
        cc = GetComponent<CharacterController>();
        currentSpeed = maxMoveSpeed - minMoveSpeed; // Just gives us some significant starting velocity
        anim = transform.GetChild(0).GetComponent<Animator>();
        SetAnimationState(CurrentFlightMode);
        MainCamera = Camera.main;
    }

    private bool frozen = true;
    public void Freeze() {
        frozen = true;
        if (CurrentFlightMode != FlightMode.Grounded && !isWalking) {
            anim.speed = 0f;
        }
    }
    public void Unfreeze() {
        frozen = false;
        anim.speed = 1f;
    }

    void FixedUpdate() {
        if (frozen) return;
        float dt = Time.deltaTime;
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        float i = Input.GetAxis("Inline"); // Shift/Space
        int ignoreLayer = 1 << 7;
        ignoreLayer = ~ignoreLayer;

        RaycastHit hit;
        bool gotHitFromRayCast = Physics.Raycast(transform.position, -(Vector3.up), out hit, landingDistance, ignoreLayer);

        if (CurrentFlightMode == FlightMode.Flying){
            currentSpeed += i * dt * accSpeed;
            currentSpeed = Mathf.Clamp(currentSpeed, minMoveSpeed, maxMoveSpeed);
            float dz = currentSpeed;

            // Check if our current speed is MinSpeed
            // If it is, we should be flapping our wings and descending
            float dy = currentSpeed == minMoveSpeed ? -minMoveSpeed : 0f;

            // See if we need to start landing in the nest
            if (currentSpeed == minMoveSpeed && (transform.position - NestPosition.position).magnitude <= landingDistance/1.5) {
                // Land in the nest
                CurrentFlightMode = FlightMode.Landing;
                SetAnimationState(FlightMode.Landing);
                landingPoint = NestPosition.position + NestOffset;
            }
            // If we are holding brake and moving at min speed and LandingDistance away from the ground, land
            if (currentSpeed == minMoveSpeed && gotHitFromRayCast) {
                CurrentFlightMode = FlightMode.Landing;
                SetAnimationState(FlightMode.Landing);
                landingPoint = hit.point;
            }

             // Actually move our bird friend
            cc.Move(transform.TransformDirection(new Vector3(0f, dy, dz)));
        
            // First, rotate in the X dimension in self space
            // transform.Rotate(v * UDRotationSpeed, 0f, 0f);
            CharXDeg += v * UDRotationSpeed;
            //Clamp the vertical axis for the body
            CharXDeg = ClampAngle(CharXDeg, -70f, 70f);

            // Debug.Log(transform.rotation.eulerAngles);
            // Then, rotate in the Y dimension in world space
            // transform.Rotate(0f, h * LRRotationSpeed, 0f, Space.World);
            CharYDeg += h * LRRotationSpeed;

            // Then set out transform to this new degree. I guess?
            Quaternion desiredRotation = Quaternion.Euler(CharXDeg, CharYDeg, 0);
            Quaternion currentRotation = transform.rotation;

            transform.rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * 7.5f);

            // Then, rotate our BODY in the z dimension proportionate to the horizontal amount
            // to make some good lean
            // lean should only exist when pressing left or right
            Quaternion target = Quaternion.Euler(0f, 0f, -h * maxLeanAngle);
            Body.transform.localRotation = Quaternion.RotateTowards(Body.transform.localRotation, target, dt * leanSpeed);

        }
        else if (CurrentFlightMode == FlightMode.Landing) {
            // Want to land at landedProg
            float ratio;
            ratio = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Bird_Flight_Landing_Anim")) ratio = 0f;
            // Now we have our ratio to know how close to hitting the ground we are
            // We don't need to interpolate velocity at all, because we can we just interpolate our transform
            // directly
            if (currentSpeed > 0) currentSpeed -= accSpeed * Time.deltaTime;
            // Now we just interpolate to our bottom
            // Note we are trying to get to hit
            Vector3 newPos = Vector3.Lerp(transform.position, landingPoint, ratio);
            transform.position = newPos;

            // If idling, we want to switch to the idle state
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Bird_Walking_Idle1_Anim")){
                CurrentFlightMode = FlightMode.Grounded;
                MainCamera.GetComponent<maxCamera>().SetKeepFreeCam(true);
            }
            // Now when landing we want to align self with camera
            Vector3 dir = MainCamera.transform.forward;
            dir.y = 0f;
            transform.rotation = Quaternion.LookRotation(dir);

            // Make sure the body is aligned on the Z axis
            Body.transform.localRotation = Quaternion.RotateTowards(Body.transform.localRotation, Quaternion.Euler(0f, 0f, 0f), dt * leanSpeed);
        }

        else if (CurrentFlightMode == FlightMode.Grounded) {
            if (v != 0f || h != 0f) {
                SetWalking(true);
                // Want to move relative to the camera, with gravity as the y component
                Vector3 totalMoveDir =
                    (MainCamera.transform.forward * v * moveSpeed * Time.deltaTime)
                    + (MainCamera.transform.right * h * moveSpeed * Time.deltaTime);
                totalMoveDir.y = gravity;
                cc.Move(totalMoveDir);
                // Now we want to point our body in the direction of the movement dir
                totalMoveDir.y = 0f;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(totalMoveDir), Time.deltaTime * 10f);
            } else SetWalking(false);
            Debug.Log(currentSpeed);
            // Check for falling off ledge
            /*
            if (!cc.isGrounded && gotHitFromRayCast && currentSpeed <= minMoveSpeed) {
                // Start landing
                SetWalking(false);
                CurrentFlightMode = FlightMode.Landing;
                SetAnimationState(FlightMode.Landing);
                landingPoint = hit.point;
            } else if (!cc.isGrounded && !gotHitFromRayCast && currentSpeed <= minMoveSpeed) {
                // Start flying
                SetWalking(false);
                CurrentFlightMode = FlightMode.Flying;
                SetAnimationState(FlightMode.Flying);
            }
            */

            if (i == 1f) {
                SetAnimationState(FlightMode.TakingOff);
                MainCamera.GetComponent<maxCamera>().SetKeepFreeCam(false);
                currentSpeed = minMoveSpeed * 4;
                CharYDeg = transform.rotation.eulerAngles.y;
                return;
            }
        }

        else if (CurrentFlightMode == FlightMode.TakingOff) {
            // Want to actually travel UP!
            cc.Move((transform.forward + Vector3.up) * minMoveSpeed/2);
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("FlightWithSounds")){
                SetAnimationState(FlightMode.Flying);
                CurrentFlightMode = FlightMode.Flying;
            }
        }
    }

    private static float ClampAngle(float angle, float min, float max) {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}

