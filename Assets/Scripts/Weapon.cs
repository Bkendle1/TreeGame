using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] public WeaponProp weaponProperties;

    [Header("Cinemachine")] 
    [SerializeField] private float camShakeIntensity = 4f;
    [SerializeField] private float camShakeDuration = .1f;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Start()
    {
        SetupWeapon();
    }

    public void UpgradeWeapon()
    {
        spriteRenderer.sprite = weaponProperties.GetUpgradedWeaponSprite;
    }    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Enemy>() != null)
        {
            other.gameObject.GetComponent<Enemy>().TakeDamage(weaponProperties.GetAttackDamage);
            CinemachineShake.Instance.ShakeCamera(camShakeIntensity,camShakeDuration);
            //apply knockback force in the direction player is facing
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(transform.parent.parent.localScale.x,0) * weaponProperties.GetKnockBackPower, ForceMode2D.Impulse);
            //other.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.parent.parent.localScale.x,0) * weaponProperties.GetKnockBackPower;
        }
    }

    private void SetupWeapon()
    {
        transform.localScale = new Vector3(weaponProperties.GetAttackRange,weaponProperties.GetAttackRange,weaponProperties.GetAttackRange);
    }
    
}
