using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    public string sceneName;

    public void NewGame()
    {
        SceneManager.LoadScene(sceneName);
    }
}
