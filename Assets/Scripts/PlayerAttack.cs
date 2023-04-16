using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using Vector2 = UnityEngine.Vector2;
using Vector3 = System.Numerics.Vector3;

public class PlayerAttack : MonoBehaviour
{
    private Animator weaponAnimator;
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

        SetupWeapon();
        Debug.Log(weaponAnimator);
    }
    
    private void Attack()
    {
        
        anim.SetTrigger("Attack");
        
        //Trigger weapon swing animation
        weaponAnimator.SetTrigger("Attack");
        weaponAnimator.speed = playerWeaponProperties.GetAttackSpeed;
        
        //Enable weapon sprite
        _weaponSprite.enabled = enabled;
    }
    
    //allow player to attack again
    private void ResetAttack()
    {
        canAttack = true;
    }

    //for now its being called in the update, but it needs to be called wherever the player changes weapons
    private void SetupWeapon()
    {
        weaponAnimator = GetComponentInChildren<Weapon>().GetComponent<Animator>();
        playerWeaponProperties = GetComponentInChildren<Weapon>().weaponProperties;
        _weaponSprite = GetComponentInChildren<Weapon>().GetComponent<SpriteRenderer>();
    }
    
}
