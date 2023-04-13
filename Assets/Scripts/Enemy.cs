using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyProp enemyProperties;
    
    void Start()
    {
        Debug.Log("Enemy Type 1 Attack: " + enemyProperties.GetAttackDamage);
    }
}
