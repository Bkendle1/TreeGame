using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is really only for the lumberjack enemy as that's the only enemy we have right now that attacks when the player is near.
/// The way it works is if the player gets within the enemy's given attack range, it'll play the attack animation which enables a circle collider
/// that's a trigger which checks if the player is in, and if so, deals damage.
/// </summary>
public class EnemyAttack : MonoBehaviour
{
    private EnemyProp enemyProperties;
    private Transform player;
    private Animator anim;
    
    [Header("Cinemachine")]                               
    [SerializeField] private float camShakeIntensity = 4f;
    [SerializeField] private float camShakeDuration = .1f;


    void Start()
    {
        //Get enemy properties from "Enemy" script
        enemyProperties = GetComponent<Enemy>().enemyProperties;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Vector2.Distance(player.position, transform.position) <= enemyProperties.GetAttackRange)
        {
            anim.SetTrigger("Attack");
            anim.speed = enemyProperties.GetAttackSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<PlayerHealth>() != null)
        {
            PlayerHealth player = col.GetComponent<PlayerHealth>();
            Movement playerMovement = col.GetComponent<Movement>();
            
            playerMovement.KBTimer = playerMovement.KBDuration;
            player.TakeDamage(enemyProperties.GetAttackDamage);
            
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, enemyProperties.GetAttackRange);
    }

    //gets called at the end of the attack but an animation event
    public void ResetAttackTrigger()
    {
        anim.ResetTrigger("Attack");
        anim.speed = 1;
    }
}
