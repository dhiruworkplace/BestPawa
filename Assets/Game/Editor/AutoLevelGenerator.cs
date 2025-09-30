using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class AutoLevelGenerator
{
    private static int numberOfLevels = 30;

    [MenuItem("Game Tools/Generate Levels")]
    public static void GenerateLevels()
    {
        string rootFolder = "Assets/Game/Scriptable Objects";
        string saveFolder = $"{rootFolder}/Levels";

        // Ensure folders exist
        if (!AssetDatabase.IsValidFolder(rootFolder))
        {
            AssetDatabase.CreateFolder("Assets/Game", "Scriptable Objects");
        }
        if (!AssetDatabase.IsValidFolder(saveFolder))
        {
            AssetDatabase.CreateFolder(rootFolder, "Levels");
        }

        for (int i = 1; i <= numberOfLevels; i++)
        {
            LevelData level = ScriptableObject.CreateInstance<LevelData>();
            level.levelName = $"Level_{i}";
            level.walls = new List<WallShape>();

            // Difficulty scaling
            int numWalls = Mathf.Clamp(1 + i / 3, 1, 10);     // up to 10 walls
            int gridWidth = 4;                                // fixed
            int gridHeight = Mathf.Clamp(4 + i / 2, 4, 10);   // grows with levels
            int pathLength = Mathf.Min(3 + i / 2, gridHeight + 3);

            for (int w = 0; w < numWalls; w++)
            {
                WallShape wall = new WallShape();
                wall.width = gridWidth;
                wall.height = gridHeight;
                wall.InitArrays();

                // fill all cubes first
                for (int idx = 0; idx < wall.gridArray.Length; idx++)
                    wall.gridArray[idx] = true;

                List<Vector2Int> path = new List<Vector2Int>();

                // --- Special case for Level 1, first wall ---
                if (i == 1 && w == 0)
                {
                    int startX = Random.Range(0, wall.width - 2); // ensure space for 3
                    for (int k = 0; k < 3; k++)
                    {
                        path.Add(new Vector2Int(startX + k, 0));
                    }
                }
                else
                {
                    // Normal case: start with one empty cube at bottom
                    Vector2Int start = new Vector2Int(Random.Range(0, wall.width), 0);
                    path.Add(start);
                }

                // Continue random path
                Vector2Int current = path[path.Count - 1];
                for (int step = 0; step < pathLength; step++)
                {
                    Vector2Int next = current;
                    int dir = Random.Range(0, 100);

                    if (dir < 50) next += Vector2Int.up;
                    else if (dir < 75) next += Vector2Int.left;
                    else next += Vector2Int.right;

                    // Clamp inside wall
                    next.x = Mathf.Clamp(next.x, 0, wall.width - 1);
                    next.y = Mathf.Clamp(next.y, 0, wall.height - 1);

                    // avoid repeats
                    if (!path.Contains(next))
                    {
                        path.Add(next);
                        current = next;
                    }
                }

                // Apply path empties
                foreach (var pos in path)
                {
                    int idx = pos.y * wall.width + pos.x;
                    wall.gridArray[idx] = false; // empty
                }

                // Place star at last empty cube
                int starIdx = current.y * wall.width + current.x;
                wall.starArray[starIdx] = true;

                // Add wall to level
                level.walls.Add(wall);
            }

            // Save asset
            string assetPath = $"{saveFolder}/{level.levelName}.asset";
            AssetDatabase.CreateAsset(level, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Debug.Log($"✅ Generated {numberOfLevels} Levels in {saveFolder}");
    }
}
