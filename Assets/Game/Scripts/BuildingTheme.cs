using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class BuildingTheme : MonoBehaviour
{
    [Header("Mesh Variants: 0=Blue, 1=Green, 2=Red, 3=Yellow")]
    public Mesh[] meshVariants = new Mesh[4];

    private MeshFilter meshFilter;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
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
        if (themeIndex >= 0 && themeIndex < meshVariants.Length && meshVariants[themeIndex] != null)
        {
            meshFilter.mesh = meshVariants[themeIndex];
        }
    }
}
