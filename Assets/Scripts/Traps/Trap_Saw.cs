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

    private Vector3[] waypointPosition;

    public int waypointIndex = 1;
    public int moveDirection = 1;
    private bool canMove = true;


    private void Awake()
    {
        anim = GetComponent<Animator>();    
        sr = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        UpdateWaypointInfo();
        transform.position = waypointPosition[0];

    }

    private void UpdateWaypointInfo()
    {
        List<Trap_SawWaypoint> wayPointList = new List<Trap_SawWaypoint>(GetComponentsInChildren<Trap_SawWaypoint>());

        if(wayPointList.Count != waypoint.Length)
        {
            waypoint = new Transform[wayPointList.Count];
            for (int i = 0; i < wayPointList.Count; i++)
            {
                waypoint[i] = wayPointList[i].transform;
            }
        }

        waypointPosition = new Vector3[waypoint.Length];

        for (int i = 0; i < waypoint.Length; i++)
        {
            waypointPosition[i] = waypoint[i].position;
        }
    }

    private void Update()
    {

        anim.SetBool("active", canMove);

        if (canMove == false)
            return;
        
        transform.position = Vector2.MoveTowards(transform.position, waypointPosition[waypointIndex], moveSpeed * Time.deltaTime);

        if(Vector2.Distance( transform.position, waypointPosition[waypointIndex]) < 0.1f)
        {
            if(waypointIndex == waypointPosition.Length -1 || waypointIndex == 0)
            {
                moveDirection = moveDirection * -1;
                StartCoroutine(StopMovement(cooldown));
            }
            waypointIndex = waypointIndex + moveDirection;
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
