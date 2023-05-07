using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// This is for the CEO enemy, this script and the enemy attack script shouldn't be on the same game object as they both check
/// for the player is in the attack range.
/// </summary>
public class EnemyShooting : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform bulletSpawnPos;
    private float timer; // shooting frequency
    private EnemyProp enemyProperties;
    private Transform player;
    private Animator anim;

    private ObjectPoolAdvanced projectilePool;
    
    void Start()
    {
        enemyProperties = GetComponent<Enemy>().enemyProperties;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
        projectilePool = FindObjectOfType<ObjectPoolAdvanced>();
    }
    

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);
        
        if (distance < enemyProperties.GetAttackRange)
        {
            //TODO with the anim.speed method, if the player is in the attack radius, the enemy's animations will all be slowed 
            //however, there's a bypass right now where im just changing the animation speed of just the attack animation in the animator
            //maybe there's a way to get a reference to that state specifically and change its speed
            anim.SetTrigger("Attack");
            // anim.speed = enemyProperties.GetAttackSpeed;
        }
    }
    
    public void Shoot()
    {
        GameObject projectile = projectilePool.GetObject(bullet);
        projectile.transform.rotation = bulletSpawnPos.rotation;
        projectile.transform.position = bulletSpawnPos.position;
    }

    public void ResetAttackTrigger()
    {
        anim.ResetTrigger("Attack");
        // anim.speed = 1;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, enemyProperties.GetAttackRange);
    }
}
