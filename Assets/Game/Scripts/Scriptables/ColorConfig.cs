using UnityEngine;

[CreateAssetMenu(menuName = "Managers/ColorConfig", fileName = "NewColorConfig")]
public class ColorConfig : ScriptableObject
{
    [Header("Color Settings")]
    public Color[] availableColors;
    public int gamesPerColorChange = 3;
}
