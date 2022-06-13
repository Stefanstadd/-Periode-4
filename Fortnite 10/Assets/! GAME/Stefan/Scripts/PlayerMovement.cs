using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement player;

    [Header("References")]
    public Transform camHolder;
    public Transform cam, targetCam, rotationCam, weaponHolder;
    public InventoryManager inventoryManager;

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

    [Header("Aiming")]
    public Transform aimingCamPos;

    Vector3 defaultCamPos, camAimVelocity;
    public float aimSensMultiplier;

    [Header("Other")]
    public float defaultFOV;
    public float maxRBSpeed;
    public LayerMask groundMask;
    Vector3 velocity;

    public float gravity;
    public float detectRange;
    public float recoilRot;

    float _speedVel;
    float currentSpeed;

    PlayerInventory inventory;
    Rigidbody rb;

    public bool Aiming { get { return Input.GetButton("Fire2"); } }
    bool IsGrounded { get { return Physics.Raycast(transform.position, Vector3.down, detectRange, groundMask); } }

    bool Sprinting { get { return Input.GetButton("Sprint") && !Aiming; } }

    private void Awake()
    {
        if (player == null)
        {
            player = this;
        }
    }
    void Start()
    {
        defaultCamPos = cam.localPosition;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        defaultFOV = Camera.main.fieldOfView;
        inventory = GetComponent<PlayerInventory>();
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        if(!inventoryManager.IsInInventory()) MouseAndRotation();

        CamSettings();
    }

    private void FixedUpdate()
    {
        if (!IsGrounded)
        {
            rb.AddForce(Vector3.down * gravity);
        }

        currentSpeed = Mathf.SmoothDamp(currentSpeed, Sprinting ? runSpeed : walkSpeed, ref _speedVel, speedChangeRate * Time.fixedDeltaTime);

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Move((transform.forward * v + transform.right * h).normalized * currentSpeed);

        if (OnSlope())
        {
            print("a");
            //rb.AddForce(Vector3.down * 10, ForceMode.Impulse);
        }

    }

    void MouseAndRotation()
    { 
        //Rotation
          float aimsens = Aiming ? aimSensMultiplier : 1;
         Vector2 targetRot = new Vector2(Input.GetAxis("Mouse Y") * xSens * Time.deltaTime * aimsens, Input.GetAxis("Mouse X") * ySens * Time.deltaTime * aimsens);

        xRot -= targetRot.x;
         xRot = Mathf.Clamp(xRot, clampAngles.x, clampAngles.y);

         rotation = Vector2.SmoothDamp(rotation, targetRot, ref rotVelocity, rotSmoothTime * Time.deltaTime);

         targetCam.transform.localEulerAngles = new Vector3(xRot, 0, 0);

         if (!inventory.NoWeaponSelected)
          weaponHolder.GetChild(inventory.selectedWeapon - 1).transform.localEulerAngles = new Vector3(xRot + recoilRot, 0, 0);

        transform.Rotate(new Vector3(0, rotation.y));
    }

    void CamSettings()
    {
        if (Input.GetButton("Left Ctrl") && !Aiming && !inventoryManager.IsInInventory())
        {
            var a = rotationCam.transform.forward * mouseScrollSpeed * Time.deltaTime * Input.mouseScrollDelta.y;
            var b = a + targetCam.transform.localPosition;
            if (b.y > camClamp.x && b.y < camClamp.y)
                targetCam.transform.position += a;
        }

        if (Aiming && !inventory.NoWeaponSelected && !inventoryManager.IsInInventory())
        {
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, aimingCamPos.transform.position, ref camAimVelocity, camSpeed * Time.deltaTime);
        }
        else
        {
            cam.transform.localPosition = Vector3.SmoothDamp(cam.transform.localPosition, Vector3.zero, ref camAimVelocity, camSpeed * Time.deltaTime);
        }


        camHolder.transform.position = Vector3.SmoothDamp(camHolder.transform.position, targetCam.transform.position, ref velocity, camSpeed * Time.deltaTime);
        cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, targetCam.transform.rotation, camRotSpeed);
    }

    private void Move(Vector3 speed)
    {
        float flatMagnitude = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;

        if (flatMagnitude < maxRBSpeed)
            rb.AddForce(speed, ForceMode.Impulse);

    }

    bool OnSlope()
    {
        if (IsGrounded)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down,out hit))
            {
                if (hit.normal != Vector3.up)
                {
                    return true;
                }
            }
        }
        return false;
    }

    //void Movement()
    //{
    //    //Movementspeed declaren
    //    

    //    //Movement
    //    Vector3 targetMov = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

    //    movement = Vector3.SmoothDamp(movement, targetMov, ref movVelocity, movSmoothTime * Time.deltaTime);

    //    if (movement.magnitude > 1) movement.Normalize();

    //    rb.AddForce(currentSpeed * movement, ForceMode.);
    //}
}
