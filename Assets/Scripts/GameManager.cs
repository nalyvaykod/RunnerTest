using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Progression")]
    [SerializeField] private int baseCoinsToWin = 10;
    [SerializeField] private int coinsIncreasePerLevel = 2;
    private int coinsToWin;
    private int currentCoins = 0;
    private int currentLevel = 1;

    [Header("UI References")]
    private TextMeshProUGUI coinCountText;
    private TextMeshProUGUI levelText;


    [Header("Scene Names")]
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private string winSceneName = "WinScene";
    [SerializeField] private string loseSceneName = "LoseScene";
    [SerializeField] private string menuSceneName = "Menu";

    private bool gameEnded = false;

    private const string CurrentLevelKey = "CurrentLevel";


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeGame();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Time.timeScale = 1f;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");

        if (scene.name == gameSceneName)
        {
            FindAndAssignUIElements();

            InitializeGame();

        }
        else if (scene.name == winSceneName || scene.name == loseSceneName || scene.name == menuSceneName)
        {
            Time.timeScale = 0f;
            coinCountText = null;
            levelText = null;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    private void FindAndAssignUIElements()
    {
        GameObject coinTextGO = GameObject.Find("CoinCountText");
        GameObject levelTextGO = GameObject.Find("LevelText");

        if (coinTextGO != null)
        {
            coinCountText = coinTextGO.GetComponent<TextMeshProUGUI>();
            if (coinCountText == null)
            {
                Debug.LogError("Coin Count Text GO found, but TextMeshProUGUI component missing!");
            }
        }
        else
        {
            Debug.LogError("Coin Count Text (TextMeshProUGUI) object with name 'CoinCountText' not found on the scene! Check spelling and object activity.");
        }

        if (levelTextGO != null)
        {
            levelText = levelTextGO.GetComponent<TextMeshProUGUI>();
            if (levelText == null)
            {
                Debug.LogError("Level Text GO found, but TextMeshProUGUI component missing!");
            }
        }
        else
        {
            Debug.LogError("Level Text (TextMeshProUGUI) object with name 'LevelText' not found on the scene! Check spelling and object activity.");
        }

        UpdateCoinUI();
        UpdateLevelUI();
    }

    private void InitializeGame()
    {
        currentLevel = PlayerPrefs.GetInt(CurrentLevelKey, 1);
        coinsToWin = baseCoinsToWin + (currentLevel - 1) * coinsIncreasePerLevel;
        currentCoins = 0;
        gameEnded = false;
        Time.timeScale = 1f;

        Debug.Log($"Game Initialized for Level {currentLevel}: Coins to Win {coinsToWin}, Current Coins {currentCoins}");
    }

    void Update()
    {
        if (gameEnded) return;
    }

    public void CollectCoin()
    {
        if (gameEnded) return;

        currentCoins++;
        Debug.Log("Coin collected! Total: " + currentCoins);
        UpdateCoinUI();

        if (currentCoins >= coinsToWin)
        {
            WinGame();
        }
    }

    private void WinGame()
    {
        if (gameEnded) return;
        gameEnded = true;
        Debug.Log("You Win!");

        currentLevel++;
        PlayerPrefs.SetInt(CurrentLevelKey, currentLevel);
        PlayerPrefs.Save();

        Time.timeScale = 0f;
        SceneManager.LoadScene(winSceneName);
    }

    public void LoseGame()
    {
        if (gameEnded) return;
        gameEnded = true;
        Debug.Log("You Lose!");

        Time.timeScale = 0f;
        SceneManager.LoadScene(loseSceneName);
    }

    private void UpdateCoinUI()
    {
        if (coinCountText != null)
        {
            coinCountText.text = "Coins: " + currentCoins + " / " + coinsToWin;
        }
        else
        {
            Debug.LogWarning("Coin Count Text (TextMeshProUGUI) is null. Cannot update UI. Ensure 'CoinCountText' object exists and is active on the scene.");
        }
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + currentLevel;
        }
        else
        {
            Debug.LogWarning("Level Text (TextMeshProUGUI) is null. Cannot update UI. Ensure 'LevelText' object exists and is active on the scene.");
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restarting current game scene...");
        SceneManager.LoadScene(gameSceneName);
    }

    public void GoToNextLevel()
    {
        Debug.Log("Going to next level...");
        SceneManager.LoadScene(gameSceneName);
    }

    public void GoToMainMenu()
    {
        Debug.Log("Going to Main Menu...");
        SceneManager.LoadScene(menuSceneName);
    }

    public bool IsGameEnded()
    {
        return gameEnded;
    }
}
