using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killzone : MonoBehaviour
{
    [SerializeField] private int damageValue = 10;
    [Tooltip("Time to wait before hurting the player again")]
    [SerializeField] private float timeBtwDamage = 1f;
    
    [Header("Cinemachine")]                               
    [SerializeField] private float camShakeIntensity = 4f;
    [SerializeField] private float camShakeDuration = .1f;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.GetComponent<PlayerHealth>() != null)
        {
            var playerMovement = col.gameObject.GetComponent<Movement>();
            col.gameObject.GetComponent<PlayerHealth>().TakeDamage(damageValue);
            playerMovement.KBTimer = playerMovement.KBDuration;
            CinemachineShake.Instance.ShakeCamera(camShakeIntensity,camShakeDuration);
        }
    }
    
}
