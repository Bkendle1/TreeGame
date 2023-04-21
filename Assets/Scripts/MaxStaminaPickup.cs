using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MaxStaminaPickup : MonoBehaviour
{
    [Tooltip("The amount by which the player's new max stamina increases.")]
    [SerializeField] private int staminaAmount = 10;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<Movement>() != null)
        {
            Movement playerMovement = col.gameObject.GetComponent<Movement>();
            if (col.gameObject.CompareTag("Player"))
            {
                playerMovement.SetMaxStamina(staminaAmount);
                Destroy(gameObject);
            }
        }
        
    }
}
