using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    public Button backButton;
    public GameObject settingPanel, ppPanel, tcPanel, htpPanel;

    [Header("UI Buttons")]
    public Button soundButton;
    public Button musicButton;

    public Button htpButton, tcButton, ppButton;
    // public Button vibrationButton;

    [Header("Sprites")]
    public Sprite onSprite;
    public Sprite offSprite;
    // public Sprite musicOnSprite;
    // public Sprite musicOffSprite;
    // public Sprite vibrationOnSprite;
    // public Sprite vibrationOffSprite;


    public Button backPPButton, backTCButton, backHtpButton;

    private const string SOUND_KEY = "SoundEnabled";
    private const string MUSIC_KEY = "MusicEnabled";
    private const string VIBRATION_KEY = "VibrationEnabled";

    private bool soundEnabled;
    private bool musicEnabled;
    private bool vibrationEnabled;

    private void Start()
    {
        // Load saved settings (default = true)
        soundEnabled = PlayerPrefs.GetInt(SOUND_KEY, 1) == 1;
        musicEnabled = PlayerPrefs.GetInt(MUSIC_KEY, 1) == 1;
        vibrationEnabled = PlayerPrefs.GetInt(VIBRATION_KEY, 1) == 1;

        // Apply settings immediately
        ApplySettings();

        // Hook up button listeners
        soundButton.onClick.AddListener(ToggleSound);
        musicButton.onClick.AddListener(ToggleMusic);

        htpButton.onClick.AddListener(OpenHTP);
        tcButton.onClick.AddListener(OpenTC);
        ppButton.onClick.AddListener(OpenPP);
        // vibrationButton.onClick.AddListener(ToggleVibration);
        backButton.onClick.AddListener(OnBack);
        backPPButton.onClick.AddListener(OnBackPP);
        backTCButton.onClick.AddListener(OnBackTC);
        backHtpButton.onClick.AddListener(OnBackHTP);

        // Refresh button sprites
        UpdateButtonSprites();
    }

    private void ToggleSound()
    {
        soundEnabled = !soundEnabled;
        PlayerPrefs.SetInt(SOUND_KEY, soundEnabled ? 1 : 0);
        SoundManager.Instance.SetSoundEnabled(soundEnabled);
        SoundManager.Instance.PlayButtonClick();
        UpdateButtonSprites();
    }

    private void ToggleMusic()
    {
        musicEnabled = !musicEnabled;
        PlayerPrefs.SetInt(MUSIC_KEY, musicEnabled ? 1 : 0);
        BGMusic.Instance.SetMusicEnabled(musicEnabled);
        SoundManager.Instance.PlayButtonClick();
        UpdateButtonSprites();
    }

    private void OpenHTP()
    {
        SoundManager.Instance.PlayButtonClick();
        htpPanel.SetActive(true);
    }

    private void OpenTC()
    {
        SoundManager.Instance.PlayButtonClick();
        tcPanel.SetActive(true);
    }

    private void OpenPP()
    {
        SoundManager.Instance.PlayButtonClick();
        ppPanel.SetActive(true);
    }

    private void ToggleVibration()
    {
        vibrationEnabled = !vibrationEnabled;
        PlayerPrefs.SetInt(VIBRATION_KEY, vibrationEnabled ? 1 : 0);
        VibrationManager.Instance.SetVibrationEnabled(vibrationEnabled);
        SoundManager.Instance.PlayButtonClick();
        UpdateButtonSprites();
    }

    private void ApplySettings()
    {
        SoundManager.Instance.SetSoundEnabled(soundEnabled);
        BGMusic.Instance.SetMusicEnabled(musicEnabled);
        VibrationManager.Instance.SetVibrationEnabled(vibrationEnabled);
    }

    private void UpdateButtonSprites()
    {
        soundButton.image.sprite = soundEnabled ? onSprite : offSprite;
        musicButton.image.sprite = musicEnabled ? onSprite : offSprite;
        // vibrationButton.image.sprite = vibrationEnabled ? vibrationOnSprite : vibrationOffSprite;
    }

    private void OnBack()
    {
        SoundManager.Instance.PlayButtonClick();
        settingPanel.SetActive(false);
    }

    private void OnBackPP()
    {
        SoundManager.Instance.PlayButtonClick();
        ppPanel.SetActive(false);
    }

    private void OnBackTC()
    {
        SoundManager.Instance.PlayButtonClick();
        tcPanel.SetActive(false);
    }

    private void OnBackHTP()
    {
        SoundManager.Instance.PlayButtonClick();
        htpPanel.SetActive(false);
    }
}
