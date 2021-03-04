using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretTargeting : MonoBehaviour
{

    public Transform target;
    public bool wantsToTarget = false;
    public bool wantsToAttack = false;
    public float visionDistance = 10;
    public float visionAngle = 45;

    private List<PlayerMovement> potentialTargets = new List<PlayerMovement>();

    float cooldownScan = 0;
    float cooldownPick = 0;

    float coolDownShoot = 0;
    public float roundsPerSecond = 1f;
    public float damage = 20f;
    public bool isCannonTopTimeToFire = true;
    public bool isCannonBottomTimeToFire = false;

    public Transform cannonTop;
    public Transform cannonBottom;

    private Vector3 startPosCannonTop;
    private Vector3 startPosCannonBottom;

    public ParticleSystem muzzleFlashPreafab;
    public Transform turretTop;
    public Transform turretBottom;

    CameraOrbit camOrbit;

    public float idleTimer = 0;

    public AudioSource sound;
    public AudioClip cannon;

    private void Start()
    {
        startPosCannonTop = cannonTop.localPosition;
        startPosCannonBottom = cannonBottom.localPosition;

        camOrbit = Camera.main.GetComponentInParent<CameraOrbit>();
    }

    void Update()
    {
        wantsToTarget = true;
        wantsToAttack = true;

        if (!wantsToTarget) target = null;

        if (idleTimer > 0) idleTimer -= Time.deltaTime;

        cooldownScan -= Time.deltaTime; // counting down...
        if (cooldownScan <= 0 || (target == null && wantsToTarget)) ScanForTargets(); // do this when countdown finished

        cooldownPick -= Time.deltaTime; // counting down...
        if (cooldownPick <= 0) PickATarget(); // do this when countdown finished

        if (coolDownShoot > 0) coolDownShoot -= Time.deltaTime;

        if (target && !canSeeThing(target)) target = null;

        if (target == null) idleSpin();

        SlideArmsHome();

        doAttack();
    }

    private void idleSpin()
    {
        Quaternion targetRot;
        if (idleTimer <= 0)
        {
            targetRot = new Quaternion(0, UnityEngine.Random.Range(0, 360), 0, 0);
            idleTimer = 5;
        }
        
    }

    private void SlideArmsHome()
    {
        cannonTop.localPosition = AnimMath.Slide(cannonTop.localPosition, startPosCannonTop, .001f);
        cannonBottom.localPosition = AnimMath.Slide(cannonBottom.localPosition, startPosCannonBottom, .001f);
    }

    private void doAttack()
    {
        if (!wantsToTarget) return;
        if (!wantsToAttack) return;
        if (target == null) return;
        if (!canSeeThing(target)) return;
        if (coolDownShoot > 0) return;


        HealthSystem targetHealth = target.GetComponent<HealthSystem>();

        if (targetHealth)
        {
            targetHealth.takeDamage(damage, .5f);
            sound.PlayOneShot(cannon);
        }

        coolDownShoot = 1 / roundsPerSecond;

        // attack

        camOrbit.Shake(1);

        if (isCannonBottomTimeToFire)
        {
            if (turretTop) Instantiate(muzzleFlashPreafab, turretTop.position, turretTop.rotation);
            cannonTop.localEulerAngles += new Vector3(-5, 0, 0);
            cannonTop.position += -cannonTop.forward * .5f;
            flipHands();
        }
        else if (isCannonTopTimeToFire)
        {
            if (turretBottom) Instantiate(muzzleFlashPreafab, turretBottom.position, turretBottom.rotation);
            cannonBottom.localEulerAngles += new Vector3(-5, 0, 0);
            cannonBottom.position += -cannonBottom.forward * .1f;
            flipHands();
        }
    }

    private void flipHands()
    {
        isCannonBottomTimeToFire = !isCannonBottomTimeToFire;
        isCannonTopTimeToFire = !isCannonTopTimeToFire;
    }

    private bool canSeeThing(Transform thing)
    {
        if (!thing) return false; // thing doesnt exist
        if (GetComponent<HealthSystem>().health <= 0) return false;
        if (thing.GetComponent<HealthSystem>().health <= 0) return false;

        //check distance
        Vector3 vToThing = thing.position - transform.position;
        if (vToThing.sqrMagnitude > visionDistance * visionDistance) return false; // too far away

        //check direction
        if (Vector3.Angle(transform.forward, vToThing) > visionAngle) return false;

        //check occlusion


        return true;
    }

    private void ScanForTargets()
    {

        // do the next scan in 1 seconds:
        cooldownScan = 1;

        // empty the list:
        potentialTargets.Clear();

        // refill the list:

        PlayerMovement[] things = GameObject.FindObjectsOfType<PlayerMovement>();
        foreach (PlayerMovement thing in things)
        {
            // check how far away thing is

            if (canSeeThing(thing.transform))
            {
                potentialTargets.Add(thing);
            }
            // check what direction it is in
        }
    }

    void PickATarget()
    {

        cooldownPick = .25f;

        //if (target) return; // we already have a target...
        target = null;

        float closestDistanceSoFar = 0;

        // find closest targetable-thing and sets it as our target:
        foreach (PlayerMovement pt in potentialTargets)
        {

            float dd = (pt.transform.position - transform.position).sqrMagnitude;

            if (dd < closestDistanceSoFar || target == null)
            {
                target = pt.transform;
                closestDistanceSoFar = dd;
            }
        }
    }
}

