using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_InGame : MonoBehaviour
{
    public static UI_InGame instance;
    public UI_FadeEffect fadeEffect { get; private set; }

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI fruitText;

    [SerializeField] private GameObject pauseUI;


    private bool isPaused;
    private void Awake()
    {
        instance = this;
        fadeEffect = GetComponentInChildren<UI_FadeEffect>();
        
        //note below
        ResetPauseState();
    }

    private void Start()
    {
        fadeEffect.ScreenFade(0, 1);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            PauseButton();
    }

    //public void PauseButton()
    //{
    //    if (isPaused)
    //    {
    //        isPaused = false;
    //        Time.timeScale = 1;
    //        pauseUI.SetActive(false);
    //    }
    //    else
    //    {
    //        isPaused = true;
    //        Time.timeScale = 0;
    //        pauseUI.SetActive(true);
    //    }
    //}

    public void PauseButton()
    {
        if (isPaused) ResetPauseState();
        else SetPausedState();
    }

    private void SetPausedState()
    {
        isPaused = true;
        Time.timeScale = 0;
        if (pauseUI != null) pauseUI.SetActive(true);
    }

    private void ResetPauseState()
    {
        isPaused = false;
        Time.timeScale = 1;
        if (pauseUI != null) pauseUI.SetActive(false);
    }



    public void GoToMainMenuButton()
    {
        ResetPauseState();

        SceneManager.LoadScene(0);
    }
    public void UpdateFruitUI(int collectedFruits, int totalFruits)
    {
        fruitText.text = collectedFruits + "/" + totalFruits;
    }

    public void UpdateTimerUI(float timer)
    {
        timerText.text = timer.ToString("00") + " s";
    }

    private void OnDestroy()
    {
        // Extra safety in case this object gets destroyed while paused
        Time.timeScale = 1;
    }


}
