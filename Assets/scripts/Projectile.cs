using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if(player)
        {
            HealthSystem playerHealth = player.GetComponent<HealthSystem>();
            if(playerHealth)
            {
                playerHealth.takeDamage(10);
            }
            Destroy(gameObject);
        }
    }
}
