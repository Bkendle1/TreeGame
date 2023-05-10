using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCamConfinerTrigger : MonoBehaviour
{
    public bool isFightingBoss = false;
    private Transform player;

    private void Start()
    {
        player = FindObjectOfType<Movement>().transform;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is fighting boss.");
            isFightingBoss = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Debug.Log("Player is started boss.");
            isFightingBoss = true;
            CinemachineManager.Instance.SwitchPriority();
        }
    }

    private void Update()
    {
        if (player.position.x < transform.position.x)
        {
            Debug.Log("Player is lost to boss.");
            isFightingBoss = false;
        }
    }

}
