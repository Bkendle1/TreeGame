using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{

    private bool isFacingRight;
    private Transform player;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
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
