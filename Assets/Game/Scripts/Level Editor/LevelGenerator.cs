using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WallData
{
    public int width = 4;
    public int height = 10;
    public bool[] gridArray;
    public bool[] starArray;

    public void InitArrays()
    {
        if (width <= 0) width = 1;
        if (height <= 0) height = 1;

        if (gridArray == null || gridArray.Length != width * height)
        {
            gridArray = new bool[width * height];
            for (int i = 0; i < gridArray.Length; i++)
                gridArray[i] = true;
        }
        if (starArray == null || starArray.Length != width * height)
        {
            starArray = new bool[width * height];
        }
    }
}

public class LevelGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject wallPrefab;
    public GameObject finishPrefab;

    [Header("Layout Settings")]
    public float wallSpacing = 50f;
    public Vector3 wallStartPos = Vector3.zero;

    [Header("Finish Line Settings")]
    public Vector2 finishPosXY = Vector2.zero;   // Offset in X/Y
    public Vector3 finishRotation = new Vector3(0, 90, 0);

    [Header("Walls Data")]
    public List<WallData> walls = new List<WallData>();

    public void GenerateLevel()
    {
        // Clear old generated level
        foreach (Transform child in transform)
            DestroyImmediate(child.gameObject);

        // --- Create Level root ---
        GameObject levelRoot = new GameObject("Level");
        levelRoot.transform.SetParent(transform);
        levelRoot.transform.localPosition = Vector3.zero;

        // --- Create Wall Manager ---
        GameObject wallManagerObj = new GameObject("Wall Manager");
        wallManagerObj.transform.SetParent(levelRoot.transform);
        wallManagerObj.transform.localPosition = Vector3.zero;

        // Assign the WallManager tag
        wallManagerObj.tag = "WallManager";

        ObstacleWallManager wallManager = wallManagerObj.AddComponent<ObstacleWallManager>();
        wallManager.walls = new List<ObstacleWallBuilder>();

        // --- Build walls ---
        for (int i = 0; i < walls.Count; i++)
        {
            WallData data = walls[i];
            data.InitArrays();

            Vector3 pos = wallStartPos + Vector3.forward * wallSpacing * i;
            GameObject wallObj = Instantiate(wallPrefab, pos, Quaternion.identity, wallManagerObj.transform);
            wallObj.name = $"Wall {i + 1}";

            ObstacleWallBuilder builder = wallObj.GetComponent<ObstacleWallBuilder>();
            builder.width = data.width;
            builder.height = data.height;
            builder.gridArray = (bool[])data.gridArray.Clone();
            builder.starArray = (bool[])data.starArray.Clone();
            builder.BuildWall();

            wallManager.walls.Add(builder);
        }

        // --- Create Finish Line ---
        Vector3 finishPos = wallStartPos + Vector3.forward * wallSpacing * walls.Count;
        finishPos.x += finishPosXY.x;
        finishPos.y += finishPosXY.y;

        GameObject finishObj = Instantiate(finishPrefab, finishPos, Quaternion.identity, levelRoot.transform);
        finishObj.name = "Finish Line";
        finishObj.transform.localRotation = Quaternion.Euler(finishRotation);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (walls == null) return;

        // Preview wall positions
        UnityEditor.Handles.color = Color.cyan;
        for (int i = 0; i < walls.Count; i++)
        {
            Vector3 pos = transform.position + wallStartPos + Vector3.forward * wallSpacing * i;
            Vector3 size = new Vector3(walls[i].width, walls[i].height, 1);
            Vector3 center = pos + new Vector3(-(walls[i].width - 1) / 2f, walls[i].height / 2f, 0);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(center, size);
            UnityEditor.Handles.Label(center + Vector3.up * 0.5f, $"Wall {i + 1}");
        }

        // Preview finish line with rotation
        if (walls.Count > 0)
        {
            Vector3 finishPos = transform.position + wallStartPos + Vector3.forward * wallSpacing * walls.Count;
            finishPos.x += finishPosXY.x;
            finishPos.y += finishPosXY.y;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(finishPos + Vector3.up * 0.5f, new Vector3(4, 1, 1));

            UnityEditor.Handles.Label(finishPos + Vector3.up * 1.5f, "Finish");

            // Show rotation arrow
            UnityEditor.Handles.ArrowHandleCap(
                0,
                finishPos + Vector3.up * 0.5f,
                Quaternion.Euler(finishRotation),
                2f,
                EventType.Repaint
            );
        }
    }
#endif
}
