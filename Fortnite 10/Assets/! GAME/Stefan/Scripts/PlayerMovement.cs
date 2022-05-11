using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement player;

    [Header("References")]
    public Transform cam;
    public Transform targetCam, rotationCam, weaponHolder;

    [Header("Movement")]
    public float walkSpeed;
    public float runSpeed;
    public float speedChangeRate;
    [Space(15)]
    public float movSmoothTime;
    public float jumpSpeed;
    Vector3 movement;
    Vector3 movVelocity;

    Vector3 targetCamPos;

    [Header("Rotation")]
    public float xSens;
    public float ySens;
    public float rotSmoothTime;
    float scrollDelta;

    public Vector2 clampAngles;
    float xRot;

    Vector2 rotation, rotVelocity;

    [Header("Camera")]
    public Vector2 camClamp;
    public float mouseScrollSpeed;
    public float camSpeed;
    public float camRotSpeed;

    [Header("Other")]

    public LayerMask groundMask;

    bool aiming { get { return Input.GetButton("Fire2"); } }
    public float detectRange;
    bool isGrounded { get { return Physics.Raycast(transform.position, Vector3.down, detectRange,groundMask); } }

    bool sprinting { get { return Input.GetButton("Sprint"); } }


    Vector3 defaultCamPos, velocity;

    Rigidbody rb;

    float _speedVel;
    float currentSpeed;
    public float gravity;

    private void Awake()
    {
        if(player == null)
        {
            player = this;
        }
    }
    void Start()
    {
        defaultCamPos = cam.position;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Movementspeed declaren
        currentSpeed = Mathf.SmoothDamp(currentSpeed, sprinting ? runSpeed : walkSpeed, ref _speedVel, speedChangeRate * Time.deltaTime);

        //Movement
        Vector3 targetMov = new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical"));

        movement = Vector3.SmoothDamp(movement,targetMov,ref movVelocity,movSmoothTime * Time.deltaTime);

        if (movement.magnitude > 1) movement.Normalize();

        transform.Translate(currentSpeed * Time.deltaTime * movement);

        //Rotation
        Vector2 targetRot = new Vector2(Input.GetAxis("Mouse Y") * xSens * Time.deltaTime,Input.GetAxis("Mouse X") * ySens * Time.deltaTime);
        xRot -= targetRot.x;
        xRot = Mathf.Clamp(xRot, clampAngles.x, clampAngles.y);
        rotation = Vector2.SmoothDamp(rotation, targetRot, ref rotVelocity, rotSmoothTime * Time.deltaTime);

        targetCam.transform.localEulerAngles = new Vector3(xRot,0,0);
        weaponHolder.transform.localEulerAngles = new Vector3(xRot, 0, 0);
        transform.Rotate(new Vector3(0, rotation.y));


        //Cam Settings

        if (Input.GetButton("Left Ctrl"))
        {
            var a = rotationCam.transform.forward * mouseScrollSpeed * Time.deltaTime * Input.mouseScrollDelta.y;
            var b = a + targetCam.transform.localPosition;
            if (b.y > camClamp.x && b.y < camClamp.y)
                targetCam.transform.position += a;
        }

        cam.transform.position = Vector3.SmoothDamp(cam.transform.position,targetCam.transform.position,ref velocity, camSpeed * Time.deltaTime);
        cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation,targetCam.transform.rotation,camRotSpeed);
        if (isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                rb.AddForce(Vector3.up * jumpSpeed, ForceMode.VelocityChange);
            }
        }
    }
}
