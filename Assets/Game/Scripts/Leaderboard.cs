using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    [Header("UI")]
    public Button Backbutton;
    public GameObject lbpanel;
    [System.Serializable]
    public class PlayerEntry
    {
        public string name;
        public int score;
        public int level;
        public int rank;
    }

    [System.Serializable]
    public class LeaderboardRow
    {
        public TextMeshProUGUI nameText;   // UI Text for name
        public TextMeshProUGUI scoreText;  // UI Text for score
        public TextMeshProUGUI levelText;  // UI Text for level
    }
    public TextMeshProUGUI MyRankText;

    [Header("Your Info")]
    public string yourName = "You";
    private int yourScore;
    public int yourLevel = 15;          // Your fixed level (not random)

    private List<string> englishNames = new List<string>()
    {
        "James", "Oliver", "William", "Benjamin", "Lucas",
        "Henry", "Alexander", "Michael", "Daniel", "Matthew",
        "Samuel", "David", "Joseph", "Thomas", "Andrew",
        "Ethan", "Jack", "Charles", "Christopher", "Anthony"
    };

    private List<PlayerEntry> leaderboard = new List<PlayerEntry>();

    [Header("Leaderboard UI Rows (10 slots)")]
    public List<LeaderboardRow> leaderboardRows = new List<LeaderboardRow>();

    void Start()
    {
        yourScore = PlayerPrefs.GetInt("BestScore", 0);
        GenerateLeaderboard();
        UpdateUI();
        if (Backbutton != null) Backbutton.onClick.AddListener(OnBackButtonClick);
    }

    void GenerateLeaderboard()
    {
        leaderboard.Clear();

        // Pick 9 random names
        List<string> randomNames = englishNames.OrderBy(x => Random.value).Take(9).ToList();

        // Assign random scores and random levels
        foreach (var name in randomNames)
        {
            leaderboard.Add(new PlayerEntry
            {
                name = name,
                score = Random.Range(100, 1000),
                level = Random.Range(1, 50)   // Random level between 1 and 50
            });
        }

        // Add your entry (fixed score + fixed level)
        leaderboard.Add(new PlayerEntry
        {
            name = yourName,
            score = yourScore,
            level = yourLevel
        });

        // Sort leaderboard by score (descending)
        leaderboard = leaderboard.OrderByDescending(p => p.score).ToList();

        // Assign ranks
        for (int i = 0; i < leaderboard.Count; i++)
        {
            leaderboard[i].rank = i + 1;
        }
    }

    void UpdateUI()
    {
        // Update top 10 entries
        for (int i = 0; i < leaderboardRows.Count && i < leaderboard.Count; i++)
        {
            leaderboardRows[i].nameText.text = $"{leaderboard[i].name}";
            leaderboardRows[i].scoreText.text = leaderboard[i].score.ToString();
            leaderboardRows[i].levelText.text = $"Level {leaderboard[i].level}";

            // Highlight your entry
            if (leaderboard[i].name == yourName)
            {
                leaderboardRows[i].nameText.color = Color.yellow;
                leaderboardRows[i].scoreText.color = Color.yellow;
                leaderboardRows[i].levelText.color = Color.yellow;
            }
            else
            {
                leaderboardRows[i].nameText.color = Color.white;
                leaderboardRows[i].scoreText.color = Color.white;
                leaderboardRows[i].levelText.color = Color.white;
            }
        }

        // Show your rank separately
        PlayerEntry myEntry = leaderboard.FirstOrDefault(p => p.name == yourName);
        if (myEntry != null)
        {
            //Debug.Log($"Your Rank is #{myEntry.rank}");
            if (MyRankText != null)
                MyRankText.text = $"#{myEntry.rank}";
        }
    }
    private void OnBackButtonClick()
    {
        SoundManager.Instance.PlayButtonClick();
        lbpanel.SetActive(false);
        // Debug.Log("Open Level Selection Panel");
        // Example: SceneManager.LoadScene("LevelSelection");
        // Or: enable your LevelSelection UI panel
    }

}
