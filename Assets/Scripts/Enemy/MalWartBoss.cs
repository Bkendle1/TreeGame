using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

public class MalWartBoss : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    private Enemy enemy;
    private Animator anim;

    [Header("Shopping Cart")]
    [SerializeField] private GameObject shoppingCart;
    [SerializeField] private bool spawnCarts = false;
    [SerializeField] private float cartSpawnRate = 1f;
    private ObjectPoolAdvanced shoppingCartPool;
    
    [Header("Machete")]
    [SerializeField] private GameObject machete;
    [SerializeField] private bool spawnMachetes = false;
    [SerializeField] private float macheteSpawnRate = 1f;
    private ObjectPoolAdvanced machetePool;

    [Header("Fishing Rod Spikes")]
    [SerializeField] private Animator spikeAnimator;
    [Tooltip("How long fishing rods are out until they retract.")]
    [SerializeField] private float fishingRodDuration = 3f;
    [SerializeField] bool activateSpikes = false;

    [Header("Cashier")] 
    [SerializeField] private GameObject cashier;
    [SerializeField] private bool spawnCashiers = false;
    [SerializeField] private float cashierSpawnRate = 2f;
    private ObjectPoolAdvanced cashierPool;
    
    [Header("Mom")] 
    [SerializeField] private GameObject mom;
    [SerializeField] private bool spawnMoms = false;
    [SerializeField] private float momSpawnRate = 2f;
    private ObjectPoolAdvanced momPool;

    [Header("Kevin")] 
    [SerializeField] private GameObject kevin;
    [SerializeField] private bool spawnKevins = false;
    [SerializeField] private float kevinSpawnRate = 2f;
    private ObjectPoolAdvanced kevinPool;
    
    [Header("Karen")] 
    [SerializeField] private GameObject karen;
    [SerializeField] private bool spawnKarens = false;
    [SerializeField] private float karenSpawnRate = 2f;
    private ObjectPoolAdvanced karenPool;

    private void Start()
    {
        shoppingCartPool = FindObjectOfType<ObjectPoolAdvanced>();
        machetePool = FindObjectOfType<ObjectPoolAdvanced>();
        cashierPool = FindObjectOfType<ObjectPoolAdvanced>();
        momPool = FindObjectOfType<ObjectPoolAdvanced>();
        kevinPool = FindObjectOfType<ObjectPoolAdvanced>();
        karenPool = FindObjectOfType<ObjectPoolAdvanced>();
        enemy = GetComponent<Enemy>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        //if boss is defeated, stop spawning objects
        if (enemy.currentHealth <= 0)
        {
            spikeAnimator.gameObject.SetActive(false);
            StopAllCoroutines();
        }

        //if malwart has less than 2/3 of their health but greater than 1/3 of their health
        if (enemy.currentHealth <= (enemy.enemyProperties.GetHealthAmount * .66) && enemy.currentHealth > enemy.enemyProperties.GetHealthAmount * .33)
        {
            anim.SetTrigger("phaseTwo");
            Debug.Log("ENTERING PHASE TWO");
        }
        //else if malwart has less than 1/3 of their health
        else if(enemy.currentHealth <= (enemy.enemyProperties.GetHealthAmount * .33))
        {
            anim.SetTrigger("phaseThree");
            Debug.Log("ENTERING PHASE THREE");
        }
        
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
        
        if (spawnCashiers)
        {
            StartCoroutine(SpawnCashiers());
            spawnCashiers = false;
        }

        if (spawnMoms)
        {
            StartCoroutine(SpawnMoms());
            spawnMoms = false;
        }

        if (spawnKevins)
        {
            StartCoroutine(SpawnKevins());
            spawnKevins = false;
        }
        
        if (spawnKarens)
        {
            StartCoroutine(SpawnKarens());
            spawnKarens = false;
        }

        if (activateSpikes)
        {
            StartCoroutine(StartRodSpikes());
        }

    }

    private void StopSpawning()
    {
        StopAllCoroutines();
    }

    private IEnumerator StartRodSpikes()
    {
        spikeAnimator.SetBool("Attack", activateSpikes);
        yield return new WaitForSeconds(fishingRodDuration);
        activateSpikes = false;
        spikeAnimator.SetBool("Attack", activateSpikes);
        yield return new WaitForSeconds(fishingRodDuration);
        activateSpikes = true;
    }
    
    private IEnumerator SpawnKarens()
    {
        yield return new WaitForSeconds(karenSpawnRate);
        GameObject obj = karenPool.GetObject(karen);
        obj.transform.position = spawnPoint.position;
        obj.transform.rotation = Quaternion.identity;
        yield return new WaitForSeconds(karenSpawnRate);
        spawnKarens = true;
    }
    
    private IEnumerator SpawnKevins()
    {
        yield return new WaitForSeconds(kevinSpawnRate);
        GameObject obj = kevinPool.GetObject(kevin);
        obj.transform.position = spawnPoint.position;
        obj.transform.rotation = Quaternion.identity;
        yield return new WaitForSeconds(kevinSpawnRate);
        spawnKevins = true;
    }
    
    private IEnumerator SpawnMoms()
    {
        yield return new WaitForSeconds(momSpawnRate);
        GameObject obj = momPool.GetObject(mom);
        obj.transform.position = spawnPoint.position;
        obj.transform.rotation = Quaternion.identity;
        yield return new WaitForSeconds(momSpawnRate);
        spawnMoms = true;
    }
    
    private IEnumerator SpawnCashiers()
    {
        yield return new WaitForSeconds(cashierSpawnRate);
        GameObject obj = cashierPool.GetObject(cashier);
        obj.transform.position = spawnPoint.position;
        obj.transform.rotation = Quaternion.identity;
        yield return new WaitForSeconds(cashierSpawnRate);
        spawnCashiers = true;
    }
    private IEnumerator SpawnShoppingCarts()
    {
        yield return new WaitForSeconds(cartSpawnRate);
        GameObject obj = shoppingCartPool.GetObject(shoppingCart);
        obj.transform.position = spawnPoint.position;
        obj.transform.rotation = Quaternion.identity;
        yield return new WaitForSeconds(cartSpawnRate);
        spawnCarts = true;
    }
    
    private IEnumerator SpawnMachetes()
    {
        yield return new WaitForSeconds(macheteSpawnRate);
        GameObject obj = machetePool.GetObject(machete);
        obj.transform.position = spawnPoint.position;
        obj.transform.rotation = Quaternion.identity;
        yield return new WaitForSeconds(macheteSpawnRate);
        spawnMachetes = true;
    }
}