using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_LevelSelection : MonoBehaviour
{
    [SerializeField] private UI_LevelButton buttonPrefab;
    [SerializeField] private Transform buttonsParent;

    private void Start()
    {
        CreateLevelButtons();
    }
    public void CreateLevelButtons()
    {
        int levelsAmount = SceneManager.sceneCountInBuildSettings - 1;

        for(int i = 1; i < levelsAmount; i++)
        {
            UI_LevelButton newButton = Instantiate(buttonPrefab, buttonsParent);
            newButton.SetupButton(i);
        }
    }
}
