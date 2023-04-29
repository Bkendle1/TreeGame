using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyObject", menuName = "ScriptableObject/Enemy")]
public class EnemyProp : ScriptableObject
{
    [SerializeField] private int m_healthAmount;
    [SerializeField] private int m_attackDamage;
    [SerializeField] private float m_patrolSpeed;
    [SerializeField] private float m_chaseSpeed;
    [SerializeField] private float m_attackSpeed;
    [Tooltip("How long enemy is stunned when they're hit.")]
    [SerializeField] private float m_stunDuration;
    [SerializeField] private Sprite m_enemySprite;
    [SerializeField] private AudioClip m_hurtSFX;
    [SerializeField] private GameObject m_deathEffect;
    [Tooltip("How fast enemy projectiles are.")]
    [SerializeField] private float projectileSpeed;
    
    public int GetHealthAmount => m_healthAmount;
    public int GetAttackDamage => m_attackDamage;
    public float GetPatrolSpeed => m_patrolSpeed;
    public float GetChaseSpeed => m_chaseSpeed;
    public float GetAttackSpeed => m_attackSpeed;
    public float GetStunDuration => m_stunDuration;
    public Sprite GetEnemySprite => m_enemySprite;
    public AudioClip GetHurtSFX => m_hurtSFX;
    public GameObject GetDeathEffect => m_deathEffect;

}
