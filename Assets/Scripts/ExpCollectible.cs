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
    [Tooltip("How close the player must be before exp acorn flies to them.")]
    [SerializeField] private float magnetDistance = 4f;
    private Rigidbody2D rb;
    private Transform player;
    private bool hasStarted = true;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Invoke("TurnOff", m_timeToLive);
        player = FindObjectOfType<Movement>().transform;
    }

    private void FixedUpdate()
    {
        Debug.Log(Vector2.Distance(transform.position, player.transform.position));
        if (Vector2.Distance(transform.position, player.transform.position) <= magnetDistance)
        {
            gameObject.transform.position = Vector2.MoveTowards(gameObject.transform.position,
                player.transform.position, moveSpeed);
        }

        if (hasStarted)
        {
            rb.AddForce(new Vector2(Random.Range(-5, 10), Random.Range(5,10)), ForceMode2D.Impulse);
            hasStarted = false;
        }
    }

    private void TurnOff()
    {
        hasStarted = true;
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
