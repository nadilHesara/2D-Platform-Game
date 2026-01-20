using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject fruitDrop;
    [SerializeField] private DifficultyType gameDifficulty;
    private GameManager gameManager;

    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D cd;


    private bool canBeControlled = false;
    [Header("Movement")]
    [SerializeField]private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private float defaultGravityScale;
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
    [Space]
    [SerializeField] private Transform enemyCheck;
    [SerializeField] private float enemyCheckRadius;
    [SerializeField] private LayerMask whatIsEnemy;
    private bool isGrounded;
    private bool isAirborne;
    private bool isWallDetected;

    private Joystick joystick;
    private float xInput;
    private float yInput;

    private bool facingRight = true;

    private int facingDir = 1;


    [Header("Player Visuals")]
    [SerializeField] private AnimatorOverrideController[] animators;
    [SerializeField] private GameObject deathVFX;
    [SerializeField] private ParticleSystem dustFx;
    [SerializeField] private int skinId;




    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CapsuleCollider2D>();
        anim = GetComponentInChildren<Animator>();

        FindFirstObjectByType<UI_JumpButton>().UpdatePlayerRef(this);
        joystick = FindFirstObjectByType<Joystick>();

    }



    private void Start()
    {
        defaultGravityScale = rb.gravityScale;
        gameManager = GameManager.instance;
        
        UpdateGameDifficulty();
        RespawnFinished(false);
        UpdateSkin();

    }

    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) KnockBack(transform.position.x);

        UpdateAirborneStatus();

        if(canBeControlled == false)
        {
            HandleCollisions();
            HandleAnimations();
            return;
        }

        if (isKnocked) return;

        HandleEnemyDetection();
        HandleInput();
        HandleWallSlide();
        HandleMovement();
        HandleFlip();
        HandleCollisions();
        HandleAnimations();
    }



    public void Damage()
    {
        if(gameDifficulty == DifficultyType.Normal)
        {

            if (gameManager.FruitsCollected() <= 0) 
            {
                Die();
                gameManager.RestartLevel();
            }
            else 
            {
                ObjectCreator.instance.CreateObject(fruitDrop, transform, true);
                gameManager.RemoveFruit();
            }

            return;
        }

        if(gameDifficulty == DifficultyType.Hard)
        {
            Die();
            gameManager.RestartLevel();
           
        }

    }



    private void UpdateGameDifficulty()
    {
        DifficultyManager difficultyManager = DifficultyManager.instance;

        if (difficultyManager != null)
            gameDifficulty = difficultyManager.difficulty;
    }

    public void UpdateSkin()
    {
        SkinManager skinManager = SkinManager.instance;

        if(skinManager == null)
            return;

        anim.runtimeAnimatorController = animators[skinManager.chosenSkinId];
    }

    private void HandleEnemyDetection()
    {
        if(rb.linearVelocity.y >= 0) return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemyCheck.position, enemyCheckRadius, whatIsEnemy);

        foreach(var enemy in colliders)
        {
            Enemy newEnemy = enemy.GetComponentInParent<Enemy>();

            if(enemy != null)
            {
                AudioManager.instance.PlaySFX(1);
                newEnemy.Die();
                Jump();
            }
        
        }
    }

    public void RespawnFinished(bool finish)
    {
        

        if (finish)
        {
            rb.gravityScale = defaultGravityScale;
            canBeControlled = true;
            cd.enabled = true;
            AudioManager.instance.PlaySFX(11);

        }
        else
        {
            rb.gravityScale = 0;
            canBeControlled = false;
            cd.enabled = false;
        }
    }
    public void KnockBack(float sourceDamageXPosition)
    {
        float knockbackDir = 1;

        if(transform.position.x < sourceDamageXPosition) 
            knockbackDir = -1;

        if (isKnocked) return;

        AudioManager.instance.PlaySFX(9);
        CameraManager.instance.ScreenShake(knockbackDir);
        StartCoroutine(KnockbackRoutine()); 
        

        rb.linearVelocity = new Vector2(knockbackPower.x * knockbackDir, knockbackPower.y);
    }

    private IEnumerator KnockbackRoutine()
    {

        isKnocked = true;
        anim.SetBool("isKnocked", true);

        yield return new WaitForSeconds(knockbackDuration);


        isKnocked = false;

        anim.SetBool("isKnocked", false);
    }

    public void Die() 
    {
        AudioManager.instance.PlaySFX(0);
        Destroy(gameObject);
        GameObject newDeathVfx = Instantiate(deathVFX, transform.position, Quaternion.identity);
    } 


    public void Push(Vector2 direction, float duration = 0)
    {
        StartCoroutine(PushCoroutine(direction, duration)); 
    }

    public void PushTrampoline(Vector2 direction, float duration = 0)
    {
        StartCoroutine(PushCoroutineTrampoline(direction, duration));
    }



    private IEnumerator PushCoroutine(Vector2 direction, float duration)
    {
        canBeControlled = false;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);

        canBeControlled = true;

    }


    private IEnumerator PushCoroutineTrampoline(Vector2 direction, float duration)
    {
        //canBeControlled = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(direction, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);

        //canBeControlled = true;

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

        if(rb.linearVelocity.y < 0) 
            ActivateCoyoteJump();
    }

    private void HandleLanding()
    {
        dustFx.Play();

        isAirborne = false;
        canDoubleJump = true;

        AttemptBufferJump();
    }

    private void HandleInput()
    {
        //xInput = Input.GetAxisRaw("Horizontal");
        //yInput = Input.GetAxisRaw("Vertical");

        xInput = joystick.Horizontal;
        yInput = joystick.Vertical;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            JumpButton();
        }
    }


    public void JumpButton()
    {
        JumpAttempt();
        RequestBufferJump();
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
    private void JumpAttempt()
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

    private void Jump() 
    {
        dustFx.Play();
        AudioManager.instance.PlaySFX(3);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    } 

    private void DoubleJump()
    {
        dustFx.Play();
        AudioManager.instance.PlaySFX(3);

        isWallJumping = false;
        canDoubleJump = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
    }

    private void WallJump()
    {
        dustFx.Play();
        AudioManager.instance.PlaySFX(12);

        canDoubleJump = true;
        rb.linearVelocity = new Vector2(wallJumpForce.x * -facingDir, wallJumpForce.y);

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
        bool canWallSlide = isWallDetected && rb.linearVelocity.y < 0;
        float yModifier = yInput < 0 ? 1 : .05f;


        if (canWallSlide == false)
            return;

        
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * yModifier);

    }



    private void HandleCollisions()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallDetected", isWallDetected);
    }

    private void HandleMovement()
    {
        if(isWallDetected) return;

        if(isWallJumping) return;

        rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);

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
        Gizmos.DrawWireSphere(enemyCheck.position, enemyCheckRadius);
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y-groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y));
    }
}
