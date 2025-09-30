using UnityEngine;

[CreateAssetMenu(fileName = "ThemeData", menuName = "Game/Theme Data")]
public class ThemeData : ScriptableObject
{
    public string themeName; // "Blue", "Green", "Red", "Yellow"

    [Header("Visuals")]
    public Material fogMaterial;
    public Material skyboxMaterial;
}
