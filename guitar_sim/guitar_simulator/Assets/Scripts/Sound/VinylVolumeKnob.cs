using UnityEngine;
using UnityEngine.EventSystems;

public class VinylVolumeKnob : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    public BGMusicPlayer musicPlayer;

    [Header("Volume Knob Settings")]
    public float minRotation = -135f;
    public float maxRotation = 135f;
    public float scrollSensitivity = 0.1f;
    public float dragSensitivity = 2f;

    private bool isMouseOver = false;
    private bool isDragging = false;
    private Vector3 lastMousePosition;

    private void Update()
    {
        // Обработка колесика мыши для крутилки громкости
        if (isMouseOver && !isDragging && musicPlayer != null)
        {
            float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            if (scrollDelta != 0)
            {
                float newValue = Mathf.Clamp01(musicPlayer.musicVolume + scrollDelta * scrollSensitivity);
                musicPlayer.SetMusicVolume(newValue);
                UpdateVolumeKnobRotation(newValue);
                Debug.Log($"[VinylVolumeKnob] Volume changed to: {newValue:F2} (scroll: {scrollDelta:F2})");
            }
        }
    }

    // Начало перетаскивания
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        lastMousePosition = (Vector3)eventData.position;
        Debug.Log("[VinylVolumeKnob] Started dragging");
    }

    // Перетаскивание
    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && musicPlayer != null)
        {
            Vector3 mouseDelta = (Vector3)eventData.position - lastMousePosition;
            float rotationDelta = mouseDelta.x * dragSensitivity;
            
            // Преобразуем изменение поворота в изменение громкости
            float rotationRange = maxRotation - minRotation;
            float volumeDelta = rotationDelta / rotationRange;
            float newValue = Mathf.Clamp01(musicPlayer.musicVolume + volumeDelta);
            
            musicPlayer.SetMusicVolume(newValue);
            UpdateVolumeKnobRotation(newValue);
            
            lastMousePosition = (Vector3)eventData.position;
            Debug.Log($"[VinylVolumeKnob] Volume changed to: {newValue:F2} (drag: {volumeDelta:F2})");
        }
    }

    // Конец перетаскивания
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

    // Обновление поворота крутилки громкости
    private void UpdateVolumeKnobRotation(float value)
    {
        
        // Поворот от -90 до +90 градусов по оси Z
        float rotation = Mathf.Lerp(-90f, 90f, value);
        Vector3 currentRotation = transform.localEulerAngles;
        transform.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y, rotation);
        Debug.Log($"[VinylVolumeKnob] Setting rotation to Z={rotation:F1} for volume={value:F2}");
        
    }

    // Интерфейсы для определения наведения мыши
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        Debug.Log("[VinylVolumeKnob] Mouse entered");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        isDragging = false;
        Debug.Log("[VinylVolumeKnob] Mouse exited");
    }
} 