using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{

    public float health { get; private set; }
    public float maxHealth = 100;
    public bool isDead = false;

    public float iframes = 0;
    public float maxIframes = 100;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(float damage, float ifrms = 0)
    {
        if (damage <= 0) return;

        health -= damage;
        iframes = ifrms;
        if (iframes > maxIframes) iframes = maxIframes;

        if (health <= 0) die();
    }

    public void die()
    {
        Destroy(gameObject);
    }
}
