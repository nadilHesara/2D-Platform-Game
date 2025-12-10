using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    private UI_FadeEffect fadeEffect;
    public string sceneName;

    private void Awake()
    {
        fadeEffect = GetComponentInChildren<UI_FadeEffect>();
    }


    private void Start()
    {
        fadeEffect.ScreenFade(0, 1.5f);
    }

    public void NewGame()
    {
        fadeEffect.ScreenFade(1, 1.5f, LoadLevelScene);
    }

    private void LoadLevelScene()=> SceneManager.LoadScene(sceneName);
}
