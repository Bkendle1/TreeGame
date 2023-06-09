using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = System.Numerics.Vector3;

public class PlayerAttack : MonoBehaviour
{
    private Animator weaponAnimator;
    private WeaponProp playerWeaponProperties;
    private SpriteRenderer _weaponSpriteRenderer;
    private AudioSource audioSource;
    private bool canAttack = true;
    private bool attackInput;
    
    private Animator anim;
    private PlayerControls controls;
    [SerializeField] private Transform shootPos;
    [SerializeField] private AudioClip swingSfx;
    private GameObject projectile;
    private ObjectPoolAdvanced projectilePool;
    
    void Awake()
    {
        controls = new PlayerControls();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        SetupWeapon();
        projectilePool = FindObjectOfType<ObjectPoolAdvanced>();
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
        //player can't attack if dialogue is playing or the game is paused
        if (DialogueManager.Instance.dialogueIsPlaying)
        {
            return;
        }
        

        if (attackInput && canAttack)
        {
            Attack();
            canAttack = false;
            Invoke("ResetAttack", playerWeaponProperties.GetTimeBtwAttacks);
        }
    }
    
    private void Attack()
    {
        //if game is paused, don't attack
        if(PauseMenu.isPaused)
        {
            return;
        }
        
        audioSource.PlayOneShot(swingSfx);
        
        //Setup weapon in case a new weapon was equipped
        SetupWeapon();
        
        //check if the weapon has been upgraded
        if (_weaponSpriteRenderer.sprite == playerWeaponProperties.GetUpgradedWeaponSprite)
        {
            GameObject obj = projectilePool.GetObject(projectile);
            obj.transform.rotation = shootPos.rotation;
            obj.transform.position = shootPos.position;
        }

        //trigger player attack animation
        anim.SetTrigger("Attack");
        
        //Trigger weapon swing animation
        weaponAnimator.SetTrigger("Attack");
        weaponAnimator.speed = playerWeaponProperties.GetAttackSpeed;
        
        //Enable weapon sprite
        _weaponSpriteRenderer.enabled = enabled;

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
        _weaponSpriteRenderer = GetComponentInChildren<Weapon>().GetComponent<SpriteRenderer>();
        projectile = playerWeaponProperties.GetProjectile;

    }
}
