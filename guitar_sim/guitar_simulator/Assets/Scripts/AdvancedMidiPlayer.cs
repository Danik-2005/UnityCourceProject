using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Linq;

public class AdvancedMidiPlayer : MonoBehaviour
{
    [Header("Playback Settings")]
    [SerializeField] private int maxConcurrentNotes = 10;
    [SerializeField] private bool enablePolyphony = true;
    
    private bool isPlaying = false;
    private MidiFileInfo currentFileInfo;
    private float currentBpm;
    private Coroutine playbackCoroutine;
    private List<NoteEvent> activeNotes = new List<NoteEvent>();

    [System.Serializable]
    public class NoteEvent
    {
        public int stringNumber;
        public int fretNumber;
        public float startTime;
        public float endTime;
        public Coroutine coroutine;
    }

    public void PlayMidiFile(MidiFileInfo fileInfo, float bpm)
    {
        if (isPlaying)
        {
            StopPlayback();
        }

        if (GuitarStringsManager.Instance == null)
        {
            Debug.LogError("GuitarStringsManager not found!");
            return;
        }

        if (fileInfo == null)
        {
            Debug.LogError("MIDI file info is null!");
            return;
        }

        currentFileInfo = fileInfo;
        currentBpm = bpm;

        try
        {
            isPlaying = true;
            MidiFile midiFile = MidiFile.Read(fileInfo.filePath);
            TempoMap tempoMap = midiFile.GetTempoMap();
            IEnumerable<Note> notes = midiFile.GetNotes();

            float originalBpm = fileInfo.bpm;
            float speedMultiplier = bpm / originalBpm;

            Debug.Log($"Playing MIDI file: {fileInfo.fileName}");
            Debug.Log($"Original BPM: {originalBpm:F1}, Target BPM: {bpm:F1}, Speed Multiplier: {speedMultiplier:F2}");
            Debug.Log($"Total notes: {notes.Count()}");

            if (enablePolyphony)
            {
                playbackCoroutine = StartCoroutine(PlayNotesWithPolyphony(notes, tempoMap, speedMultiplier));
            }
            else
            {
                playbackCoroutine = StartCoroutine(PlayNotesSequentially(notes, tempoMap, speedMultiplier));
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading MIDI file {fileInfo.fileName}: {e.Message}");
            isPlaying = false;
        }
    }

    private IEnumerator PlayNotesSequentially(IEnumerable<Note> notes, TempoMap tempoMap, float speedMultiplier)
    {
        var sortedNotes = notes.OrderBy(note => note.Time).ToList();
        Debug.Log($"Starting sequential playback of {sortedNotes.Count} notes");

        foreach (var note in sortedNotes)
        {
            if (!isPlaying) break;

            var metricTime = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap);
            var metricLength = TimeConverter.ConvertTo<MetricTimeSpan>(note.Length, tempoMap);
            float noteStartTime = (float)metricTime.TotalSeconds / speedMultiplier;
            float noteDuration = (float)metricLength.TotalSeconds / speedMultiplier;

            if (noteStartTime > 0)
            {
                yield return new WaitForSeconds(noteStartTime);
            }

            if (!isPlaying) break;

            var (stringNumber, fretNumber) = GuitarStringsManager.Instance.FindBestStringAndFret(note.NoteNumber);
            if (stringNumber != -1)
            {
                GuitarStringsManager.Instance.PlayNoteWithVisuals(stringNumber, fretNumber);
                yield return new WaitForSeconds(noteDuration);
                GuitarStringsManager.Instance.StopNoteWithVisuals(stringNumber, fretNumber);
            }
        }

        Debug.Log("MIDI playback completed");
        isPlaying = false;
    }

