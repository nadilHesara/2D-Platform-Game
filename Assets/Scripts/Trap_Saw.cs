using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_Saw : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer sr;

    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float cooldown = 1;
    [SerializeField] private Transform[] waypoint;

    public int waypointIndex = 1;
    private bool canMove = true;


    private void Awake()
    {
        anim = GetComponent<Animator>();    
        sr = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        transform.position = waypoint[0].position;
    }

    private void Update()
    {

        anim.SetBool("active", canMove);

        if (canMove == false)
            return;
        
        transform.position = Vector2.MoveTowards(transform.position, waypoint[waypointIndex].position, moveSpeed * Time.deltaTime);

        if(Vector2.Distance( transform.position, waypoint[waypointIndex].position) < 0.1f)
        {
            waypointIndex++;

            if(waypointIndex >= waypoint.Length)
            {
                waypointIndex = 0;
                StartCoroutine(StopMovement(cooldown) );

            }
        }
    }


    private IEnumerator StopMovement(float delay)
    {
        canMove = false;
        yield return new WaitForSeconds(delay);

        canMove = true;
        sr.flipX = !sr.flipX;
    }
}
