using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
  public void  OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Player player = collision.gameObject.GetComponent<Player>();

            if(player != null)
            {
                player.KnockBack();
            }
            
        }
    }
}
