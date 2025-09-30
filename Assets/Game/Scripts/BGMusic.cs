using UnityEngine;

public class BGMusic : MonoBehaviour
{
    public static BGMusic Instance { get; private set; }

    public AudioClip myMusicClip;

    private AudioSource musicSource;
    private bool musicEnabled = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        BGMusic.Instance.PlayMusic(myMusicClip);
    }

    public void PlayMusic(AudioClip music)
    {
        if (musicSource.clip == music) return;

        musicSource.clip = music;
        if (musicEnabled)
            musicSource.Play();
    }

    public void SetMusicEnabled(bool enabled)
    {
        musicEnabled = enabled;
        if (musicSource == null) return;

        if (enabled && !musicSource.isPlaying)
            musicSource.Play();
        else if (!enabled && musicSource.isPlaying)
            musicSource.Pause();
    }
}
