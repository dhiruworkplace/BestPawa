using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObstacleWallBuilder))]
public class ObstacleWallBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ObstacleWallBuilder builder = (ObstacleWallBuilder)target;

        DrawDefaultInspector();

        // Ensure arrays are initialized
        int totalCells = builder.width * builder.height;

        if (builder.gridArray == null || builder.gridArray.Length != totalCells)
        {
            builder.gridArray = new bool[totalCells];
            for (int i = 0; i < builder.gridArray.Length; i++)
                builder.gridArray[i] = true;
        }

        if (builder.starArray == null || builder.starArray.Length != totalCells)
        {
            builder.starArray = new bool[totalCells];
        }

        GUILayout.Space(10);
        GUILayout.Label("Draw Grid Shape (Left click = Cube, Right click = Star):");

        Event e = Event.current;

        for (int y = builder.height - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < builder.width; x++)
            {
                int index = y * builder.width + x;

                // Determine button color
                if (builder.gridArray[index] && builder.starArray[index])
                    GUI.backgroundColor = new Color(1f, 0.5f, 0f); // Orange = cube + star
                else if (builder.gridArray[index])
                    GUI.backgroundColor = Color.green; // Cube only
                else if (builder.starArray[index])
                    GUI.backgroundColor = Color.yellow; // Star only
                else
                    GUI.backgroundColor = Color.red; // Empty

                Rect buttonRect = GUILayoutUtility.GetRect(20, 20);
                if (GUI.Button(buttonRect, ""))
                {
                    if (e.button == 1) // Right click = star toggle
                        builder.starArray[index] = !builder.starArray[index];
                    else // Left click = cube toggle
                        builder.gridArray[index] = !builder.gridArray[index];

                    EditorUtility.SetDirty(builder);
                }
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Build Wall"))
        {
            builder.BuildWall();
        }
    }
}
