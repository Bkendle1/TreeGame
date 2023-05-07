using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossBattleTrigger : MonoBehaviour
{

    private BoxCollider2D boxCol;
    private Enemy enemy;
    
    private void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        boxCol = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (enemy.currentHealth <= 0)
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
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            enemy.healthBar.enabled = true;
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (child != null)
                {
                    child.SetActive(true);
                }
            }

            boxCol.enabled = false;
            CinemachineManager.Instance.SwitchPriority();
        }
    }
}
