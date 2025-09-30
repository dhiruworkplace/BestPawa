using UnityEngine;

[CreateAssetMenu(fileName = "DifficultySettings", menuName = "HollywoodCubes/Difficulty Settings", order = 2)]
public class DifficultySettings : ScriptableObject
{
    [Header("Walls")]
    public int startWalls = 1;
    public int maxWalls = 10;
    public int wallGrowthRate = 3; // new wall every X levels

    [Header("Grid")]
    public int gridWidth = 4;
    public int minGridHeight = 4;
    public int maxGridHeight = 10;
    public int gridHeightGrowthRate = 2; // +1 height every X levels

    [Header("Path Length")]
    public int minPathLength = 3;
    public int maxPathLength = 15;
    public int pathGrowthRate = 2; // +1 path length every X levels
}
