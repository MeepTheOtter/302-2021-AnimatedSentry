using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretPointAt : MonoBehaviour
{
    private TurretTargeting turretTargeting;

    private Quaternion startingRotation;

    public bool lockRotationX;
    public bool lockRotationY;
    public bool lockRotationZ;

    public bool doIdleRot = false;

    public float idleTimer = 0;
    Quaternion targetRot;
    Vector3 euler3;

    void Start()
    {
        startingRotation = transform.localRotation;
        turretTargeting = GetComponentInParent<TurretTargeting>();
    }

    void Update()
    {
        if (idleTimer > 0) idleTimer -= Time.deltaTime;
        TurnTowardsTarget();
    }

    private void TurnTowardsTarget()
    {

        if (turretTargeting && turretTargeting.target && turretTargeting.wantsToTarget)
        {

            Vector3 disToTarget = turretTargeting.target.position - transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(disToTarget, Vector3.up);

            Vector3 euler1 = transform.localEulerAngles; // get local angles BEFORE rotation
            Quaternion prevRot = transform.rotation;
            transform.rotation = targetRotation; // set rotation
            Vector3 euler2 = transform.localEulerAngles; // get local angles AFTER rotation

            if (lockRotationX) euler2.x = euler1.x; // revert x to previous value
            if (lockRotationY) euler2.y = euler1.y; // revert y to previous value
            if (lockRotationZ) euler2.z = euler1.z; // revert z to previous value

            transform.rotation = prevRot; // revert rotation

            // animate rotation:
            transform.localRotation = AnimMath.Slide(transform.localRotation, Quaternion.Euler(euler2), .001f);

        }
        else
        {
            // figure out bone rotation, no target:
            if (!doIdleRot) return;
            if (idleTimer <= 0)
            {
                Vector3 euler1 = transform.localEulerAngles; // get local angles BEFORE rotation
                Quaternion prevRot = transform.rotation;
                transform.rotation = UnityEngine.Random.rotation; // set rotation
                euler3 = transform.localEulerAngles; // get local angles AFTER rotation

                if (lockRotationX) euler3.x = euler1.x; // revert x to previous value
                if (lockRotationY) euler3.y = euler1.y; // revert y to previous value
                if (lockRotationZ) euler3.z = euler1.z; // revert z to previous value

                transform.rotation = prevRot; // revert rotation
                idleTimer = Random.Range(3, 12);
            }

            transform.localRotation = AnimMath.Slide(transform.localRotation, Quaternion.Euler(euler3), .001f);
        }
    }
}
