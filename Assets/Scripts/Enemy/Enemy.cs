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

public class Enemy : MonoBehaviour
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
    [Tooltip("How long until the enemy respawns")]
    [SerializeField] private float respawnTime = 60f;
    
    private AudioSource _audioSource;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private ObjectPoolAdvanced deathEffectPool;
    private ObjectPoolAdvanced expPool;
    private UnityEngine.Object enemyRef;
    private Vector3 startPos;

    [SerializeField] private AudioClip[] grunts;
    [SerializeField] private AudioClip deathSFX;
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
        expPool = FindObjectOfType<ObjectPoolAdvanced>();
        deathEffectPool = FindObjectOfType<ObjectPoolAdvanced>();
        //set up health bar's max hp
        healthBar.SetMaxHealth(enemyProperties.GetHealthAmount);
        
        //Ignore collisions this gameObject's box collider and child box collider (CollisionBlocker) that has 
        //a kinematic rigid body preventing the enemy from pushing the player and vice versa
        if (ColliderBlocker != null)
        {
            Physics2D.IgnoreCollision(boxCollider, ColliderBlocker, true);
        }
        
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

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth > 0)
        {
            int randInt = Random.Range(0, grunts.Length);
            Debug.Log(randInt);
            if (_audioSource != null)
            {
                _audioSource.PlayOneShot(grunts[randInt]);
                _audioSource.pitch = Random.Range(1, 1.2f);
            }
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
            //expPool.Get(new Vector2(transform.position.x, transform.position.y + 1), Quaternion.identity);
            GameObject acorn = expPool.GetObject(expAcorn);
            acorn.transform.rotation = transform.rotation;
            acorn.transform.position = new Vector2(transform.position.x, transform.position.y + 1);
        }
        
        //play sfx
        _audioSource.PlayOneShot(deathSFX);
        
        //Disable health bar on death
        healthBar.gameObject.SetActive(false);

        //Die animation
        anim.SetBool("isDead", true);

        //Disable box collider but also change rigidbody type
        //to static so the enemy doesn't fall 
        //through the ground kinematic work also
        boxCollider.enabled = false;
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }

        GameObject deathEffect = deathEffectPool.GetObject(enemyProperties.GetDeathEffect);
        deathEffect.transform.rotation = transform.rotation;
        deathEffect.transform.position = transform.position;

        //Disable child's collision blocker collider 
        if (ColliderBlocker != null)
        {
            ColliderBlocker.enabled = false;
        }
        
        //if the MalWart boss is defeated
        if (gameObject.name == "MalWartBoss")
        {
            CinemachineManager.Instance.SwitchPriority();
        }

        if (gameObject.name != "MalWartBoss")
        {
            Invoke("RespawnEnemy", respawnTime);
        }
    }
    
    //TODO enemy still fades away sometimes
    private void RespawnEnemy()
    {
        Debug.Log("In respawn enemy");
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
            //if malwart is defeated, set game object inactive so the walls go down as well
            if (gameObject.name == "MalWartBoss")
            {
                gameObject.SetActive(false);
            }
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
    }
}
