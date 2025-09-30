using System.Collections.Generic;
using UnityEngine;

public class ObstacleWallManager : MonoBehaviour
{
    public List<ObstacleWallBuilder> walls;  // Assign all walls in order in inspector

    private int currentWallIndex = 0;

    void Awake()
    {
        currentWallIndex = 0;
    }

    public ObstacleWallBuilder GetCurrentWall()
    {
        if (currentWallIndex < walls.Count)
            return walls[currentWallIndex];
        return null;
    }

    public void MoveToNextWall()
    {
        currentWallIndex++;
    }

    public bool AllWallsCompleted()
    {
        return currentWallIndex >= walls.Count;
    }
}
