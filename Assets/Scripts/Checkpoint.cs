using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
   
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Movement>() != null)
        {
            GameManager.Instance.lastCheckPointPos = transform.position;
        }
    }
}
