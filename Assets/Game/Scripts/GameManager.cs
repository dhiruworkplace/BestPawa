using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level Loading")]
    public LevelLoader levelLoader;
    [Header("Levels")]
    public List<LevelData> classicLevels; // assign in inspector
    public List<LevelData> proLevels;     // assign in inspector
    public Challanges[] challengeLevels;

    [Header("Mode Flags")]
    public static bool IsProMode = false;
    public static int CurrentProLevelIndex = -1; // stores last Pro Mode level
    public static int selectedCube = 0;
    public static List<string> letters = new List<string>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // After reload, check if we should load a level directly
        if (GameUIManager.StartDirectly)
        {
            LoadLevelByMode(LevelLoader.gameType);
        }

        // ❌ No auto-load here — menu buttons will call LoadLevelByMode()
    }

    /// <summary>
    /// Loads a level depending on Normal or Pro Mode
    /// </summary>
    public void LoadLevelByMode(GameType gameType)
    {
        IsProMode = LevelLoader.gameType.Equals(GameType.Expert);

        int levelToLoad = IsProMode ? GetProLevelToLoad() : GetClassicLevelToLoad();

        if (LevelLoader.gameType.Equals(GameType.Expert))
        {
            if (levelToLoad >= 0 && levelToLoad < proLevels.Count)
                levelLoader.LoadLevel(proLevels[levelToLoad]);
            else
                Debug.LogError("Invalid Pro level index: " + levelToLoad);
        }
        else if (LevelLoader.gameType.Equals(GameType.Beginner))
        {
            if (levelToLoad >= 0 && levelToLoad < classicLevels.Count)
                levelLoader.LoadLevel(classicLevels[levelToLoad]);
            else
                Debug.LogError("Invalid Classic level index: " + levelToLoad);
        }
        else
        {
            levelLoader.LoadLevel(challengeLevels[LevelLoader.selectedChall].levels[Random.Range(0, 3)]);
        }
    }

    /// <summary>
    /// For Classic Mode: returns selected or last unlocked level
    /// </summary>
    private int GetClassicLevelToLoad()
    {
        var progress = ProgressManager.Instance.Progress;

        // If user selected a valid unlocked level → load it
        if (progress.selectedLevelIndex >= 0 &&
            progress.selectedLevelIndex < classicLevels.Count &&
            progress.unlockedLevels[progress.selectedLevelIndex])
        {
            return progress.selectedLevelIndex;
        }

        // Otherwise → load last unlocked (default 0 if nothing unlocked)
        int lastUnlocked = Mathf.Clamp(progress.lastUnlockedLevel - 1, 0, classicLevels.Count - 1);
        return lastUnlocked;
    }

    /// <summary>
    /// For Pro Mode: returns selected or last unlocked level
    /// </summary>
    private int GetProLevelToLoad()
    {
        var progress = ProgressManager.Instance.Progress;

        // If user selected a valid unlocked level → load it
        if (progress.selectedProLevelIndex >= 0 &&
            progress.selectedProLevelIndex < proLevels.Count &&
            progress.proUnlockedLevels[progress.selectedProLevelIndex])
        {
            return progress.selectedProLevelIndex;
        }

        // Otherwise → load last unlocked (default 0 if nothing unlocked)
        int lastUnlocked = Mathf.Clamp(progress.proLastUnlockedLevel - 1, 0, proLevels.Count - 1);
        return lastUnlocked;
    }

    public void SelectChallenge(int index)
    {
        LevelLoader.gameType = GameType.Challenge;
        LevelLoader.selectedChall = index;

        GameUIManager.StartDirectly = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Picks a new random Pro Mode level (for Next Level button)
    /// </summary>
    // public void LoadRandomProLevel()
    // {
    //     CurrentProLevelIndex = Random.Range(10, 31);
    //     LoadLevelByMode(true);
    // }

    /// <summary>
    /// For Normal Mode: returns selected or last unlocked level
    /// </summary>
    // private int GetNormalLevelToLoad()
    // {
    //     var progress = ProgressManager.Instance.Progress;

    //     // If user selected a valid unlocked level → load it
    //     if (progress.selectedLevelIndex >= 0 &&
    //         progress.selectedLevelIndex < levels.Count &&
    //         progress.unlockedLevels[progress.selectedLevelIndex])
    //     {
    //         return progress.selectedLevelIndex;
    //     }

    //     // Otherwise → load last unlocked (default 0 if nothing unlocked)
    //     int lastUnlocked = Mathf.Clamp(progress.lastUnlockedLevel - 1, 0, levels.Count - 1);
    //     return lastUnlocked;
    // }    
}

[System.Serializable]
public class Challanges {
    public List<LevelData> levels;
}