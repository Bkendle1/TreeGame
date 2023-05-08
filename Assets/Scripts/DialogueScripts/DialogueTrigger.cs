using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private GameObject visualCue;
    [SerializeField] private TextAsset nextInky;
    [SerializeField] private TextAsset firstInky;
     
    private bool playerInRange = false;
    private void Awake()
    {
        visualCue.SetActive(false);
    }

    private void Update()
    {
        
        if (playerInRange && !DialogueManager.Instance.dialogueIsPlaying)
        {
            visualCue.SetActive(true);
            
            //start a new story
            if (Movement.Instance.GetInteractedPressed())
            {
                DialogueManager.Instance.EnterDialogueMode(firstInky);
                //this is to change the dialogue from the first conversation with mother nature to the next
                firstInky = nextInky;
            }
        }
        else
        {
            visualCue.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            playerInRange = true;
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
