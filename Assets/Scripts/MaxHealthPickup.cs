using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MaxHealthPickup : MonoBehaviour
{
    [Tooltip("The amount by which the player's new max health increases.")]
    [SerializeField] private int healthAmount = 10;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<PlayerHealth>() != null)
        {
            PlayerHealth playerHealth = col.gameObject.GetComponent<PlayerHealth>();
            if (col.gameObject.CompareTag("Player"))
            {
                playerHealth.SetMaxHealth(healthAmount);
                Destroy(gameObject);
            }
        }
        
    }
}
