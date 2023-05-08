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
    private AudioSource audioSource;
    
    private void Start()
    {
        SetupWeapon();
        audioSource = GetComponent<AudioSource>();
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
            if (other.gameObject.GetComponent<Rigidbody2D>() != null)
            {
                other.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(transform.parent.parent.localScale.x,0) * weaponProperties.GetKnockBackPower, ForceMode2D.Impulse);
            }
            audioSource.PlayOneShot(weaponProperties.GetHitSFX);
            audioSource.pitch = UnityEngine.Random.Range(1f, 1.2f);

        }
    }

    private void SetupWeapon()
    {
        transform.localScale = new Vector3(weaponProperties.GetAttackRange,weaponProperties.GetAttackRange,weaponProperties.GetAttackRange);
    }
    
}
