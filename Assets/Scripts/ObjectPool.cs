using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    private List<GameObject> _pool = new List<GameObject>();
    private int poolStartSize = 10;
    
    [Tooltip("What GameObject are you pooling?")]
    [SerializeField] private GameObject objectToPool;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        for (int i = 0; i < poolStartSize; i++)
        {
            GameObject obj = Instantiate(objectToPool);
            obj.SetActive(false);
            _pool.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            //if we find an inactive gameobject in pool, return that
            if (!_pool[i].activeInHierarchy)
            {
                return _pool[i];
            }
        }
        return null;
    }
}
