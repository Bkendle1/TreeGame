using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformFall : MonoBehaviour
{
    [Tooltip("How long to wait before the platform falls.")]
    [SerializeField] private float dropTime = 2f;
    [Tooltip("How long to wait before the platform returns.")]
    [SerializeField] private float returnTime = 2f;
    
    
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;
    private Vector2 initialPosition;
    private Animator anim;
    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInParent<Animator>();
        initialPosition = transform.position;
    }


    private void OnCollisionEnter2D(Collision2D col)
    {
        //check if collision happened on top of platform
        if (col.contacts[0].normal == Vector2.down)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                StartCoroutine(DropPlatform());
            }
        }
        
    }

    private IEnumerator DropPlatform()
    {
        anim.SetTrigger("Shake");
        yield return new WaitForSeconds(dropTime);
        rb.isKinematic = false;
        boxCollider.enabled = false;
    }

    private IEnumerator OnBecameInvisible()
    {
        yield return new WaitForSeconds(returnTime);
        transform.position = initialPosition;
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
    }

    private void OnBecameVisible()
    {
        boxCollider.enabled = true;
    }
}
