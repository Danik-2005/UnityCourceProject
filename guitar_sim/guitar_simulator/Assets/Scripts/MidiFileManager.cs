using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

[System.Serializable]
public class MidiFileInfo
{
    public string fileName;
    public string filePath;
    public TextAsset midiAsset;
    public float duration; // в секундах
    public float bpm; // beats per minute
    public string description;
    public bool isLoaded = false;
}

public class MidiFileManager : MonoBehaviour
{
    [Header("MIDI Files Settings")]
    [SerializeField] private string midiFolderPath = "Midi";
    [SerializeField] private bool loadOnStart = true;
    
    [Header("File Information")]
    [SerializeField] private List<MidiFileInfo> midiFiles = new List<MidiFileInfo>();
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI infoText; // Для отображения информации о файле
    
    [Header("Playback Settings")]
    [SerializeField] private float defaultBpm = 120f;
    [SerializeField] private MonoBehaviour playbackController; // Ссылка на контроллер воспроизведения
    
    private MidiFileInfo selectedFile;
    private MidiFileInfo hoveredFile;
    
    public List<MidiFileInfo> MidiFiles => midiFiles;
    public MidiFileInfo SelectedFile => selectedFile;
    public MidiFileInfo HoveredFile => hoveredFile;
    
    private void Start()
    {
        if (loadOnStart)
        {
            LoadMidiFiles();
        }
    }
    
    [ContextMenu("Load MIDI Files")]
    public void LoadMidiFiles()
    {
        midiFiles.Clear();
        
        // Получаем путь к папке MIDI
        string fullPath = Path.Combine(Application.dataPath, midiFolderPath);
        
        if (!Directory.Exists(fullPath))
        {
            Debug.LogError($"MIDI folder not found: {fullPath}");
            return;
        }
        
        // Получаем все .mid файлы
        string[] midiFilePaths = Directory.GetFiles(fullPath, "*.mid", SearchOption.TopDirectoryOnly);
        
        Debug.Log($"Found {midiFilePaths.Length} MIDI files in {fullPath}");
        
        foreach (string filePath in midiFilePaths)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            
            // Создаем MidiFileInfo
            MidiFileInfo fileInfo = new MidiFileInfo
            {
                fileName = fileName,
                filePath = filePath,
                description = $"MIDI file: {fileName}",
                isLoaded = true
            };
            
            Debug.Log($"Starting to parse MIDI file: {fileName}");
            
            // Парсим информацию о файле используя DryWetMidi
            ParseMidiInfoWithDryWetMidi(fileInfo);
            
            midiFiles.Add(fileInfo);
            
            Debug.Log($"Successfully loaded MIDI file: {fileName} (Duration: {fileInfo.duration:F1}s, BPM: {fileInfo.bpm:F1})");
        }
        
        Debug.Log($"Successfully loaded {midiFiles.Count} MIDI files");
    }
    
    private void ParseMidiInfoWithDryWetMidi(MidiFileInfo fileInfo)
    {
        try
        {
            // Загружаем MIDI файл используя DryWetMidi
            MidiFile midiFile = MidiFile.Read(fileInfo.filePath);
            
            // Получаем карту темпа
            TempoMap tempoMap = midiFile.GetTempoMap();
            
            // Получаем все ноты
            IEnumerable<Note> notes = midiFile.GetNotes();
            
            // Находим последнюю ноту для определения длительности
            long maxTime = 0;
            foreach (var note in notes)
            {
                long noteEndTime = note.Time + note.Length;
                if (noteEndTime > maxTime)
                {
                    maxTime = noteEndTime;
                }
            }
            
            // Конвертируем время в секунды
            var totalTime = TimeConverter.ConvertTo<MetricTimeSpan>(maxTime, tempoMap);
            fileInfo.duration = (float)totalTime.TotalSeconds;
            
            // Получаем BPM из карты темпа (берем первый темп)
            var tempoChanges = tempoMap.GetTempoChanges().ToList();
            if (tempoChanges.Count > 0)
            {
                var firstTempo = tempoChanges[0];
                fileInfo.bpm = (float)firstTempo.Value.BeatsPerMinute;
            }
            else
            {
                fileInfo.bpm = defaultBpm;
            }
            
            Debug.Log($"Parsed {fileInfo.fileName}: BPM={fileInfo.bpm:F1}, Duration={fileInfo.duration:F1}s");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Failed to parse MIDI info for {fileInfo.fileName}: {e.Message}");
            fileInfo.bpm = defaultBpm;
            fileInfo.duration = 60f; // 1 минута по умолчанию
        }
    }
    
    public void SelectFile(MidiFileInfo fileInfo)
    {
        selectedFile = fileInfo;
        UpdateInfoDisplay();
        Debug.Log($"Selected MIDI file: {fileInfo.fileName}");
    }
    
    public void HoverFile(MidiFileInfo fileInfo)
    {
        hoveredFile = fileInfo;
        UpdateInfoDisplay();
    }
    
    public void ClearHover()
    {
        hoveredFile = null;
        UpdateInfoDisplay();
    }
    
    private void UpdateInfoDisplay()
    {
        if (infoText == null) return;
        
        MidiFileInfo displayFile = hoveredFile ?? selectedFile;
        
        if (displayFile != null)
        {
            string info = $"File: {displayFile.fileName}\n";
            info += $"Duration: {displayFile.duration:F1}s\n";
            info += $"BPM: {displayFile.bpm:F0}";
            infoText.text = info;
        }
        else
        {
            infoText.text = "No file selected";
        }
    }
    
    public void ConfirmPlayback()
    {
        if (selectedFile != null && playbackController != null)
        {
            // Вызываем метод воспроизведения через рефлексию
            var playMethod = playbackController.GetType().GetMethod("PlayMidiFile");
            if (playMethod != null)
            {
                playMethod.Invoke(playbackController, new object[] { selectedFile.filePath, selectedFile.bpm });
                Debug.Log($"Starting playback of {selectedFile.fileName} at {selectedFile.bpm} BPM");
            }
            else
            {
                Debug.LogError("PlayMidiFile method not found in playback controller");
            }
        }
        else
        {
            Debug.LogWarning("No file selected or playback controller not assigned");
        }
    }
    
    public MidiFileInfo GetFileByName(string fileName)
    {
        return midiFiles.Find(f => f.fileName == fileName);
    }
    
    public int GetFileCount()
    {
        return midiFiles.Count;
    }
    
    [ContextMenu("Refresh MIDI Files")]
    public void RefreshMidiFiles()
    {
        LoadMidiFiles();
    }
} 