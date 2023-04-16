using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = System.Numerics.Vector3;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Animator weaponAnimator;
    private WeaponProp playerWeaponProperties;
    private SpriteRenderer _weaponSprite;
    
    private bool canAttack = true;
    private bool attackInput;
    
    private Animator anim;
    private PlayerControls controls;
    
    void Awake()
    {
        controls = new PlayerControls();
        anim = GetComponent<Animator>();
        playerWeaponProperties = GetComponentInChildren<Weapon>().weaponProperties;
        _weaponSprite = GetComponentInChildren<Weapon>().GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _weaponSprite.sprite = playerWeaponProperties.GetWeaponSprite;
    }

    private void OnEnable()
    {
        controls.Player.PrimaryFire.performed += OnPrimaryFirePerformed;
        controls.Player.PrimaryFire.canceled += OnPrimaryFireCanceled;
        
        controls.Player.SecondaryFire.performed += OnSecondaryFirePerformed;
        
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Player.PrimaryFire.performed -= OnPrimaryFirePerformed;
        controls.Player.PrimaryFire.canceled -= OnPrimaryFireCanceled;
        
        controls.Player.SecondaryFire.performed -= OnSecondaryFirePerformed;
        
        controls.Disable();
    }

    
    private void OnPrimaryFirePerformed(InputAction.CallbackContext context)
    {
        attackInput = true;
    }

    private void OnPrimaryFireCanceled(InputAction.CallbackContext context)
    {
        attackInput = false;
    }
    
    private void OnSecondaryFirePerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Secondary fire");
    }

    void Update()
    {
        if (attackInput && canAttack)
        {
            Attack();
            canAttack = false;
            
            Invoke("ResetAttack", playerWeaponProperties.GetTimeBtwAttacks);
            
        }
    }
    
    private void Attack()
    {
        
        anim.SetTrigger("Attack");
        
        //Enable collider so weapon can attack enemies
        //_weaponSprite.GetComponent<CircleCollider2D>().enabled = true;
        
        //Trigger weapon swing animation
        weaponAnimator.SetTrigger("Attack");
        weaponAnimator.speed = playerWeaponProperties.GetAttackSpeed;
        
        //Enable weapon sprite
        _weaponSprite.enabled = enabled;
        

        //Detect objects in enemy layer
        //Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, playerWeaponProperties.GetAttackRange, enemyLayer);

        //Damage enemies hit
        // foreach (Collider2D enemy in hitEnemies)
        // {
        //     CinemachineShake.Instance.ShakeCamera(camShakeIntensity,camShakeDuration);
        //     Debug.Log("Hit: " + enemy.name);
        //     enemy.GetComponent<Enemy>().TakeDamage(playerWeaponProperties.GetAttackDamage);
        // }
    }
    
    //allow player to attack again
    private void ResetAttack()
    {
        canAttack = true;
    }
    
}
