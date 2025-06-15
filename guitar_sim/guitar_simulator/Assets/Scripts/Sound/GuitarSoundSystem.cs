using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using GuitarSimulator;

public class GuitarSoundSystem : MonoBehaviour
{
    private static GuitarSoundSystem instance;
    public static GuitarSoundSystem Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GuitarSoundSystem>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GuitarSoundSystem");
                    instance = go.AddComponent<GuitarSoundSystem>();
                }
            }
            return instance;
        }
    }

    [Header("Audio Settings")]
    public AudioMixerGroup outputMixerGroup;
    public float baseVolume = 1f;
    public float releaseTime = 1.5f;
    public PickupType currentPickup = PickupType.Neck;
    
    [Header("Connection Management")]
    [SerializeField] private bool useConnectionManager = true;
    [SerializeField] private float mutedVolume = 0.01f; // Громкость когда не подключено

    private AudioSourcePool audioPool;
    private Dictionary<(int stringNumber, int fretNumber), AudioSource> activeNotes = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        audioPool = FindObjectOfType<AudioSourcePool>();
        if (audioPool == null)
        {
            Debug.LogError("AudioSourcePool not found!");
            return;
        }
        
        // Подписываемся на события ConnectionManager
        if (useConnectionManager && ConnectionManager.Instance != null)
        {
            ConnectionManager.Instance.OnConnectionStateChanged += UpdateAllNotesVolume;
            Debug.Log("GuitarSoundSystem: Subscribed to ConnectionManager events");
        }
    }

    /// <summary>
    /// Получить текущую громкость в зависимости от состояния подключений
    /// </summary>
    private float GetCurrentVolume()
    {
        if (!useConnectionManager) return baseVolume;
        
        // Проверяем ConnectionManager
        if (ConnectionManager.Instance != null)
        {
            if (ConnectionManager.Instance.IsFullyConnected)
            {
                return baseVolume; // Нормальная громкость
            }
            else
            {
                return mutedVolume; // Приглушенная громкость
            }
        }
        
        return baseVolume; // По умолчанию нормальная громкость
    }

    /// <summary>
    /// Обновить громкость всех активных нот
    /// </summary>
    public void UpdateAllNotesVolume()
    {
        if (!useConnectionManager) return;
        
        float currentVolume = GetCurrentVolume();
        
        foreach (var note in activeNotes)
        {
            AudioSource source = note.Value;
            if (source != null && source.isPlaying)
            {
                // Восстанавливаем модификатор громкости струны
                int stringNumber = note.Key.Item1;
                float stringVolumeMod = Mathf.Lerp(0.8f, 1.2f, (6f - stringNumber) / 5f);
                source.volume = currentVolume * stringVolumeMod;
            }
        }
    }

    public (int stringNumber, int fretNumber) FindBestStringAndFret(int targetMidiNote)
    {
        // Перебираем все струны от 6-й к 1-й
        for (int stringNum = 6; stringNum >= 1; stringNum--)
        {
            int openNote = GetOpenNoteForString(stringNum);
            int fret = targetMidiNote - openNote;
            
            // Проверяем, можно ли сыграть эту ноту на данной струне
            if (fret >= 0 && fret <= 22)
            {
                return (stringNum, fret);
            }
        }
        
        return (-1, -1);
    }

    public void PlayNote(int stringNumber, int fretNumber)
    {

        StopNote(stringNumber, fretNumber); // Остановить предыдущую ноту, если была

        // Получаем информацию о ближайшем доступном сэмпле
        var (clip, isExactMatch, baseFret) = NoteSampleBank.Instance.GetClipForStringAndFret(stringNumber, fretNumber, currentPickup);
        
        if (clip == null)
        {
           
            return;
        }
        

        AudioSource source = audioPool.GetSource();
        if (source == null)
        {
            return;
        }

        source.clip = clip;
        source.outputAudioMixerGroup = outputMixerGroup;
        source.loop = false;
        
        // Базовая громкость зависит от номера струны (толстые струны громче)
        float stringVolumeMod = Mathf.Lerp(0.8f, 1.2f, (6f - stringNumber) / 5f);
        source.volume = GetCurrentVolume() * stringVolumeMod;

        // Применяем питч-шифтинг только если это не точное совпадение
        if (!isExactMatch)
        {
            // Вычисляем разницу в ладах между целевым и базовым сэмплом
            float fretDifference = fretNumber - baseFret;
            source.pitch = Mathf.Pow(2f, fretDifference / 12f);
        }
        else
        {
            source.pitch = 1f;
        }

        source.Play();
        activeNotes[(stringNumber, fretNumber)] = source;
    }

    public void StopNote(int stringNumber, int fretNumber, bool immediate = false)
    {
        var noteKey = (stringNumber, fretNumber);
        if (activeNotes.TryGetValue(noteKey, out AudioSource source))
        {
            if (immediate)
            {
                source.Stop();
                audioPool.ReturnSource(source);
            }
            else
            {
                StartCoroutine(FadeOutAndStop(source, releaseTime));
            }
            activeNotes.Remove(noteKey);
        }
    }

    public void StopAllNotes(bool immediate = false)
    {
        foreach (var note in new Dictionary<(int, int), AudioSource>(activeNotes))
        {
            StopNote(note.Key.Item1, note.Key.Item2, immediate);
        }
    }

    private System.Collections.IEnumerator FadeOutAndStop(AudioSource source, float fadeTime)
    {
        float startVolume = source.volume;
        float startTime = Time.time;

        while (Time.time - startTime < fadeTime)
        {
            float t = (Time.time - startTime) / fadeTime;
            source.volume = startVolume * (1f - t);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
        audioPool.ReturnSource(source);
    }

    public int GetMIDINoteFromStringAndFret(int stringNumber, int fretNumber)
    {
        int baseNote = GetOpenNoteForString(stringNumber);
        return baseNote + fretNumber;
    }

    public int GetOpenNoteForString(int stringNumber) =>
        stringNumber switch
        {
            6 => 40, // E2
            5 => 45, // A2
            4 => 50, // D3
            3 => 55, // G3
            2 => 59, // B3
            1 => 64, // E4
            _ => 40  // Default to E2
        };

    private void OnDisable()
    {
        StopAllNotes(true);
        
        // Отписываемся от событий ConnectionManager
        if (useConnectionManager && ConnectionManager.Instance != null)
        {
            ConnectionManager.Instance.OnConnectionStateChanged -= UpdateAllNotesVolume;
        }
    }
} 