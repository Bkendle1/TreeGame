using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MaxHealthPickup : MonoBehaviour
{
    [Tooltip("The amount by which the player's new max health increases.")]
    [SerializeField] private int healthAmount = 10;

    [SerializeField] private float destructionTimer = 1f;
    [SerializeField] AudioClip pickupSFX;
    private AudioSource audioSource;
    private SpriteRenderer sr;
    private BoxCollider2D boxCol;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sr = GetComponent<SpriteRenderer>();
        boxCol = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<PlayerHealth>() != null)
        {
            PlayerHealth playerHealth = col.gameObject.GetComponent<PlayerHealth>();
            if (col.gameObject.CompareTag("Player"))
            {
                sr.enabled = false;
                boxCol.enabled = false;
                playerHealth.SetMaxHealth(healthAmount);
                audioSource.PlayOneShot(pickupSFX);
                Invoke("SelfDestruct", destructionTimer);
            }
        }
    }
    
    private void SelfDestruct()
    {
        Destroy(gameObject);
    } 
}
