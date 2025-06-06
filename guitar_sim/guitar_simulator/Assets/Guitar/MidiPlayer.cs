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

        if (GuitarSoundSystem.Instance == null)
        {
            Debug.LogError("GuitarSoundSystem not found!");
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

            // Используем новый метод из GuitarSoundSystem для поиска струны и лада
            var (stringNumber, fretNumber) = GuitarSoundSystem.Instance.FindBestStringAndFret(note.NoteNumber);
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
            LoadAndPlayMidi();
        }
    }

    private IEnumerator PlayNoteWithTiming(float startTime, float duration, int stringNumber, int fretNumber)
    {
        yield return new WaitForSeconds(startTime);
        GuitarSoundSystem.Instance.PlayNote(stringNumber, fretNumber);
        
        yield return new WaitForSeconds(duration);
        GuitarSoundSystem.Instance.StopNote(stringNumber, fretNumber);
    }

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

        if (GuitarSoundSystem.Instance != null)
        {
            GuitarSoundSystem.Instance.StopAllNotes(true);
        }
    }
}
