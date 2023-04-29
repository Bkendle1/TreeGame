using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Put this on whatever game object goes into the pool
/// </summary>
public class ReturnToPool : MonoBehaviour
{
    [Tooltip("Time before gameObject returns to pool")]
    [SerializeField] private float timeToLive = 2f;

    private void OnEnable()
    {
        Invoke("Return", timeToLive);
    }

    private void Return()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }
}
