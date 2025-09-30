using System;
using UnityEngine;

[System.Serializable]
public class GameProgress
{
    public int stars;
    public bool[] unlockedCharacters;
    public int selectedCharacterIndex;

    // Classic Mode
    public bool[] unlockedLevels;
    public int lastUnlockedLevel;

    public int selectedLevelIndex;  // level currently chosen

    // Pro Mode 👇
    public int proStars;
    public bool[] proUnlockedLevels;
    public int proLastUnlockedLevel;
    public int selectedProLevelIndex;
}

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }
    public GameProgress Progress { get; private set; }

    [Header("Config")]
    public int totalCharacters = 5; // adjust in Inspector
    public int totalLevels = 30;    // adjust in Inspector
    public int totalProLevels = 30; // Pro mode levels 👈 NEW

    private const string SAVE_KEY = "GameProgress";

    // Event fired when character is selected
    public event Action<int> OnCharacterSelected;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Save / Load
    public void LoadProgress()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            Progress = JsonUtility.FromJson<GameProgress>(PlayerPrefs.GetString(SAVE_KEY));
        }
        else
        {
            InitializeProgress();
            SaveProgress();
        }

        // Ensure arrays match current config
        if (Progress.unlockedCharacters == null || Progress.unlockedCharacters.Length != totalCharacters)
            Progress.unlockedCharacters = new bool[totalCharacters];

        if (Progress.unlockedLevels == null || Progress.unlockedLevels.Length != totalLevels)
            Progress.unlockedLevels = new bool[totalLevels];

        if (Progress.proUnlockedLevels == null || Progress.proUnlockedLevels.Length != totalProLevels)
            Progress.proUnlockedLevels = new bool[totalProLevels];
    }

    private void InitializeProgress()
    {
        Progress = new GameProgress
        {
            stars = 0,
            unlockedCharacters = new bool[totalCharacters],
            selectedCharacterIndex = 0,

            unlockedLevels = new bool[totalLevels],
            lastUnlockedLevel = 1,
            selectedLevelIndex = 0,

            proUnlockedLevels = new bool[totalProLevels],
            proLastUnlockedLevel = 1,
            selectedProLevelIndex = 0
        };

        // Unlock first character & select it
        if (totalCharacters > 0)
        {
            Progress.unlockedCharacters[0] = true;
            Progress.selectedCharacterIndex = 0;
        }

        // Unlock first level
        if (totalLevels > 0)
        {
            Progress.unlockedLevels[0] = true;
            Progress.lastUnlockedLevel = 1;
        }

        if (totalProLevels > 0)
        {
            Progress.proUnlockedLevels[0] = true;
            Progress.proLastUnlockedLevel = 1;
        }
    }

    public void SaveProgress()
    {
        string json = JsonUtility.ToJson(Progress);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    public void DeleteProgress()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.Save();
        InitializeProgress();
        SaveProgress();
        Debug.Log("Progress reset.");
    }
    #endregion

    #region Helper Methods
    public bool SpendStars(int amount)
    {
        if (Progress.stars >= amount)
        {
            Progress.stars -= amount;
            SaveProgress();
            return true;
        }
        return false;
    }

    public void AddStars(int amount)
    {
        Progress.stars += amount;
        SaveProgress();
    }

    public void UnlockCharacter(int index)
    {
        if (index >= 0 && index < Progress.unlockedCharacters.Length)
        {
            Progress.unlockedCharacters[index] = true;
            SaveProgress();
        }
    }

    public void SelectCharacter(int index)
    {
        if (index >= 0 && index < Progress.unlockedCharacters.Length &&
            Progress.unlockedCharacters[index])
        {
            Progress.selectedCharacterIndex = index;
            SaveProgress();

            OnCharacterSelected?.Invoke(index); // 🔔 notify listeners
        }
    }

    public void UnlockLevel(int index)
    {
        if (index >= 0 && index < Progress.unlockedLevels.Length)
        {
            Progress.unlockedLevels[index] = true;
            if (index + 1 > Progress.lastUnlockedLevel)
                Progress.lastUnlockedLevel = index + 1;
            SaveProgress();
        }
    }

    public void UnlockProLevel(int index)
    {
        if (index >= 0 && index < Progress.proUnlockedLevels.Length)
        {
            Progress.proUnlockedLevels[index] = true;
            if (index + 1 > Progress.proLastUnlockedLevel)
                Progress.proLastUnlockedLevel = index + 1;
            SaveProgress();
        }
    }


    /// <summary>
    /// Unlocks the next level after the given one.
    /// </summary>
    public void UnlockNextLevel(int currentLevelIndex)
    {
        int nextIndex = currentLevelIndex + 1;
        if (nextIndex < Progress.unlockedLevels.Length)
        {
            UnlockLevel(nextIndex);
        }
    }

    public void UnlockNextProLevel(int currentLevelIndex)
    {
        int nextIndex = currentLevelIndex + 1;
        if (nextIndex < Progress.proUnlockedLevels.Length)
            UnlockProLevel(nextIndex);
    }

    /// <summary>
    /// Select a level for playing.
    /// </summary>
    public void SelectLevel(int index)
    {
        if (index >= 0 && index < Progress.unlockedLevels.Length &&
            Progress.unlockedLevels[index])
        {
            Progress.selectedLevelIndex = index;
            SaveProgress();
        }
    }

    public void SelectProLevel(int index)
    {
        if (index >= 0 && index < Progress.proUnlockedLevels.Length &&
            Progress.proUnlockedLevels[index])
        {
            Progress.selectedProLevelIndex = index;
            SaveProgress();
        }
    }


    #endregion
}
