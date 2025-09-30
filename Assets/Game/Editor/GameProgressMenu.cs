using UnityEngine;
using UnityEditor;

public static class GameProgressMenu
{
    [MenuItem("Game Tools/Delete Game Progress", false, 1)]
    public static void DeleteGameProgress()
    {
        if (EditorUtility.DisplayDialog(
            "Delete Game Progress",
            "Are you sure you want to delete all saved game progress?",
            "Yes", "No"))
        {
            PlayerPrefs.DeleteKey("GameProgress");
            PlayerPrefs.Save();
            Debug.Log("Game progress deleted from Editor.");
        }
    }

    [MenuItem("Game Tools/Add 100 Coins", false, 2)]
    public static void AddCoins()
    {
        var progress = ProgressManager.Instance.Progress;
        progress.stars += 100;
        ProgressManager.Instance.SaveProgress();
        Debug.Log("Added 100 coins. Current total: " + progress.stars);
    }

    [MenuItem("Game Tools/Unlock All Levels", false, 3)]
    public static void UnlockAllLevels()
    {
        var progress = ProgressManager.Instance.Progress;
        for (int i = 0; i < progress.unlockedLevels.Length; i++)
            progress.unlockedLevels[i] = true;

        progress.lastUnlockedLevel = progress.unlockedLevels.Length;
        ProgressManager.Instance.SaveProgress();
        Debug.Log("All levels unlocked!");
    }

    [MenuItem("Game Tools/Unlock Next Level", false, 4)]
    public static void UnlockNextLevel()
    {
        var progress = ProgressManager.Instance.Progress;
        if (progress.lastUnlockedLevel < progress.unlockedLevels.Length)
        {
            progress.unlockedLevels[progress.lastUnlockedLevel] = true;
            progress.lastUnlockedLevel++;
            ProgressManager.Instance.SaveProgress();
            Debug.Log("Unlocked level " + progress.lastUnlockedLevel);
        }
        else
        {
            Debug.Log("All levels already unlocked.");
        }
    }
}
