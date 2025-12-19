using System.Collections;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay;
    public Player player;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (respawnPoint == null)
        {
            respawnPoint = FindFirstObjectByType<Startpoint>().transform;
        }

        if (player == null)
            player = FindAnyObjectByType<Player>();
    }


    public void RespawnPlayer()
    {
        DifficultyManager difficultyManager = DifficultyManager.instance;

        if (difficultyManager != null && difficultyManager.difficulty == DifficultyType.Hard)
            return;

        StartCoroutine(RespawnCoroutine());
    }


    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        GameObject newPlayer = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
        player = newPlayer.GetComponent<Player>();

        // Reactivate any falling platforms that were switched off by the player
        ReactivateAllFallingPlatforms();
    }


    private void ReactivateAllFallingPlatforms()
    {
        // Replace this line in ReactivateAllFallingPlatforms():

        Tra[] platforms = FindObjectsByType<Tra>(FindObjectsSortMode.None);
        foreach (var p in platforms)
        {

            if (p != null)
                p.ReactivatePlatform();
        }
    }

    public void UpdateRespawnPosition(Transform newRespawnPoint) => respawnPoint = newRespawnPoint;

}
