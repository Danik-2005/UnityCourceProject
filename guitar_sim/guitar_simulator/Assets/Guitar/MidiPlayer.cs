using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

public class MidiPlayer : MonoBehaviour
{
    public string midiFilePath = "Assets/Midi/GF.mid";
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

        if (GuitarSoundEngine.Instance == null)
        {
            Debug.LogError("GuitarSoundEngine not found!");
            return;
        }

        isPlaying = true;
        MidiFile midiFile = MidiFile.Read(midiFilePath);
        TempoMap tempoMap = midiFile.GetTempoMap();
        IEnumerable<Note> notes = midiFile.GetNotes();

        foreach (var note in notes)
        {
            var metricTime = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap);
            var metricLength = TimeConverter.ConvertTo<MetricTimeSpan>(note.Length, tempoMap);
            float noteStartTime = (float)metricTime.TotalSeconds / bpmMultiplier;
            float noteDuration = (float)metricLength.TotalSeconds / bpmMultiplier;

            // Находим подходящую струну и лад для ноты
            (int stringNumber, int fretNumber) = FindBestStringAndFret(note.NoteNumber);
            if (stringNumber != -1)
            {
                var coroutine = StartCoroutine(PlayNoteWithTiming(noteStartTime, noteDuration, stringNumber, fretNumber));
                activeCoroutines.Add(coroutine);
            }
        }
    }

    public void SetBPMMultiplier(float multiplier)
    {
        bpmMultiplier = Mathf.Clamp(multiplier, 0.1f, 2f);
        if (isPlaying)
        {
            // Перезапускаем воспроизведение с новым темпом
            LoadAndPlayMidi();
        }
    }

    private IEnumerator PlayNoteWithTiming(float startTime, float duration, int stringNumber, int fretNumber)
    {
        yield return new WaitForSeconds(startTime);
        GuitarSoundEngine.Instance.StartNote(stringNumber, fretNumber);
        
        yield return new WaitForSeconds(duration);
        GuitarSoundEngine.Instance.StopNote(stringNumber, fretNumber);
    }

    private (int stringNumber, int fretNumber) FindBestStringAndFret(int targetNote)
    {
        // Перебираем все струны от 6-й к 1-й
        for (int stringNum = 6; stringNum >= 1; stringNum--)
        {
            int openNote = GetOpenNoteForString(stringNum);
            int fret = targetNote - openNote;
            
            // Проверяем, можно ли сыграть эту ноту на данной струне
            if (fret >= 0 && fret <= 22)
            {
                return (stringNum, fret);
            }
        }
        
        return (-1, -1);
    }

    private int GetOpenNoteForString(int stringNum) =>
        stringNum switch
        {
            6 => 40, // E2
            5 => 45, // A2
            4 => 50, // D3
            3 => 55, // G3
            2 => 59, // B3
            1 => 64, // E4
            _ => 40  // Default to E2
        };

    public void StopPlayback()
    {
        isPlaying = false;
        
        foreach (var coroutine in activeCoroutines)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
        activeCoroutines.Clear();

        if (GuitarSoundEngine.Instance != null)
        {
            GuitarSoundEngine.Instance.StopAllNotes(true);
        }
    }
}
