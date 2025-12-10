using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Level Management")]
    [SerializeField] private int currentLevelIndex;

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay;
    public Player player;

    [Header("Fruit Management")]
    public bool fruitsAreRandom;
    public int fruitsCollected;
    public int totalFruits;


    [Header("Checkpoints")]
    public bool canReactivate;

    [Header("Traps")]
    public GameObject arrowPrefab;



    public void Awake()
    {
        if(instance == null) 
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        CollectFruitsInfo();
    }

    private void CollectFruitsInfo()
    {
        Fruit[] allFruits = FindObjectsByType<Fruit>(FindObjectsSortMode.None);
        totalFruits = allFruits.Length;
    }

    public void UpdateRespawnPosition(Transform newRespawnPoint) => respawnPoint = newRespawnPoint; 

    public void RespawnPlayer() => StartCoroutine(RespawnCoroutine());


    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        GameObject newPlayer = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
        player = newPlayer.GetComponent<Player>();
 
    }

    public void AddFruit() => fruitsCollected++;
    public bool FruitsHaveRandomLook() => fruitsAreRandom;


    public void CreateObject(GameObject prefab, Transform target, float delay=0)
    {
        StartCoroutine(CreateObjectCoroutine(prefab, target, delay));
    }
    private IEnumerator CreateObjectCoroutine(GameObject prefab, Transform target, float delay)
    {
        Vector3 newPosition = target.position;
        yield return new WaitForSeconds(delay);
        GameObject newObject = Instantiate(prefab, newPosition, Quaternion.identity);
    }

    private void LoadTheEndScene() => SceneManager.LoadScene("TheEnd");

    private void LoadNextLevel() { 

        int nextLevelIndex = currentLevelIndex + 1;
        SceneManager.LoadScene("Level_"+nextLevelIndex);
    
    
    } 

    public void LevelFinished()
    {
        UI_FadeEffect fadeEffect = UI_InGame.instance.fadeEffect;

        int lastLevelIndex = SceneManager.sceneCountInBuildSettings - 2; 
        bool noMoreLevels = currentLevelIndex == lastLevelIndex;

        if(noMoreLevels)
            fadeEffect.ScreenFade(1, 1.5f, LoadTheEndScene);
        else
            fadeEffect.ScreenFade(1, 1.5f, LoadNextLevel);
    }
}
