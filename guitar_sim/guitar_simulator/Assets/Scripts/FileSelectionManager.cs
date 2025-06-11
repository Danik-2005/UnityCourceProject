using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

[System.Serializable]
public class AudioFileData
{
    public string fileName;
    public AudioClip audioFile;
    public string description;
    public AudioMixerGroup mixerGroup;
}

[System.Serializable]
public class MidiFileData
{
    public string fileName;
    public TextAsset midiFile;
    public string description;
    public bool isEnabled = true;
}

public class FileSelectionManager : MonoBehaviour
{
    [Header("Audio Files")]
    [SerializeField] private AudioFileData[] audioFiles;
    
    [Header("MIDI Files")]
    [SerializeField] private MidiFileData[] midiFiles;
    
    [Header("UI Settings")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private AudioSource audioSource;
    
    [Header("Build Settings")]
    [SerializeField] private bool loadFromResources = true;
    [SerializeField] private string resourcesFolder = "AudioFiles";
    
    [Header("MIDI Player Integration")]
    [SerializeField] private MonoBehaviour midiPlayer; // Ссылка на ваш MIDI плеер
    
    private List<FileSelectionButton> createdButtons = new List<FileSelectionButton>();
    private FileSelectionButton currentSelectedButton;
    
    private void Start()
    {
        InitializeAudioFiles();
        InitializeMidiFiles();
        CreateButtons();
    }
    
    private void InitializeAudioFiles()
    {
        if (loadFromResources && audioFiles.Length == 0)
        {
            // Загружаем файлы из Resources папки
            AudioClip[] clips = Resources.LoadAll<AudioClip>(resourcesFolder);
            audioFiles = new AudioFileData[clips.Length];
            
            for (int i = 0; i < clips.Length; i++)
            {
                audioFiles[i] = new AudioFileData
                {
                    fileName = clips[i].name,
                    audioFile = clips[i],
                    description = $"Audio file {i + 1}",
                    mixerGroup = null
                };
            }
            
            Debug.Log($"Loaded {audioFiles.Length} audio files from Resources/{resourcesFolder}");
        }
        
        // Проверяем корректность загруженных файлов
        for (int i = 0; i < audioFiles.Length; i++)
        {
            if (audioFiles[i].audioFile == null)
            {
                Debug.LogError($"Audio file {i} is null! Check your Resources folder or inspector settings.");
            }
        }
    }
    
    private void InitializeMidiFiles()
    {
        if (loadFromResources && midiFiles.Length == 0)
        {
            // Загружаем MIDI файлы из папки Midi
            TextAsset[] midiAssets = Resources.LoadAll<TextAsset>("Midi");
            midiFiles = new MidiFileData[midiAssets.Length];
            
            for (int i = 0; i < midiAssets.Length; i++)
            {
                midiFiles[i] = new MidiFileData
                {
                    fileName = midiAssets[i].name,
                    midiFile = midiAssets[i],
                    description = $"MIDI file {i + 1}",
                    isEnabled = true
                };
            }
            
            Debug.Log($"Loaded {midiFiles.Length} MIDI files from Resources/Midi");
        }
        
        // Проверяем корректность загруженных файлов
        for (int i = 0; i < midiFiles.Length; i++)
        {
            if (midiFiles[i].midiFile == null)
            {
                Debug.LogError($"MIDI file {i} is null! Check your Resources/Midi folder or inspector settings.");
            }
        }
    }
    
    private void CreateButtons()
    {
        if (buttonPrefab == null || buttonContainer == null)
        {
            Debug.LogError("Button prefab or container not assigned!");
            return;
        }
        
        // Очищаем существующие кнопки
        foreach (var button in createdButtons)
        {
            if (button != null)
                Destroy(button.gameObject);
        }
        createdButtons.Clear();
        
        // Создаем кнопки для MIDI файлов
        for (int i = 0; i < midiFiles.Length; i++)
        {
            if (midiFiles[i].isEnabled)
            {
                GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
                FileSelectionButton button = buttonObj.GetComponent<FileSelectionButton>();
                
                if (button != null)
                {
                    button.SetupMidiButton(midiFiles[i], this);
                    createdButtons.Add(button);
                }
            }
        }
        
        Debug.Log($"Created {createdButtons.Count} buttons for MIDI files");
    }
    
    public void OnMidiButtonSelected(FileSelectionButton selectedButton)
    {
        // Снимаем выделение с предыдущей кнопки
        if (currentSelectedButton != null && currentSelectedButton != selectedButton)
        {
            currentSelectedButton.SetSelected(false);
        }
        
        currentSelectedButton = selectedButton;
        
        // Загружаем MIDI файл в плеер
        if (selectedButton != null)
        {
            MidiFileData data = selectedButton.GetMidiData();
            if (data.midiFile != null)
            {
                LoadMidiFile(data);
                Debug.Log($"Loading MIDI file: {data.fileName} - {data.description}");
            }
        }
    }
    
    private void LoadMidiFile(MidiFileData data)
    {
        if (midiPlayer != null)
        {
            // Попытка вызвать метод LoadMidiFile через рефлексию
            var loadMethod = midiPlayer.GetType().GetMethod("LoadMidiFile");
            if (loadMethod != null)
            {
                loadMethod.Invoke(midiPlayer, new object[] { data.midiFile });
            }
            else
            {
                // Альтернативные имена методов
                var alternativeMethods = new[] { "LoadFile", "SetMidiFile", "PlayMidiFile" };
                foreach (var methodName in alternativeMethods)
                {
                    var method = midiPlayer.GetType().GetMethod(methodName);
                    if (method != null)
                    {
                        method.Invoke(midiPlayer, new object[] { data.midiFile });
                        Debug.Log($"Called {methodName} on MIDI player");
                        break;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("MIDI Player not assigned! Assign your MIDI player component.");
        }
    }
    
    public void StopCurrentMidi()
    {
        if (midiPlayer != null)
        {
            // Попытка вызвать метод Stop через рефлексию
            var stopMethod = midiPlayer.GetType().GetMethod("Stop");
            if (stopMethod != null)
            {
                stopMethod.Invoke(midiPlayer, null);
            }
        }
        
        if (currentSelectedButton != null)
        {
            currentSelectedButton.SetSelected(false);
            currentSelectedButton = null;
        }
    }
    
    // Методы для совместимости со старой системой
    public void AddMidiFile(MidiFileData fileData)
    {
        // Implementation for adding files dynamically
    }
    
    public void RemoveMidiFile(int index)
    {
        // Implementation for removing files
    }
    
    public void ClearMidiFiles()
    {
        midiFiles = new MidiFileData[0];
        foreach (var button in createdButtons)
        {
            if (button != null)
                Destroy(button.gameObject);
        }
        createdButtons.Clear();
    }
    
    public MidiFileData GetCurrentMidiFile()
    {
        if (currentSelectedButton != null)
        {
            return currentSelectedButton.GetMidiData();
        }
        return null;
    }
    
    // Метод для ручного обновления кнопок
    [ContextMenu("Refresh MIDI Buttons")]
    public void RefreshButtons()
    {
        InitializeMidiFiles();
        CreateButtons();
    }
    
    // Метод для проверки готовности к билду
    [ContextMenu("Validate Build Readiness")]
    public void ValidateBuildReadiness()
    {
        Debug.Log("=== MIDI Build Validation ===");
        
        if (loadFromResources)
        {
            TextAsset[] midiAssets = Resources.LoadAll<TextAsset>("Midi");
            Debug.Log($"Resources folder 'Midi' contains {midiAssets.Length} MIDI files");
            
            if (midiAssets.Length == 0)
            {
                Debug.LogWarning($"No MIDI files found in Resources/Midi. Make sure your MIDI files are in the Resources folder!");
            }
        }
        else
        {
            int validFiles = 0;
            foreach (var file in midiFiles)
            {
                if (file.midiFile != null)
                    validFiles++;
            }
            Debug.Log($"Inspector-assigned MIDI files: {validFiles}/{midiFiles.Length} valid");
            
            if (validFiles == 0)
            {
                Debug.LogWarning("No MIDI files assigned in inspector! Assign TextAsset references manually.");
            }
        }
        
        if (buttonPrefab == null)
            Debug.LogError("Button prefab not assigned!");
            
        if (buttonContainer == null)
            Debug.LogError("Button container not assigned!");
            
        if (midiPlayer == null)
            Debug.LogWarning("MIDI Player not assigned! You'll need to assign your MIDI player component.");
            
        Debug.Log("=== Validation Complete ===");
    }
} 