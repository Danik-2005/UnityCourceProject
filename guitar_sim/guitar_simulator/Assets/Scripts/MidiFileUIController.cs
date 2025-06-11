using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Linq;

public class MidiFileUIController : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Button confirmPlayButton;
    [SerializeField] private Button stopPlayButton;
    
    [Header("BPM Controls")]
    [SerializeField] private Slider bpmSlider;
    [SerializeField] private TextMeshProUGUI bpmValueText;
    
    [Header("References")]
    [SerializeField] private MidiFileManager fileManager;
    [SerializeField] private MidiPlayer midiPlayer;
    
    private List<MidiFileButton> createdButtons = new List<MidiFileButton>();
    private MidiFileButton selectedButton;
    
    private void Start()
    {
        SetupUI();
        
        // Принудительно активируем обе кнопки
        if (confirmPlayButton != null)
        {
            confirmPlayButton.interactable = true;
        }
        
        if (stopPlayButton != null)
        {
            stopPlayButton.interactable = true;
        }
        
        // Не создаем кнопки сразу - ждем загрузки файлов
        StartCoroutine(WaitForFilesAndCreateButtons());
    }
    
    private System.Collections.IEnumerator WaitForFilesAndCreateButtons()
    {
        // Ждем один кадр, чтобы MidiFileManager успел загрузить файлы
        yield return null;
        
        // Проверяем, есть ли файлы
        if (fileManager != null && fileManager.MidiFiles.Count > 0)
        {
            CreateButtons();
        }
        else
        {
            Debug.LogWarning("No MIDI files loaded yet. Buttons will be created when files are available.");
        }
    }
    
    private void SetupUI()
    {
        // Настраиваем кнопку подтверждения
        if (confirmPlayButton != null)
        {
            confirmPlayButton.onClick.AddListener(OnConfirmPlayback);
        }
        else
        {
            Debug.LogError("Confirm Play Button is not assigned in the inspector!");
        }
        
        // Настраиваем кнопку остановки
        if (stopPlayButton != null)
        {
            stopPlayButton.onClick.AddListener(OnStopPlayback);
        }
        else
        {
            Debug.LogWarning("Stop Play Button is not assigned in the inspector!");
        }
        
        // Настраиваем интеграцию с MidiPlayer для BPM слайдера
        if (midiPlayer != null)
        {
            midiPlayer.SetBpmSlider(bpmSlider);
            midiPlayer.SetBpmValueText(bpmValueText);
        }
        else
        {
            Debug.LogError("MidiPlayer is not assigned!");
        }
        
        // Назначаем текстовое поле в менеджер
        if (fileManager != null && infoText != null)
        {
            // Используем рефлексию для установки infoText
            var infoTextField = fileManager.GetType().GetField("infoText", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (infoTextField != null)
            {
                infoTextField.SetValue(fileManager, infoText);
            }
        }
    }
    
    private void CreateButtons()
    {
        if (fileManager == null)
        {
            Debug.LogError("FileManager is not assigned!");
            return;
        }
        
        if (buttonPrefab == null)
        {
            Debug.LogError("ButtonPrefab is not assigned!");
            return;
        }
        
        if (buttonContainer == null)
        {
            Debug.LogError("ButtonContainer is not assigned!");
            return;
        }
        
        Debug.Log($"Creating buttons for {fileManager.MidiFiles.Count} MIDI files");
        
        // Очищаем существующие кнопки
        ClearButtons();
        
        // Создаем кнопки для каждого MIDI файла
        foreach (var midiFile in fileManager.MidiFiles)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
            MidiFileButton button = buttonObj.GetComponent<MidiFileButton>();
            
            if (button != null)
            {
                // Автоматически находим и настраиваем текст кнопки, если он не назначен
                SetupButtonText(button, midiFile);
                
                button.SetupButton(midiFile, fileManager, this);
                createdButtons.Add(button);
                Debug.Log($"Created button for: {midiFile.fileName}");
            }
            else
            {
                Debug.LogError($"MidiFileButton component not found on prefab!");
            }
        }
        
        Debug.Log($"Successfully created {createdButtons.Count} MIDI file buttons");
    }
    
    /// <summary>
    /// Автоматически находит и настраивает текст кнопки
    /// </summary>
    private void SetupButtonText(MidiFileButton button, MidiFileInfo midiFile)
    {
        // Если fileNameText не назначен, пытаемся найти его в дочерних объектах
        if (button.fileNameText == null)
        {
            button.fileNameText = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            
            if (button.fileNameText != null)
            {
                Debug.Log($"Auto-assigned TextMeshProUGUI for button: {midiFile.fileName}");
            }
            else
            {
                Debug.LogWarning($"No TextMeshProUGUI found in button prefab for: {midiFile.fileName}");
            }
        }
        
        // Если нашли текстовый компонент, устанавливаем текст
        if (button.fileNameText != null)
        {
            button.fileNameText.text = midiFile.fileName;
            Debug.Log($"Set button text to: {midiFile.fileName}");
        }
    }
    
    private void ClearButtons()
    {
        foreach (var button in createdButtons)
        {
            if (button != null)
                Destroy(button.gameObject);
        }
        createdButtons.Clear();
    }
    
    private void OnConfirmPlayback()
    {
        if (fileManager != null && fileManager.SelectedFile != null && midiPlayer != null)
        {
            // Получаем выбранный файл
            var selectedFile = fileManager.SelectedFile;
            
            // Получаем значение BPM из MidiPlayer
            float targetBpm = midiPlayer.GetCurrentBpm();
            
            // Вызываем воспроизведение через MidiPlayer
            midiPlayer.PlayMidiFile(selectedFile, targetBpm);
            
            Debug.Log($"Starting playback of {selectedFile.fileName} at {targetBpm:F0} BPM");
            
            // Обновляем состояние кнопки остановки
            UpdateButtonStates();
        }
        else
        {
            Debug.LogWarning("No file selected or MidiPlayer not assigned");
        }
    }
    
    private void OnStopPlayback()
    {
        if (midiPlayer != null)
        {
            midiPlayer.StopPlayback();
            Debug.Log("MIDI playback stopped");
            
            // Обновляем состояние кнопок
            UpdateButtonStates();
        }
        else
        {
            Debug.LogWarning("MidiPlayer not assigned");
        }
    }
    
    public void UpdateSelectedButton(MidiFileButton newSelectedButton)
    {
        // Снимаем выделение с предыдущей кнопки
        if (selectedButton != null && selectedButton != newSelectedButton)
        {
            selectedButton.SetSelected(false);
        }
        
        selectedButton = newSelectedButton;
        
        // Обновляем состояние кнопок
        UpdateButtonStates();
    }
    
    private void UpdateButtonStates()
    {
        bool isPlaying = midiPlayer != null && midiPlayer.IsPlaying();
        
        // Обновляем состояние кнопки воспроизведения
        if (confirmPlayButton != null)
        {
            confirmPlayButton.interactable = !isPlaying && fileManager != null && fileManager.SelectedFile != null;
        }
        
        // Обновляем состояние кнопки остановки
        if (stopPlayButton != null)
        {
            stopPlayButton.interactable = isPlaying;
        }
    }
    
    public void RefreshOnOpen()
    {
        Debug.Log("Refreshing UI on window open");
        
        // Обновляем состояние кнопок
        UpdateButtonStates();
        
        // Обновляем отображение BPM
        if (midiPlayer != null && bpmValueText != null)
        {
            bpmValueText.text = $"BPM: {midiPlayer.GetCurrentBpm():F0}";
        }
    }
    
    [ContextMenu("Force Refresh Buttons")]
    public void ForceRefreshButtons()
    {
        Debug.Log("Force refreshing buttons");
        CreateButtons();
    }
    
    public void SetFileManager(MidiFileManager manager)
    {
        fileManager = manager;
        Debug.Log("FileManager assigned to UIController");
    }
    
    public void SetMidiPlayer(MidiPlayer player)
    {
        midiPlayer = player;
        Debug.Log("MidiPlayer assigned to UIController");
        
        // Настраиваем интеграцию с новым MidiPlayer
        if (midiPlayer != null)
        {
            midiPlayer.SetBpmSlider(bpmSlider);
            midiPlayer.SetBpmValueText(bpmValueText);
        }
    }
    
    public MidiFileManager GetFileManager()
    {
        return fileManager;
    }
    
    public int GetButtonCount()
    {
        return createdButtons.Count;
    }
    
    [ContextMenu("Force Enable Confirm Button")]
    public void ForceEnableConfirmButton()
    {
        if (confirmPlayButton != null)
        {
            confirmPlayButton.interactable = true;
            Debug.Log("Confirm button force enabled");
        }
    }
    
    [ContextMenu("Force Enable Stop Button")]
    public void ForceEnableStopButton()
    {
        if (stopPlayButton != null)
        {
            stopPlayButton.interactable = true;
            Debug.Log("Stop button force enabled");
        }
    }
    
    [ContextMenu("Refresh UI")]
    public void RefreshUIFromContext()
    {
        Debug.Log("Refreshing UI from context menu");
        RefreshOnOpen();
    }
} 