using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
public class GuitarKnob : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Knob Type")]
    public KnobType knobType;

    [Header("Rotation Settings")]
    public float minRotation = -135f;
    public float maxRotation = 135f;
    public float scrollSensitivity = 0.1f;

    [Header("References")]
    public GuitarKnobsController controller;

    private bool isMouseOver = false;
    private Transform knobTransform;

    public enum KnobType
    {
        Volume,
        Tone,
        Bass
    }

    private void Start()
    {
        knobTransform = transform;

        // Проверяем наличие коллайдера
        var collider = GetComponent<Collider>();
        if (collider != null && !collider.isTrigger)
        {
            collider.isTrigger = true;
        }

        // Устанавливаем начальный поворот
        UpdateRotation(GetCurrentValue());
    }

    // Реализация интерфейсов IPointerEnterHandler и IPointerExitHandler
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        Debug.Log($"Mouse over {gameObject.name} ({knobType})");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        Debug.Log($"Mouse exit {gameObject.name} ({knobType})");
    }

    private void Update()
    {
        if (isMouseOver && controller != null)
        {
            float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
            if (scrollDelta != 0)
            {
                // Получаем текущее значение
                float currentValue = GetCurrentValue();
                
                // Изменяем значение
                float newValue = Mathf.Clamp01(currentValue + scrollDelta * scrollSensitivity);
                
                // Обновляем значение в контроллере
                SetValue(newValue);
                
                // Обновляем поворот крутилки
                UpdateRotation(newValue);

                Debug.Log($"Knob {knobType} value changed to: {newValue:F2}");
            }
        }
    }

    private float GetCurrentValue()
    {
        if (controller == null) return 0f;

        switch (knobType)
        {
            case KnobType.Volume:
                return controller.volumeKnob;
            case KnobType.Tone:
                return controller.toneKnob;
            case KnobType.Bass:
                return controller.bassKnob;
            default:
                return 0f;
        }
    }

    private void SetValue(float value)
    {
        if (controller == null) return;

        switch (knobType)
        {
            case KnobType.Volume:
                controller.SetVolume(value);
                break;
            case KnobType.Tone:
                controller.SetTone(value);
                break;
            case KnobType.Bass:
                controller.SetBass(value);
                break;
        }
    }

    private void UpdateRotation(float value)
    {
        float rotation = Mathf.Lerp(minRotation, maxRotation, value);
        knobTransform.localRotation = Quaternion.Euler(0, 0, rotation);
    }
} 