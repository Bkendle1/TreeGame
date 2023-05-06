 using System;
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<ExpCollectible>() != null)
        {
            //give the collectible the player's position so it knows where to go
            other.GetComponent<ExpCollectible>().SetTarget(transform.parent.position);
        }
    }
}
