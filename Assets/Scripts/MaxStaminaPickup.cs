using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MaxStaminaPickup : MonoBehaviour
{
    [Tooltip("The amount by which the player's new max stamina increases.")]
    [SerializeField] private int staminaAmount = 10;
    [SerializeField] private float destructionTimer = 1f;
    [SerializeField] AudioClip pickupSFX;
    private AudioSource audioSource;
    private SpriteRenderer sr;
    private BoxCollider2D boxCol;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        boxCol = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<Movement>() != null)
        {
            Movement playerMovement = col.gameObject.GetComponent<Movement>();
            if (col.gameObject.CompareTag("Player"))
            {
                sr.enabled = false;
                boxCol.enabled = false;
                playerMovement.SetMaxStamina(staminaAmount);
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
