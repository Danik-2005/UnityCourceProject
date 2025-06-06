using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class StringProperties
{
    public float width = 0.002f;
    public float colliderScale = 3f; // Множитель размера коллайдера относительно струны
}

public class GuitarStringsManager : MonoBehaviour
{
    [Header("String Settings")]
    public Material defaultStringMaterial;
    public Material highlightMaterial;
    
    [Header("String References")]
    public Transform[] stringStartPoints; // Массив из 6 точек начала струн
    public Transform[] stringEndPoints;   // Массив из 6 точек конца струн

    [Header("String Properties")]
    [Tooltip("Размеры струн от 1-й (самой тонкой) до 6-й (самой толстой)")]
    public StringProperties[] stringProperties = new StringProperties[]
    {
        new StringProperties { width = 0.0015f, colliderScale = 3f }, // 1-я струна (E4)
        new StringProperties { width = 0.0017f, colliderScale = 3f }, // 2-я струна (B3)
        new StringProperties { width = 0.002f,  colliderScale = 3f }, // 3-я струна (G3)
        new StringProperties { width = 0.0025f, colliderScale = 3f }, // 4-я струна (D3)
        new StringProperties { width = 0.003f,  colliderScale = 3f }, // 5-я струна (A2)
        new StringProperties { width = 0.0035f, colliderScale = 3f }  // 6-я струна (E2)
    };

    [Header("Fret Properties")]
    public int numberOfFrets = 22;

    private List<GuitarStringVisual> strings = new List<GuitarStringVisual>();
    private static GuitarStringsManager instance;
    public static GuitarStringsManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GuitarStringsManager>();
            }
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        if (stringStartPoints == null || stringEndPoints == null || 
            stringStartPoints.Length != 6 || stringEndPoints.Length != 6)
        {
            Debug.LogError("Please assign all string start and end points!");
            return;
        }

        if (stringProperties == null || stringProperties.Length != 6)
        {
            Debug.LogError("Please configure properties for all 6 strings!");
            return;
        }

        CreateAllStrings();
    }

    void CreateAllStrings()
    {
        // Создаем 6 струн
        for (int i = 0; i < 6; i++)
        {
            GameObject stringObj = new GameObject($"String_{i + 1}");
            stringObj.transform.parent = transform;
            
            // Добавляем компонент струны
            GuitarStringVisual stringVisual = stringObj.AddComponent<GuitarStringVisual>();
            
            // Настраиваем параметры
            stringVisual.stringNumber = i + 1;
            stringVisual.stringWidth = stringProperties[i].width;
            stringVisual.colliderScale = stringProperties[i].colliderScale;
            stringVisual.numberOfFrets = numberOfFrets;
            stringVisual.stringMaterial = defaultStringMaterial;
            stringVisual.highlightMaterial = highlightMaterial;
            stringVisual.startPoint = stringStartPoints[i];
            stringVisual.endPoint = stringEndPoints[i];
            
            strings.Add(stringVisual);
        }
    }

    // Метод для проигрывания ноты с подсветкой (используется для MIDI и прямых нажатий)
    public void PlayNoteWithVisuals(int stringNumber, int fret)
    {
        if (stringNumber < 1 || stringNumber > 6)
        {
            Debug.LogWarning($"Invalid string number: {stringNumber}");
            return;
        }

        GuitarStringVisual stringVisual = strings[stringNumber - 1];
        stringVisual.PlayNoteWithVisuals(fret);
    }

    // Метод для остановки ноты и снятия подсветки
    public void StopNoteWithVisuals(int stringNumber, int fret)
    {
        if (stringNumber < 1 || stringNumber > 6)
        {
            Debug.LogWarning($"Invalid string number: {stringNumber}");
            return;
        }

        GuitarStringVisual stringVisual = strings[stringNumber - 1];
        stringVisual.StopNoteWithVisuals(fret);
    }

    // Метод для проигрывания MIDI-ноты (автоматически находит подходящую струну и лад)
    public void PlayMidiNote(int midiNote)
    {
        var (stringNumber, fret) = FindBestStringAndFret(midiNote);
        if (stringNumber != -1)
        {
            PlayNoteWithVisuals(stringNumber, fret);
        }
    }

    // Метод для остановки MIDI-ноты
    public void StopMidiNote(int midiNote)
    {
        var (stringNumber, fret) = FindBestStringAndFret(midiNote);
        if (stringNumber != -1)
        {
            StopNoteWithVisuals(stringNumber, fret);
        }
    }

    // Находит подходящую струну и лад для MIDI-ноты
    public (int stringNumber, int fretNumber) FindBestStringAndFret(int targetMidiNote)
    {
        // Перебираем все струны от 6-й к 1-й
        for (int stringNum = 6; stringNum >= 1; stringNum--)
        {
            int openNote = GetOpenNoteForString(stringNum);
            int fret = targetMidiNote - openNote;
            
            if (fret >= 0 && fret <= numberOfFrets)
            {
                return (stringNum, fret);
            }
        }
        
        return (-1, -1);
    }

    private int GetOpenNoteForString(int stringNum) =>
        stringNum switch
        {
            6 => 40, // E2
            5 => 45, // A2
            4 => 50, // D3
            3 => 55, // G3
            2 => 59, // B3
            1 => 64, // E4
            _ => 40  // Default to E2
        };

    // Вспомогательный метод для остановки всех звуков (можно вызывать при смене сцены или паузе)
    public void StopAllStrings()
    {
        foreach (var guitarString in strings)
        {
            guitarString.ResetAllMaterials();
        }
    }

    void OnDisable()
    {
        StopAllStrings();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // Создаем массив свойств струн, если его нет
        if (stringProperties == null || stringProperties.Length != 6)
        {
            stringProperties = new StringProperties[]
            {
                new StringProperties { width = 0.0015f, colliderScale = 3f }, // 1-я струна (E4)
                new StringProperties { width = 0.0017f, colliderScale = 3f }, // 2-я струна (B3)
                new StringProperties { width = 0.002f,  colliderScale = 3f }, // 3-я струна (G3)
                new StringProperties { width = 0.0025f, colliderScale = 3f }, // 4-я струна (D3)
                new StringProperties { width = 0.003f,  colliderScale = 3f }, // 5-я струна (A2)
                new StringProperties { width = 0.0035f, colliderScale = 3f }  // 6-я струна (E2)
            };
        }
    }
#endif
} 