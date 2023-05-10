using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossBattleTrigger : MonoBehaviour
{

    private BoxCollider2D boxCol;
    [SerializeField] private MalWartBoss malWart;
    public bool activateBossFight;

    private Transform player;
    
    [SerializeField] private GameObject enemyHealthBar;
    private void Start()
    {
        boxCol = GetComponent<BoxCollider2D>();
        player = FindObjectOfType<Movement>().transform;
    }

    private void Update()
    {
        if (!malWart.gameObject.activeSelf)
        {
            Debug.Log("MALWART DEFEATED");
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (child != null)
                {
                    child.SetActive(false);
                }
            }

            activateBossFight = false;
        }
        
        //if player is on the left side of the trigger, deactivate walls
        if (player.transform.position.x < transform.position.x)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (child != null)
                {
                    child.SetActive(false);
                }
            }
        }
        
        else if(player.transform.position.x  > transform.position.x && malWart.gameObject.activeSelf) //if player is on the right side of the trigger, activate walls
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (child != null)
                {
                    child.SetActive(true);
                }
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Movement>() != null)
        {
            Debug.Log(col.gameObject.name);
            enemyHealthBar.SetActive(true);
            activateBossFight = true;
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (child != null)
                {
                    child.SetActive(true);
                }
            }
            boxCol.enabled = false;
        }
    }
}
