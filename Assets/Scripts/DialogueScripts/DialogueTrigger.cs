using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private GameObject visualCue;
    [SerializeField] private TextAsset inkJSON;
    
     
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
            
            //TODO: Disable interact input from player (E key) once dialogue starts so the dialogue doesn't repeat
            
            //start a new story
            if (Movement.Instance.GetInteractedPressed())
            {
                DialogueManager.Instance.EnterDialogueMode(inkJSON);
            }
        }
        else if (DialogueManager.Instance.dialogueIsPlaying)
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
