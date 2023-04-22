using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatforms : MonoBehaviour
{
    private Rigidbody2D rb;
    [Tooltip("How long to wait before the platform falls after the player steps on it.")]
    [SerializeField] private float dropTime = 1f;
    [Tooltip("How long to wait until platform returns after it fades away.")]
    [SerializeField] private float returnTime = 1f;
    [SerializeField] private float timeBeforeFade = .2f;
    [SerializeField] private float _fadeSpeed = 2f;
    private Vector2 initialPosition;
    private SpriteRenderer spriteRenderer;
    private bool startFade;
    private BoxCollider2D boxCollider;
    private bool platformReturning;
    private Color originalColor;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialPosition = transform.position;
        boxCollider = GetComponent<BoxCollider2D>();
        originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (startFade)
        {
            StartCoroutine(FadeOut());
        }

        if (platformReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, initialPosition, 20f * Time.deltaTime);
            FadeIn();
        }

        if (transform.position.y == initialPosition.y)
        {
            platformReturning = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DropPlatform());
        }
    }

    private IEnumerator DropPlatform()
    {
        yield return new WaitForSeconds(dropTime);
        rb.isKinematic = false;
        startFade = true;
        yield return new WaitForSeconds(returnTime);
        GetPlatformBack();
    }

    private void GetPlatformBack()
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        platformReturning = true;
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
            boxCollider.enabled = false;
            startFade = false;
            StopCoroutine(FadeOut());
        }
    }
    
    private void FadeIn()
    {
        Color newSpriteColor = spriteRenderer.color;

        float fadeAmount = newSpriteColor.a + (_fadeSpeed * Time.deltaTime);

        newSpriteColor = new Color(newSpriteColor.r, newSpriteColor.b, newSpriteColor.g, fadeAmount);

        spriteRenderer.color = newSpriteColor;
        
        if (newSpriteColor.a >= 1)
        {
            Debug.Log("Fade in done");
            boxCollider.enabled = true;
        }

    }
}
