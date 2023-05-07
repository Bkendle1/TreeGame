using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private float force;
    [Tooltip("How much damage to deal to player.")]
    [SerializeField] private int damageValue = 10;
    [Tooltip("How long before the bullet deactivates itself.")]
    [SerializeField] private float deactivationTimer = 3f;
    private Transform player;
    private Rigidbody2D rb;
    private float timer;
    private Vector2 direction;

    private ObjectPoolAdvanced bulletPool;
    
    [Header("Cinemachine")]                               
    [SerializeField] private float camShakeIntensity = 4f;
    [SerializeField] private float camShakeDuration = .1f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        bulletPool = FindObjectOfType<ObjectPoolAdvanced>();
        
        rb.velocity = new Vector2(direction.x, direction.y).normalized * force;
        
        direction = player.transform.position - transform.position;
        float rotationAngle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotationAngle + 90);

    }

    private void OnEnable()
    {
        if (rb != null)
        {
            rb.velocity = new Vector2(direction.x, direction.y).normalized * force;
        }
        Invoke("Deactivate", deactivationTimer);
    }

    private void Deactivate()
    {
        if (bulletPool != null)
        {
            bulletPool.ReturnGameObject(gameObject);
        }
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<PlayerHealth>() != null)
        {
            col.GetComponent<PlayerHealth>().TakeDamage(damageValue);
            Movement playerMovement = col.GetComponent<Movement>();
            
            playerMovement.KBTimer = playerMovement.KBDuration;
            
            if (col.transform.position.x <= transform.position.x)
            {
                playerMovement.KnockFromRight = true;
            }
            else
            {
                playerMovement.KnockFromRight = false;
            }
            CinemachineShake.Instance.ShakeCamera(camShakeIntensity,camShakeDuration);
            bulletPool.ReturnGameObject(gameObject);
        }
    }
}
