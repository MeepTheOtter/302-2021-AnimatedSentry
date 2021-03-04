using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour {

    public PlayerMovement moveScript;
    private PlayerTargeting targetScript;
    private Camera cam;

    private float yaw = 0;
    private float pitch = 0;

    public float cameraSensitivityX = 10;
    public float cameraSensitivityY = 10;

    public float shakeIntensity = 0;

    private void Start()
    {
        targetScript = moveScript.GetComponent<PlayerTargeting>();
        cam = GetComponentInChildren<Camera>();
    }

    void Update() {
        PlayerOrbitCamera();

        if (moveScript != null && targetScript != null) transform.position = moveScript.transform.position;

        // if aiming, set camera rotation to look at target
        rotateCamToLookAtTarget();

        // zoom in camera
        zoomCamera();

        shakeCamera();
    }

    public void Shake(float intensity = 1, float timeMult = 1)
    {
        shakeIntensity = intensity;
    }

    private void shakeCamera()
    {
        if (shakeIntensity < 0) shakeIntensity = 0;
        if (shakeIntensity > 0) shakeIntensity -= Time.deltaTime ;
        else return;

        // pick a small random rotation
        Quaternion targetRot = AnimMath.Lerp(Random.rotation, Quaternion.identity, .999f);

        //cam.transform.localRotation *= targetRot;
        cam.transform.localRotation = AnimMath.Lerp(cam.transform.localRotation, cam.transform.localRotation * targetRot, shakeIntensity * shakeIntensity);
    }

    private void zoomCamera()
    {
        float dis = 10;

        if (isTargeting()) dis = 3;

        cam.transform.localPosition = AnimMath.Slide(cam.transform.localPosition, new Vector3(0, 0, -dis), .001f);

    }

    private bool isTargeting()
    {
        return (targetScript != null && targetScript.target != null && targetScript.wantsToTarget);
    }

    private void rotateCamToLookAtTarget()
    {

        if (isTargeting())
        {
            // if targeting, set rotation to look at target
            Vector3 vToTarget = targetScript.target.position - cam.transform.position;

            Quaternion targetRot = Quaternion.LookRotation(vToTarget, Vector3.up);

            cam.transform.rotation = AnimMath.Slide(cam.transform.rotation, targetRot, .001f);

        } else
        {
            // if not targeting, reset rotation
            cam.transform.localRotation = AnimMath.Slide(cam.transform.localRotation, Quaternion.identity, .01f); // no rotation
        }
    }

    private void PlayerOrbitCamera() {
        float mx = Input.GetAxisRaw("Mouse X");
        float my = Input.GetAxisRaw("Mouse Y");

        yaw += mx * cameraSensitivityX;
        pitch += my * cameraSensitivityY;

        if (isTargeting()) // z-targeting
        {
            float playerYaw = moveScript.transform.eulerAngles.y;
            yaw = Mathf.Clamp(yaw, playerYaw - 30, playerYaw + 30);

            pitch = Mathf.Clamp(pitch, 15, 40);

        } else // not targeting || free look
        {
            pitch = Mathf.Clamp(pitch, 15, 89);
        }       

        transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .01f);
    }
}