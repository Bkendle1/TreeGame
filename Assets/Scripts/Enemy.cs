using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyProp enemyProperties;
    [SerializeField] private float _fadeSpeed = 2f;
    [SerializeField] private float timeBeforeFade = 2f;
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

    private void Update()
    {
        if (currentHealth <= 0)
        {
            StartCoroutine("FadeOut");
        }
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

        Instantiate(enemyProperties.GetDeathEffect, transform.localPosition, transform.localRotation);
        
        //Ignore collision between the player's layer (8) and this gameObject's layer (gameObject.layer)
        Physics2D.IgnoreLayerCollision(8, gameObject.layer);
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
    private void SetupEnemySettings()
    {
        spriteRenderer.sprite = enemyProperties.GetEnemySprite;
        currentHealth = enemyProperties.GetHealthAmount;
    }
}
