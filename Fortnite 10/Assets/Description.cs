using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Description : MonoBehaviour
{
    public Image fill;
    public TextMeshProUGUI objectName, description;
    public float interactTime = 0.5f;
    public float scaleTreshold = 0.1f;
    public float scaleSmoothTime;

    float interactTimer;

    Vector3 defaultScale { get; set; }

    Vector3 targetScale;
    Vector3 scaleVelocity;
    Vector3 targetPos;
    public Vector3 posOffset;

    public KeyCode interactKey;

    public delegate void OnInteract();
    public static OnInteract onInteract;

    bool active;
    PlayerMovement player;
    private void Start()
    {
        defaultScale = transform.localScale;
        targetScale = defaultScale;
        interactKey = KeyCode.E;
        Toggle(true);
        player = PlayerMovement.player;
    }
    private void Update()
    {
        if (PlayerMovement.Dead) 
        { 
            Disable();
            return;
        }

        SetSize();
        SetPos();
        if (!active) return;

        CheckInput();
    }

    void SetPos()
    {
        transform.position = Camera.main.WorldToScreenPoint(targetPos) + posOffset;
    }

    void CheckInput()
    {
        if (Input.GetKey(interactKey) && !player.Aiming)
        {
            interactTimer += Time.deltaTime;

            if(interactTimer > interactTime)
            {
                onInteract?.Invoke();
                Disable();
            }
        }
        else if(interactTimer > 0)
        {
            interactTimer -= Time.deltaTime * 0.3f;
        }

        fill.fillAmount = Mathf.InverseLerp(0, interactTime, interactTimer);
    }

    public void Initialize(string objectName, string description)
    {
        Initialize(KeyCode.E, objectName, description);
    }
    public void Initialize(KeyCode key, string objectName, string description) 
    {
        Toggle(true);
        interactKey = key;
        this.objectName.text = objectName;
        this.description.text = description;
    }


    void SetSize()
    {
        transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale, ref scaleVelocity, scaleSmoothTime);
        if (Vector3.Distance(transform.localScale, Vector3.zero) < scaleTreshold)
        {
            Toggle(false);
        }
    }
    private void Toggle(bool value)
    {
        active = value;
        gameObject.SetActive(value);
    }
    public void Enable(Vector3 pos) 
    {
        targetPos = pos;
        if (active) return;
        active = true;
        targetScale = defaultScale;
        interactTimer = 0;
    }
    public void Disable()
    {
        if (!active) return;
        active = false;
        targetScale = Vector3.zero;
        print("disable");
    }
    
}
