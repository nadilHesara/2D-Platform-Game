using NUnit.Framework;
using System.Collections.Generic;
using Unity.InferenceEngine;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy_Bee : Enemy
{
    [Header("Bee Details")]
    [SerializeField] private EnemyBullet_Bee bulletPrefab;
    [SerializeField] private Transform gunPoint;
    [SerializeField] private float bulletSpeed = 7;
    [SerializeField] private float bulletLifetime = 5f;
    [SerializeField] private float attackCooldown = 1.5f;
    private float lastTimeAttacked;

    [SerializeField] private float offset = 0.25f;
    private List<Vector3> wayPoints = new List<Vector3>();
    private int wayIndex;

    private Transform target;

    protected override void Start()
    {
        base.Start();
        canMove = false;
        CreateWayPoints();

        float randomValue = Random.Range(0, 0.6f);
        Invoke(nameof(AllowMovement), randomValue);
    }

    private void CreateWayPoints()
    {
        wayPoints.Add(transform.position + new Vector3(offset, offset));
        wayPoints.Add(transform.position + new Vector3(offset, -offset));
        wayPoints.Add(transform.position + new Vector3(-offset, -offset));
        wayPoints.Add(transform.position + new Vector3(-offset, offset));
    }

    protected override void Update()
    {
        base.Update();

        HandleMovement();
        FindTargetIfEmpty();

        bool canAttack = Time.time > lastTimeAttacked + attackCooldown && target != null; //slow down the plant shooting anim by measuring the game time and see whether
                                                                                          //it is greater than the last shot time plus attack cooldown

        if (canAttack)
            Attack();
    }

    private void FindTargetIfEmpty()
    {
        if (target == null)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, float.MaxValue, whatIsPlayer);

            if (hit.transform != null)
            {
                target = hit.transform;
            }
        }
    }

    private void HandleMovement()
    {
        if(canMove == false)
            return;

        if (isDead)
            return;

        transform.position = Vector3.MoveTowards(transform.position, wayPoints[wayIndex], moveSpeed * Time.deltaTime);

        if(Vector3.Distance(transform.position, wayPoints[wayIndex]) < 0.1f)
        {
            wayIndex++;
            if(wayIndex >= wayPoints.Count)
            {
                wayIndex = 0;
            }
        }
    }

    private void Attack()
    {
        lastTimeAttacked = Time.time;
        anim.SetTrigger("attack");

    }

    private void CreateBullet()
    {
        if (!target) 
            return;

        EnemyBullet_Bee newBullet = Instantiate(bulletPrefab, gunPoint.position, Quaternion.identity);
        newBullet.SetUpBullet(target, bulletSpeed, bulletLifetime);


        target = null;
        
    }

    private void AllowMovement() => canMove = true;
    protected override void HandleAnimator()
    {
        //keeping this empty because to get rid of the warning msg saying about no xVelocity declaration.
    }
}
