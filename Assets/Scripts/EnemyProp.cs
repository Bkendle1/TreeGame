using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyObject", menuName = "ScriptableObject/Enemy")]
public class EnemyProp : ScriptableObject
{
    [SerializeField] private float m_healthAmount;
    [SerializeField] private float m_attackDamage;
    [SerializeField] private float m_patrolSpeed;
    [SerializeField] private float m_chaseSpeed;
    [SerializeField] private float m_attackSpeed;
    [SerializeField] private float m_stunDuration; //how long enemy is stunned when they're hit
    [SerializeField] private GameObject m_enemyType;
    [SerializeField] private AudioClip m_hurtSFX;
    
    public float GetHealthAmount => m_healthAmount;
    public float GetAttackDamage => m_attackDamage;
    public float GetPatrolSpeed => m_patrolSpeed;
    public float GetChaseSpeed => m_chaseSpeed;
    public float GetAttackSpeed => m_attackSpeed;
    public float GetStunDuration => m_stunDuration;
    public GameObject GetEnemySprite => m_enemyType;
    public AudioClip GetHurtSFX => m_hurtSFX;


}
