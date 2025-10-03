using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LootStarLeaderboard : MonoBehaviour
{
    public Transform parent;
    public Sprite[] medals;
    public PlayerDetail playerDetail;
    public Sprite myPanel;

    public List<string> names = new List<string>()
    {
        "John Doe",
        "Jane Smith",
        "Michael Johnson",
        "Emily Davis",
        "Daniel Martinez",
        "Sophia Hernandez",
        "David Brown",
        "Olivia Wilson",
        "James Garcia",
        "Emma Rodriguez",
        "Alexander Lee",
        "Isabella White",
        "Christopher Thomas",
        "Mia Lopez",
        "Matthew Walker",
        "Ava Hall",
        "Joshua Allen",
        "Charlotte Young",
        "Ethan King",
        "Abigail Scott",
        "Benjamin Green",
        "Madison Adams",
        "Samuel Baker",
        "Grace Nelson",
        "Andrew Clark",
        "Chloe Lewis",
        "Logan Perez",
        "Sofia Sanchez",
        "Jacob Roberts",
        "Avery Turner",
        "Ryan Parker",
        "Amelia Collins",
        "Nathan Edwards",
        "Ella Stewart",
        "Caleb Morris",
        "Lily Hughes",
        "Jack Foster",
        "Harper Mitchell",
        "Henry Morgan",
        "Scarlett Rivera",
        "Lucas Cook",
        "Zoe Bell",
        "Owen Russell",
        "Nora Sanders",
        "Mason Peterson",
        "Ella Morris",
        "Aiden Rogers",
        "Camila Reed",
        "Jackson Cooper",
        "Layla Bailey"
    };

    // Start is called before the first frame update
    void Start()
    {
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        List<string> nameList = names.OrderBy(x => Random.value).ToList();
        List<int> pointList = new List<int>();

        List<PlayerData> players = new List<PlayerData>();
        PlayerData me = new PlayerData();
        me.me = true;
        me.name = "You";
        me.points = PlayerPrefs.GetInt("BestScore", 0);
        players.Add(me);

        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
        int min = me.points <= 5 ? 5 : (me.points - 5);
        int max = me.points <= 5 ? 15 : (me.points + 5);

        for (int i = 0; i < 10; i++)
        {
            pointList.Add(Random.Range(min, max));
        }
        pointList = pointList.OrderBy(x => x).ToList();

        for (int i = 0; i < 9; i++)
        {
            PlayerData player = new PlayerData();
            player.me = false;
            player.name = nameList[i];
            player.points = Random.Range(min, max);
            players.Add(player);
        }
        players = players.OrderByDescending(x => x.points).ToList();

        for (int i = 0; i < 10; i++)
        {
            PlayerDetail pd = Instantiate(playerDetail, parent);
            pd.SetData((i + 1), players[i].name, players[i].points);
            pd.rank.text = "#" + (i + 1);

            if (i < 3)
            {
                pd.medal.sprite = medals[i];
                pd.rank.gameObject.SetActive(false);
            }
            else
            {
                pd.medal.gameObject.SetActive(false);
                pd.rank.gameObject.SetActive(true);
            }

            if (players[i].me)
            {
                pd.panel.sprite = myPanel;
                pd.playerName.color = Color.black;
            }
        }
    }
}

public class PlayerData
{
    public bool me;
    public string name;
    public int points;
}