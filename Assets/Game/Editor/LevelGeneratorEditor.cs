#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelGenerator gen = (LevelGenerator)target;
        DrawDefaultInspector();

        GUILayout.Space(10);
        GUILayout.Label("Wall Layout Editor", EditorStyles.boldLabel);

        // Add wall button
        if (GUILayout.Button("Add Wall"))
        {
            gen.walls.Add(new WallData());
            EditorUtility.SetDirty(gen);
        }

        // Draw each wall
        for (int w = 0; w < gen.walls.Count; w++)
        {
            WallData wall = gen.walls[w];
            wall.InitArrays();

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"Wall {w + 1} ({wall.width}x{wall.height})", EditorStyles.boldLabel);

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                gen.walls.RemoveAt(w);
                EditorUtility.SetDirty(gen);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                break;
            }
            EditorGUILayout.EndHorizontal();

            wall.width = EditorGUILayout.IntField("Width", wall.width);
            wall.height = EditorGUILayout.IntField("Height", wall.height);
            wall.InitArrays();

            // Grid UI
            for (int y = wall.height - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();
                for (int x = 0; x < wall.width; x++)
                {
                    int index = y * wall.width + x;

                    if (wall.gridArray[index] && wall.starArray[index])
                        GUI.backgroundColor = new Color(1f, 0.5f, 0f); // Orange
                    else if (wall.gridArray[index])
                        GUI.backgroundColor = Color.green;
                    else if (wall.starArray[index])
                        GUI.backgroundColor = Color.yellow;
                    else
                        GUI.backgroundColor = Color.red;

                    if (GUILayout.Button("", GUILayout.Width(20), GUILayout.Height(20)))
                    {
                        Event e = Event.current;
                        if (e.button == 1) // Right click = star
                            wall.starArray[index] = !wall.starArray[index];
                        else // Left click = cube
                            wall.gridArray[index] = !wall.gridArray[index];

                        EditorUtility.SetDirty(gen);
                    }
                    GUI.backgroundColor = Color.white;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Generate Level"))
        {
            gen.GenerateLevel();
        }
    }
}
#endif
