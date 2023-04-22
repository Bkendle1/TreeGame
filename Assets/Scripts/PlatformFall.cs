using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlatformFall : MonoBehaviour
{
    [SerializeField] private float waitToDrop = 1f;
    [SerializeField] private float waitToReturn = 1f;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DropPlatform());
        }
    }

    private IEnumerator DropPlatform()
    {
        yield return new WaitForSeconds(waitToDrop);
        anim.SetTrigger("Fall");
        yield return new WaitForSeconds(waitToReturn);
        anim.SetTrigger("Rise");
    }
    
}
