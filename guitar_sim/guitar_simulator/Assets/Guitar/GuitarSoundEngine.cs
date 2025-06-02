using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System;

public class GuitarSoundEngine : MonoBehaviour
{
    public static GuitarSoundEngine Instance { get; private set; }

    [Header("Audio Settings")]
    public AudioMixerGroup outputMixerGroup;
    public float baseVolume = 1f;
    public float releaseTime = 0.3f; // Время затухания при отпускании струны
    public PickupType pickup = PickupType.Neck;

    private AudioSourcePool audioPool;
    private Dictionary<(int stringNumber, int fretNumber), AudioSource> activeNotes = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
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

    public void StartNote(int stringNumber, int fretNumber)
    {
        StopNote(stringNumber, fretNumber); // Остановить предыдущую ноту, если была

        int midiNote = GetMIDINoteFromStringAndFret(stringNumber, fretNumber);
        AudioClip clip = NoteSampleBank.Instance.GetClipForNote(midiNote, pickup);
        
        if (clip == null)
        {
            Debug.LogWarning($"No clip found for note {midiNote} (string {stringNumber}, fret {fretNumber})");
            return;
        }

        AudioSource source = audioPool.GetSource();
        if (source == null) return;

        source.clip = clip;
        source.outputAudioMixerGroup = outputMixerGroup;
        source.loop = true; // Включаем зацикливание для удержания ноты
        
        // Базовая громкость зависит от номера струны (толстые струны громче)
        float stringVolumeMod = Mathf.Lerp(0.8f, 1.2f, (6f - stringNumber) / 5f);
        source.volume = baseVolume * stringVolumeMod;

        // Определяем базовую MIDI-ноту для сэмпла
        string[] parts = clip.name.Split('_');
        int baseMidiNote = midiNote;
        if (parts.Length >= 2 && int.TryParse(parts[1], out int baseFret))
        {
            // Вычисляем базовую MIDI-ноту сэмпла
            baseMidiNote = GetMIDINoteFromStringAndFret(stringNumber, baseFret);
        }

        // Применяем питч-шифтинг на основе разницы MIDI-нот
        float semitoneShift = midiNote - baseMidiNote;
        source.pitch = Mathf.Pow(2f, semitoneShift / 12f);

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
            }
            else
            {
                StartCoroutine(FadeOutAndStop(source, releaseTime));
            }
            activeNotes.Remove(noteKey);
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
        source.volume = startVolume; // Восстанавливаем громкость для следующего использования
    }

    public void StopAllNotes(bool immediate = false)
    {
        foreach (var note in new Dictionary<(int, int), AudioSource>(activeNotes))
        {
            StopNote(note.Key.Item1, note.Key.Item2, immediate);
        }
    }

    private int GetMIDINoteFromStringAndFret(int stringNumber, int fretNumber)
    {
        int baseNote = stringNumber switch
        {
            6 => 40, // E2
            5 => 45, // A2
            4 => 50, // D3
            3 => 55, // G3
            2 => 59, // B3
            1 => 64, // E4
            _ => 40  // Default to E2
        };

        return baseNote + fretNumber;
    }

    private void OnDisable()
    {
        StopAllNotes(true);
    }
} 