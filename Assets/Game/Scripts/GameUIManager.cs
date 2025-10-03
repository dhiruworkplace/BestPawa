using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Solo.MOST_IN_ONE;
using Cinemachine;
using System.Collections;
using System.Data;
using DG.Tweening;
using UnityEngine.Video;
using UnityEngine.SocialPlatforms.Impl;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject startPanel;

    public GameObject gameplayPanel;  // New gameplay UI panel
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject gameCompletePanel, winPanel;
    [Header("UI Elements")]

    public TextMeshProUGUI starsText;
    public TextMeshProUGUI menuStarsText;
    public TextMeshProUGUI rewardText;  // For displaying reward amount or message
    public Button pauseButton;           // Pause button in gameplay UI
    public Image levelFillImage;       // Progress bar using image fill
    public PlayerController playerController;
    public Transform playerTransform;   // Player's transform
    private GameObject finishPoint;       // Finish point transform
    public CinemachineVirtualCamera endCamera;

    public GameObject[] confettis;
    public float confettiDelay = 0.5f; // seconds between each

    [Header("Main Menu Buttons")]

    public Button beginnerModeButton;
    public Button expertModeButton;
    public Button levelButton;
    public Button shopButton;
    public Button settingsButton;

    public Button quitButton;
    public Button dailyReward;
    public Button howtoPlay;
    public Button LeaderBoardButton;

    [Header("Menu Panels")]
    public GameObject levelPanel;

    public GameObject proLevelPanel; // 👈 NEW
    public GameObject shopPanel;
    public GameObject settingPanel;
    public GameObject dailyRewardPanel;
    public GameObject htpPanel;
    public GameObject lbPanel;

    [Header("Tutorial UI")]
    public GameObject tutorialPanel;

    [Header("Score Texts")]
    public TextMeshProUGUI winscoreText;
    public TextMeshProUGUI LosescoreText;



    [Header("Score Texts in Start Panel")]
    public TextMeshProUGUI scoretextBest;
    public TextMeshProUGUI levelTextLatest;

    public Button tapToStart;
    // public TextMeshProUGUI tutorialText;

    public ColorConfig colorConfig; // assign ScriptableObject in Inspector



    private bool isPaused = false;
    private float totalDistance;

    private static bool hasLaunchedBefore = false; // ✅ static flag

    private void Awake()
    {
        ColorManager.Initialize(colorConfig);

        //heck if it's the first time the game runs
        if (!PlayerPrefs.HasKey("isInitialized"))
        {
            ProgressManager.Instance.Progress.stars = 100; // ✅ give starting stars
            // PlayerPrefs.SetInt("coins", 100);   // Give starting coins once
            PlayerPrefs.SetInt("isInitialized", 1); // Mark as initialized
            PlayerPrefs.Save();
        }

        // Always load coin text from PlayerPrefs
        // int coins = PlayerPrefs.GetInt("coins", 0);
        // menuStarsText.text = coins.ToString();

        menuStarsText.text = ProgressManager.Instance.Progress.stars.ToString();

        // Singleton pattern to ensure only one instance
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Optional, if you want to persist between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // void UpdateStarsUI()
    // {
    //     starsText.text = ProgressManager.Instance.Progress.stars.ToString();
    //     //menuStarsText.text = ProgressManager.Instance.Progress.stars.ToString();
    //     int coins = PlayerPrefs.GetInt("coins", 0);
    //     menuStarsText.text = coins.ToString();
    //     int currentLevel = ProgressManager.Instance.Progress.selectedLevelIndex;
    //     levelTextLatest.text = (currentLevel + 1).ToString();
    //     scoretextBest.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
    // }
    public void UpdateStarsUI()
    {
        Debug.Log("ok=");
        starsText.text = ProgressManager.Instance.Progress.stars.ToString();
        // int coins = PlayerPrefs.GetInt("coins", 0);
        // menuStarsText.text = coins.ToString();
        menuStarsText.text = ProgressManager.Instance.Progress.stars.ToString();

        if (GameManager.IsProMode)
        {
            int currentLevel = ProgressManager.Instance.Progress.selectedProLevelIndex;
            levelTextLatest.text = (currentLevel + 1).ToString();
        }
        else
        {
            int currentLevel = ProgressManager.Instance.Progress.selectedLevelIndex;
            levelTextLatest.text = (currentLevel + 1).ToString();
        }

        scoretextBest.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
    }


    private void Start()
    {

        levelComplete = false;


        if (StartDirectly)
        {
            StartDirectly = false; // reset
            StartGameDirectly();           // ✅ jump straight into gameplay
        }
        else
        {

        }
        // Time.timeScale = 0f;  // Pause game at start

        // Setup pause button event
        if (beginnerModeButton != null) beginnerModeButton.onClick.AddListener(OnLevelButton);
        if (expertModeButton != null) expertModeButton.onClick.AddListener(OnProLevelButton);
        if (pauseButton != null) pauseButton.onClick.AddListener(PauseGame);
        if (levelButton != null) levelButton.onClick.AddListener(OnLevelButton);
        if (shopButton != null) shopButton.onClick.AddListener(OnShopButton);
        if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsButton);
        if (quitButton != null) quitButton.onClick.AddListener(OnQuitButton);
        if (tapToStart != null) tapToStart.onClick.AddListener(OnTapStart);
        if (dailyReward != null) dailyReward.onClick.AddListener(OnDailyRewardButton);
        if (howtoPlay != null) howtoPlay.onClick.AddListener(OnHTPButton);
        if (LeaderBoardButton != null) LeaderBoardButton.onClick.AddListener(OnLeaderBoardButton);

        finishPoint = GameObject.FindWithTag("Finish");

        UpdateStarsUI();
    }


    private bool levelComplete = false;
    private void Update()
    {
        if (levelComplete)
            return;

        // 👉 Check for first tap/click anywhere
        if (tapToStart.gameObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
            {
                OnTapStart();
            }
        }

        if (gameplayPanel.activeSelf && playerTransform != null && finishPoint != null)
        {
            UpdateLevelProgress();

            // Check if player reached or passed finish point (using Z-axis)
            if (playerTransform.position.z >= finishPoint.transform.position.z)
            {
                levelComplete = true;
                // Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.Success);
                VibrationManager.Instance.Vibrate(Most_HapticFeedback.HapticTypes.Success);

                LevelComplete();
            }
        }
    }

    public void ShowStartPanel()
    {
        startPanel.SetActive(true);
        gameplayPanel.SetActive(false);
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gameCompletePanel.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
    }

    // public void StartNormalMode()
    // {
    //     SoundManager.Instance.PlayButtonClick();

    //     if (!hasLaunchedBefore)
    //     {
    //         // First launch in this app session → show tutorial
    //         ShowTutorialFirstTime();
    //         hasLaunchedBefore = true; // ✅ mark tutorial as shown
    //     }
    //     else
    //     {
    //         // Not first time → normal start with TapToStart
    //         EnableTapToStart();
    //     }

    //     GameManager.Instance.LoadLevelByMode(false); // normal
    // }

    public void StartProMode()
    {
        SoundManager.Instance.PlayButtonClick();

        // if (!hasLaunchedBefore)
        // {
        //     // First launch in this app session → show tutorial
        //     ShowTutorialFirstTime();
        //     hasLaunchedBefore = true; // ✅ mark tutorial as shown
        // }
        // else
        // {
        //     // Not first time → normal start with TapToStart
        //     EnableTapToStart();
        // }

        GameManager.IsProMode = true;
        GameManager.CurrentProLevelIndex = -1; // force random on first load
        StartDirectly = true;

        // GameManager.Instance.LoadLevelByMode(true); // pro mode
        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    private void ShowTutorialFirstTime()
    {
        // Pause game
        Time.timeScale = 0f;
        isPaused = true;

        startPanel.SetActive(false);
        gameplayPanel.SetActive(true);
        tapToStart.gameObject.SetActive(false);
        tutorialPanel.SetActive(true); // 👈 show tutorial
    }

    public void EnableTapToStart()
    {
        startPanel.SetActive(false);
        gameplayPanel.SetActive(true);
        tapToStart.gameObject.SetActive(true);
    }

    public void OnTapStart()
    {
        tapToStart.gameObject.SetActive(false);

        // Resume game
        Time.timeScale = 1f;
        isPaused = false;

        playerController.StartGame();
    }

    public void StartGameDirectly()
    {
        SoundManager.Instance.PlayButtonClick();
        startPanel.SetActive(false);
        gameplayPanel.SetActive(true);

        if (!hasLaunchedBefore)
        {
            // First launch in this app session → show tutorial
            ShowTutorialFirstTime();
            hasLaunchedBefore = true; // ✅ mark tutorial as shown
        }
        else
        {
            // Not first time → normal start with TapToStart
            EnableTapToStart();
        }

        // EnableTapToStart();

        // Show tutorial only the first time
        // tutorialPanel.SetActive(true);

        // playerController.StartGame();

        // Stop game until tutorial dismissed
        Time.timeScale = 0f;
        isPaused = true;
    }


    private void UpdateLevelProgress()
    {
        float distanceTravelled = playerTransform.position.z - 0f; // Start Z at 0
        float totalDistance = finishPoint.transform.position.z - 0f;          // Finish relative to origin

        float progress = Mathf.Clamp01(distanceTravelled / totalDistance);

        if (levelFillImage != null)
        {
            levelFillImage.fillAmount = progress;
        }
    }

    public static bool StartDirectly = false;

    public void LevelFailed()
    {
        LosescoreText.text = "0";
        SoundManager.Instance.PlayLevelFailed();
        gameOverPanel.SetActive(true);
        // Time.timeScale = 0f;
        // isPaused = true;

        ColorManager.OnGameEnd(); // ✅ centralized call
    }

    // public void LevelComplete()
    // {
    //     SoundManager.Instance.PlayLevelComplete();
    //     endCamera.Priority = 20; // higher than gameplay camera
    //     int currentLevel = ProgressManager.Instance.Progress.selectedLevelIndex;
    //     endCamera.GetComponent<EndLevelCameraRotate>().StartRotation();
    //     int Score = ((currentLevel + 1) * 100);
    //     winscoreText.text = ((currentLevel + 1) * 100).ToString();

    //     if (Score > PlayerPrefs.GetInt("BestScore", 0))
    //     {
    //         PlayerPrefs.SetInt("BestScore", Score);
    //         PlayerPrefs.Save();
    //     }


    //     gameCompletePanel.SetActive(true);
    //     // Time.timeScale = 0f;
    //     playerController.GameOver();

    //     // PlayConfettiSequence();
    //     // isPaused = true;

    //     // --- Unlock next level if it's locked ---
    //     int nextLevel = currentLevel + 1;

    //     if (nextLevel < ProgressManager.Instance.Progress.unlockedLevels.Length &&
    //         !ProgressManager.Instance.Progress.unlockedLevels[nextLevel])
    //     {
    //         ProgressManager.Instance.UnlockLevel(nextLevel);
    //         // Debug.Log($"Level {nextLevel} unlocked!");
    //     }

    //     // calcualate start given to player

    //     int wallCount = playerController.TotalWallToClear();
    //     int stars = wallCount * 5;
    //     ProgressManager.Instance.AddStars(stars);
    //     ShowReward(stars);

    //     ColorManager.OnGameEnd(); // ✅ centralized call
    // }

    private void PlayConfettiSequence()
    {
        StartCoroutine(ConfettiSequenceCoroutine());
    }

    private IEnumerator ConfettiSequenceCoroutine()
    {
        yield return new WaitForSeconds(confettiDelay);

        for (int i = 0; i < confettis.Length; i++)
        {
            confettis[i].SetActive(true);
            yield return new WaitForSeconds(confettiDelay);
        }
    }

    public void LevelComplete()
    {
        SoundManager.Instance.PlayLevelComplete();
        endCamera.Priority = 20;
        endCamera.GetComponent<EndLevelCameraRotate>().StartRotation();

        int currentLevel = GameManager.IsProMode
            ? ProgressManager.Instance.Progress.selectedProLevelIndex
            : ProgressManager.Instance.Progress.selectedLevelIndex;

        int Score = ((currentLevel + 1) * 100);
        winscoreText.text = Score.ToString();

        if (Score > PlayerPrefs.GetInt("BestScore", 0))
        {
            PlayerPrefs.SetInt("BestScore", Score);
            PlayerPrefs.Save();
        }

        gameCompletePanel.SetActive(false);
        playerController.GameOver();

        int nextLevel = currentLevel + 1;

        if (GameManager.IsProMode)
        {
            if (nextLevel < ProgressManager.Instance.Progress.proUnlockedLevels.Length &&
                !ProgressManager.Instance.Progress.proUnlockedLevels[nextLevel])
            {
                ProgressManager.Instance.UnlockProLevel(nextLevel);
            }
        }
        else
        {
            if (nextLevel < ProgressManager.Instance.Progress.unlockedLevels.Length &&
                !ProgressManager.Instance.Progress.unlockedLevels[nextLevel])
            {
                ProgressManager.Instance.UnlockLevel(nextLevel);
            }
        }

        // calculate stars reward
        int wallCount = playerController.TotalWallToClear();
        int stars = wallCount * 5;
        if (LevelLoader.gameType.Equals(GameType.Challenge))
            stars = AddCoin();
        ProgressManager.Instance.AddStars(stars); // shared pool
        ShowReward(stars);

        ColorManager.OnGameEnd();

        //gameCompletePanel.SetActive(true);
        PlayConfettiSequence();

        // ✅ Delay popup
        StartCoroutine(ShowWinPopupWithDelay(2f));

        for (int i = 0; i < GameManager.letters.Count; i++)
        {
            Debug.Log("l : " + GameManager.letters[i]);
        }
    }

    private int AddCoin()
    {
        if (LevelLoader.selectedChall.Equals(0))
            return 100;
        else if (LevelLoader.selectedChall.Equals(1))
            return 100;
        else if (LevelLoader.selectedChall.Equals(2))
            return 100;
        else if (LevelLoader.selectedChall.Equals(3))
            return 150;
        else if (LevelLoader.selectedChall.Equals(4))
            return 150;
        else if (LevelLoader.selectedChall.Equals(5))
            return 200;
        else if (LevelLoader.selectedChall.Equals(6))
            return 200;
        else
            return 100;
    }

    private IEnumerator ShowWinPopupWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameCompletePanel.SetActive(true);

    }



    public void ShowReward(int rewardAmount)
    {
        rewardText.text = rewardAmount.ToString();
        // PlayerPrefs.SetInt("coins", rewardAmount + PlayerPrefs.GetInt("coins"));
        // PlayerPrefs.Save();
        UpdateStarsUI();
        // Add reward logic here
    }


    private void OnLevelButton()
    {
        LevelLoader.gameType = GameType.Beginner;
        SoundManager.Instance.PlayButtonClick();
        levelPanel.SetActive(true);
        // Debug.Log("Open Level Selection Panel");
        // Example: SceneManager.LoadScene("LevelSelection");
        // Or: enable your LevelSelection UI panel
    }

    private void OnProLevelButton()
    {
        LevelLoader.gameType = GameType.Expert;
        SoundManager.Instance.PlayButtonClick();
        proLevelPanel.SetActive(true); // open Pro level selection
    }

    private void OnLeaderBoardButton()
    {
        SoundManager.Instance.PlayButtonClick();
        lbPanel.SetActive(true);
        // Debug.Log("Open Level Selection Panel");
        // Example: SceneManager.LoadScene("LevelSelection");
        // Or: enable your LevelSelection UI panel
    }

    private void OnShopButton()
    {
        SoundManager.Instance.PlayButtonClick();
        shopPanel.SetActive(true);
        // Debug.Log("Open Shop Panel");
        // Example: SceneManager.LoadScene("Shop");
        // Or: enable your Shop UI panel
    }
    private void OnDailyRewardButton()
    {
        SoundManager.Instance.PlayButtonClick();
        dailyRewardPanel.SetActive(true);
        // Debug.Log("Open Shop Panel");
        // Example: SceneManager.LoadScene("Shop");
        // Or: enable your Shop UI panel
    }
    private void OnHTPButton()
    {
        SoundManager.Instance.PlayButtonClick();
        htpPanel.SetActive(true);
        // Debug.Log("Open Shop Panel");
        // Example: SceneManager.LoadScene("Shop");
        // Or: enable your Shop UI panel
    }
    private void OnSettingsButton()
    {
        SoundManager.Instance.PlayButtonClick();
        settingPanel.SetActive(true);
        // Debug.Log("Open Settings Panel");
        // Example: open a settings menu GameObject
    }


    // public void LoadNextLevel()
    // {
    //     SoundManager.Instance.PlayButtonClick();

    //     if (GameManager.IsProMode)
    //     {
    //         // pick new random level between 10–30
    //         GameManager.CurrentProLevelIndex = Random.Range(10, 31);
    //         StartDirectly = true;
    //         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    //         return;
    //     }

    //     // Get current level index
    //     int currentLevel = ProgressManager.Instance.Progress.selectedLevelIndex;
    //     int nextLevel = currentLevel + 1;

    //     // Check if next level exists
    //     if (nextLevel < ProgressManager.Instance.Progress.unlockedLevels.Length)
    //     {
    //         // Ensure the next level is unlocked
    //         ProgressManager.Instance.UnlockLevel(nextLevel);

    //         // Select the next level in progress
    //         ProgressManager.Instance.SelectLevel(nextLevel);

    //         StartDirectly = true; // ✅ tell the next scene to skip main menu

    //         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    //     }
    //     else
    //     {
    //         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    //         Debug.Log("No more levels available!");
    //     }
    // }

    public void LoadNextLevel()
    {
        SoundManager.Instance.PlayButtonClick();

        if (GameManager.IsProMode)
        {
            int currentLevel = ProgressManager.Instance.Progress.selectedProLevelIndex;
            int nextLevel = currentLevel + 1;

            if (nextLevel < ProgressManager.Instance.Progress.proUnlockedLevels.Length)
            {
                ProgressManager.Instance.UnlockProLevel(nextLevel);
                ProgressManager.Instance.SelectProLevel(nextLevel);

                StartDirectly = true;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                Debug.Log("No more Pro levels available!");
            }
        }
        else
        {
            int currentLevel = ProgressManager.Instance.Progress.selectedLevelIndex;
            int nextLevel = currentLevel + 1;

            if (nextLevel < ProgressManager.Instance.Progress.unlockedLevels.Length)
            {
                ProgressManager.Instance.UnlockLevel(nextLevel);
                ProgressManager.Instance.SelectLevel(nextLevel);

                StartDirectly = true;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                Debug.Log("No more Classic levels available!");
            }
        }
    }

    public void PauseGame()
    {
        if (isPaused) return;

        SoundManager.Instance.PlayButtonClick();
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        SoundManager.Instance.PlayButtonClick();
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void RestartGame()
    {
        SoundManager.Instance.PlayButtonClick();
        Time.timeScale = 1f;
        StartDirectly = true; // ✅ tell the next scene to skip main menu
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Home()
    {
        SoundManager.Instance.PlayButtonClick();
        Time.timeScale = 1f;
        StartDirectly = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnQuitButton()
    {
        SoundManager.Instance.PlayButtonClick();
        Application.Quit();
    }

    public void ExitGame()
    {
        SoundManager.Instance.PlayButtonClick();
        Application.Quit();
    }


}
