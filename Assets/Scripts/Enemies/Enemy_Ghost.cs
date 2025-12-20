using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Ghost : Enemy
{
    [Header("Ghost Details")]
    [SerializeField] private float activeDuration;
    private float activeTimer;
    [Space]
    [SerializeField] private float xMinDistance;
    [SerializeField] private float yMinDistance;
    [SerializeField] private float yMaxDistance;

    private bool isChasing;
    private Transform target;

    protected override void Start()
    {
        base.Start();

        // Try to acquire the current player transform (if any)
        if (PlayerManager.instance != null && PlayerManager.instance.player != null)
            target = PlayerManager.instance.player.transform;

        // Subscribe to player respawn so we can update the target safely
        PlayerManager.OnPlayerRespawn += OnPlayerRespawn;
    }

    private void OnDestroy()
    {
        PlayerManager.OnPlayerRespawn -= OnPlayerRespawn;
    }

    private void OnPlayerRespawn()
    {
        // Update the target when the player respawns
        if (PlayerManager.instance != null && PlayerManager.instance.player != null)
            target = PlayerManager.instance.player.transform;
        else
            target = null;
    }

    protected override void Update()
    {
        base.Update();

        if(isDead)
            return;

        activeTimer -= Time.deltaTime;

        if(isChasing==false && idleTimer < 0)
        {
            StartChase();
        }
        else if(isChasing && activeTimer < 0)
        {
            EndChase();
        }

        HandleMovement();
    }


    private void HandleMovement()
    {
        if (canMove == false)
            return;

        // Guard against a missing
        if (target == null)
        {
            // If we were chasing but the target is gone, stop chasing
            if (isChasing)
                EndChase();
            return;
        }

        HandleFlip(target.position.x);

        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

    }

    private void StartChase()
    {
        // Safely get player from PlayerManager
        Player player = PlayerManager.instance != null ? PlayerManager.instance.player : null;

        if (player == null)
        {
            EndChase();
            return;
        }

        target = player.transform;

        float xOffset = Random.Range(0,100) < 50? -1: 1;
        float yPosition = Random.Range(yMinDistance, yMaxDistance);

        transform.position = target.position + new Vector3(xMinDistance * xOffset, yPosition);
        activeTimer = activeDuration;
        isChasing = true;
        anim.SetTrigger("appear");
    }

    private void EndChase()
    {
        idleTimer = idleDuration;
        isChasing = false;
        anim.SetTrigger("desappear");
    }

    private void MakeInvincible() 
    {
        sr.color = Color.clear;
        EnableColliders(false);
    }

    private void MakeVisible() 
    {
        sr.color = Color.white;
        EnableColliders(true);
    }

    public override void Die()
    {
        base.Die();

        canMove = false;
    }
}
