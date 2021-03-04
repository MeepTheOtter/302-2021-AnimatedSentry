using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{

    public float health { get; private set; }
    public float maxHealth = 100;
    public bool isDead = false;

    public float deathTimer = 0;
    public float deathTimerReset = 5;

    public bool isPlayer = false;
    public float healthRegenTimer = 0;

    private bool rotateRightOnDeath = false;

    public float iframes = 0;
    public float maxIframes = 100;

    private Vector3 scaleChange = new Vector3(.05f, .05f, .05f);

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        if (GetComponent<PlayerTargeting>() != null) isPlayer = true;
        int random = Random.Range(0, 2);        
        if (random == 1) rotateRightOnDeath = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (healthRegenTimer > 0) healthRegenTimer -= Time.deltaTime;
        if (healthRegenTimer <= 0 && health > 0 && isPlayer) health += 35 * Time.deltaTime;

        if (health > maxHealth) health = maxHealth;

        if (deathTimer > 0) deathTimer -= Time.deltaTime;

        if (iframes > 0) iframes -= Time.deltaTime;

        if (health <= 0 && deathTimer > 0) doDeathAnimation();   
        
        if (isPlayer)
        {
            float m = health;
            m = map(m, 0, maxHealth, 0, 1);
            HealthBarHandler.SetHealthBarValue(m);
        }

        if (health <= 0 && deathTimer <= 0)
        {
            die();
        }
    }

    float map(float x, float x1, float x2, float y1, float y2)
    {
        var m = (y2 - y1) / (x2 - x1);
        var c = y1 - m * x1; // point of interest: c is also equal to y2 - m * x2, though float math might lead to slightly different results.

        return m * x + c;
    }

    public void takeDamage(float damage, float ifrms = 0)
    {
        if (damage <= 0) return;
        if (iframes > 0) return; 

        health -= damage;
        iframes = ifrms;
        healthRegenTimer = 5;
        if (iframes > maxIframes) iframes = maxIframes;

        if (health <= 0)
        {
            deathTimer = deathTimerReset;
            iframes = 10000;
            doDeathAnimation();
        }
    }

    private void doDeathAnimation()
    {        
        //do Player Death anims
        if (isPlayer)
        {
            Vector3 targetScale = transform.localScale - scaleChange;
            transform.localScale = AnimMath.Lerp(transform.localScale, targetScale, .01f);

            if(rotateRightOnDeath) transform.Rotate(0, 360 * Time.deltaTime, 0);
            if(!rotateRightOnDeath) transform.Rotate(0, -360 * Time.deltaTime, 0);
        } 
        //do turret death anims
        else if (!isPlayer)
        {
            Vector3 targetScale = transform.localScale - scaleChange;
            transform.localScale = AnimMath.Lerp(transform.localScale, targetScale, .01f);

            if (rotateRightOnDeath) transform.Rotate(0, 360 * Time.deltaTime, 0);
            if (!rotateRightOnDeath) transform.Rotate(0, -360 * Time.deltaTime, 0);
        }
    }

    public void die()
    {
        Destroy(gameObject);
    }
}
