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

    public float minDistToScale;
    public float scaleMultiplier;
    public Vector3 minSize, maxSize;

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

    Vector3 lastHitPos;
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

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref posVelocity, posSmoothTime);

        transform.localScale = Vector3.MoveTowards(transform.localScale,targetSize/* + DistanceSize()*/  ,maxSizeDelta * Time.deltaTime);

        transform.localEulerAngles = Vector3.MoveTowards(transform.localEulerAngles,targetRotation, maxRotDelta * Time.deltaTime);
    }

    Vector3 DistanceSize()
    {
        return Vector3.Lerp(minSize, maxSize, Mathf.Clamp01(Vector3.Distance(PlayerMovement.player.transform.position, lastHitPos) * scaleMultiplier));
    }
    public void Rotate(float zRot)
    {
        targetRotation.z = zRot;
    }

    public void SetColor(Color color)
    {
        targetColor = color;
        _colorLerp = 0;
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
        lastHitPos = hitPoint;
    }

    public void ResetCrosshair()
    {
        targetPos = defaultPos;
        targetSize = defaultSize;
        targetRotation = defaultRotation;
        targetColor = Color.white;
    }
}
