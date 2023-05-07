using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponObject", menuName = "ScriptableObject/Weapon")]
public class WeaponProp : ScriptableObject
{
    [SerializeField] private int m_attackDamage;
    [SerializeField] private float m_attackRange; // this will affect the scale of the game object not their collider size
    [SerializeField] private float m_attackSpeed; // how fast the attack is executed 
    [SerializeField] private float m_timeBtwAttacks; // how much time passes between attacks
    [SerializeField] private float m_knockbackPower;
    [SerializeField] private Sprite m_weaponSprite;
    [SerializeField] private Sprite m_upgradedWeaponSprite;
    [SerializeField] private GameObject m_projectile;
    [SerializeField] private GameObject m_impactEffect;
    
    public int GetAttackDamage => m_attackDamage;
    public float GetAttackRange => m_attackRange;
    public float GetAttackSpeed => m_attackSpeed;
    public float GetKnockBackPower => m_knockbackPower;
    public float GetTimeBtwAttacks => m_timeBtwAttacks;
    public Sprite GetWeaponSprite => m_weaponSprite;
    public Sprite GetUpgradedWeaponSprite => m_upgradedWeaponSprite;
    public GameObject GetProjectile => m_projectile;
    public GameObject GetImpactEffect => m_impactEffect;
}