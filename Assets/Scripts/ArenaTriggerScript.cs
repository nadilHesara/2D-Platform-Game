using System.Collections;
using UnityEngine;

public class ArenaDoorCloser2D : MonoBehaviour
{
    [Header("Door Parents")]
    [SerializeField] private Transform leftDoor;
    [SerializeField] private Transform rightDoor;

    [Header("Targets")]
    [SerializeField] private Transform leftClosedTarget;
    [SerializeField] private Transform rightClosedTarget;

    [SerializeField] private Transform leftOpenTarget;
    [SerializeField] private Transform rightOpenTarget;

    [Header("Motion")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float snapDistance = 0.02f;

    private Coroutine moveRoutine;

    public void CloseDoors()
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveDoors(leftClosedTarget.position, rightClosedTarget.position));
    }

    public void OpenDoors()
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveDoors(leftOpenTarget.position, rightOpenTarget.position));
    }

    private IEnumerator MoveDoors(Vector3 leftTarget, Vector3 rightTarget)
    {
        while (true)
        {
            bool leftDone = MoveDoor(leftDoor, leftTarget);
            bool rightDone = MoveDoor(rightDoor, rightTarget);

            if (leftDone && rightDone)
                break;

            yield return null;
        }
    }

    private bool MoveDoor(Transform door, Vector3 targetPos)
    {
        if (door == null) return true;

        door.position = Vector3.MoveTowards(door.position, targetPos, moveSpeed * Time.deltaTime);
        return Vector3.Distance(door.position, targetPos) <= snapDistance;
    }
}
