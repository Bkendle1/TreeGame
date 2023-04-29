using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private EnemyProp enemyProperties;
    
    [Header("Patrol")] 
    [SerializeField] private Transform castPos;
    [Tooltip("How far the enemy can see.")]
    [SerializeField] private float baseCastDistance;
    private bool isFacingRight = true;

    private Rigidbody2D rb;
    private Animator anim;
    private HealthBar healthBar;
    
    void Start()
    {
        healthBar = GetComponent<Enemy>().healthBar;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Patrol();

        if (rb.velocity.x != 0)
        {
            anim.SetBool("isPatrolling", true);
        }
        else
        {
            anim.SetBool("isPatrolling", false);
        }

        if (IsHittingWall() || IsNearEdge())
        {
            Flip();
        }
    }
    
    private void Patrol()
    {
        
        float patrolSpeed = enemyProperties.GetPatrolSpeed;

        if (!isFacingRight)
        {
            patrolSpeed = -enemyProperties.GetPatrolSpeed;
        }
        
        //if enemy isn't stunned move
        if (!GetComponent<Enemy>().isStunned)
        {
            //move enemy
            
            rb.velocity = new Vector2(patrolSpeed, rb.velocity.y);
        }
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
}
