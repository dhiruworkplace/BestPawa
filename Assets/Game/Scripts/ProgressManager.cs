using System;
using TMPro;
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
    public Action<int> OnCharacterSelected;

    public TextMeshProUGUI questTimer;

    private void Awake()
    {
        Instance = this;
        LoadProgress();
    }

    private void Start()
    {
        InvokeRepeating(nameof(CheckQuest), 0f, 1f);
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

    private void StopTimer()
    {
        CancelInvoke(nameof(CheckQuest));
    }

    private void CheckQuest()
    {
        DateTime lastDT = new DateTime();
        if (!PlayerPrefs.HasKey("rewardTime"))
        {
            //PlayerPrefs.SetString("lastSpin", DateTime.Now.AddHours(24).ToString());
            //PlayerPrefs.Save();

            questTimer.text = "Claim";
            StopTimer();
            return;
        }
        else
        {
            int inx = PlayerPrefs.GetInt("dayValue", 0);
            if (inx >= 7)
            {
                questTimer.text = "All Claimed";
                StopTimer();
                return;
            }
        }
        lastDT = DateTime.Parse(PlayerPrefs.GetString("rewardTime"));

        TimeSpan diff = (lastDT - DateTime.Now);
        questTimer.text = string.Format("{0:D2}:{1:D2}:{2:D2}", diff.Hours, diff.Minutes, diff.Seconds);

        if (diff.TotalSeconds <= 0)
        {
            StopTimer();
            questTimer.text = "Claim";
        }
    }

    public void StartTimer()
    {
        PlayerPrefs.SetString("rewardTime", DateTime.Now.AddHours(24).ToString());
        PlayerPrefs.Save();

        InvokeRepeating(nameof(CheckQuest), 0f, 1f);
    }
}