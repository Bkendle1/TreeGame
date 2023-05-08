using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossBattleTrigger : MonoBehaviour
{

    private BoxCollider2D boxCol;
    [SerializeField] private GameObject enemy;
    public bool activateBossFight;
    
    [SerializeField] private GameObject enemyHealthBar;
    private void Start()
    {
        
        boxCol = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (enemy.GetComponent<Enemy>().currentHealth <= 0)
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
            CinemachineManager.Instance.SwitchPriority();
        }
    }
}
