using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelItemUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text levelText;
    public Button playButton;
    public Image backgroundImage;

    [Header("Sprites")]
    public Sprite completedSprite;
    public Sprite currentSprite;
    public Sprite lockedSprite;

    private int levelIndex;
    private System.Action<int> onClick;

    /// <summary>
    /// Setup the level item UI
    /// </summary>
    /// <param name="index">Level index (1-based)</param>
    /// <param name="completed">Level already completed</param>
    /// <param name="current">Is this the current level</param>
    /// <param name="unlocked">Is this level unlocked</param>
    /// <param name="callback">Callback when clicked</param>
    public void Setup(int index, bool completed, bool current, bool unlocked, System.Action<int> callback)
    {
        levelIndex = index;
        onClick = callback;

        // Reset text visibility
        levelText.gameObject.SetActive(false);

        if (completed)
        {
            backgroundImage.sprite = completedSprite;
            levelText.text = index.ToString();
            levelText.gameObject.SetActive(true);
        }
        else if (current)
        {
            backgroundImage.sprite = currentSprite;
            // Hide text for current level
        }
        else if (!unlocked)
        {
            backgroundImage.sprite = lockedSprite;
            // Locked → no text
        }

        playButton.interactable = unlocked;
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(() => onClick?.Invoke(levelIndex));
    }
}
