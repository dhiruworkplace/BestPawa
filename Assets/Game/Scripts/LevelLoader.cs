using UnityEngine;
using System.Collections.Generic;

public class LevelLoader : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject wallManagerPrefab;
    public GameObject wallPrefab; // contains ObstacleWallBuilder
    public GameObject finishLinePrefab;

    public Vector3 finishPositin;

    [Header("Setup")]
    public Transform levelParent; // parent to hold level hierarchy

    private ObstacleWallManager currentWallManager;
    private GameObject currentLevelGO;

    public MyLetter letter;
    public Sprite[] letters;
    private List<string> words = new List<string>() { "B,E,S,T", "P,A,W,A", "C,U,B,E", "R,U,N" };
    private int charInx = 0;
    private string[] word;

    public static GameType gameType;
    public static int selectedChall;

    public void LoadLevel(LevelData levelData)
    {
        Debug.Log("type : " + gameType);
        charInx = 0;
        string takeStr = words[Random.Range(0, words.Count)];
        if (gameType.Equals(GameType.Challenge))
            takeStr = GiveWord();
        if (!string.IsNullOrEmpty(takeStr))
            word = takeStr.Trim().Split(',');
        else
            word = new string[0];
        Debug.Log("takeStr : " + takeStr);

        if (levelData == null)
        {
            // Debug.LogError("❌ LevelData is null!");
            return;
        }
        // Clear old level
        if (currentLevelGO != null)
            Destroy(currentLevelGO);

        // Create parent
        currentLevelGO = new GameObject(levelData.levelName);
        if (levelParent != null)
            currentLevelGO.transform.SetParent(levelParent);

        // Create Wall Manager
        GameObject wmObj = Instantiate(wallManagerPrefab, currentLevelGO.transform);
        wmObj.name = "Wall Manager";
        wmObj.tag = "WallManager";
        currentWallManager = wmObj.GetComponent<ObstacleWallManager>();
        currentWallManager.walls = new List<ObstacleWallBuilder>();

        // Position walls
        float zOffset = 60f; // First wall starts at 50
        float zStep = 60f;   // distance between walls

        foreach (var wallShape in levelData.walls)
        {
            GameObject wallGO = Instantiate(wallPrefab, wmObj.transform);
            wallGO.transform.localPosition = new Vector3(0, 0, zOffset);

            ObstacleWallBuilder builder = wallGO.GetComponent<ObstacleWallBuilder>();
            if (builder == null)
            {
                // Debug.LogError("Wall Prefab must have ObstacleWallBuilder!");
                continue;
            }

            // Apply shape to builder
            builder.width = wallShape.width;
            builder.height = wallShape.height;
            builder.gridArray = (bool[])wallShape.gridArray.Clone();
            builder.starArray = (bool[])wallShape.starArray.Clone();

            builder.BuildWall();

            currentWallManager.walls.Add(builder);
            PlaceLatter(levelData.walls.Count, (zOffset - zStep), wmObj.transform);
            zOffset += zStep;
        }

        // Add Finish Line at the end
        GameObject finishGO = Instantiate(finishLinePrefab, currentLevelGO.transform);
        finishGO.name = "Finish Line";
        finishGO.transform.localPosition = new Vector3(finishPositin.x, finishPositin.y, zOffset);
        finishGO.transform.rotation = Quaternion.Euler(0, 90, 0);

        FindFirstObjectByType<PlayerController>().SetGround(levelData.lanes);
    }

    private void PlaceLatter(int walls, float zPos, Transform parent)
    {
        zPos += 10;
        if (walls.Equals(1))
        {
            for (int i = 0; i < word.Length; i++)
            {
                if (charInx < word.Length)
                {
                    zPos += 10;
                    MyLetter ml = Instantiate(letter, new Vector3(0, 0, zPos), Quaternion.identity, parent);
                    ml.letter.sprite = GiveLetter(word[charInx]);
                    charInx++;
                }
            }
        }
        else if (walls.Equals(2))
        {
            for (int i = 0; i < 2; i++)
            {
                if (charInx < word.Length)
                {
                    zPos += 10;
                    MyLetter ml = Instantiate(letter, new Vector3(0, 0, zPos), Quaternion.identity, parent);
                    ml.letter.sprite = GiveLetter(word[charInx]);
                    charInx++;
                }
            }
        }
        else
        {
            if (charInx < word.Length)
            {
                zPos += 10;
                MyLetter ml = Instantiate(letter, new Vector3(0, 0, zPos), Quaternion.identity, parent);
                ml.letter.sprite = GiveLetter(word[charInx]);
                charInx++;
            }
        }
    }

    public ObstacleWallManager GetWallManager()
    {
        return currentWallManager;
    }

    private Sprite GiveLetter(string letter)
    {
        if (letter.Equals("A"))
            return letters[0];
        else if (letter.Equals("B"))
            return letters[1];
        else if (letter.Equals("C"))
            return letters[2];
        else if (letter.Equals("E"))
            return letters[3];
        else if (letter.Equals("F"))
            return letters[4];
        else if (letter.Equals("L"))
            return letters[5];
        else if (letter.Equals("N"))
            return letters[6];
        else if (letter.Equals("O"))
            return letters[7];
        else if (letter.Equals("P"))
            return letters[8];
        else if (letter.Equals("R"))
            return letters[9];
        else if (letter.Equals("S"))
            return letters[10];
        else if (letter.Equals("T"))
            return letters[11];
        else if (letter.Equals("U"))
            return letters[12];
        else if (letter.Equals("W"))
            return letters[13];
        else
            return letters[0];
    }

    private string GiveWord()
    {
        if (selectedChall.Equals(0))
            return "P,A,W,A";
        else if (selectedChall.Equals(2))
            return "C,U,B,E";
        else if (selectedChall.Equals(4))
            return "P,O,W,E,R";
        else if (selectedChall.Equals(6))
            return "P,A,W,A,R,F,U,L";
        else
            return "";
    }
}

public enum GameType
{
    Beginner,
    Expert,
    Challenge
}