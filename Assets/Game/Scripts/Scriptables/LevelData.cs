using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class WallShape
{
    public int width = 3;
    public int height = 6;
    public bool[] gridArray; // wall cubes
    public bool[] starArray; // optional stars

    public void InitArrays()
    {
        if (gridArray == null || gridArray.Length != width * height)
        {
            gridArray = new bool[width * height];
            for (int i = 0; i < gridArray.Length; i++)
                gridArray[i] = true; // default filled
        }
        if (starArray == null || starArray.Length != width * height)
        {
            starArray = new bool[width * height];
        }
    }
}

[CreateAssetMenu(fileName = "LevelData", menuName = "HollywoodCubes/Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    public string levelName;
    public int lanes = 4;
    public List<WallShape> walls;
}