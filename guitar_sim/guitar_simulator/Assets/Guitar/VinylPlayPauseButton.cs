using UnityEngine;
using UnityEngine.EventSystems;

public class VinylPlayPauseButton : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    public BGMusicPlayer musicPlayer;

    [Header("Button Settings")]
    public float buttonPressDepth = 0.01f; // Насколько глубоко нажимается кнопка
    public float buttonPressSpeed = 10f; // Скорость нажатия/отпускания

    private Vector3 buttonOriginalPosition;
    private Vector3 buttonPressedPosition;
    private bool isButtonPressed = false;

    private void Start()
    {
        buttonOriginalPosition = transform.localPosition;
        buttonPressedPosition = buttonOriginalPosition + Vector3.down * buttonPressDepth;
    }

    private void Update()
    {
        // Анимация нажатия кнопки
        Vector3 targetPosition = isButtonPressed ? buttonPressedPosition : buttonOriginalPosition;
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * buttonPressSpeed);
    }

    // Обработка клика
    public void OnPointerClick(PointerEventData eventData)
    {
        if (musicPlayer == null) return;

        TogglePlayPause();
    }

    // Переключение play/pause
    private void TogglePlayPause()
    {
        if (musicPlayer.IsPlaying())
        {
            musicPlayer.PauseMusic();
            isButtonPressed = false;
        }
        else
        {
            musicPlayer.ResumeMusic();
            isButtonPressed = true;
        }
    }
} 