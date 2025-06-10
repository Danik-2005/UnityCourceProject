using UnityEngine;

public class TremoloController : MonoBehaviour
{
    [Header("References")]
    public GuitarKnobsController knobsController;
    public Transform tremoloArm;      // Рычаг тремоло
    public Collider tremoloArmCollider; // Коллайдер рычага тремоло

    [Header("Tremolo Settings")]
    [Range(0.1f, 5f)]
    public float maxRotationAngle = 5f;   // Максимальный угол наклона рычага
    [Range(1f, 20f)]
    public float tremoloSpeed = 8f;        // Скорость движения тремоло
    [Range(5f, 20f)]
    public float returnSpeed = 10f;        // Скорость возврата в исходное положение
    [Range(0.1f, 1f)]
    public float pitchRange = 0.2f;        // Диапазон изменения питча (±0.2 от нормального)
    
    [Header("Mouse Control")]
    [Range(0.1f, 5f)]
    public float mouseSensitivity = 1f;    // Чувствительность мыши
    public bool invertMouseY = false;      // Инвертировать ось Y мыши

    private float initialRotationX;
    private float currentRotation = 0f;
    private float targetRotation = 0f;
    private bool isTremoloActive = false;
    private bool isMouseControlActive = false;
    private float tremoloTime = 0f;
    private Vector3 lastMousePosition;
    private bool isDragging = false;

    private void Start()
    {
        if (tremoloArm == null)
        {
            Debug.LogError("Tremolo arm reference not set!");
            return;
        }

        if (knobsController == null)
        {
            knobsController = FindObjectOfType<GuitarKnobsController>();
            if (knobsController == null)
            {
                Debug.LogError("GuitarKnobsController not found!");
                return;
            }
        }

        // Если коллайдер не назначен, пытаемся найти его на рычаге
        if (tremoloArmCollider == null && tremoloArm != null)
        {
            tremoloArmCollider = tremoloArm.GetComponent<Collider>();
            if (tremoloArmCollider == null)
            {
                Debug.LogWarning("No collider found on tremolo arm! Adding BoxCollider...");
                tremoloArmCollider = tremoloArm.gameObject.AddComponent<BoxCollider>();
            }
        }

        // Сохраняем начальный угол поворота
        initialRotationX = tremoloArm.localRotation.eulerAngles.x;
    }

    private void Update()
    {
        // Обработка нажатий мыши
        HandleMouseInput();

        if (isMouseControlActive)
        {
            HandleMouseControl();
        }
        else if (isTremoloActive)
        {
            HandleTremoloEffect();
        }
        else
        {
            HandleReturnToRest();
        }

        // Применяем поворот и обновляем питч
        ApplyRotationAndPitch();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Визуализируем луч для отладки
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1f);

            if (tremoloArmCollider != null && Physics.Raycast(ray, out hit))
            {
                if (hit.collider == tremoloArmCollider)
                {
                    isDragging = true;
                    StartMouseControl();
                }
            }
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            StopMouseControl();
        }
    }

    private void HandleMouseControl()
    {
        if (!isDragging) return;

        Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
        float mouseY = invertMouseY ? -mouseDelta.y : mouseDelta.y;
        
        // Преобразуем движение мыши в изменение угла
        targetRotation = Mathf.Clamp(
            targetRotation - mouseY * mouseSensitivity * Time.deltaTime,
            -maxRotationAngle,
            maxRotationAngle
        );

        // Плавно переходим к целевому углу
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, tremoloSpeed * Time.deltaTime);
        
        lastMousePosition = Input.mousePosition;
    }

    private void HandleTremoloEffect()
    {
        // Обновляем время для синусоидального движения
        tremoloTime += Time.deltaTime * tremoloSpeed;
        
        // Создаем колебательное движение
        currentRotation = Mathf.Sin(tremoloTime) * maxRotationAngle;
    }

    private void HandleReturnToRest()
    {
        if (!Mathf.Approximately(currentRotation, 0f))
        {
            // Плавно возвращаем рычаг в исходное положение
            currentRotation = Mathf.MoveTowards(currentRotation, 0f, returnSpeed * Time.deltaTime);
            targetRotation = 0f;
        }
    }

    private void ApplyRotationAndPitch()
    {
        // Применяем поворот к рычагу тремоло
        if (tremoloArm != null)
        {
            Vector3 currentEuler = tremoloArm.localRotation.eulerAngles;
            tremoloArm.localRotation = Quaternion.Euler(
                initialRotationX + currentRotation,
                currentEuler.y,
                currentEuler.z
            );
        }

        // Вычисляем и применяем питч
        float normalizedRotation = currentRotation / maxRotationAngle; // -1 to 1
        float pitchValue = 1f + (normalizedRotation * pitchRange);
        knobsController.SetPitch(pitchValue);

        // Если вернулись в исходное положение, сбрасываем питч точно на 1
        if (Mathf.Approximately(currentRotation, 0f) && !isTremoloActive && !isMouseControlActive)
        {
            knobsController.SetPitch(1f);
        }
    }

    // Публичные методы для управления тремоло
    public void StartTremolo()
    {
        isTremoloActive = true;
        isMouseControlActive = false;
        tremoloTime = 0f;
    }

    public void StopTremolo()
    {
        isTremoloActive = false;
    }

    public void StartMouseControl()
    {
        isMouseControlActive = true;
        isTremoloActive = false;
        lastMousePosition = Input.mousePosition;
        targetRotation = currentRotation;
    }

    public void StopMouseControl()
    {
        isMouseControlActive = false;
    }
} 