    private IEnumerator PlayNotesWithPolyphony(IEnumerable<Note> notes, TempoMap tempoMap, float speedMultiplier)
    {
        var sortedNotes = notes.OrderBy(note => note.Time).ToList();
        Debug.Log($"Starting polyphonic playback of {sortedNotes.Count} notes");

        float currentTime = 0f;
        int noteIndex = 0;

        while (noteIndex < sortedNotes.Count && isPlaying)
        {
            // Обрабатываем ноты, которые должны начаться сейчас
            while (noteIndex < sortedNotes.Count && isPlaying)
            {
                var note = sortedNotes[noteIndex];
                var metricTime = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap);
                float noteStartTime = (float)metricTime.TotalSeconds * speedMultiplier;

                if (noteStartTime > currentTime)
                {
                    break; // Эта нота еще не должна начаться
                }

                // Запускаем ноту
                StartNote(note, tempoMap, speedMultiplier, currentTime);
                noteIndex++;
            }

            // Ждем небольшой интервал времени
            yield return new WaitForSeconds(0.01f);
            currentTime += 0.01f;

            // Очищаем завершенные ноты
            CleanupFinishedNotes(currentTime);
        }

        // Ждем завершения всех активных нот
        while (activeNotes.Count > 0 && isPlaying)
        {
            yield return new WaitForSeconds(0.1f);
            CleanupFinishedNotes(currentTime);
            currentTime += 0.1f;
        }

        Debug.Log("Polyphonic MIDI playback completed");
        isPlaying = false;
    }

    private void StartNote(Note note, TempoMap tempoMap, float speedMultiplier, float currentTime)
    {
        var metricTime = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap);
        var metricLength = TimeConverter.ConvertTo<MetricTimeSpan>(note.Length, tempoMap);
        float noteStartTime = (float)metricTime.TotalSeconds / speedMultiplier;
        float noteDuration = (float)metricLength.TotalSeconds / speedMultiplier;

        var (stringNumber, fretNumber) = GuitarStringsManager.Instance.FindBestStringAndFret(note.NoteNumber);
        if (stringNumber != -1)
        {
            // Проверяем лимит одновременных нот
            if (activeNotes.Count >= maxConcurrentNotes)
            {
                Debug.LogWarning($"Reached maximum concurrent notes limit ({maxConcurrentNotes})");
                return;
            }

            var noteEvent = new NoteEvent
            {
                stringNumber = stringNumber,
                fretNumber = fretNumber,
                startTime = noteStartTime,
                endTime = noteStartTime + noteDuration
            };

            noteEvent.coroutine = StartCoroutine(PlayNoteCoroutine(noteEvent));
            activeNotes.Add(noteEvent);

            Debug.Log($"Started note: String {stringNumber}, Fret {fretNumber}, Duration: {noteDuration:F2}s");
        }
    }

    private IEnumerator PlayNoteCoroutine(NoteEvent noteEvent)
    {
        GuitarStringsManager.Instance.PlayNoteWithVisuals(noteEvent.stringNumber, noteEvent.fretNumber);
        
        float duration = noteEvent.endTime - noteEvent.startTime;
        yield return new WaitForSeconds(duration);
        
        GuitarStringsManager.Instance.StopNoteWithVisuals(noteEvent.stringNumber, noteEvent.fretNumber);
    }

    private void CleanupFinishedNotes(float currentTime)
    {
        for (int i = activeNotes.Count - 1; i >= 0; i--)
        {
            if (activeNotes[i].endTime <= currentTime)
            {
                if (activeNotes[i].coroutine != null)
                {
                    StopCoroutine(activeNotes[i].coroutine);
                }
                activeNotes.RemoveAt(i);
            }
        }
    }

    public void StopPlayback()
    {
        isPlaying = false;
        
        if (playbackCoroutine != null)
        {
            StopCoroutine(playbackCoroutine);
            playbackCoroutine = null;
        }

        // Останавливаем все активные ноты
        foreach (var noteEvent in activeNotes)
        {
            if (noteEvent.coroutine != null)
            {
                StopCoroutine(noteEvent.coroutine);
            }
            GuitarStringsManager.Instance.StopNoteWithVisuals(noteEvent.stringNumber, noteEvent.fretNumber);
        }
        activeNotes.Clear();

        if (GuitarStringsManager.Instance != null)
        {
            GuitarStringsManager.Instance.StopAllStrings();
        }
        
        Debug.Log("MIDI playback stopped");
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    public MidiFileInfo GetCurrentFileInfo()
    {
        return currentFileInfo;
    }

    public float GetCurrentBpm()
    {
        return currentBpm;
    }

    public int GetActiveNotesCount()
    {
        return activeNotes.Count;
    }
} 