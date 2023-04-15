using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] public WeaponProp weaponProperties;
    private CircleCollider2D weaponRadius;

    [Header("Cinemachine")] 
    [SerializeField] private float camShakeIntensity = 4f;
    [SerializeField] private float camShakeDuration = .1f;

    
    private void Start()
    {
        weaponRadius = GetComponent<CircleCollider2D>();
        weaponRadius.radius = weaponProperties.GetAttackRange;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Enemy>() != null)
        {
            Debug.Log("Enemy hit");
            other.gameObject.GetComponent<Enemy>().TakeDamage(weaponProperties.GetAttackDamage);
            CinemachineShake.Instance.ShakeCamera(camShakeIntensity,camShakeDuration);
        }
    }
    
}
