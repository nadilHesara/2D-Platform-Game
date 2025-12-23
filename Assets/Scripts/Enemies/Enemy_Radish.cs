using System;
using System.Collections;
using UnityEngine;

public class Enemy_Radish : Enemy
{
    [Header("Radish Detaikls")]
    [SerializeField] private float flyForce;
    [SerializeField] private float walkDuration = 2f;

    private float xOriginalPosition;
    private float walkTimer;
    private float minFlyDistance;

    private RaycastHit2D groundBelowDetected;
    private bool isFlying;

    [Space]
    [SerializeField] private Material flashMaterial;
    private Material originalMaterial;
    protected override void Start()
    {
        base.Start();

        originalMaterial = sr.material;
        xOriginalPosition = transform.position.x;
        isFlying = true;
        minFlyDistance = Physics2D.Raycast(transform.position, Vector2.down, float.MaxValue, whatIsGround).distance;

    }
    protected override void Update()
    {
        base.Update();

        if (isDead)
            return;

        walkTimer -= Time.deltaTime;

        if (isFlying) 
        {
            HandleFlying();
        }
        else
        {
            float xDifference = Mathf.Abs(transform.position.x - xOriginalPosition);

            if (walkTimer < 0 && xDifference < 0.1f) 
            {
                rb.gravityScale = 1;
                isFlying = true;
            }

            HandleMovement();
            HandleTurnAround();
            

        }


    }

    private void HandleFlying()
    {
        if (groundBelowDetected.distance < minFlyDistance)
            rb.linearVelocity = new Vector2(0, flyForce);
    }

    private void HandleTurnAround()
    {
        if(isGrounded == false)
            return;

        if (!isGroundInfrontDetected || isWallDetected)
        {
            Flip();
            idleTimer = idleDuration;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void HandleMovement()
    {
        if(isGrounded == false)
            return;

        if (idleTimer > 0)
            return;

        if (isGroundInfrontDetected)
            rb.linearVelocity = new Vector2(moveSpeed * facingDir, rb.linearVelocity.y);
    }

    protected override void HandleAnimator()
    {
        base.HandleAnimator();

        anim.SetBool("isFlying", isFlying);
    }


    protected override void HandleCollision()
    {
        base.HandleCollision();

        groundBelowDetected= Physics2D.Raycast(transform.position, Vector2.down, float.MaxValue, whatIsGround);
    }


    public override void Die()
    {
        if (isFlying)
        {
            StartCoroutine(FlashFxCo());
            isFlying = false;
            walkTimer = walkDuration;
            rb.gravityScale = 3;

        }
        else
        {
            base.Die();
        }
    }

    private IEnumerator FlashFxCo()
    {
        sr.material = flashMaterial;

        yield return new WaitForSeconds(0.1f);

        sr.material = originalMaterial;
    }
}
