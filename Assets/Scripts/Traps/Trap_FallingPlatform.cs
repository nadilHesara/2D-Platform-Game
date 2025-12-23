using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tra : MonoBehaviour
{

    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D[] colliders;

    [SerializeField] private float speed = .75f;
    [SerializeField] private float travelDistance;
    private Vector3[] wayPoints;
    private int wayPointIndex;
    private bool canMove = false;


    [Header("Platform Fall Details")]
    [SerializeField] private float impactSpeed = 3;
    [SerializeField] private float impactDuration = .1f;
    private float impactTimer;
    private bool impactHappened;
    [Space]
    [SerializeField] private float fallDelay = .5f;

    // store initial position so we can reset on player respawn
    private Vector3 initialPosition;


    [Header("Respawn")]
    [SerializeField] private float respawnDelay = 5f;

    private Coroutine respawnCoroutine;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<BoxCollider2D>();

        // remember start position
        initialPosition = transform.position;
    }
    private IEnumerator Start()
    {
        SetupWayPoints();
        float randomDelay = Random.Range(0, 0.6f);
        yield return new WaitForSeconds(randomDelay);
        canMove = true;
    }


    private void ActivatePlatform() => canMove = true;


    private void SetupWayPoints()
    {
        wayPoints = new Vector3[2];

        float yOffset = travelDistance / 2;

        wayPoints[0] = transform.position + new Vector3(0, yOffset, 0);
        wayPoints[1] = transform.position + new Vector3(0, -yOffset, 0);
    }


    private void Update()
    {
        HandleImpact();
        HandleMovement();
    }




    private void HandleMovement()
    {

        if (canMove == false)
        {
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, wayPoints[wayPointIndex], speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, wayPoints[wayPointIndex]) < 0.1f)
        {
            wayPointIndex++;

            if (wayPointIndex >= wayPoints.Length)
                wayPointIndex = 0;

        }
    }


    private void HandleImpact()
    {
        if (impactTimer < 0)
            return;

        impactTimer -= Time.deltaTime;

        transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3.down * 10), impactSpeed * Time.deltaTime);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (impactHappened)
            return;

        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            Invoke(nameof(SwitchedOffPlatform), fallDelay);
            impactTimer = impactDuration;
            impactHappened = true;

            if (respawnCoroutine == null)
                respawnCoroutine = StartCoroutine(RespawnAfterFall());
        }
    }

    private void SwitchedOffPlatform()
    {
        anim.SetTrigger("deactivate");

        canMove = false;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3.5f;
        rb.linearDamping = .5f;

        foreach (BoxCollider2D collider in colliders)
        {
            collider.enabled = false;
        }
    }

    private IEnumerator RespawnAfterFall()
    {
        yield return new WaitForSeconds(fallDelay);

        
        yield return new WaitForSeconds(respawnDelay);

        ReactivatePlatform(true);
        respawnCoroutine = null;

    }


    // Public method to reactivate the platform (called when player respawns)
    public void ReactivatePlatform(bool resetPosition = true)
    {
      

        impactHappened = false;
        impactTimer = -1f;

        canMove = true;

        
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.linearDamping = 0f;

        foreach (BoxCollider2D collider in colliders)
        {
            collider.enabled = true;
        }

        
        if (resetPosition)
            transform.position = initialPosition;

        
        SetupWayPoints();
        wayPointIndex = 0;
    }
}
