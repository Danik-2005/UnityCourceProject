using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

/// <summary>
/// Кнопка для выбора аудио файла
/// </summary>
public class FileSelectionButton : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button button;
    [SerializeField] private Text fileNameText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Image backgroundImage;
    
    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color hoverColor = Color.yellow;
    
    private MidiFileData midiData;
    private FileSelectionManager manager;
    private bool isSelected = false;
    
    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
        
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }
    
    public void SetupMidiButton(MidiFileData data, FileSelectionManager managerRef)
    {
        midiData = data;
        manager = managerRef;
        
        // Обновляем UI
        if (fileNameText != null)
            fileNameText.text = data.fileName;
        
        if (descriptionText != null)
            descriptionText.text = data.description;
        
        // Устанавливаем нормальный цвет
        SetSelected(false);
        
        // Делаем кнопку активной только если файл включен
        if (button != null)
        {
            button.interactable = data.isEnabled;
        }
        
        Debug.Log($"MIDI Button setup: {data.fileName} (Enabled: {data.isEnabled})");
    }
    
    private void OnButtonClick()
    {
        if (manager != null && midiData != null && midiData.isEnabled)
        {
            manager.OnMidiButtonSelected(this);
            SetSelected(true);
        }
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        if (backgroundImage != null)
        {
            if (selected)
            {
                backgroundImage.color = selectedColor;
            }
            else if (midiData != null && !midiData.isEnabled)
            {
                backgroundImage.color = Color.gray;
            }
            else
            {
                backgroundImage.color = normalColor;
            }
        }
    }
    
    public MidiFileData GetMidiData()
    {
        return midiData;
    }
    
    public bool IsSelected()
    {
        return isSelected;
    }
    
    public bool IsEnabled()
    {
        return midiData != null && midiData.isEnabled;
    }
    
    // Методы для совместимости со старой системой (Audio)
    private AudioFileData audioData;
    
    public void SetupButton(AudioFileData data, FileSelectionManager managerRef)
    {
        audioData = data;
        manager = managerRef;
        
        // Обновляем UI
        if (fileNameText != null)
            fileNameText.text = data.fileName;
        
        if (descriptionText != null)
            descriptionText.text = data.description;
        
        // Устанавливаем нормальный цвет
        SetSelected(false);
        
        Debug.Log($"Audio Button setup: {data.fileName}");
    }
    
    public AudioFileData GetAudioData()
    {
        return audioData;
    }
    
    public void SetAudioFile(AudioClip clip, string name)
    {
        if (audioData == null)
            audioData = new AudioFileData();
        
        audioData.audioFile = clip;
        audioData.fileName = name;
        
        if (fileNameText != null)
            fileNameText.text = name;
    }
    
    public void SetDescription(string description)
    {
        if (audioData == null)
            audioData = new AudioFileData();
        
        audioData.description = description;
        
        if (descriptionText != null)
            descriptionText.text = description;
    }
    
    public void SetMixerGroup(AudioMixerGroup mixerGroup)
    {
        if (audioData == null)
            audioData = new AudioFileData();
        
        audioData.mixerGroup = mixerGroup;
    }
} 