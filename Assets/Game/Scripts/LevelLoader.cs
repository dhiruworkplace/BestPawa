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

    public void LoadLevel(LevelData levelData)
    {
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
        float zOffset = 50f; // First wall starts at 50
        float zStep = 50f;   // distance between walls

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

            zOffset += zStep;
        }

        // Add Finish Line at the end
        GameObject finishGO = Instantiate(finishLinePrefab, currentLevelGO.transform);
        finishGO.name = "Finish Line";
        finishGO.transform.localPosition = new Vector3(finishPositin.x, finishPositin.y, zOffset);
        finishGO.transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    public ObstacleWallManager GetWallManager()
    {
        return currentWallManager;
    }
}
