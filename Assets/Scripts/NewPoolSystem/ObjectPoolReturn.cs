using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolReturn : MonoBehaviour
{
    private ObjectPoolAdvanced objectPool;
    [Tooltip("How long object has before deactivation.")]
    [SerializeField] private float m_timeToLive = 3f;
    void Start()
    {
        objectPool = FindObjectOfType<ObjectPoolAdvanced>();
        Invoke("Deactivate", m_timeToLive);
    }
    
    private void Deactivate()
    {
        if (objectPool != null)
        {
            objectPool.ReturnGameObject(gameObject);
        }
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}
