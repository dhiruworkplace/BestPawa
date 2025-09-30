using UnityEngine;

public class FogController : MonoBehaviour
{
    private Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        if (ThemeManager.Instance != null)
            ThemeManager.Instance.OnThemeChanged += ApplyTheme;
    }

    private void OnDisable()
    {
        if (ThemeManager.Instance != null)
            ThemeManager.Instance.OnThemeChanged -= ApplyTheme;
    }

    private void ApplyTheme(ThemeData theme, int themeIndex)
    {
        if (theme != null && theme.fogMaterial != null && rend != null)
        {
            rend.sharedMaterial = theme.fogMaterial;
        }
    }
}
