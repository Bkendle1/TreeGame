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
        SetupWeapon();
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Enemy>() != null)
        {
            Debug.Log("Enemy hit");
            other.gameObject.GetComponent<Enemy>().TakeDamage(weaponProperties.GetAttackDamage);
            CinemachineShake.Instance.ShakeCamera(camShakeIntensity,camShakeDuration);
            Debug.Log(transform.parent.localScale);
            //apply knockback force in the direction player is facing
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(transform.parent.localScale.x,0) * weaponProperties.GetKnockBackPower, ForceMode2D.Impulse);
            //other.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.parent.localScale.x,0) * weaponProperties.GetKnockBackPower;
        }
    }

    private void SetupWeapon()
    {
        weaponRadius.radius = weaponProperties.GetAttackRange;
    }
    
}
