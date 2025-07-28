using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    private Animator anim;

    [Header("Movement")]
    [SerializeField]private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    public bool canDoubleJump;

    [Header("Buffer & Coyote Jump")]
    [SerializeField] private float bufferJumpWindow = .25f;
    private float bufferJumpActivated = -1;
    [SerializeField] private float coyoteJumpWindow = .5f;
    private float coyoteJumpActivated = -1;


    [Header("Wall Interanctions")]
    [SerializeField] private float wallJumpDuration = .6f;
    [SerializeField] private Vector2 wallJumpForce;
    private bool isWallJumping;

    [Header("Knockback")]
    [SerializeField] private float knockbackDuration = 1;
    [SerializeField] private Vector2 knockbackPower;
    private bool isKnocked;
    private bool canBeKnocked;

    [Header("Collision")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;
    private bool isAirborne;
    private bool isWallDetected;


    private float xInput;
    private float yInput;

    private bool facingRight = true;

    private int facingDir = 1;

    
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) KnockBack();

        UpdateAirborneStatus();

        if(isKnocked) return;

        HandleInput();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollisions();
        HandleAnimations();
    }


    public void KnockBack()
    {
        if (isKnocked) return;

        StartCoroutine(KnockbackRoutine()); 
        anim.SetTrigger("knockback");

        rb.velocity = new Vector2(knockbackPower.x * -facingDir, knockbackPower.y);
    }

    private IEnumerator KnockbackRoutine()
    {

        isKnocked = true;

        yield return new WaitForSeconds(knockbackDuration);


        isKnocked = false;
    }


    private void UpdateAirborneStatus()
    {
        if (isAirborne && isGrounded)
        {
            HandleLanding();
        }

        if (!isAirborne && !isGrounded)
        {
            BecomeAirborne();
        }
    }

    private void BecomeAirborne()
    {
        isAirborne = true;

        if(rb.velocity.y < 0) 
            ActivateCoyoteJump();
    }

    private void HandleLanding()
    {
        isAirborne = false;
        canDoubleJump = true;

        AttemptBufferJump();
    }

    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");


        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();
            RequestBufferJump();

        }
    }



    #region Buffer & Coyote Jump
    private void RequestBufferJump()
    {
        if (isAirborne)
        {
            bufferJumpActivated = Time.time;
        }
    }

    private void AttemptBufferJump()
    {
        if(Time.time < bufferJumpActivated + bufferJumpWindow)
        {
            bufferJumpActivated = Time.time - 1;
            Jump();
        }
    }

    private void ActivateCoyoteJump() => coyoteJumpActivated = Time.time;

    private void CancelCoyoteJump() => coyoteJumpActivated = Time.time - 1;

    #endregion
    private void JumpButton()
    {
        bool coyoteJumpAvailable = Time.time < coyoteJumpActivated + coyoteJumpWindow;

        if (isGrounded || coyoteJumpAvailable)
        {
            
                
            Jump();
        }

        else if (isWallDetected && !isGrounded)
        {
            WallJump();
        }

        else if (canDoubleJump)
        {
            DoubleJump();
        }

        CancelCoyoteJump();
    }

    private void Jump() => rb.velocity = new Vector2(rb.velocity.x, jumpForce);

    private void DoubleJump()
    {
        isWallJumping = false;
        canDoubleJump = false;
        rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
    }

    private void WallJump()
    {

        canDoubleJump = true;
        rb.velocity = new Vector2(wallJumpForce.x * -facingDir, wallJumpForce.y);

        Flip();

        StopAllCoroutines();
        StartCoroutine(WallJumpRoutine());
    }

    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;

        yield return new WaitForSeconds(wallJumpDuration);

        isWallJumping = false;


    }

    private void HandleWallSlide()
    {
        bool canWallSlide = isWallDetected && rb.velocity.y < 0;
        float yModifier = yInput < 0 ? 1 : .05f;

        if (canWallSlide == false)
            return;

        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * yModifier);

    }



    private void HandleCollisions()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallDetected", isWallDetected);
    }

    private void HandleMovement()
    {
        if(isWallDetected) return;

        if(isWallJumping) return;

        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);

    }

    private void HandleFlip()
    {
        if(xInput < 0 && facingRight || xInput > 0 && !facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingDir = facingDir * -1;
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y-groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y));
    }
}
