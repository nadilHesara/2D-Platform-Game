using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet_Bee : MonoBehaviour
{
    private Transform target;
    private List<Vector3> wayPoints = new List<Vector3>();
    [SerializeField] private int wayIndex;

    [SerializeField] private GameObject pickupVfx;
    [SerializeField] private float wayPointUpdateCooldown;
    private float speed;

    public void SetUpBullet(Transform newTarget, float newSpeed, float lifeDuration)
    {
        target = newTarget;
        speed = newSpeed;
        transform.up = transform.position - target.position;

        StartCoroutine(AddWayPointCoroutine());
        Destroy(gameObject, lifeDuration);
    }

    private void Update()
    {
        if (wayPoints.Count <= 0)
            return;

        if(wayIndex >= wayPoints.Count || wayIndex < 0)
            return;

        transform.position = Vector3.MoveTowards(transform.position, wayPoints[wayIndex], speed * Time.deltaTime);

        if(Vector3.Distance(transform.position, wayPoints[wayIndex]) < 0.1f)
        {
            wayIndex++;

            if (wayIndex >= wayPoints.Count)
                return;

            transform.up = transform.position-wayPoints[wayIndex];
        }
    }

    private IEnumerator AddWayPointCoroutine()
    {
        while (true)
        {
            AddWayPoints();

            yield return new WaitForSeconds(wayPointUpdateCooldown);
        }
    }
    private void AddWayPoints()
    {
        if(target == null)
            return;

        foreach(Vector3 waypoint in wayPoints)
        {
            if(waypoint == target.position)
                return;
        }

        wayPoints.Add(target.position);
    }

    /*private void OnDestroy()
    {
    
        GameObject newFx = Instantiate(pickupVfx, transform.position, Quaternion.identity);
        newFx.transform.localScale = new Vector3(.6f, .6f, .6f);

    }*/


    private static bool isShuttingDown;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetShutdownFlag()
    {
        isShuttingDown = false;
    }

    private void OnApplicationQuit() => isShuttingDown = true;

    private void OnDestroy()
    {
        // If we are quitting / stopping play mode / unloading, don't spawn anything
        if (isShuttingDown) return;

        // If scene is unloading, don't spawn anything
        if (!gameObject.scene.isLoaded) return;

        if (pickupVfx == null) return;

        GameObject newFx = Instantiate(pickupVfx, transform.position, Quaternion.identity);
        newFx.transform.localScale = new Vector3(.6f, .6f, .6f);

        // IMPORTANT: ensure it gets cleaned up
        Destroy(newFx, 2f);
    }

}
