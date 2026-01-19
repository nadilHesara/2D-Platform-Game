using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaWaveController : MonoBehaviour
{
    [Header("Arena Trigger (this same object should have BoxCollider2D IsTrigger)")]
    [SerializeField] private Collider2D arenaTrigger;

    [Header("Doors")]
    [SerializeField] private ArenaDoorCloser2D doorCloser;

    [Header("Spawning")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float timeBetweenSpawns = 0.2f;
    [SerializeField] private float timeBetweenWaves = 1.0f;

    [System.Serializable]
    public class SpawnGroup
    {
        public GameObject enemyPrefab;
        public int count = 3;
    }

    [System.Serializable]
    public class Wave
    {
        public List<SpawnGroup> groups = new List<SpawnGroup>();
    }

    [Header("Waves")]
    [SerializeField] private List<Wave> waves = new List<Wave>();

    private int aliveEnemies;
    private bool arenaStarted;

    private void OnEnable()
    {
        Enemy.OnAnyEnemyDied += HandleEnemyDied;
    }

    private void OnDisable()
    {
        Enemy.OnAnyEnemyDied -= HandleEnemyDied;
    }

    private void Awake()
    {
        if (arenaTrigger == null)
            arenaTrigger = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (arenaStarted) return;

        if (collision.CompareTag("Player"))
        {
            arenaStarted = true;

            // 1) Close doors immediately
            if (doorCloser != null)
                doorCloser.CloseDoors();

            // 2) Stop re-triggering
            if (arenaTrigger != null)
                arenaTrigger.enabled = false;

            // 3) Start waves
            StartCoroutine(RunWaves());
        }
    }

    private IEnumerator RunWaves()
    {
        for (int w = 0; w < waves.Count; w++)
        {
            yield return StartCoroutine(SpawnWave(waves[w]));

            // Wait until all enemies from this wave are dead
            while (aliveEnemies > 0)
                yield return null;

            yield return new WaitForSeconds(timeBetweenWaves);
        }

        // All waves done -> open doors
        if (doorCloser != null)
            doorCloser.OpenDoors();
    }

    private IEnumerator SpawnWave(Wave wave)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("ArenaWaveController: No spawnPoints assigned!");
            yield break;
        }

        foreach (var group in wave.groups)
        {
            if (group.enemyPrefab == null || group.count <= 0)
                continue;

            for (int i = 0; i < group.count; i++)
            {
                Transform sp = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
                Instantiate(group.enemyPrefab, sp.position, Quaternion.identity);

                aliveEnemies++;

                yield return new WaitForSeconds(timeBetweenSpawns);
            }
        }
    }

    private void HandleEnemyDied(Enemy enemy)
    {
        if (!arenaStarted) return;
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);
    }
}
