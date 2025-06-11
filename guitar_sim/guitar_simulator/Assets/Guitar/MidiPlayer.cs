using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class MidiPlayer : MonoBehaviour
{
    [Header("Playback Settings")]
    [Range(50f, 200f)]
    public float targetBpm = 120f;
    
    [Header("UI Integration")]
    [SerializeField] private Slider bpmSlider;
    [SerializeField] private TextMeshProUGUI bpmValueText;
    
    private bool isPlaying = false;
    private MidiFileInfo currentFileInfo;
    private List<Coroutine> activeCoroutines = new List<Coroutine>();
    
    private void Start()
    {
        // Настраиваем слайдер BPM если он назначен
        if (bpmSlider != null)
        {
            bpmSlider.minValue = 50f;
            bpmSlider.maxValue = 200f;
            bpmSlider.value = targetBpm;
            bpmSlider.onValueChanged.AddListener(OnBpmSliderChanged);
        }
        
        UpdateBpmDisplay();
    }
    
    public void PlayMidiFile(MidiFileInfo fileInfo, float bpm)
    {
        if (isPlaying)
        {
            StopPlayback();
        }

        if (GuitarStringsManager.Instance == null)
        {
            return;
        }

        if (fileInfo == null)
        {
            return;
        }

        currentFileInfo = fileInfo;
        targetBpm = bpm;
        
        // Обновляем слайдер если он назначен
        if (bpmSlider != null)
        {
            bpmSlider.value = targetBpm;
        }
        
        UpdateBpmDisplay();

        try
        {
            isPlaying = true;
            MidiFile midiFile = MidiFile.Read(fileInfo.filePath);
            TempoMap tempoMap = midiFile.GetTempoMap();
            IEnumerable<Note> notes = midiFile.GetNotes();

            // Очищаем предыдущие корутины
            activeCoroutines.Clear();

            // Запускаем воспроизведение каждой ноты
            foreach (var note in notes)
            {
                var metricTime = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap);
                var metricLength = TimeConverter.ConvertTo<MetricTimeSpan>(note.Length, tempoMap);
                
                // Вычисляем множитель скорости
                float speedMultiplier = targetBpm / fileInfo.bpm;
                
                float noteStartTime = (float)metricTime.TotalSeconds / speedMultiplier;
                float noteDuration = (float)metricLength.TotalSeconds / speedMultiplier;

                // Используем новый метод из GuitarStringsManager для поиска струны и лада
                var (stringNumber, fretNumber) = GuitarStringsManager.Instance.FindBestStringAndFret(note.NoteNumber);
                if (stringNumber != -1)
                {
                    var coroutine = StartCoroutine(PlayNoteWithTiming(noteStartTime, noteDuration, stringNumber, fretNumber));
                    activeCoroutines.Add(coroutine);
                }
            }
        }
        catch (System.Exception e)
        {
            isPlaying = false;
        }
    }

    private IEnumerator PlayNoteWithTiming(float startTime, float duration, int stringNumber, int fretNumber)
    {
        yield return new WaitForSeconds(startTime);
        
        if (isPlaying)
        {
            GuitarStringsManager.Instance.PlayNoteWithVisuals(stringNumber, fretNumber);
            
            yield return new WaitForSeconds(duration);
            
            if (isPlaying)
            {
                GuitarStringsManager.Instance.StopNoteWithVisuals(stringNumber, fretNumber);
            }
        }
    }

    public void SetTargetBpm(float bpm)
    {
        targetBpm = Mathf.Clamp(bpm, 50f, 200f);
        
        // Обновляем слайдер если он назначен
        if (bpmSlider != null)
        {
            bpmSlider.value = targetBpm;
        }
        
        UpdateBpmDisplay();
        
        // Если воспроизведение активно, перезапускаем с новым BPM
        if (isPlaying && currentFileInfo != null)
        {
            PlayMidiFile(currentFileInfo, targetBpm);
        }
    }

    private void OnBpmSliderChanged(float bpmValue)
    {
        SetTargetBpm(bpmValue);
    }
    
    private void UpdateBpmDisplay()
    {
        if (bpmValueText != null)
        {
            bpmValueText.text = $"BPM: {targetBpm:F0}";
        }
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

        if (GuitarStringsManager.Instance != null)
        {
            GuitarStringsManager.Instance.StopAllStrings();
        }
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
        return targetBpm;
    }
    
    // Методы для интеграции с UI
    public void SetBpmSlider(Slider slider)
    {
        bpmSlider = slider;
        if (bpmSlider != null)
        {
            bpmSlider.minValue = 50f;
            bpmSlider.maxValue = 200f;
            bpmSlider.value = targetBpm;
            bpmSlider.onValueChanged.AddListener(OnBpmSliderChanged);
        }
    }
    
    public void SetBpmValueText(TextMeshProUGUI text)
    {
        bpmValueText = text;
        UpdateBpmDisplay();
    }
}
