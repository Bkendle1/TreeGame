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
    [SerializeField] private Transform weaponTransform;
    
    [Header("Cinemachine")] 
    [SerializeField] private float camShakeIntensity = 4f;
    [SerializeField] private float camShakeDuration = .1f;
    
    private bool canAttack = true;
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

        //weaponTransform.transform.rotation.z = Mathf.Lerp(100, -45, playerWeaponProperties.GetAttackSpeed);
    }

    
    private void Attack()
    {
        anim.SetTrigger("Attack");
        
        //Weapon swing animation
        _weaponSprite.enabled = enabled;
        
        
        
        //Detect objects in enemy layer
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, playerWeaponProperties.GetAttackRange, enemyLayer);
        
        //Damage enemies hit
        foreach (Collider2D enemy in hitEnemies)
        {
            CinemachineShake.Instance.ShakeCamera(camShakeIntensity,camShakeDuration);
            Debug.Log("Hit: " + enemy.name);
            enemy.GetComponent<Enemy>().TakeDamage(playerWeaponProperties.GetAttackDamage);
        }

    }
    private void ResetAttack()
    {
        canAttack = true;
        // _weaponSprite.enabled = false;

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
