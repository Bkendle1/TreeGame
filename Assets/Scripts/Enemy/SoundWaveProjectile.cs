using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// /This is for projectiles that shouldn't flip
/// </summary>
public class SoundWaveProjectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float deactivationTime = 3f;
    [SerializeField] private int damageValue = 10;
    
    private Rigidbody2D rb;
    private int moveDirection;
    private ObjectPoolAdvanced objectPool;
    
    [Header("Cinemachine")]                               
    [SerializeField] private float camShakeIntensity = 4f;
    [SerializeField] private float camShakeDuration = .1f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        objectPool = FindObjectOfType<ObjectPoolAdvanced>();
    }

    private void OnEnable()
    {
        moveDirection = UnityEngine.Random.Range(0,2);
        if (moveDirection == 0)
        {
            moveDirection = -1;
        }
        Invoke("Deactivation", deactivationTime);

    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveDirection,0 ) * moveSpeed;
    }

    private void Deactivation()
    {
        if (objectPool != null)
        {
            objectPool.ReturnGameObject(gameObject);
        }
        
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
            Deactivation();
        }
    }
}
