using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelSelectionManager : MonoBehaviour
{
    [Header("Level Data")]
    // public int classicLevels = 30;
    // public int proLevels = 30;
    public GameObject levelItemPrefab;
    public Transform levelContent;

    [Header("Mode")]
    public bool isProMode = false; // 👈 toggle in inspector or set via GameUIManager

    [Header("Navigation")]
    public Button backButton;

    void Start()
    {
        PopulateLevels();

        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);
    }


    // void PopulateLevels()
    // {
    //     foreach (Transform child in levelContent)
    //         Destroy(child.gameObject);

    //     var progress = ProgressManager.Instance.Progress;

    //     for (int i = 0; i < totalLevels; i++)
    //     {
    //         int levelIndex = i + 1;
    //         var itemObj = Instantiate(levelItemPrefab, levelContent);
    //         var levelItemUI = itemObj.GetComponent<LevelItemUI>();

    //         bool unlocked = (i < progress.unlockedLevels.Length) && progress.unlockedLevels[i];

    //         // Derive states from unlocked / lastUnlockedLevel
    //         bool completed = i < (progress.lastUnlockedLevel - 1);   // all before last unlocked
    //         bool current = i == (progress.lastUnlockedLevel - 1);    // last unlocked = current

    //         levelItemUI.Setup(
    //             levelIndex,
    //             completed,
    //             current,
    //             unlocked,
    //             OnLevelSelected
    //         );
    //     }
    // }



    // void OnLevelSelected(int levelIndex)
    // {
    //     SoundManager.Instance.PlayButtonClick();
    //     var progress = ProgressManager.Instance.Progress;

    //     if (progress.unlockedLevels[levelIndex - 1])
    //     {
    //         // Save selected level
    //         ProgressManager.Instance.SelectLevel(levelIndex - 1);

    //         // Tell GameManager this is NOT Pro Mode
    //         GameManager.IsProMode = false;
    //         GameManager.CurrentProLevelIndex = -1;

    //         // Debug.Log("Starting Level " + levelIndex);
    //         // Example: SceneManager.LoadScene("Level" + levelIndex);
    //         GameUIManager.StartDirectly = true;
    //         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    //     }
    //     else
    //     {
    //         // Debug.Log("Level " + levelIndex + " is locked!");
    //     }
    // }

    void PopulateLevels()
    {
        foreach (Transform child in levelContent)
            Destroy(child.gameObject);

        var progress = ProgressManager.Instance.Progress;

        if (isProMode)
        {
            bool[] unlocked = progress.proUnlockedLevels;
            int lastUnlocked = progress.proLastUnlockedLevel;

            for (int i = 0; i < ProgressManager.Instance.totalProLevels; i++)
            {
                int levelIndex = i + 1;
                var itemObj = Instantiate(levelItemPrefab, levelContent);
                var levelItemUI = itemObj.GetComponent<LevelItemUI>();

                bool isUnlocked = (i < unlocked.Length) && unlocked[i];
                bool completed = i < (lastUnlocked - 1);
                bool current = i == (lastUnlocked - 1);

                levelItemUI.Setup(levelIndex, completed, current, isUnlocked, OnProLevelSelected);
            }
        }
        else
        {
            bool[] unlocked = progress.unlockedLevels;
            int lastUnlocked = progress.lastUnlockedLevel;

            for (int i = 0; i < ProgressManager.Instance.totalLevels; i++)
            {
                int levelIndex = i + 1;
                var itemObj = Instantiate(levelItemPrefab, levelContent);
                var levelItemUI = itemObj.GetComponent<LevelItemUI>();

                bool isUnlocked = (i < unlocked.Length) && unlocked[i];
                bool completed = i < (lastUnlocked - 1);
                bool current = i == (lastUnlocked - 1);

                levelItemUI.Setup(levelIndex, completed, current, isUnlocked, OnClassicLevelSelected);
            }
        }
    }


    void OnClassicLevelSelected(int levelIndex)
    {
        SoundManager.Instance.PlayButtonClick();
        var progress = ProgressManager.Instance.Progress;

        if (progress.unlockedLevels[levelIndex - 1])
        {
            ProgressManager.Instance.SelectLevel(levelIndex - 1);
            GameManager.IsProMode = false;

            GameUIManager.StartDirectly = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void OnProLevelSelected(int levelIndex)
    {
        SoundManager.Instance.PlayButtonClick();
        var progress = ProgressManager.Instance.Progress;

        if (progress.proUnlockedLevels[levelIndex - 1])
        {
            ProgressManager.Instance.SelectProLevel(levelIndex - 1);
            GameManager.IsProMode = true;

            GameUIManager.StartDirectly = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }


    // public void UnlockNextLevel(int completedLevelIndex)
    // {
    //     ProgressManager.Instance.UnlockNextLevel(completedLevelIndex);
    //     PopulateLevels();
    // }

    public void OnBackButton()
    {
        SoundManager.Instance.PlayButtonClick();
        gameObject.SetActive(false);
    }
}
