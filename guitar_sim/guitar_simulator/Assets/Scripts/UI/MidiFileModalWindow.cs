using UnityEngine;
using UnityEngine.UI;

public class MidiFileModalWindow : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject modalPanel;
    [SerializeField] private Button openButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button outsideCloseButton; // Кнопка для закрытия по клику вне окна
    
    [Header("References")]
    [SerializeField] private MidiFileUIController uiController;
    [SerializeField] private MidiFileManager fileManager;
    [SerializeField] private MidiPlayer midiPlayer;
    
    private bool isOpen = false;
    
    private void Start()
    {
        SetupUI();
        
        // Изначально окно закрыто
        if (modalPanel != null)
        {
            modalPanel.SetActive(false);
        }
    }
    
    private void SetupUI()
    {
        // Настраиваем кнопку открытия
        if (openButton != null)
        {
            openButton.onClick.AddListener(OpenModal);
        }
        
        // Настраиваем кнопку закрытия
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseModal);
        }
        
        // Настраиваем кнопку закрытия по клику вне окна
        if (outsideCloseButton != null)
        {
            outsideCloseButton.onClick.AddListener(CloseModal);
        }
        
        // Настраиваем ссылки в UI контроллере
        if (uiController != null)
        {
            if (fileManager != null)
            {
                uiController.SetFileManager(fileManager);
            }
            
            if (midiPlayer != null)
            {
                uiController.SetMidiPlayer(midiPlayer);
            }
        }
    }
    
    public void OpenModal()
    {
        if (modalPanel != null)
        {
            modalPanel.SetActive(true);
            isOpen = true;
            
            // Обновляем UI при открытии
            if (uiController != null)
            {
                uiController.RefreshOnOpen();
            }
            
            Debug.Log("MIDI file selection modal opened");
        }
    }
    
    public void CloseModal()
    {
        if (modalPanel != null)
        {
            modalPanel.SetActive(false);
            isOpen = false;
            
            Debug.Log("MIDI file selection modal closed");
        }
    }
    
    public void ToggleModal()
    {
        if (isOpen)
        {
            CloseModal();
        }
        else
        {
            OpenModal();
        }
    }
    
    public bool IsOpen()
    {
        return isOpen;
    }
    
    // Методы для внешнего доступа
    public void SetUIController(MidiFileUIController controller)
    {
        uiController = controller;
        SetupUI();
    }
    
    public void SetFileManager(MidiFileManager manager)
    {
        fileManager = manager;
        SetupUI();
    }
    
    public void SetMidiPlayer(MidiPlayer player)
    {
        midiPlayer = player;
        SetupUI();
    }
    
    // Обработка нажатия Escape
    private void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseModal();
        }
    }
} 