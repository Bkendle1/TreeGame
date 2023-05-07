using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private Vector2 direction;

    //private ObjectPoolAdvanced bulletPool;
    
    [Header("Cinemachine")]                               
    [SerializeField] private float camShakeIntensity = 4f;
    [SerializeField] private float camShakeDuration = .1f;

    void Start()
    {
        player = FindObjectOfType<Movement>().transform;
        rb = GetComponent<Rigidbody2D>();
        //bulletPool = FindObjectOfType<ObjectPoolAdvanced>();
        
        Vector3 direction = player.transform.position - transform.position;
        rb.velocity = new Vector2(direction.x, direction.y).normalized * force;
        float rotationAngle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0,rotationAngle + 90);
    }

    private void OnEnable()
    {
        Invoke("Deactivate", deactivationTimer);
    }
    
    private void Deactivate()
    {
        // if (bulletPool != null)
        // {
        //     bulletPool.ReturnGameObject(gameObject);
        // }
        Destroy(gameObject);
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
            CinemachineShake.Instance.ShakeCamera(camShakeIntensity,camShakeDuration);
            
            playerMovement.KBTimer = playerMovement.KBDuration;
            
            if (col.transform.position.x <= transform.position.x)
            {
                playerMovement.KnockFromRight = true;
            }
            else
            {
                playerMovement.KnockFromRight = false;
            }
            Deactivate();
        }
    }
}
