using UnityEngine;
using System.Collections.Generic;

public class ObstacleWallBuilder : MonoBehaviour
{
    public int width = 4;
    public int height = 10;

    public bool[] gridArray;
    public bool[] starArray;

    public GameObject cubePrefab;
    public GameObject starPrefab;

    [Header("Materials")]
    public Material filledMaterial;   // assign in inspector
    public Material emptyMaterial;    // assign in inspector

    private HashSet<Vector3> requiredPositions = new HashSet<Vector3>();
    public List<GameObject> emptyCubes = new List<GameObject>();

    void OnValidate()
    {
        InitArrays();
    }

    /// <summary>
    /// Ensures gridArray and starArray are initialized correctly.
    /// </summary>
    public void InitArrays()
    {
        if (width <= 0) width = 1;
        if (height <= 0) height = 1;

        if (gridArray == null || gridArray.Length != width * height)
        {
            gridArray = new bool[width * height];
            for (int i = 0; i < gridArray.Length; i++)
                gridArray[i] = true; // Default to all cubes filled
        }

        if (starArray == null || starArray.Length != width * height)
        {
            starArray = new bool[width * height];
        }
    }

    public void BuildWall()
    {
        InitArrays();
        emptyCubes.Clear();

        // Remove old children
        foreach (Transform child in transform)
            DestroyImmediate(child.gameObject);

        requiredPositions.Clear();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                float posX = -x; // X from 0 to -width+1
                Vector3 localPos = new Vector3(posX, y, 0);

                GameObject cube = Instantiate(cubePrefab, transform);
                cube.transform.localPosition = localPos;

                MeshRenderer mr = cube.GetComponent<MeshRenderer>();
                Collider col = cube.GetComponent<Collider>();

                if (mr == null || col == null)
                {
                    // Debug.LogWarning("Cube prefab needs MeshRenderer and Collider!");
                    continue;
                }

                if (gridArray[index]) // Filled cube
                {
                    mr.enabled = true;
                    col.isTrigger = false;
                    requiredPositions.Add(localPos);

                    mr.material = filledMaterial;
                    mr.material.color = ColorManager.GetCurrentColor(); // ✅ apply centralized color
                }
                else // Empty cube
                {
                    mr.enabled = true;  // keep mesh visible
                    col.isTrigger = true;
                    emptyCubes.Add(cube);

                    if (emptyMaterial != null)
                        mr.material = emptyMaterial;
                }


                if (starArray[index])
                {
                    GameObject star = Instantiate(starPrefab, transform);
                    star.transform.localPosition = localPos + Vector3.forward * -0.5f;
                }
            }
        }
    }
}
