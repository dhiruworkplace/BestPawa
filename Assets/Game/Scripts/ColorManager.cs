using UnityEngine;

public static class ColorManager
{
    private static ColorConfig config;
    private static int currentColorIndex = 0;
    private static int gamesPlayed = 0;

    public static void Initialize(ColorConfig cfg)
    {
        config = cfg;
    }

    public static Color GetCurrentColor()
    {
        if (config == null || config.availableColors.Length == 0)
            return Color.white;

        return config.availableColors[currentColorIndex];
    }

    public static void OnGameEnd()
    {
        if (config == null || config.availableColors.Length == 0) return;

        gamesPlayed++;

        Debug.Log("Game Played" + gamesPlayed);

        if (gamesPlayed % config.gamesPerColorChange == 0)
        {
            currentColorIndex = (currentColorIndex + 1) % config.availableColors.Length;
        }
    }
}