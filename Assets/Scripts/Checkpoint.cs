using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private GameObject visualCue;
    [Tooltip("The point where Mother Nature should teleport when called.")]
    [SerializeField] private Transform teleportPosition;
    private bool playerInRange;
    private Transform motherNature;
    [SerializeField] private AudioClip sfx;
    private AudioSource audioSource;
    
    private void Start()
    {
        visualCue.SetActive(false);
        motherNature = GameObject.FindGameObjectWithTag("MotherNature").transform;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (playerInRange)
        {
            visualCue.SetActive(true);
            if (Movement.Instance.GetInteractedPressed())
            {
                //TODO play sfx
                motherNature.position = teleportPosition.position;
            }
        }
        else
        {
            visualCue.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<PlayerHealth>() != null)
        {
            playerInRange = true;
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(sfx);
            }
            
            GameManager.Instance.lastCheckPointPos = transform.position;
            
            //restore player health to max
            col.GetComponent<PlayerHealth>().Heal(999);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
