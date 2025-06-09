using UnityEngine;
using UnityEngine.Audio;

public class BGMusicPlayer : MonoBehaviour
{
    [Header("Music Settings")]
    public string resourcesFolder = "BGMusic";
    public float musicVolume = 1f;
    public float rotationSpeed = 90f; // градусов в секунду
    public bool rotateAroundLocalZ = true; // если false, вращать вокруг глобальной Y
    public GameObject vinylObject; // объект-пластинка

    [Header("Mixer Settings")]
    public AudioMixer bgMusicMixer;
    public string volumeParameter = "BGMusicVolume";
    public AudioMixerGroup mixerGroup;

    private AudioSource audioSource;
    private AudioClip[] musicClips;
    private int currentTrack = 0;
    private bool isPaused = false;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D звук
        if (mixerGroup != null)
            audioSource.outputAudioMixerGroup = mixerGroup;

        // Загружаем все треки из Resources/BGMusic
        musicClips = Resources.LoadAll<AudioClip>(resourcesFolder);
        if (musicClips.Length == 0)
        {
            Debug.LogWarning($"No BGMusic found in Resources/{resourcesFolder}");
            return;
        }
    }

    void Start()
    {
        SetMusicVolume(musicVolume);
        PlayCurrentTrack();
    }

    void Update()
    {
        // Вращаем пластинку, если музыка играет
        if (vinylObject != null && audioSource.isPlaying && !isPaused)
        {
            float angle = rotationSpeed * Time.deltaTime;
            if (rotateAroundLocalZ)
                vinylObject.transform.Rotate(Vector3.forward, angle, Space.Self);
            else
                vinylObject.transform.Rotate(Vector3.up, angle, Space.World);
        }

        // Если трек закончился, переходим к следующему
        if (!audioSource.isPlaying && !isPaused && musicClips.Length > 0)
        {
            NextTrack();
        }
    }

    public void PlayCurrentTrack()
    {
        if (musicClips.Length == 0) return;
        audioSource.clip = musicClips[currentTrack];
        audioSource.Play();
        isPaused = false;
    }

    public void NextTrack()
    {
        currentTrack = (currentTrack + 1) % musicClips.Length;
        PlayCurrentTrack();
    }

    public void PauseMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            isPaused = true;
        }
    }

    public void ResumeMusic()
    {
        if (audioSource.clip != null && isPaused)
        {
            audioSource.UnPause();
            isPaused = false;
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (bgMusicMixer != null && !string.IsNullOrEmpty(volumeParameter))
        {
            // Переводим громкость из 0-1 в децибелы (-80dB = тишина, 0dB = макс)
            float db = Mathf.Lerp(-80f, 0f, musicVolume);
            bgMusicMixer.SetFloat(volumeParameter, db);
        }
        // audioSource.volume = musicVolume; // Не используем напрямую, только через миксер
    }

    public bool IsPlaying()
    {
        return audioSource.isPlaying && !isPaused;
    }

    // Для теста: клавиши управления
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 200, 120));
        GUILayout.Label($"BG Music: {(IsPlaying() ? "Playing" : (isPaused ? "Paused" : "Stopped"))}");
        if (GUILayout.Button("Pause")) PauseMusic();
        if (GUILayout.Button("Resume")) ResumeMusic();
        if (GUILayout.Button("Next Track")) NextTrack();
        GUILayout.Label($"Volume: {musicVolume:F2}");
        float newVol = GUILayout.HorizontalSlider(musicVolume, 0f, 1f);
        if (Mathf.Abs(newVol - musicVolume) > 0.001f) SetMusicVolume(newVol);
        GUILayout.EndArea();
    }
} 