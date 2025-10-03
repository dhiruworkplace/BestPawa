using UnityEngine;

public class PlayerAppearance : MonoBehaviour
{
    [Header("Renderer Settings")]
    public Renderer playerRenderer;
    public Material defaultMaterial;

    public Material[] characters;

    private void Start()
    {
        ApplySelectedCharacterMaterial(GameManager.selectedCube);

        // Listen for character change event
        ProgressManager.Instance.OnCharacterSelected += ApplySelectedCharacterMaterial;
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid errors when reloading scenes
        if (ProgressManager.Instance != null)
            ProgressManager.Instance.OnCharacterSelected -= ApplySelectedCharacterMaterial;
    }

    private void ApplySelectedCharacterMaterial(int index)
    {
        //if (index < 0 || index >= characters.Length)
        //{
        //    if (playerRenderer != null && defaultMaterial != null)
        //        playerRenderer.material = defaultMaterial;
        //    return;
        //}

        //var selectedChar = characters[index];
        //if (selectedChar != null && selectedChar.material != null)
        //    playerRenderer.material = selectedChar.material;
        //else if (defaultMaterial != null)
        //    playerRenderer.material = defaultMaterial;

        playerRenderer.material = characters[index];
    }
}
