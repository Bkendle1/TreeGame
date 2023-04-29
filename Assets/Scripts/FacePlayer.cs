using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{

    private bool isFacingRight = true;
    private Transform player;
    private Enemy enemy;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemy = GetComponent<Enemy>();
    }

    void Update()
    {
        //if the enemy is dead, don't flip sprite
        if (enemy != null)
        {
            if (enemy.currentHealth <= 0)
            {
                return;
            }
        }
        
        //if player is on the right and you're facing left
        if (player.transform.position.x > transform.position.x && !isFacingRight)
        {
            Flip();
        } // else if player is on the left and you're facing right
        else if (player.transform.position.x < transform.position.x && isFacingRight)
        {
            Flip();
        }

        
    }
    
    private void Flip()
    {
        isFacingRight = !isFacingRight;
        // flip game object sprite
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }
}
