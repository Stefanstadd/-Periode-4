using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    Camera cam;
    public PlayerMovement player;
    float targetFOV;
    float defaultFOV;
    float targetRecoil;
    public float fovSmoothTime;
    public float recoilSmoothTime;
    float fovVelocity;
    float recoilVelocity;

    private void Start()
    {
        cam = GetComponent<Camera>();
        defaultFOV = cam.fieldOfView;
        targetFOV = defaultFOV;
    }
    void Update()
    {
        if (GameMenuManager.Paused || PlayerMovement.Dead) return;

        targetRecoil = Mathf.SmoothDamp(targetRecoil, 0, ref recoilVelocity, recoilSmoothTime * Time.deltaTime);
        cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, targetFOV, ref fovVelocity, fovSmoothTime * Time.deltaTime);
        player.recoilRot = targetRecoil;
    }

    public void SetFOV(float target)
    {
        targetFOV =  defaultFOV - target;
    }
    public void ResetToDefault()
    {
        targetFOV = defaultFOV;
    }
    public void AddRecoil(float toAdd)
    {
        targetRecoil -= toAdd;
    }
}
