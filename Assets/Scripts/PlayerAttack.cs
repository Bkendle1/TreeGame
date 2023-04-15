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
    [SerializeField] private SpriteRenderer _weaponSprite;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] WeaponProp playerWeaponProperties;

    private float timeBtwAttackTimer;
    private bool attackInput;
    
    private Animator anim;
    private PlayerControls controls;
    
    void Awake()
    {
        controls = new PlayerControls();
        anim = GetComponent<Animator>();
        
    }

    private void Start()
    {
        _weaponSprite.enabled = enabled;
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
        if (timeBtwAttackTimer <= 0)
        {
            timeBtwAttackTimer = playerWeaponProperties.GetAttackSpeed;
            if (attackInput)
            {
                Attack();
            }
        }
        else
        {
            timeBtwAttackTimer -= Time.deltaTime;
        }
    }

    private void Attack()
    {
        anim.SetTrigger("Attack");
        
        //Detect objects in enemy layer
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, playerWeaponProperties.GetAttackRange, enemyLayer);
        
        //Damage enemies hit
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit: " + enemy.name);
            enemy.GetComponent<Enemy>().TakeDamage(playerWeaponProperties.GetAttackDamage);
        }
    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null)
        {
            return;
        }
        
        Gizmos.DrawWireSphere(attackPoint.position,playerWeaponProperties.GetAttackRange);
    }
}
