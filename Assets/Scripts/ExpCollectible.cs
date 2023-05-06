using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ExpCollectible : PoolObject
{
    [Tooltip("How much each exp acorn is worth.")]
    [SerializeField] private int pointValue = 1;
    [Tooltip("How fast the exp acorn moves towards the player.")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("How long the exp acorn stays before it disappears.")]
    [SerializeField] private float m_timeToLive = 3f;
    private Rigidbody2D rb;
    private bool hasTarget;
    private Vector3 targetPosition;
    private bool hasSpawned = true;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Invoke("TurnOff", m_timeToLive);
    }

    private void FixedUpdate()
    {
        if (hasTarget)
        {
            Vector2 targetDirection = (targetPosition - transform.position).normalized;
            rb.velocity = new Vector2(targetDirection.x, targetDirection.y) * moveSpeed;
        }

        //this if statement could work with just the add force line in the start method instead but I wanted the rigidbody
        //manipulation to happen in the fixed update, I'm not sure if its made a difference though.
        if (hasSpawned)
        {
            rb.AddForce(new Vector2(Random.Range(-5, 10), Random.Range(5,10)), ForceMode2D.Impulse);
            hasSpawned = false;
        }
    }

    public void SetTarget(Vector3 position)
    {
        targetPosition = position;
        hasTarget = true;
    }

    private void TurnOff()
    {
        ReturnToPool();
    }
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            GameManager.Instance.UpdateExp(pointValue);
            ReturnToPool();
        }
    }
}
