using UnityEngine;

public class PickupSwitchController : MonoBehaviour
{
    [Header("References")]
    public Transform switchHandle;    // Сам переключатель

    [Header("Switch Settings")]
    public float neckPosition = 30f;      // Угол для позиции Neck
    public float middlePosition = 0f;     // Угол для позиции Middle
    public float bridgePosition = -30f;   // Угол для позиции Bridge
    public float snapThreshold = 5f;      // Порог для "прилипания" к позиции
    public float rotationSpeed = 10f;     // Скорость поворота переключателя
    public float mouseSensitivity = 0.5f; // Чувствительность мыши

    [Header("Debug")]
    public bool showDebugInfo = true;     // Показывать отладочную информацию

    private bool isDragging = false;
    private Vector3 lastMousePosition;
    private float targetRotation;
    private float currentRotation;
    private PickupType currentPickup = PickupType.Neck;
    private Camera mainCamera;
    private Collider switchCollider;

    private void OnEnable()
    {
        SetupCollider();
    }

    private void SetupCollider()
    {
        if (switchHandle != null)
        {
            switchCollider = switchHandle.GetComponent<Collider>();
            if (switchCollider == null)
            {
                switchCollider = switchHandle.gameObject.AddComponent<BoxCollider>();
                if (showDebugInfo) Debug.Log("Added BoxCollider to switch handle");
            }
        }
        else
        {
            Debug.LogError("Switch handle is not assigned!");
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            return;
        }

        if (switchHandle == null)
        {
            Debug.LogError("Switch handle is not assigned!");
            return;
        }

        // Запоминаем текущий поворот как начальный
        currentRotation = switchHandle.localEulerAngles.y;
        targetRotation = currentRotation;

        // Устанавливаем начальную позицию
        SetPickupPosition(PickupType.Neck);
        
        if (showDebugInfo) Debug.Log($"Initialization complete. Current rotation: {currentRotation}");
    }

    private void Update()
    {
        HandleMouseInput();
        UpdateSwitchRotation();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (showDebugInfo) Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1f);

            if (Physics.Raycast(ray, out hit))
            {
                if (showDebugInfo) Debug.Log($"Hit object: {hit.transform.name}");
                if (hit.transform == switchHandle)
                {
                    isDragging = true;
                    lastMousePosition = Input.mousePosition;
                    if (showDebugInfo) Debug.Log("Started dragging pickup switch");
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                isDragging = false;
                SnapToNearestPosition();
                if (showDebugInfo) Debug.Log("Stopped dragging pickup switch");
            }
        }

        if (isDragging)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            
            // Изменяем целевой угол поворота на основе движения мыши
            targetRotation = Mathf.Clamp(
                targetRotation - mouseDelta.x * mouseSensitivity,
                bridgePosition,
                neckPosition
            );

            if (showDebugInfo) Debug.Log($"Rotation: current={currentRotation:F2}, target={targetRotation:F2}, mouseDelta={mouseDelta.x:F2}");

            lastMousePosition = Input.mousePosition;
        }
    }

    private void UpdateSwitchRotation()
    {
        if (switchHandle == null) return;

        // Плавно обновляем текущий угол поворота
        currentRotation = Mathf.Lerp(currentRotation, targetRotation, Time.deltaTime * rotationSpeed);

        // Применяем поворот напрямую к локальной оси Y
        Vector3 currentEuler = switchHandle.localEulerAngles;
        switchHandle.localEulerAngles = new Vector3(currentEuler.x, currentRotation, currentEuler.z);

        if (showDebugInfo && Mathf.Abs(currentRotation - targetRotation) > 0.01f)
        {
            Debug.Log($"Updating rotation: current={currentRotation:F2}, target={targetRotation:F2}, " +
                     $"euler={switchHandle.localEulerAngles}");
        }
    }

    private void SnapToNearestPosition()
    {
        // Находим ближайшую позицию
        float[] positions = { neckPosition, middlePosition, bridgePosition };
        float closestDistance = float.MaxValue;
        float closestPosition = middlePosition;

        foreach (float position in positions)
        {
            float distance = Mathf.Abs(currentRotation - position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPosition = position;
            }
        }

        // Устанавливаем целевую позицию
        targetRotation = closestPosition;
        if (showDebugInfo) Debug.Log($"Snapping to position: {closestPosition} (distance: {closestDistance})");

        // Определяем и устанавливаем тип звукоснимателя
        if (Mathf.Abs(closestPosition - neckPosition) < snapThreshold)
        {
            SetPickupType(PickupType.Neck);
        }
        else if (Mathf.Abs(closestPosition - middlePosition) < snapThreshold)
        {
            SetPickupType(PickupType.Middle);
        }
        else if (Mathf.Abs(closestPosition - bridgePosition) < snapThreshold)
        {
            SetPickupType(PickupType.Bridge);
        }
    }

    private void SetPickupType(PickupType pickup)
    {
        if (currentPickup != pickup)
        {
            currentPickup = pickup;
            if (GuitarSoundSystem.Instance != null)
            {
                GuitarSoundSystem.Instance.currentPickup = pickup;
                if (showDebugInfo) Debug.Log($"Switched to {pickup} pickup");
            }
        }
    }

    public void SetPickupPosition(PickupType pickup)
    {
        targetRotation = pickup switch
        {
            PickupType.Neck => neckPosition,
            PickupType.Middle => middlePosition,
            PickupType.Bridge => bridgePosition,
            _ => middlePosition
        };
        SetPickupType(pickup);
    }

    private void OnDrawGizmos()
    {
        if (!showDebugInfo) return;

        if (switchHandle != null)
        {
            // Рисуем ось вращения
            Gizmos.color = Color.green;
            Gizmos.DrawRay(switchHandle.position, switchHandle.up * 0.1f);

            // Рисуем границы коллайдера
            if (switchCollider != null)
            {
                Gizmos.color = isDragging ? Color.green : Color.cyan;
                if (switchCollider is BoxCollider boxCollider)
                {
                    Matrix4x4 oldMatrix = Gizmos.matrix;
                    Gizmos.matrix = switchHandle.localToWorldMatrix;
                    Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
                    Gizmos.matrix = oldMatrix;
                }
            }
        }
    }
} 