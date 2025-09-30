using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Header("UI Elements")]
    public Image loadingFillImage; // Assign in Inspector (type: Filled Image)
    public GameObject loadingCanvas; // Optional: loading screen container

    [Header("Settings")]
    public string sceneToLoad; // Name of the scene to load
    public float loadingDuration = 3f; // Duration for fake progress bar

    void Start()
    {
        LoadScene();
    }

    public void LoadScene()
    {
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        if (loadingCanvas != null)
            loadingCanvas.SetActive(true);

        // Start loading the scene in background
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        float elapsed = 0f;

        while (elapsed < loadingDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / loadingDuration);

            if (loadingFillImage != null)
                loadingFillImage.fillAmount = progress;

            yield return null;
        }

        // Ensure bar is full
        if (loadingFillImage != null)
            loadingFillImage.fillAmount = 1f;

        // Scene is likely loaded already, just activate
        operation.allowSceneActivation = true;
    }
}
