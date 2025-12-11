using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private UI_InGame inGameUi;
    
    [Header("Level Management")]
    [SerializeField] private float levelTimer; 
    [SerializeField] private int currentLevelIndex;
    private int nextLevelIndex;

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
        inGameUi = UI_InGame.instance;
        
        currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        nextLevelIndex = currentLevelIndex + 1;
        CollectFruitsInfo();

    }

    private void Update()
    {
        levelTimer += Time.deltaTime;
        inGameUi.UpdateTimerUI(levelTimer);
    }
    private void CollectFruitsInfo()
    {
        Fruit[] allFruits = FindObjectsByType<Fruit>(FindObjectsSortMode.None);
        totalFruits = allFruits.Length;

        inGameUi.UpdateFruitUI(fruitsCollected, totalFruits);

        PlayerPrefs.SetInt("Level" + currentLevelIndex + "TotalFruits", totalFruits);
    }

    public void UpdateRespawnPosition(Transform newRespawnPoint) => respawnPoint = newRespawnPoint; 

    public void RespawnPlayer() => StartCoroutine(RespawnCoroutine());


    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        GameObject newPlayer = Instantiate(playerPrefab, respawnPoint.position, Quaternion.identity);
        player = newPlayer.GetComponent<Player>();
 
    }

    public void AddFruit() 
    {
        fruitsCollected++;
        inGameUi.UpdateFruitUI(fruitsCollected, totalFruits);

    }
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
    public void LevelFinished()
    {
        SaveLevelProgression();
        SaveBestTime();
        SaveFruitsInfo();
        LoadNextScene();
    }

    private void SaveFruitsInfo()
    {
        

        int fruitsCollectedBefore = PlayerPrefs.GetInt("Level" + currentLevelIndex + "FruitsCollected");

        if(fruitsCollectedBefore < fruitsCollected)
            PlayerPrefs.SetInt("Level" + currentLevelIndex + "FruitsCollected", fruitsCollected);

        int totalFruitsInBank = PlayerPrefs.GetInt("TotalFruitsAmount");
        PlayerPrefs.SetInt("TotalFruitsAmount", totalFruitsInBank + fruitsCollected);
    }
    private void SaveBestTime()
    {
        float lastTime = PlayerPrefs.GetFloat("Level" + currentLevelIndex + "BestTime", 99);

        if (levelTimer < lastTime)
            PlayerPrefs.SetFloat("Level" + currentLevelIndex + "BestTime", levelTimer);
    }

    private void SaveLevelProgression()
    {
        PlayerPrefs.SetInt("Level" + nextLevelIndex + "Unlocked", 1);

        if (NoMoreLevels() == false)
            PlayerPrefs.SetInt("ContinueLevelNumber", nextLevelIndex);
    }

    private void LoadTheEndScene() => SceneManager.LoadScene("TheEnd");

    private void LoadNextLevel()
    {

        SceneManager.LoadScene("Level_" + nextLevelIndex);
    } 

    private void LoadNextScene()
    {
        UI_FadeEffect fadeEffect = UI_InGame.instance.fadeEffect;

        if (NoMoreLevels())
            fadeEffect.ScreenFade(1, 1.5f, LoadTheEndScene);
        else
            fadeEffect.ScreenFade(1, 1.5f, LoadNextLevel);
    }

    private bool NoMoreLevels()
    {
        int lastLevelIndex = SceneManager.sceneCountInBuildSettings - 2;
        bool noMoreLevels = currentLevelIndex == lastLevelIndex;

        return noMoreLevels;
    }
}
