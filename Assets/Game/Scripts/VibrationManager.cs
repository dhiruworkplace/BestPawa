using UnityEngine;
using Solo.MOST_IN_ONE;

public class VibrationManager : MonoBehaviour
{
    public static VibrationManager Instance { get; private set; }

    private bool vibrationEnabled = true;
    private const string VIBRATION_KEY = "VibrationEnabled";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load saved vibration setting (default = on)
            vibrationEnabled = PlayerPrefs.GetInt(VIBRATION_KEY, 1) == 1;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetVibrationEnabled(bool enabled)
    {
        vibrationEnabled = enabled;
        PlayerPrefs.SetInt(VIBRATION_KEY, enabled ? 1 : 0);
    }

    public void Vibrate(Most_HapticFeedback.HapticTypes type = Most_HapticFeedback.HapticTypes.MediumImpact)
    {
        if (vibrationEnabled)
        {
            Most_HapticFeedback.Generate(type);
        }
    }
}
