using UnityEngine;

public class SawGroupResetter : MonoBehaviour
{
    private Vector3 startPos;
    private Quaternion startRot;
    private Trap_Saw[] saws;

    private void Awake()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        saws = GetComponentsInChildren<Trap_Saw>(true);
    }

    private void OnEnable() => RespawnEvents.OnRespawn += ResetGroup;
    private void OnDisable() => RespawnEvents.OnRespawn -= ResetGroup;

    private void ResetGroup()
    {
        transform.position = startPos;
        transform.rotation = startRot;

        foreach (var saw in saws)
            saw.ResetSaw();

        Debug.Log("SawGroupResetter: RESET!");
    }
}
