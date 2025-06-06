using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

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
    public float releaseTime = 0.3f;
    public PickupType currentPickup = PickupType.Neck;

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
        Debug.Log($"[GuitarSoundSystem] Starting note playback: String {stringNumber}, Fret {fretNumber}");
        
        StopNote(stringNumber, fretNumber); // Остановить предыдущую ноту, если была

        // Получаем информацию о ближайшем доступном сэмпле
        var (clip, isExactMatch, baseFret) = NoteSampleBank.Instance.GetClipForStringAndFret(stringNumber, fretNumber, currentPickup);
        
        if (clip == null)
        {
            Debug.LogWarning($"[GuitarSoundSystem] No clip found for string {stringNumber}, fret {fretNumber}");
            return;
        }
        Debug.Log($"[GuitarSoundSystem] Found sample: {clip.name}, isExactMatch: {isExactMatch}, baseFret: {baseFret}");

        AudioSource source = audioPool.GetSource();
        if (source == null)
        {
            Debug.LogError("[GuitarSoundSystem] Failed to get audio source from pool");
            return;
        }

        source.clip = clip;
        source.outputAudioMixerGroup = outputMixerGroup;
        source.loop = false;
        
        // Базовая громкость зависит от номера струны (толстые струны громче)
        float stringVolumeMod = Mathf.Lerp(0.8f, 1.2f, (6f - stringNumber) / 5f);
        source.volume = baseVolume * stringVolumeMod;

        // Применяем питч-шифтинг только если это не точное совпадение
        if (!isExactMatch)
        {
            // Вычисляем разницу в ладах между целевым и базовым сэмплом
            float fretDifference = fretNumber - baseFret;
            source.pitch = Mathf.Pow(2f, fretDifference / 12f);
            Debug.Log($"[GuitarSoundSystem] Applied pitch shifting: {fretDifference} frets, multiplier: {source.pitch:F3}");
        }
        else
        {
            source.pitch = 1f;
            Debug.Log("[GuitarSoundSystem] Using exact sample, no pitch shifting needed");
        }

        source.Play();
        activeNotes[(stringNumber, fretNumber)] = source;
        Debug.Log($"[GuitarSoundSystem] Note started playing with volume {source.volume:F2} and pitch {source.pitch:F2}");
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
                Debug.Log($"[GuitarSoundSystem] Immediately stopped note: String {stringNumber}, Fret {fretNumber}");
            }
            else
            {
                StartCoroutine(FadeOutAndStop(source, releaseTime));
                Debug.Log($"[GuitarSoundSystem] Started fade out for note: String {stringNumber}, Fret {fretNumber}");
            }
            activeNotes.Remove(noteKey);
        }
    }

    public void StopAllNotes(bool immediate = false)
    {
        Debug.Log("[GuitarSoundSystem] Stopping all notes");
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
        Debug.Log("[GuitarSoundSystem] Completed fade out and stopped note");
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
    }
} 