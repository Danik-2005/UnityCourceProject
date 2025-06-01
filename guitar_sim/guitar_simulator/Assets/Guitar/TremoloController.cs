using UnityEngine;

public class TremoloController : MonoBehaviour
{
    [Header("References")]
    public GuitarKnobsController knobsController;
    public Transform tremoloArm;  // Рычаг тремоло

    [Header("Tremolo Settings")]
    public float maxRotationAngle = 15f;   // Максимальный угол наклона рычага
    public float tremoloSpeed = 8f;        // Скорость движения тремоло
    public float returnSpeed = 10f;        // Скорость возврата в исходное положение
    public float pitchRange = 0.2f;        // Диапазон изменения питча (±0.2 от нормального)

    private float initialRotationX;
    private float currentRotation = 0f;
    private bool isTremoloActive = false;
    private float tremoloTime = 0f;

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

        initialRotationX = tremoloArm.localRotation.eulerAngles.x;
    }

    private void Update()
    {
        if (isTremoloActive)
        {
            // Обновляем время для синусоидального движения
            tremoloTime += Time.deltaTime * tremoloSpeed;
            
            // Создаем колебательное движение
            currentRotation = Mathf.Sin(tremoloTime) * maxRotationAngle;
            
            // Применяем поворот к рычагу
            ApplyRotation();

            // Вычисляем и применяем питч
            float normalizedRotation = currentRotation / maxRotationAngle; // -1 to 1
            float pitchValue = 1f + (normalizedRotation * pitchRange);    // Изменяем питч относительно 1
            knobsController.SetPitch(pitchValue);
        }
        else if (currentRotation != 0f)
        {
            // Плавно возвращаем рычаг в исходное положение
            currentRotation = Mathf.MoveTowards(currentRotation, 0f, returnSpeed * Time.deltaTime);
            
            // Применяем поворот
            ApplyRotation();

            // Возвращаем питч к нормальному значению
            float normalizedRotation = currentRotation / maxRotationAngle;
            float pitchValue = 1f + (normalizedRotation * pitchRange);
            knobsController.SetPitch(pitchValue);

            // Если вернулись в исходное положение, сбрасываем питч точно на 1
            if (Mathf.Approximately(currentRotation, 0f))
            {
                knobsController.SetPitch(1f);
            }
        }
    }

    private void ApplyRotation()
    {
        if (tremoloArm != null)
        {
            Vector3 currentEuler = tremoloArm.localRotation.eulerAngles;
            tremoloArm.localRotation = Quaternion.Euler(
                initialRotationX + currentRotation,
                currentEuler.y,
                currentEuler.z
            );
        }
    }

    // Публичные методы для управления тремоло
    public void StartTremolo()
    {
        isTremoloActive = true;
        tremoloTime = 0f;
    }

    public void StopTremolo()
    {
        isTremoloActive = false;
    }

    // Визуализация для отладки
    private void OnDrawGizmosSelected()
    {
        if (tremoloArm != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(tremoloArm.position, 
                          tremoloArm.position + tremoloArm.up * 0.2f);
        }
    }
} 