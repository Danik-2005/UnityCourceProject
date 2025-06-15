using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuitarStringVisual : MonoBehaviour
{
    [Header("String Properties")]
    public int stringNumber = 1;
    public float stringWidth = 0.002f;
    public float colliderScale = 3f;
    public Material stringMaterial;
    public Material highlightMaterial;
    
    [Header("String Points")]
    public Transform startPoint;
    public Transform endPoint;
    
    [Header("Fret Properties")]
    public int numberOfFrets = 22;
    
    [Header("Animation")]
    public float vibrationAmplitude = 0.01f;
    public float vibrationSpeed = 30f;
    public float vibrationDuration = 0.5f;
    public float dampingSpeed = 0.95f; // Скорость затухания при повторных нажатиях

    private List<GameObject> stringSegments = new List<GameObject>();
    private List<GameObject> fretColliders = new List<GameObject>();
    private bool isVibrating = false;
    private Coroutine vibrationCoroutine;
    private float stringLength;
    private Vector3 stringDirection;
    private int currentHighlightedFret = -1;
    private float currentVibrationAmplitude;
    private Dictionary<int, int> activeNotes = new Dictionary<int, int>(); // fret -> count

    void Start()
    {
        if (startPoint == null || endPoint == null)
        {
            Debug.LogError($"Start or end point not set for string {stringNumber}!");
            return;
        }

        UpdateStringProperties();
        CreateStringSegments();
        CreateFretColliders();
    }

    void UpdateStringProperties()
    {
        // Обновляем длину и направление струны
        stringLength = Vector3.Distance(startPoint.position, endPoint.position);
        stringDirection = (endPoint.position - startPoint.position).normalized;
        currentVibrationAmplitude = vibrationAmplitude;
    }

    void LateUpdate()
    {
        // Обновляем позиции сегментов струны каждый кадр
        if (!isVibrating)
        {
            UpdateStringSegments();
        }
    }

    void UpdateStringSegments()
    {
        UpdateStringProperties();

        // Обновляем позиции всех сегментов
        for (int fret = 0; fret <= numberOfFrets; fret++)
        {
            float startPos = CalculateFretPosition(fret);
            float endPos = CalculateFretPosition(fret + 1);
            float segmentLength = endPos - startPos;

            if (fret < stringSegments.Count)
            {
                GameObject segment = stringSegments[fret];
                
                // Обновляем позицию и поворот сегмента
                Vector3 segmentPosition = startPoint.position + stringDirection * (startPos + segmentLength / 2f);
                segment.transform.position = segmentPosition;
                segment.transform.rotation = Quaternion.FromToRotation(Vector3.up, stringDirection);
            }
        }

        // Обновляем позиции коллайдеров
        UpdateFretColliders();
    }

    void CreateStringSegments()
    {
        // Создаем сегменты для каждого лада
        for (int fret = 0; fret <= numberOfFrets; fret++)
        {
            float startPos = CalculateFretPosition(fret);
            float endPos = CalculateFretPosition(fret + 1);
            float segmentLength = endPos - startPos;

            GameObject segment = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            segment.name = $"String_{stringNumber}_Segment_{fret}";
            segment.transform.parent = transform;

            // Настраиваем размеры сегмента
            segment.transform.localScale = new Vector3(stringWidth, segmentLength / 2f, stringWidth);

            // Позиционируем сегмент
            Vector3 segmentPosition = startPoint.position + stringDirection * (startPos + segmentLength / 2f);
            segment.transform.position = segmentPosition;
            segment.transform.rotation = Quaternion.FromToRotation(Vector3.up, stringDirection);

            // Применяем материал
            var renderer = segment.GetComponent<Renderer>();
            renderer.material = stringMaterial;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // Отключаем отбрасывание теней

            // Удаляем коллайдер
            Destroy(segment.GetComponent<Collider>());

            stringSegments.Add(segment);
        }
    }

    void CreateFretColliders()
    {
        for (int fret = 0; fret <= numberOfFrets; fret++)
        {
            // Вычисляем позицию лада по формуле
            float fretPosition = CalculateFretPosition(fret);
            float nextFretPosition = CalculateFretPosition(fret + 1);
            float fretLength = nextFretPosition - fretPosition;
            
            // Создаем коллайдер для лада
            GameObject fretCollider = new GameObject($"Fret_{fret}_Collider");
            fretCollider.transform.parent = transform;
            
            // Добавляем BoxCollider
            BoxCollider collider = fretCollider.AddComponent<BoxCollider>();
            collider.size = new Vector3(stringWidth * colliderScale, stringWidth * colliderScale, fretLength);
            collider.center = Vector3.zero;
            collider.isTrigger = true;
            
            // Позиционируем коллайдер и ориентируем вдоль струны
            Vector3 fretCenter = startPoint.position + stringDirection * (fretPosition + fretLength/2f);
            fretCollider.transform.position = fretCenter;
            fretCollider.transform.rotation = Quaternion.FromToRotation(Vector3.forward, stringDirection);
            
            // Добавляем компонент для обработки нажатий
            FretTrigger trigger = fretCollider.AddComponent<FretTrigger>();
            trigger.stringNumber = stringNumber;
            trigger.fretNumber = fret;
            trigger.parentString = this;
            
            fretColliders.Add(fretCollider);
        }
    }

    void UpdateFretColliders()
    {
        for (int fret = 0; fret <= numberOfFrets; fret++)
        {
            if (fret < fretColliders.Count)
            {
                float fretPosition = CalculateFretPosition(fret);
                float nextFretPosition = CalculateFretPosition(fret + 1);
                float fretLength = nextFretPosition - fretPosition;

                GameObject fretCollider = fretColliders[fret];
                Vector3 fretCenter = startPoint.position + stringDirection * (fretPosition + fretLength/2f);
                fretCollider.transform.position = fretCenter;
                fretCollider.transform.rotation = Quaternion.FromToRotation(Vector3.forward, stringDirection);
            }
        }
    }

    float CalculateFretPosition(int fretNumber)
    {
        if (fretNumber == 0) return 0;
        if (fretNumber > numberOfFrets) return stringLength;
        
        // Формула для расчета позиции лада: L * (1 - 1/2^(n/12))
        return stringLength * (1f - Mathf.Pow(2f, -fretNumber / 12f));
    }

    public void PlayNoteWithVisuals(int fret)
    {
        if (fret < 0 || fret > numberOfFrets) return;

        // Увеличиваем счетчик активных нот для данного лада
        if (!activeNotes.ContainsKey(fret))
        {
            activeNotes[fret] = 0;
        }
        activeNotes[fret]++;

        // Проигрываем звук
        GuitarSoundSystem.Instance.PlayNote(stringNumber, fret);
        
        // Запускаем анимацию
        StartVibration();
        
        // Подсвечиваем сегмент струны
        HighlightFret(fret);
    }

    public void StopNoteWithVisuals(int fret)
    {
        if (fret < 0 || fret > numberOfFrets) return;

        // Уменьшаем счетчик активных нот
        if (activeNotes.ContainsKey(fret))
        {
            activeNotes[fret]--;
            if (activeNotes[fret] <= 0)
            {
                activeNotes.Remove(fret);
                // Проверяем все сегменты и обновляем их материалы
                UpdateAllSegmentMaterials();
            }
        }

        // Останавливаем звук
        GuitarSoundSystem.Instance.StopNote(stringNumber, fret);
    }

    void UpdateAllSegmentMaterials()
    {
        // Проходим по всем сегментам и обновляем их материалы
        for (int fret = 0; fret < stringSegments.Count; fret++)
        {
            var renderer = stringSegments[fret].GetComponent<Renderer>();
            if (activeNotes.ContainsKey(fret))
            {
                renderer.material = highlightMaterial;
                currentHighlightedFret = fret;
            }
            else
            {
                renderer.material = stringMaterial;
                if (currentHighlightedFret == fret)
                {
                    currentHighlightedFret = -1;
                }
            }
        }
    }

    // Обработчики нажатий от коллайдеров
    public void OnFretPressed(int fret)
    {
        PlayNoteWithVisuals(fret);
    }

    public void OnFretReleased(int fret)
    {
        StopNoteWithVisuals(fret);
    }

    void HighlightFret(int fret)
    {
        if (fret >= 0 && fret < stringSegments.Count)
        {
            var renderer = stringSegments[fret].GetComponent<Renderer>();
            renderer.material = highlightMaterial;
            currentHighlightedFret = fret;
        }
    }

    void UnhighlightCurrentFret()
    {
        if (currentHighlightedFret >= 0 && currentHighlightedFret < stringSegments.Count)
        {
            var renderer = stringSegments[currentHighlightedFret].GetComponent<Renderer>();
            renderer.material = stringMaterial;
            currentHighlightedFret = -1;
        }
    }

    public void ResetAllMaterials()
    {
        // Очищаем словарь активных нот
        activeNotes.Clear();
        currentHighlightedFret = -1;
        
        // Возвращаем исходный материал всем сегментам
        foreach (var segment in stringSegments)
        {
            var renderer = segment.GetComponent<Renderer>();
            renderer.material = stringMaterial;
        }
    }

    void OnDisable()
    {
        ResetAllMaterials();
    }

    void StartVibration()
    {
        // Если анимация уже идет, увеличиваем амплитуду
        if (isVibrating)
        {
            currentVibrationAmplitude = Mathf.Min(vibrationAmplitude * 2f, currentVibrationAmplitude + vibrationAmplitude);
        }
        else
        {
            currentVibrationAmplitude = vibrationAmplitude;
        }

        // Останавливаем предыдущую анимацию, если она есть
        if (vibrationCoroutine != null)
        {
            StopCoroutine(vibrationCoroutine);
        }
        
        vibrationCoroutine = StartCoroutine(VibrateString());
    }

    IEnumerator VibrateString()
    {
        isVibrating = true;
        float elapsed = 0f;
        float startingAmplitude = currentVibrationAmplitude;
        
        while (currentVibrationAmplitude > vibrationAmplitude * 0.01f)
        {
            float dampingFactor = Mathf.Exp(-elapsed * dampingSpeed);
            currentVibrationAmplitude = startingAmplitude * dampingFactor;
            
            float offset = Mathf.Sin(elapsed * vibrationSpeed) * currentVibrationAmplitude;
            
            // Получаем актуальное направление струны и перпендикулярное направление для вибрации
            Vector3 currentStringDirection = (endPoint.position - startPoint.position).normalized;
            Vector3 rightDirection = Vector3.Cross(currentStringDirection, Vector3.up).normalized;
            
            // Если rightDirection слишком мал (струна вертикальная), используем альтернативное направление
            if (rightDirection.magnitude < 0.1f)
            {
                rightDirection = Vector3.Cross(currentStringDirection, Vector3.forward).normalized;
            }
            
            for (int i = 0; i < stringSegments.Count; i++)
            {
                // Применяем параболическое смещение (максимум в середине струны)
                float normalizedPos = (float)i / (stringSegments.Count - 1);
                float amplitudeMod = 4f * normalizedPos * (1f - normalizedPos); // Параболическая функция
                float finalOffset = offset * amplitudeMod;

                // Получаем базовую позицию сегмента относительно актуального положения гитары
                float startPos = CalculateFretPosition(i);
                float endPos = CalculateFretPosition(i + 1);
                float segmentLength = endPos - startPos;
                Vector3 basePosition = startPoint.position + currentStringDirection * (startPos + segmentLength / 2f);
                
                // Применяем смещение относительно актуального направления
                stringSegments[i].transform.position = basePosition + rightDirection * finalOffset;
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Возвращаем сегменты в правильные позиции
        UpdateStringSegments();
        
        isVibrating = false;
        currentVibrationAmplitude = vibrationAmplitude;
    }
} 