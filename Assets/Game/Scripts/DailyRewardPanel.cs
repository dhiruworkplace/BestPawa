using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DailyRewardPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text streakText;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private Button claimButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_Text claimButtonText;
    [SerializeField] private TextMeshProUGUI coinText;


    [Header("Reward Settings")]
    [SerializeField] private int baseReward = 100;
    [SerializeField] private int incrementPerDay = 50;
    [SerializeField] private double rewardCooldownHours = 24;

    private int currentStreak = 0;
    private DateTime nextRewardTime;

    private void Start()
    {
        LoadData();
        UpdateUI();
        claimButton.onClick.AddListener(ClaimReward);
        backButton.onClick.AddListener(OnBack);
    }

    private void Update()
    {
        if (CanClaim())
        {
            claimButton.interactable = true;
            claimButtonText.text = "Claim";
        }
        else
        {
            claimButton.interactable = false;
            TimeSpan remaining = nextRewardTime - DateTime.UtcNow;
            if (remaining.TotalSeconds < 0) remaining = TimeSpan.Zero;

            claimButtonText.text = string.Format("{0:D2}:{1:D2}:{2:D2}",
                remaining.Hours, remaining.Minutes, remaining.Seconds);
        }
    }

    private void ClaimReward()
    {
        if (!CanClaim()) return;

        SoundManager.Instance.PlayButtonClick();

        currentStreak++;
        int reward = baseReward + (incrementPerDay * (currentStreak - 1));

        // int previous = PlayerPrefs.GetInt("coins", 0); 

        // PlayerPrefs.SetInt("coins",previous + reward);
        // PlayerPrefs.Save();
        // coinText.text = PlayerPrefs.GetInt("coins", 0).ToString();
        // TODO: Add reward to player's currency system

        // ✅ Add stars through ProgressManager
        ProgressManager.Instance.AddStars(reward);

        // ✅ Update UI
        coinText.text = ProgressManager.Instance.Progress.stars.ToString();

        // Debug.Log($"Claimed reward: {reward}");

        nextRewardTime = DateTime.UtcNow.AddHours(rewardCooldownHours);
        SaveData();
        UpdateUI();
    }

    private void OnBack()
    {
        SoundManager.Instance.PlayButtonClick();
        gameObject.SetActive(false);
    }

    private bool CanClaim()
    {
        return DateTime.UtcNow >= nextRewardTime;
    }

    private void UpdateUI()
    {
        streakText.text = $"{currentStreak + 1} Days";
        int reward = baseReward + (incrementPerDay * currentStreak);
        rewardText.text = reward.ToString();
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("DailyReward_Streak", currentStreak);
        PlayerPrefs.SetString("DailyReward_NextTime", nextRewardTime.ToBinary().ToString());
        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        currentStreak = PlayerPrefs.GetInt("DailyReward_Streak", 0);

        string timeStr = PlayerPrefs.GetString("DailyReward_NextTime", string.Empty);
        if (!string.IsNullOrEmpty(timeStr))
        {
            long binary = Convert.ToInt64(timeStr);
            nextRewardTime = DateTime.FromBinary(binary);
        }
        else
        {
            nextRewardTime = DateTime.UtcNow;
        }
    }
}
