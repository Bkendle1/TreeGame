using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killzone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<PlayerHealth>().TakeDamage(800);
        } else if (col.CompareTag("Enemy"))
        {
            col.GetComponent<Enemy>().TakeDamage(999);
        }
    }
}
