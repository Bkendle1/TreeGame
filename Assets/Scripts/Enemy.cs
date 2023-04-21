using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyProp enemyProperties;
    [SerializeField] private float _fadeSpeed = 2f;
    [SerializeField] private float timeBeforeFade = 2f;
    
    [SerializeField] private BoxCollider2D ColliderBlocker;
    
    [Header("Hit Properties")]
    [SerializeField] private float flashDuration;
    private Color originalColor;
    private Coroutine flashRoutine;

    [Header("Cinemachine")]                               
    [SerializeField] private float camShakeIntensity = 4f;
    [SerializeField] private float camShakeDuration = .1f;
    
    private AudioSource _audioSource;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    
    private int currentHealth;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        originalColor = spriteRenderer.color;
        
        //Ignore collisions this gameObject's box collider and child box collider (CollisionBlocker) that has 
        //a kinematic rigid body preventing the enemy from pushing the player and vice versa
        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), ColliderBlocker, true);
        
        SetupEnemySettings();
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            StartCoroutine("FadeOut");
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth > 0)
        {
            Debug.Log("Been hit for: " + damage + " damage.");

            // Flash Effect
            Flash();

            // Play hurt animation
            anim.SetTrigger("Hurt");
        }
        else
        {
            Die();
            Flash();
        }
    }

    private void Flash()
    {
        // If the flashRoutine is not null, then its currently running 
        // so if flashRoutine is called again, we'll stop the coroutine that's
        // still running so only one flashRoutine is running at a time
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }
        flashRoutine = StartCoroutine("FlashRoutine");
    }
    
    private void Die()
    {
        
        //Die animation
        anim.SetBool("isDead", true);

        Instantiate(enemyProperties.GetDeathEffect, transform.localPosition, transform.localRotation);
        
        //Ignore collision between the player's layer (8) and this gameObject's layer (gameObject.layer)
        Physics2D.IgnoreLayerCollision(8, gameObject.layer);
        //Disable child's collision blocker collider 
        ColliderBlocker.enabled = false;
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(timeBeforeFade);
        
        Color newSpriteColor = spriteRenderer.color;

        float fadeAmount = newSpriteColor.a - (_fadeSpeed * Time.deltaTime);

        newSpriteColor = new Color(newSpriteColor.r, newSpriteColor.b, newSpriteColor.g, fadeAmount);

        spriteRenderer.color = newSpriteColor;

        if (newSpriteColor.a <= 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator FlashRoutine()
    {
        // Swap to flash material.
        spriteRenderer.color= Color.red;
        
        // Pause the execution of this function for "duration" seconds.
        yield return new WaitForSeconds(flashDuration);
        
        // After the pause, swap back to the original material.
        spriteRenderer.color = originalColor;
        
        // Set the routine to null, signaling that it's finished.
        flashRoutine = null;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            var playerMovement = col.gameObject.GetComponent<Movement>();
            Debug.Log("Collided with player");
            playerMovement.KBTimer = col.gameObject.GetComponent<Movement>().KBDuration;
            playerMovement.TakeDamage(enemyProperties.GetAttackDamage);
            
            
            if (col.transform.position.x <= transform.position.x)
            {
                playerMovement.KnockFromRight = true;
            }
            else
            {
                playerMovement.KnockFromRight = false;
            }

            CinemachineShake.Instance.ShakeCamera(camShakeIntensity,camShakeDuration);
        }
    }
    

    private void SetupEnemySettings()
    {
        spriteRenderer.sprite = enemyProperties.GetEnemySprite;
        currentHealth = enemyProperties.GetHealthAmount;
    }
}
