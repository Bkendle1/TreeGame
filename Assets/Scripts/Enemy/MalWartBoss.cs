using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class MalWartBoss : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    
    [Header("Shopping Cart")]
    [SerializeField] private GameObject shoppingCart;
    private ObjectPoolAdvanced shoppingCartPool;
    [SerializeField] private bool spawnCarts = true;
    [SerializeField] private float cartSpawnRate = 1f;
    
    [Header("Machete")]
    [SerializeField] private GameObject machete;
    private ObjectPoolAdvanced machetePool;
    [SerializeField] private bool spawnMachetes = true;
    [SerializeField] private float macheteSpawnRate = 1f;

    
    
    private void Start()
    {
        shoppingCartPool = FindObjectOfType<ObjectPoolAdvanced>();
        machetePool = FindObjectOfType<ObjectPoolAdvanced>();
    }

    private void Update()
    {
        if (spawnCarts)
        {
            StartCoroutine(SpawnShoppingCarts());
            spawnCarts = false;
        }

        if (spawnMachetes)
        {
            StartCoroutine(SpawnMachetes());
            spawnMachetes = false;
        }
    }

    private IEnumerator SpawnShoppingCarts()
    {
        GameObject obj = shoppingCartPool.GetObject(shoppingCart);
        obj.transform.position = spawnPoint.position;
        obj.transform.rotation = Quaternion.identity;
        yield return new WaitForSeconds(cartSpawnRate);
        spawnCarts = true;
    }
    private IEnumerator SpawnMachetes()
    {
        GameObject obj = machetePool.GetObject(machete);
        obj.transform.position = spawnPoint.position;
        obj.transform.rotation = Quaternion.identity;
        yield return new WaitForSeconds(macheteSpawnRate);
        spawnMachetes = true;
    }
}
