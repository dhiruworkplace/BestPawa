using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [Header("UI Elements")]
    public Image loadingFillImage; // Assign in Inspector (type: Filled Image)

    [Header("Settings")]
    public string sceneToLoad; // Name of the scene to load
    public float loadingDuration = 3f; // Duration for fake progress bar

    void Start()
    {
        Invoke(nameof(LoadScene), 3f);
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("Game");
        //StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
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
