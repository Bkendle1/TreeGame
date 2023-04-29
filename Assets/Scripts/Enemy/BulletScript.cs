using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] private float force;
    [Tooltip("How much damage to deal to player.")]
    [SerializeField] private int damageValue = 10;
    [Tooltip("How long before the bullet destroys itself.")]
    [SerializeField] private float destructionTimer = 3f;
    private Transform player;
    private Rigidbody2D rb;
    private float timer;
    
    [Header("Cinemachine")]                               
    [SerializeField] private float camShakeIntensity = 4f;
    [SerializeField] private float camShakeDuration = .1f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        
        Vector2 direction = player.transform.position - transform.position;
        rb.velocity = new Vector2(direction.x, direction.y).normalized * force;
        
        float rotationAngle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotationAngle + 90);
        
    }


    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > destructionTimer)
        {
            gameObject.SetActive(false);
            timer = 0;
        }
        
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
            gameObject.SetActive(false);
        }
    }
}
