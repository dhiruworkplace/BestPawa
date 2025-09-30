using UnityEngine;
using UnityEngine.SceneManagement;

public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance { get; private set; }

    [Header("Available Themes")]
    public ThemeData[] themes;

    public ThemeData CurrentTheme { get; private set; }
    public int CurrentThemeIndex { get; private set; }

    public delegate void ThemeChanged(ThemeData theme, int themeIndex);
    public event ThemeChanged OnThemeChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PickRandomTheme();
    }

    private void PickRandomTheme()
    {
        if (themes == null || themes.Length == 0) return;

        CurrentThemeIndex = Random.Range(0, themes.Length); // 0=Blue, 1=Green, etc.
        CurrentTheme = themes[CurrentThemeIndex];

        // Apply skybox immediately
        if (CurrentTheme.skyboxMaterial != null)
            RenderSettings.skybox = CurrentTheme.skyboxMaterial;

        // Notify listeners (fog + buildings)
        OnThemeChanged?.Invoke(CurrentTheme, CurrentThemeIndex);

        // Debug.Log($"🎨 Theme Applied: {CurrentTheme.themeName}");
    }
}
