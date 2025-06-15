using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MidiFileButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Components")]
    [SerializeField] private Button button;
    [SerializeField] public TextMeshProUGUI fileNameText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Image backgroundImage;
    
    [Header("Visual States")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color disabledColor = Color.gray;
    
    [Header("References")]
    [SerializeField] private MidiFileManager fileManager;
    [SerializeField] private MidiFileUIController uiController;
    
    private MidiFileInfo midiFileInfo;
    private bool isSelected = false;
    private bool isHovered = false;
    
    private void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
            
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
    }
    
    public void SetupButton(MidiFileInfo fileInfo, MidiFileManager manager, MidiFileUIController controller)
    {
        midiFileInfo = fileInfo;
        fileManager = manager;
        uiController = controller;
        
        // Автоматически находим TextMeshProUGUI, если не назначен
        AutoSetupTextComponent();
        
        UpdateButtonText();
        UpdateVisualState();
        
        // Настраиваем кнопку
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClick);
        }
    }
    
    /// <summary>
    /// Автоматически находит и настраивает текстовый компонент
    /// </summary>
    private void AutoSetupTextComponent()
    {
        // Если fileNameText не назначен, пытаемся найти его в дочерних объектах
        if (fileNameText == null)
        {
            fileNameText = GetComponentInChildren<TextMeshProUGUI>();
            
            if (fileNameText != null)
            {
                Debug.Log($"Auto-assigned TextMeshProUGUI component for button: {midiFileInfo?.fileName}");
            }
            else
            {
                Debug.LogWarning($"No TextMeshProUGUI found in button prefab for: {midiFileInfo?.fileName}");
            }
        }
    }
    
    private void UpdateButtonText()
    {
        if (midiFileInfo == null) return;
        
        // Обновляем текст имени файла
        if (fileNameText != null)
        {
            fileNameText.text = midiFileInfo.fileName;
        }
        
        // Обновляем информационный текст
        if (infoText != null)
        {
            string info = $"Duration: {midiFileInfo.duration:F1}s\nBPM: {midiFileInfo.bpm:F0}";
            infoText.text = info;
        }
    }
    
    private void UpdateVisualState()
    {
        if (backgroundImage == null) return;
        
        Color targetColor = normalColor;
        
        if (isSelected)
        {
            targetColor = selectedColor;
        }
        else if (isHovered)
        {
            targetColor = hoverColor;
        }
        
        backgroundImage.color = targetColor;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        OnButtonClick();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        UpdateVisualState();
        
        // Уведомляем менеджер о наведении
        if (fileManager != null)
        {
            fileManager.HoverFile(midiFileInfo);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        UpdateVisualState();
        
        // Убираем наведение
        if (fileManager != null)
        {
            fileManager.ClearHover();
        }
    }
    
    private void OnButtonClick()
    {
        if (midiFileInfo == null) return;
        
        // Выбираем файл
        if (fileManager != null)
        {
            fileManager.SelectFile(midiFileInfo);
        }
        
        // Уведомляем UI контроллер
        if (uiController != null)
        {
            uiController.UpdateSelectedButton(this);
        }
        
        SetSelected(true);
        
        Debug.Log($"Selected MIDI file: {midiFileInfo.fileName} (BPM: {midiFileInfo.bpm:F0})");
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisualState();
    }
    
    public void SetInteractable(bool interactable)
    {
        if (button != null)
        {
            button.interactable = interactable;
        }
        
        // Обновляем цвет для неактивного состояния
        if (!interactable && backgroundImage != null)
        {
            backgroundImage.color = disabledColor;
        }
        else
        {
            UpdateVisualState();
        }
    }
    
    public MidiFileInfo GetMidiFileInfo()
    {
        return midiFileInfo;
    }
    
    public bool IsSelected()
    {
        return isSelected;
    }
    
    // Методы для внешнего доступа
    public string GetFileName()
    {
        return midiFileInfo?.fileName ?? "";
    }
    
    public float GetDuration()
    {
        return midiFileInfo?.duration ?? 0f;
    }
    
    public float GetBpm()
    {
        return midiFileInfo?.bpm ?? 120f;
    }
    
    public bool IsLoaded()
    {
        return midiFileInfo?.isLoaded ?? false;
    }
    
    [ContextMenu("Refresh Button")]
    public void RefreshButton()
    {
        UpdateButtonText();
        UpdateVisualState();
    }
} 