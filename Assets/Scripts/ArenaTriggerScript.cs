using System.Collections;
using UnityEngine;

public class ArenaDoorCloser2D : MonoBehaviour
{
    [Header("Door Parents (drag LeftDoor and RightDoor here)")]
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;

    [Header("Closed Targets (drag the target empty objects here)")]
    [SerializeField] private Transform leftClosedTarget;
    [SerializeField] private Transform rightClosedTarget;

    [Header("Motion")]
    [SerializeField] private float closeSpeed = 6f;
    [SerializeField] private float snapDistance = 0.02f;

    private bool triggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;

        if (collision.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(CloseDoors());

            // stops triggering again
            GetComponent<Collider2D>().enabled = false;
        }
    }

    private IEnumerator CloseDoors()
    {
        while (true)
        {
            bool leftDone = MoveDoor(leftDoor, leftClosedTarget.position);
            bool rightDone = MoveDoor(rightDoor, rightClosedTarget.position);

            if (leftDone && rightDone)
                break;

            yield return null;
        }
    }

    private bool MoveDoor(Transform door, Vector3 targetPos)
    {
        if (door == null) return true;

        door.position = Vector3.MoveTowards(
            door.position,
            targetPos,
            closeSpeed * Time.deltaTime
        );

        return Vector3.Distance(door.position, targetPos) <= snapDistance;
    }
}
