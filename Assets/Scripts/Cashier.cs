using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cashier : MonoBehaviour
{
    private ObjectPoolAdvanced cashRegisterPool;
    [SerializeField] private float startAngle = 270f;
    [SerializeField] private float endAngle = 90f;
    [SerializeField] private int bulletAmount = 10;
    [SerializeField] private GameObject projectile;
    private Vector2 projectileDirection;
    
    [SerializeField] private float fireRate = .5f;

    private void Awake()
    {
        cashRegisterPool = FindObjectOfType<ObjectPoolAdvanced>();
    }

    private void OnEnable()
    {
        StartCoroutine(ShootCashRegister());
    }

    private void OnDisable()
    {
        StopCoroutine(ShootCashRegister());
    }

    private IEnumerator ShootCashRegister()
    {
        float angleStep = (endAngle - startAngle) / bulletAmount;
        float angle = startAngle;

        for (int i = 0; i < bulletAmount; i++)
        {
            float bulDirX = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180f);
            float bulDirY = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180f);

            Vector3 bulMoveVector = new Vector3(bulDirX, bulDirY, 0f);
            Vector2 bulDir = (bulMoveVector - transform.position).normalized;

            GameObject obj = cashRegisterPool.GetObject(projectile);
                obj.transform.position = transform.position;
                obj.transform.rotation = transform.rotation;
                obj.GetComponentInChildren<CashRegisterProjectile>().SetMoveDirection(bulDir);
            
            angle += angleStep;
            yield return new WaitForSeconds(fireRate);
        }
    }
}
