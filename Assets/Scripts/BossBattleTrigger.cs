using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossBattleTrigger : MonoBehaviour
{

    private BoxCollider2D boxCol;
    private Enemy enemy;
    public static bool isBossFighting = false; 
    
    [SerializeField] private GameObject enemyHealthBar;
    private void Start()
    {
        enemy = GetComponentInParent<Enemy>();
        boxCol = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (enemy.currentHealth <= 0)
        {
            isBossFighting = false;
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
            enemyHealthBar.SetActive(true);
            isBossFighting = true;
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