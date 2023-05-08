using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingRodSpike : MonoBehaviour
{
    [SerializeField] private int damageValue = 10;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<PlayerHealth>() != null)
        {
            col.GetComponent<PlayerHealth>().TakeDamage(damageValue);
        }
    }
}
