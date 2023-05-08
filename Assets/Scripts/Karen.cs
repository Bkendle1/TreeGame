using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Karen : MonoBehaviour
{
    private ObjectPoolAdvanced projectilePool;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float fireRate = .5f;

    private void Awake()
    {
        projectilePool = FindObjectOfType<ObjectPoolAdvanced>();
    }

    private void OnEnable()
    {
        StartCoroutine(ShootProjectile());
    }

    private void OnDisable()
    {
        StopCoroutine(ShootProjectile());
    }

    private IEnumerator ShootProjectile()
    {
        GameObject obj = projectilePool.GetObject(projectile);
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
        
        yield return new WaitForSeconds(fireRate);
        
    }
}