using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

public class MidiPlayer : MonoBehaviour
{
    public string midiFilePath = "Assets/Midi/GF.mid";
    public AudioSource audioSourcePrefab;
    public PickupType pickup = PickupType.Neck;
    
    [Range(0.1f, 2f)]
    public float bpmMultiplier = 1f;
    private List<Coroutine> activeCoroutines = new List<Coroutine>();
    private bool isPlaying = false;

    public void LoadAndPlayMidi()
    {
        if (isPlaying)
        {
            StopPlayback();
        }

        isPlaying = true;
        MidiFile midiFile = MidiFile.Read(midiFilePath);
        TempoMap tempoMap = midiFile.GetTempoMap();
        IEnumerable<Note> notes = midiFile.GetNotes();

        foreach (var note in notes)
        {
            var metricTime = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap);
            float noteStartTime = (float)metricTime.TotalSeconds / bpmMultiplier;
            int noteNumber = note.NoteNumber;

            var clip = NoteSampleBank.Instance.GetClipForNote(noteNumber, pickup);
            if (clip != null)
            {
                // Calculate the target fret based on the note number
                int targetFret = 0;
                for (int stringNum = 6; stringNum >= 1; stringNum--)
                {
                    int openNote = GetOpenNoteForString(stringNum);
                    int fret = noteNumber - openNote;
                    if (fret >= 0 && fret <= 21)
                    {
                        targetFret = fret;
                        break;
                    }
                }
                var coroutine = StartCoroutine(PlayNoteWithDelay(noteStartTime, clip, targetFret));
                activeCoroutines.Add(coroutine);
            }
        }
    }

    public void StopPlayback()
    {
        isPlaying = false;
        
        // Останавливаем все активные корутины
        foreach (var coroutine in activeCoroutines)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
        activeCoroutines.Clear();

        // Останавливаем только AudioSource из пула
        if (AudioSourcePool.Instance != null)
        {
            AudioSourcePool.Instance.StopAllSources();
        }
    }

    // Метод для динамического изменения BPM
    public void SetBPMMultiplier(float multiplier)
    {
        bpmMultiplier = Mathf.Clamp(multiplier, 0.1f, 2f);
        if (isPlaying)
        {
            // Перезапускаем воспроизведение с новым темпом
            LoadAndPlayMidi();
        }
    }

    private int GetOpenNoteForString(int stringNum) =>
        stringNum switch
        {
            6 => 40,
            5 => 45,
            4 => 50,
            3 => 55,
            2 => 59,
            1 => 64,
            _ => 40
        };

    IEnumerator PlayNoteWithDelay(float delay, AudioClip clip, int targetFret)
    {
        yield return new WaitForSeconds(delay);

        if (!isPlaying) yield break; // Проверяем, не остановлено ли воспроизведение

        if (clip == null)
        {
            Debug.LogWarning("No clip to play!");
            yield break;
        }

        AudioSource source = AudioSourcePool.Instance.GetSource();
        source.clip = clip;

        string[] parts = clip.name.Split('_');
        if (parts.Length >= 2 && int.TryParse(parts[1], out int baseFret))
        {
            float pitch = Mathf.Pow(2, (targetFret - baseFret) / 12f);
            source.pitch = pitch;
        }
        else
        {
            source.pitch = 1f;
        }

        source.priority = 128;
        source.Play();

        StartCoroutine(DisableAfter(source, clip.length / source.pitch + 0.1f));
    }

    IEnumerator DisableAfter(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (source != null)
        {
            source.Stop();
            source.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        StopPlayback();
    }
}
