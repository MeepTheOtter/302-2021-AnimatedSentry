using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour {

    public Transform target;
    public bool wantsToTarget = false;
    public bool wantsToAttack = false;
    public float visionDistance = 10;
    public float visionAngle = 45;

    private List<TargetableThing> potentialTargets = new List<TargetableThing>();

    float cooldownScan = 0;
    float cooldownPick = 0;

    float coolDownShoot = 0;
    public float roundsPerSecond = 10f;
    public bool isArmRightTimeToFire = true;
    public bool isArmLeftTimeToFire = false;

    public Transform armL;
    public Transform armR;

    private Vector3 startPosArmL;
    private Vector3 startPosArmR;

    public ParticleSystem muzzleFlashPreafab;
    public Transform handL;
    public Transform handR;

    CameraOrbit camOrbit;

    //TO-DO: Change it so the whole torse gets pushed back instead of arms
    //TO-DO: change it so the arms attack one after another

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        startPosArmL = armL.localPosition;
        startPosArmR = armR.localPosition;

        camOrbit = Camera.main.GetComponentInParent<CameraOrbit>();
    }

    void Update() {
        wantsToTarget = Input.GetButton("Fire2");
        wantsToAttack = Input.GetButton("Fire1");

        if (!wantsToTarget) target = null;

        cooldownScan -= Time.deltaTime; // counting down...
        if (cooldownScan <= 0 || (target == null && wantsToTarget)) ScanForTargets(); // do this when countdown finished

        cooldownPick -= Time.deltaTime; // counting down...
        if (cooldownPick <= 0) PickATarget(); // do this when countdown finished
        
        if(coolDownShoot > 0) coolDownShoot -= Time.deltaTime;

        if (target && !canSeeThing(target)) target = null;

        SlideArmsHome();

        doAttack();
    }

    private void SlideArmsHome()
    {
        armL.localPosition = AnimMath.Slide(armL.localPosition, startPosArmL, .001f);
        armR.localPosition = AnimMath.Slide(armR.localPosition, startPosArmR, .001f);

    }

    private void doAttack()
    {
        if (!wantsToTarget) return;
        if (!wantsToAttack) return;
        if (target == null) return;
        if (!canSeeThing(target)) return;
        if (coolDownShoot > 0) return;


        HealthSystem targetHealth = target.GetComponent<HealthSystem>();

        if(targetHealth)
        {
            targetHealth.takeDamage(20);
        }

        coolDownShoot = 1 / roundsPerSecond;

        // attack

        camOrbit.Shake(1);

        if(handL) Instantiate(muzzleFlashPreafab, handL.position, handL.rotation);
        if(handR) Instantiate(muzzleFlashPreafab, handR.position, handR.rotation);

        // trigger arm animation

        armL.localEulerAngles += new Vector3(-20, 0, 0);
        armR.localEulerAngles += new Vector3(-20, 0, 0);


        armL.position += -armL.forward * .1f;
        armR.position += -armR.forward * .1f;
    }

    private bool canSeeThing(Transform thing)
    {
        if (!thing) return false; // thing doesnt exist

        //check distance
        Vector3 vToThing = thing.position - transform.position;
        if (vToThing.sqrMagnitude > visionDistance * visionDistance) return false; // too far away

        //check direction
        if (Vector3.Angle(transform.forward, vToThing) > visionAngle) return false;

        //check occlusion


        return true;
    }

    private void ScanForTargets() {

        // do the next scan in 1 seconds:
        cooldownScan = 1;

        // empty the list:
        potentialTargets.Clear();
        
        // refill the list:

        TargetableThing[] things = GameObject.FindObjectsOfType<TargetableThing>();
        foreach(TargetableThing thing in things) {
            // check how far away thing is

            if (canSeeThing(thing.transform))
            {
                potentialTargets.Add(thing);
            }
            // check what direction it is in
        }
    }

    void PickATarget() {

        cooldownPick = .25f;

        //if (target) return; // we already have a target...
        target = null;

        float closestDistanceSoFar = 0;

        // find closest targetable-thing and sets it as our target:
        foreach(TargetableThing pt in potentialTargets) {
            
            float dd = (pt.transform.position - transform.position).sqrMagnitude;

            if(dd < closestDistanceSoFar || target == null) {
                target = pt.transform;
                closestDistanceSoFar = dd;
            }            
        }
    }
}
