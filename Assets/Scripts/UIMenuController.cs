using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMenuController : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "Menu";
    [SerializeField] private string gameSceneName = "LevelScene";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

    }

    public void OnRetryButtonClick()
    {
        Debug.Log("Retry button clicked!");
        SceneManager.LoadScene(gameSceneName);

    }

    public void OnMainMenuButtonClick()
    {
        Debug.Log("Main Menu button clicked!");
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnPlayButtonClick()
    {
        Debug.Log("PLAY button clicked! Loading game scene...");
        PlayerPrefs.SetInt("CurrentLevel", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnNextLevelButtonClick()
    {
        Debug.Log("Next Level button clicked! Loading next level...");

        SceneManager.LoadScene(gameSceneName);
    }
}