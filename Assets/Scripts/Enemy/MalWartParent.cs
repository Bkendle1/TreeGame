using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MalWartParent : MonoBehaviour
{
    private BossBattleTrigger bossTrigger;

    private void Start()
    {
        bossTrigger = GetComponentInChildren<BossBattleTrigger>();
    }

    private void Update()
    {
        if (bossTrigger.activateBossFight)
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
}
