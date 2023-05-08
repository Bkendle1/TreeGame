using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{

    [Header("Health")] 
    [Tooltip("Player's max health")]
    [SerializeField] public int maxHealth = 100;
    [SerializeField] private HealthBar healthBar;
    public int currentHealth { get; private set; }
    
    [Header("Hurt")]
    [Tooltip("How long the player will be in their hurt color")]
    [SerializeField] private float hurtDuration = 1f;
    [Tooltip("How long player is invincible for make sure hurtDuration and hurt animation all match up.")]
    [SerializeField] private float iFrameDuration = 1f;
    private Color originalColor;

    
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        originalColor = spriteRenderer.color;
        currentHealth = maxHealth;
        //set up health bar's max hp
        healthBar.SetMaxHealth(maxHealth);
        gameObject.transform.position = GameManager.Instance.lastCheckPointPos;
        
        //make sure collisions between player (8) and enemies (7) are active
        //because take damage doesn't re-enable them if the player dies
        Physics2D.IgnoreLayerCollision(7, 8, false);
    }

    private void Update()
    {
        if (DialogueManager.Instance.dialogueIsPlaying)
        {
            healthBar.gameObject.SetActive(false);
        }
        else
        {
            healthBar.gameObject.SetActive(true);
        }
        
    }

    public void TakeDamage(int value)
    {
        //TODO play hurt sfx

        //if the damage is more than the player's max health, set it to 0 
        if (value >= maxHealth)
        {
            currentHealth = 0;
        }
        
        //start blinking
        StartCoroutine(Blink());
        
        //start iFrames
        StartCoroutine(Invulnerable());
        
        anim.SetTrigger("Hurt");
        
        currentHealth -= value;
        
        //update health bar
        healthBar.SetHealth(currentHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }

    }

    private void Die()
    {
        Debug.Log("YOU DIED! GAME OVER!");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameManager.Instance.UpdateLives(-1);
        //Respawn player at new position
        gameObject.transform.position = GameManager.Instance.lastCheckPointPos;
        if (FindObjectOfType<BossCamConfinerTrigger>().isFightingBoss)
        {
            Debug.Log("Start fighting");
            CinemachineManager.Instance.SwitchPriority();
        }
    }
    
    public void Heal(int value)
    {
        //TODO play heal sfx
        
        //heal player
        currentHealth += value;
        
        //update health bar
        healthBar.SetHealth(currentHealth);
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        } 
    }

    public void SetMaxHealth(int value)
    {
        //increase max health
        maxHealth += value;
        //update health UI so max value is new max health
        healthBar.SetMaxHealth(maxHealth);
        //restore player's health to new max health
        currentHealth = maxHealth;
    }
    
    //Blinking effect for when player is hit
    private IEnumerator Blink()
    {
    
        // Swap to hurt color
        spriteRenderer.color = Color.red;
            
        // Pause the execution of this function for "duration" seconds.
        yield return new WaitForSeconds(hurtDuration);
            
        // After the pause, swap back to the original material.
        spriteRenderer.color = originalColor;
    }

    private IEnumerator Invulnerable()
    {
        Physics2D.IgnoreLayerCollision(7, 8, true);
        yield return new WaitForSeconds(iFrameDuration);
        Physics2D.IgnoreLayerCollision(7, 8, false);
    }
    
}
