using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator anim => GetComponent<Animator>();
    private bool active;
    
    [SerializeField] public bool canBeActivated;


    private void Start()
    {
        canBeActivated = GameManager.instance.canReactivate;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (active && canBeActivated == false) return;

        Player player = collision.GetComponent<Player>();

        if(player != null)
        {
            ActivateCheckpoint();
        }

    }

    public void ActivateCheckpoint()
    {
        active = true;
        anim.SetTrigger("activate");
        PlayerManager.instance.UpdateRespawnPosition(transform); 
    }
}
