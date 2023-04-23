using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyProp enemyProperties;
    
    [Header("Fade")]
    [Tooltip("How fast the enemy fades away after death.")]
    [SerializeField] private float _fadeSpeed = 2f;
    [Tooltip("How long to wait before enemy fades away.")]
    [SerializeField] private float timeBeforeFade = 2f;
    
    
    [Header("Hurt Properties")]
    [Tooltip("How long enemy flashes red.")]
    [SerializeField] private float flashDuration;
    private Color originalColor;
    private Coroutine flashRoutine;

    [Header("Cinemachine")]                               
    [SerializeField] private float camShakeIntensity = 4f;
    [SerializeField] private float camShakeDuration = .1f;

    [Header("Health")] 
    [SerializeField] private HealthBar healthBar;
    private int currentHealth;
    
    [Header("Patrol")] 
    [SerializeField] private Transform castPos;
    [Tooltip("How far the enemy can see.")]
    [SerializeField] private float baseCastDistance;
    private bool isFacingRight = true;

    private bool isStunned;
    
    [SerializeField] private BoxCollider2D ColliderBlocker;
    private AudioSource _audioSource;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    
    
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        originalColor = spriteRenderer.color;
        
        
        //set up health bar's max hp
        healthBar.SetMaxHealth(enemyProperties.GetHealthAmount);
        
        //Ignore collisions this gameObject's box collider and child box collider (CollisionBlocker) that has 
        //a kinematic rigid body preventing the enemy from pushing the player and vice versa
        Physics2D.IgnoreCollision(boxCollider, ColliderBlocker, true);
        
        SetupEnemySettings();
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            StartCoroutine("FadeOut");
        }
    }

    private void FixedUpdate()
    {
        float patrolSpeed = enemyProperties.GetPatrolSpeed;

        if (!isFacingRight)
        {
            patrolSpeed = -enemyProperties.GetPatrolSpeed;
        }

        if (rb.velocity.x != 0)
        {
            anim.SetBool("isPatrolling", true);
        }
        else
        {
            anim.SetBool("isPatrolling", false);
        }
        //if enemy isn't stunned move
        if (!isStunned)
        {
            //move enemy
            rb.velocity = new Vector2(patrolSpeed, rb.velocity.y);
        }
        
        if (IsHittingWall() || IsNearEdge())
        {
            Flip();
        }
        
    }

    private IEnumerator ResetStun()
    {
        yield return new WaitForSeconds(enemyProperties.GetStunDuration);
        isStunned = false; 
    }
    
    public void TakeDamage(int damage)
    {
        isStunned = true;
        StartCoroutine(ResetStun());
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
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
        //Disable health bar on death
        healthBar.gameObject.SetActive(false);

        //Die animation
        anim.SetBool("isDead", true);

        //Disable box collider but also change rigidbody type
        //to static so the enemy doesn't fall 
        //through the ground kinematic work also
        boxCollider.enabled = false;
        rb.bodyType = RigidbodyType2D.Static;
        
        Instantiate(enemyProperties.GetDeathEffect, transform.localPosition, transform.localRotation);

        //Disable child's collision blocker collider 
        ColliderBlocker.enabled = false;
    }

    private bool IsHittingWall()
    {
        bool hittingWall  = false;
        float castDistance = baseCastDistance;
        
        //define cast distance for left and right
        if (!isFacingRight)
        {
            castDistance = -baseCastDistance;
        }
        
        //determine the end point based on the cast distance
        Vector3 targetPosition = castPos.position;
        targetPosition.x += castDistance;

        //see line cast
        Debug.DrawLine(castPos.position, targetPosition, Color.cyan);
        
        //check if linecast hits an object in the ground layer
        if (Physics2D.Linecast(castPos.position, targetPosition, 1 << LayerMask.NameToLayer("Ground")))
        {
            hittingWall = true;
        }
        return hittingWall;
    }
    
    private bool IsNearEdge()
    {
        bool nearEdge  = true;
        
        float castDistance = baseCastDistance;
        
        //determine the end point based on the cast distance
        Vector3 targetPosition = castPos.position;
        targetPosition.y -= castDistance;

        //see line cast
        Debug.DrawLine(castPos.position, targetPosition, Color.magenta);
        
        //check if linecast doesn't an object in the ground layer, meaning there's an edge
        if (Physics2D.Linecast(castPos.position, targetPosition, 1 << LayerMask.NameToLayer("Ground")))
        {
            nearEdge = false;
        }
        
        return nearEdge;
    }
    
    private void Flip()
    {
        isFacingRight = !isFacingRight;
     
        // flip game object sprite
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;

        // flip health bar sprite        
        Vector3 healthBarScale = healthBar.transform.localScale;
        healthBarScale.x *= -1f;
        healthBar.transform.localScale = healthBarScale;
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
            playerMovement.KBTimer = playerMovement.KBDuration;
            col.gameObject.GetComponent<PlayerHealth>().TakeDamage(enemyProperties.GetAttackDamage);
            
            
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
