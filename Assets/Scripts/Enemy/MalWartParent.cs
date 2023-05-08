using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MalWartParent : MonoBehaviour
{
    private BossBattleTrigger bossTrigger;
    [SerializeField] private GameObject malWart;
    private bool hasSpawnedMalWart = false;
    
    private void Start()
    {
        bossTrigger = GetComponentInChildren<BossBattleTrigger>();
        malWart.SetActive(false);
    }

    private void Update()
    {
        if (bossTrigger.activateBossFight && !hasSpawnedMalWart)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (child != null)
                {
                    child.SetActive(true);
                }
            }

            hasSpawnedMalWart = true;
        }
    }
}
