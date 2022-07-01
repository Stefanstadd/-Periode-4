using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement player;

    [Header("References")]
    public Transform camHolder;
    public Transform cam, targetCam,setTargetCam, rotationCam, weaponHolder;
    public InventoryManager inventoryManager;

    [Header("HP")]
    public static float currentHP;
    public float maxHP;

    public Animator deathScreen;
    public Slider healthSlider;

    public static bool Dead { get; private set; }

    [Header("Movement")]
    public float walkSpeed;
    public float runSpeed;
    public float speedChangeRate;
    [Space(15)]
    public float movSmoothTime;
    public float jumpSpeed;

    [Header("Rotation")]
    public float xSens;
    public float ySens;
    public float rotSmoothTime;

    public Vector2 clampAngles;
    float xRot;

    Vector2 rotation, rotVelocity;

    [Header("Camera")]
    public Vector2 camClamp;
    public float mouseScrollSpeed;
    public float camSpeed;
    public float camRotSpeed;
    public float dstToPlayer;

    [Header("Aiming")]
    public Transform aimingCamPos;

    Vector3 camAimVelocity;
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
    Animator animator;

    public bool Aiming 
    { 
        get 
        {
            return Input.GetButton("Fire2");
        }
    }
    public bool IsGrounded 
    { 
        get 
        { 
            return Physics.Raycast(transform.position, Vector3.down, detectRange, groundMask);
        } 
    }

    public bool Sprinting 
    { 
        get 
        { 
            return Input.GetButton("Sprint") && !Aiming; 
        } 
    }

    public bool Moving 
    { 
        get 
        { 
            if (Input.GetButton("Horizontal") || Input.GetButton("Vertical")) return true; 
            return false;
        } 
    }

    public bool BackwardsMovement
    {
        get
        {
            return Input.GetAxis("Vertical") < 0;
        }
    }

    private void Awake()
    {
        if (player == null)
        {
            player = this;
        }
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        defaultFOV = Camera.main.fieldOfView;
        inventory = GetComponent<PlayerInventory>();
        currentSpeed = walkSpeed;
        animator = GetComponent<Animator>();

        currentHP = maxHP;
    }

    void Update()
    {
        if (Dead)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            return;
        }
        if (GameMenuManager.Paused) return;

        if(!inventoryManager.IsInInventory()) MouseAndRotation();

        UpdateHealthBar();
        SetTargetCam();
        CamSettings();
        ManageAnimations();
    }

    private void FixedUpdate()
    {
        if (!IsGrounded)
        {
            rb.AddForce(Vector3.down * gravity);
        }

        currentSpeed = Mathf.SmoothDamp(currentSpeed, Sprinting ? runSpeed : walkSpeed, ref _speedVel, speedChangeRate);

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Move((transform.forward * v + transform.right * h).normalized * currentSpeed);

        if (OnSlope())
        {
            //rb.AddForce(Vector3.down * 10, ForceMode.Impulse);
        }

    }

    void ManageAnimations()
    {
        //Run and walk animations
        // index 0 = idle
        // index 1 = walking
        // index 2 = running

        string sprintName = "MoveState";
        string speedName = "Speed";
        string aimName = "Aiming";

        animator.SetBool(aimName, Aiming);

        if (!Moving)
        {
            animator.SetInteger(sprintName, 0);
            return;
        }

        animator.SetInteger(sprintName, Sprinting? 2 : 1);


        //Reversing

        animator.SetFloat(speedName, BackwardsMovement? -1 : 1);
    }

    void UpdateHealthBar()
    {
        healthSlider.maxValue = maxHP;
        healthSlider.value = currentHP;
    } 

    void SetTargetCam()
    {
        if (Physics.Raycast(setTargetCam.position, setTargetCam.forward, out RaycastHit hit, dstToPlayer))
        {
            targetCam.transform.position = hit.point;
        }
    }

    void MouseAndRotation()
    { 
        //Rotation
          float aimsens = Aiming ? aimSensMultiplier : 1;
         Vector2 targetRot = new(Input.GetAxis("Mouse Y") * xSens * Time.deltaTime * aimsens, Input.GetAxis("Mouse X") * ySens * Time.deltaTime * aimsens);

        xRot -= targetRot.x;
         xRot = Mathf.Clamp(xRot, clampAngles.x, clampAngles.y);

         rotation = Vector2.SmoothDamp(rotation, targetRot, ref rotVelocity, rotSmoothTime);

         targetCam.transform.localEulerAngles = new Vector3(xRot, 0, 0);

         if (!inventory.NoWeaponSelected)
          weaponHolder.GetChild(inventory.selectedWeapon - 1).transform.localEulerAngles = new Vector3(xRot + recoilRot,0 , 0);

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
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, aimingCamPos.transform.position, ref camAimVelocity, camSpeed);
        }
        else
        {
            cam.transform.localPosition = Vector3.SmoothDamp(cam.transform.localPosition, Vector3.zero, ref camAimVelocity, camSpeed);
        }


        camHolder.transform.position = Vector3.SmoothDamp(camHolder.transform.position, targetCam.transform.position, ref velocity, camSpeed);
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

    public void TakeDamage(float dmg)
    {
        print($"took {dmg} damage");
        currentHP -= dmg;
        if(currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        deathScreen.gameObject.SetActive(true);
        deathScreen.SetTrigger("Die");
        Dead = true;

        rb.freezeRotation = false;

        rb.AddForce(Vector3.up * Random.Range(1, 5), ForceMode.Impulse);
        rb.AddExplosionForce(Random.Range(1, 5), Random.insideUnitSphere, 3);
        rb.AddTorque(Random.insideUnitSphere, ForceMode.Impulse);

        animator.SetInteger("MoveState", 0);
        animator.SetBool("Aiming", false);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
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
