using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXPscript : MonoBehaviour
{
    [Tooltip("How much each exp acorn is worth")]
    [SerializeField] private int pointValue = 1;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            GameManager.Instance.UpdateExp(pointValue);
            Destroy(gameObject);
        }
    }
}
