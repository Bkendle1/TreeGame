using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.InputSystem.Processors;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Enemy : PoolObject
{
    [SerializeField] public EnemyProp enemyProperties;
    
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
    [SerializeField] public HealthBar healthBar;
    public int currentHealth;
    
    
    private Coroutine resetStunCoroutine;
    public bool isStunned;

    [Header("Experience Points")]
    [SerializeField] private GameObject expAcorn;
    [Tooltip("Minimum number of exp dropped.(Inclusive)")]
    [SerializeField] private int expMin = 3;
    [Tooltip("Maximum number of exp dropped.(Inclusive)")]
    [SerializeField] private float expMax = 10f;

    [Header("Miscellaneous")]
    [SerializeField] private BoxCollider2D ColliderBlocker;
    [Tooltip("Name of the enemy, in the Resources folder, that you want to respawn.")]
    [SerializeField] private string enemyName;
    private AudioSource _audioSource;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private Pooling deathEffectPool;
    private Pooling expPool;
    private UnityEngine.Object enemyRef;
    private Vector3 startPos;

    
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        originalColor = spriteRenderer.color;
        enemyRef = Resources.Load(enemyName);
        startPos = transform.position;
        
        //set up health bar's max hp
        healthBar.SetMaxHealth(enemyProperties.GetHealthAmount);
        
        //Ignore collisions this gameObject's box collider and child box collider (CollisionBlocker) that has 
        //a kinematic rigid body preventing the enemy from pushing the player and vice versa
        Physics2D.IgnoreCollision(boxCollider, ColliderBlocker, true);
        
        SetupEnemySettings();
        
        //GameManager.Instance.LiveLost += RespawnEnemy;

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
        //TODO the enemy gets stunned if they're hit once but the stun doesn't reset if they're hit successively

    }
    
    public void TakeDamage(int damage)
    {
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

        if (!isStunned)
        {
            isStunned = true;

            if (resetStunCoroutine != null)
            {
                StopCoroutine(resetStunCoroutine);
            }

            resetStunCoroutine = StartCoroutine(ResetStun());
        }
        
    }

    private IEnumerator ResetStun()
    {
        yield return new WaitForSeconds(enemyProperties.GetStunDuration);
        isStunned = false; 
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

        for (int i = 0; i < Random.Range(expMin,expMax); i++)
        {
            expPool.Get(transform.position, Quaternion.identity);
        }
        
        //Disable health bar on death
        healthBar.gameObject.SetActive(false);

        //Die animation
        anim.SetBool("isDead", true);

        //Disable box collider but also change rigidbody type
        //to static so the enemy doesn't fall 
        //through the ground kinematic work also
        boxCollider.enabled = false;
        rb.bodyType = RigidbodyType2D.Static;
        
        deathEffectPool.Get(transform.localPosition, transform.localRotation);

        //Disable child's collision blocker collider 
        ColliderBlocker.enabled = false;
        
        //Invoke("RespawnEnemy", 1f);
    }
    
    //TODO enemy still fades away sometimes
    private void RespawnEnemy()
    {
        //Respawn enemy
        GameObject enemyClone = (GameObject)Instantiate(enemyRef);
        enemyClone.transform.position = startPos;
        Destroy(gameObject);
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
            //Destroy(gameObject);
            spriteRenderer.enabled = false;
            
            enemyProperties.GetDeathEffect.SetActive(false);
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
        if (!PoolManager.DoesPoolExist(enemyProperties.GetDeathEffect.gameObject.name))
        {
            PoolManager.CreatePool(enemyProperties.GetDeathEffect.gameObject.name, enemyProperties.GetDeathEffect, 10);
        }

        if (!PoolManager.DoesPoolExist(expAcorn.gameObject.name))
        {
            PoolManager.CreatePool(expAcorn.gameObject.name, expAcorn, 30);
        }

        expPool = PoolManager.GetPool(expAcorn.gameObject.name);
        deathEffectPool = PoolManager.GetPool(enemyProperties.GetDeathEffect.gameObject.name);
    }
    
    private void OnDestroy()
    {
        PoolManager.DeletePool(enemyProperties.GetDeathEffect.gameObject.name);
        PoolManager.DeletePool(expAcorn.gameObject.name);
    }
}
