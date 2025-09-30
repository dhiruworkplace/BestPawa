using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    private LevelData levelData;

    private void OnEnable()
    {
        levelData = (LevelData)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);
        GUILayout.Label("Wall Previews", EditorStyles.boldLabel);

        if (levelData.walls == null || levelData.walls.Count == 0)
        {
            if (GUILayout.Button("Add Wall"))
            {
                WallShape newWall = new WallShape { width = 4, height = 6 };
                newWall.InitArrays();
                levelData.walls.Add(newWall);
                EditorUtility.SetDirty(levelData);
            }
            return;
        }

        for (int i = 0; i < levelData.walls.Count; i++)
        {
            WallShape wall = levelData.walls[i];

            GUILayout.BeginVertical("box");
            GUILayout.Label($"Wall {i + 1}");

            wall.width = EditorGUILayout.IntSlider("Width", wall.width, 4, 8);
            wall.height = EditorGUILayout.IntSlider("Height", wall.height, 4, 12);
            wall.InitArrays();

            GUILayout.Space(5);
            Event e = Event.current;

            for (int y = wall.height - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < wall.width; x++)
                {
                    int index = y * wall.width + x;

                    if (wall.gridArray[index] && wall.starArray[index])
                        GUI.backgroundColor = new Color(1f, 0.5f, 0f); // orange
                    else if (wall.gridArray[index])
                        GUI.backgroundColor = Color.green; // cube
                    else if (wall.starArray[index])
                        GUI.backgroundColor = Color.yellow; // star
                    else
                        GUI.backgroundColor = Color.red; // empty

                    Rect buttonRect = GUILayoutUtility.GetRect(20, 20);
                    if (GUI.Button(buttonRect, ""))
                    {
                        if (e.button == 1) // right-click = toggle star
                            wall.starArray[index] = !wall.starArray[index];
                        else // left-click = toggle cube
                            wall.gridArray[index] = !wall.gridArray[index];

                        EditorUtility.SetDirty(levelData);
                    }
                    GUI.backgroundColor = Color.white;
                }
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(5);
            if (GUILayout.Button("Remove Wall"))
            {
                levelData.walls.RemoveAt(i);
                EditorUtility.SetDirty(levelData);
                return;
            }
            GUILayout.EndVertical();
        }

        if (GUILayout.Button("Add Wall"))
        {
            WallShape newWall = new WallShape { width = 4, height = 6 };
            newWall.InitArrays();
            levelData.walls.Add(newWall);
            EditorUtility.SetDirty(levelData);
        }
    }
}
