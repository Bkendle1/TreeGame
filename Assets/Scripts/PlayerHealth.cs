using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    [Header("Health")] 
    [Tooltip("Player's max health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private HealthBar healthBar;
    private int currentHealth;
    
    [Header("Hurt")]
    [Tooltip("How long the player will be in their hurt color")]
    [SerializeField] private float hurtDuration = 1f;
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
    }

    public void TakeDamage(int value)
    {
        //play hurt sfx
        
        
        StartCoroutine("Blink");
        
        anim.SetTrigger("Hurt");
        
        currentHealth -= value;
        
        //update health bar
        healthBar.SetHealth(currentHealth);
        
        if (currentHealth <= 0)
        {
            //Death
        }
        Debug.Log("Mabel has taken " + value + " points of damage!");
        Debug.Log("Mabel has: " + currentHealth + " points of HP left.");
    }
        
    public void Heal(int value)
    {
        //play hurt sfx

        currentHealth += value;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        } 
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
}
