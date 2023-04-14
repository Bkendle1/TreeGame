using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyProp enemyProperties;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetupEnemySettings();
        Debug.Log("Enemy Type 1 Attack: " + enemyProperties.GetAttackDamage);
    }

    private void SetupEnemySettings()
    {
        //spriteRenderer.sprite = enemyProperties.GetEnemySprite;
    }
}
