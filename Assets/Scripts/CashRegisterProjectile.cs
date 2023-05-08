using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashRegisterProjectile : MonoBehaviour
{
    private Vector2 moveDirection;
    [SerializeField] private float moveSpeed = 10f;
    
    private ObjectPoolAdvanced objectPool;
    [SerializeField] private float deactivationTime = 3f;
    [SerializeField] private int damageValue = 10;
    
    [Header("Cinemachine")]                               
    [SerializeField] private float camShakeIntensity = 4f;
    [SerializeField] private float camShakeDuration = .1f;
    
    private void Start()
    {
        objectPool = FindObjectOfType<ObjectPoolAdvanced>();
    }

    private void OnEnable()
    {
        Invoke("Deactivate", deactivationTime);
    }

    private void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    public void SetMoveDirection(Vector2 dir)
    {
        moveDirection = dir;
    }
    
    private void Deactivate()
    {
        objectPool.ReturnGameObject(gameObject);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
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
