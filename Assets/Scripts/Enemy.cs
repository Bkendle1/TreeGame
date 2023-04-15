using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyProp enemyProperties;
    private AudioSource _audioSource;
    private SpriteRenderer spriteRenderer;
    private Animator anim;
        
    private int currentHealth;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        
        SetupEnemySettings();
    }

    public void TakeDamage(int damage)
    {
        
        if (currentHealth > 0)
        {
            currentHealth -= damage;
            Debug.Log("Been hit for: " + damage + " damage.");
            // play hurt animation
            anim.SetTrigger("Hurt");
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        //Die animation
        anim.SetBool("isDead", true);
        //Ignore collision between the player's layer (8) and this gameObject's layer (gameObject.layer)
        Physics2D.IgnoreLayerCollision(8, gameObject.layer);

    }
    
    private void SetupEnemySettings()
    {
        spriteRenderer.sprite = enemyProperties.GetEnemySprite;
        currentHealth = enemyProperties.GetHealthAmount;
    }
}
