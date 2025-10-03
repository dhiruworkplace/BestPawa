using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("SFX Clips")]
    public AudioClip buttonClick;
    public AudioClip addCube;
    public AudioClip removeCube;
    public AudioClip levelComplete;
    public AudioClip levelFailed;
    public AudioClip coin;

    private AudioSource audioSource;
    private bool soundEnabled = true;

    private void Awake()
    {
        Instance = this;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void SetSoundEnabled(bool enabled)
    {
        soundEnabled = enabled;
    }

    public void PlaySound(AudioClip clip)
    {
        if (soundEnabled && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    #region Shortcuts
    public void PlayButtonClick() => PlaySound(buttonClick);
    public void PlayAddCube() => PlaySound(addCube);
    public void PlayRemoveCube() => PlaySound(removeCube);
    public void PlayLevelComplete() => PlaySound(levelComplete);
    public void PlayLevelFailed() => PlaySound(levelFailed);
    public void PlayCoin() => PlaySound(coin);
    #endregion
}
