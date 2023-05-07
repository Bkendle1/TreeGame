using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [Tooltip("Speed of projectile.")]
    [SerializeField] private float projectileSpeed;
    private Rigidbody2D rb;
    [SerializeField] private GameObject ImpactEffect;
    [SerializeField] private int projectileDamage;
    [Tooltip("How long until projectile is destroyed if it doesn't collide with anything.")]
    [SerializeField] private float destructionTimer = 3f;

    [Header("Cinemachine")]                               
    [SerializeField] private float camShakeIntensity = 4f;
    [SerializeField] private float camShakeDuration = .1f;
    private Transform player;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Movement>().transform;
        rb.velocity = new Vector2(player.localScale.x, rb.velocity.y).normalized * projectileSpeed;
        Invoke("SelfDestroy", destructionTimer);
        
    }

    private void SelfDestroy()
    {
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        //Destroy game object if it collides with enemy or objects in the ground layer
        if (col.GetComponent<Enemy>() != null)
        {
            //TODO instantiate impact effect
            col.GetComponent<Enemy>().TakeDamage(projectileDamage);
            CinemachineShake.Instance.ShakeCamera(camShakeIntensity,camShakeDuration);
            Destroy(gameObject);
        } else if (col.gameObject.layer == 6)
        {
            Destroy(gameObject);
        }
    }
}
