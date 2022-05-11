using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public Vector3 targetPos;
    public Vector3 defaultPos;
    Vector3 posVelocity;
    public float posSmoothTime;

    public Vector3 defaultSize { get; private set; }
    public float value;

    Vector3 defaultRotation;

    public Color targetColor;
    float _colorLerp;
    public float colorLerpSpeed;

    public Vector3 targetSize;
    public float maxSizeDelta;

    Vector3 targetRotation;
    public float maxRotDelta;

    public Vector3 OnRotate;

    public Image[] images;

    public Animator[] animators;
    public Animator onHit;

    // Start is called before the first frame update
    void Start()
    {
        defaultPos = transform.position; 
        defaultSize = transform.localScale;
        defaultRotation = transform.localEulerAngles;

        targetSize = defaultSize;
        targetColor = images[0].color;
        targetRotation = transform.localEulerAngles;
    }

    private void Update()
    {
        _colorLerp += colorLerpSpeed * Time.deltaTime;
        
        foreach(Image image in images)
        image.color = Color.Lerp(image.color, targetColor, _colorLerp);

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref posVelocity, posSmoothTime * Time.deltaTime);

        transform.localScale = Vector3.MoveTowards(transform.localScale,targetSize,maxSizeDelta * Time.deltaTime);

        transform.localEulerAngles = Vector3.MoveTowards(transform.localEulerAngles,targetRotation, maxRotDelta * Time.deltaTime);
    }

    public void Rotate(bool rotate)
    {
        if (rotate) targetRotation = OnRotate;
        else targetRotation = defaultRotation;
    }

    public void SetColor(Color color)
    {
        targetColor = color;
        _colorLerp = 0;
    }
    public void SetTargetSize(Vector3 size)
    {
        targetSize = size;
    }

    public void OnShoot()
    {
        foreach(Animator animator in animators)
        animator.SetTrigger("Shoot");
    }

    public void OnHitEnemy()
    {
        onHit.SetTrigger("Hit");
    }
    public void OnKillEnemy()
    {
        onHit.SetTrigger("Kill");
    }

    public void SetCrosshair(Vector3 hitPoint)
    {
        targetPos = Camera.main.WorldToScreenPoint(hitPoint);
    }

    public void ResetCrosshair()
    {
        targetPos = defaultPos;
        targetSize = defaultSize;
        targetRotation = defaultRotation;
        targetColor = Color.white;
    }
}
