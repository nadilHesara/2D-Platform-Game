using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Snail : Enemy
{
    [Header("Snail details")]
    [SerializeField] private Enemy_SnailBody bodyPrefab;
    [SerializeField] private float maxSpeed = 10; 
    public bool hasBody = true;


    protected override void Update()
    {
        base.Update();

        if (isDead)
            return;

        HandleMovement();

        if (isGrounded)
            HandleTurnAround();
    }

    public override void Die()
    {
        if (hasBody)
        {
            canMove = false;
            hasBody = false;
            anim.SetTrigger("hit");
            rb.linearVelocity = Vector2.zero;

            idleDuration = 0;

        }
        else if (canMove == false && hasBody == false)
        {
            anim.SetTrigger("hit");
            canMove = true;
            moveSpeed = maxSpeed;
        }
        else
        {
            base.Die();
        }
    }

    private void HandleTurnAround()
    {
        bool canFlipFromLedge = !isGroundInfrontDetected && hasBody;

        if (canFlipFromLedge || isWallDetected)
        {


            Flip();
            idleTimer = idleDuration;
            rb.linearVelocity = Vector2.zero;

        }
    }

    private void HandleMovement()
    {
        if (idleTimer > 0)
            return;

        if(canMove == false)
            return;

        if (isGroundInfrontDetected)
            rb.linearVelocity = new Vector2(moveSpeed * facingDir, rb.linearVelocity.y);
    }

    private void CreateBody()
    {
        Enemy_SnailBody newBody = Instantiate(bodyPrefab, transform.position, Quaternion.identity);

        if (Random.Range(0, 100) < 50)
            deathRotationDirection = deathRotationDirection * -1;

        newBody.SetupBody(deathImpactSpeed, deathRotationSpeed*deathRotationDirection, facingDir);

        Destroy(newBody.gameObject, 10f);
    }

    protected override void Flip()
    {
        base.Flip();

        if(hasBody == false)
            anim.SetTrigger("wallHit");
    }
}
