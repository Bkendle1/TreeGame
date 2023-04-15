using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponObject", menuName = "ScriptableObject/Weapon")]
public class WeaponProp : ScriptableObject
{
    [SerializeField] private int m_attackDamage;
    [SerializeField] private float m_attackRange;
    [SerializeField] private float m_attackSpeed; // how much time passes between attacks
    [SerializeField] private float m_knockbackPower;
    [SerializeField] private Sprite m_weaponSprite;
    [SerializeField] private Sprite m_projectileSprite;
    [SerializeField] private GameObject m_impactEffect;
    
    public int GetAttackDamage => m_attackDamage;
    public float GetAttackRange => m_attackRange;
    public float GetAttackSpeed => m_attackSpeed;
    public float GetKnockBackPower => m_knockbackPower;
    public Sprite GetWeaponSprite => m_weaponSprite;
    public Sprite GetProjectileSprite => m_projectileSprite;
    public GameObject GetImpactEffect => m_impactEffect;
}