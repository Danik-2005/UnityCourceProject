using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class NoteSampleBank : MonoBehaviour
{
    public static NoteSampleBank Instance { get; private set; }

    private Dictionary<(PickupType, int, int), AudioClip> sampleMap = new();
    private Dictionary<int, int[]> recordedFretsForString = new();

    void Awake()
    {
        Instance = this;
        LoadAllSamples();
    }

    void LoadAllSamples()
    {
        Debug.Log("Starting LoadAllSamples");
        var clips = Resources.LoadAll<AudioClip>("GuitarSamples");
        Debug.Log($"Found {clips.Length} clips in Resources/GuitarSamples");
        
        Regex regex = new(@"([nmb])([1-6])_([0-9]+)");

        foreach (var clip in clips)
        {
            Debug.Log($"Processing clip: {clip.name}");
            var match = regex.Match(clip.name.ToLower());
            if (!match.Success) 
            {
                Debug.LogWarning($"Clip name doesn't match pattern: {clip.name}");
                continue;
            }

            PickupType pickup = match.Groups[1].Value switch
            {
                "n" => PickupType.Neck,
                "m" => PickupType.Middle,
                "b" => PickupType.Bridge,
                _ => PickupType.Neck
            };
            int stringNum = int.Parse(match.Groups[2].Value);
            int fret = int.Parse(match.Groups[3].Value);

            sampleMap[(pickup, stringNum, fret)] = clip;
            
            // Сохраняем информацию о записанных ладах для каждой струны
            if (!recordedFretsForString.ContainsKey(stringNum))
            {
                recordedFretsForString[stringNum] = new int[] { fret };
            }
            else
            {
                var currentFrets = recordedFretsForString[stringNum];
                var newFrets = new int[currentFrets.Length + 1];
                currentFrets.CopyTo(newFrets, 0);
                newFrets[currentFrets.Length] = fret;
                recordedFretsForString[stringNum] = newFrets;
            }
            
            Debug.Log($"Added clip {clip.name} for pickup:{pickup}, string:{stringNum}, fret:{fret}");
        }
        Debug.Log($"Total clips in sampleMap: {sampleMap.Count}");
    }

    public (AudioClip clip, bool isExactMatch, int baseFret) GetClipForStringAndFret(int stringNum, int fret, PickupType pickup)
    {
        // Сначала пробуем найти точное совпадение
        if (sampleMap.TryGetValue((pickup, stringNum, fret), out var exactClip))
        {
            Debug.Log($"[NoteSampleBank] Found exact sample for string {stringNum}, fret {fret}");
            return (exactClip, true, fret);
        }

        // Если точного совпадения нет, ищем ближайший записанный лад для этой струны
        if (recordedFretsForString.TryGetValue(stringNum, out var recordedFrets))
        {
            int closestFret = recordedFrets[0];
            int minDistance = Mathf.Abs(fret - closestFret);

            foreach (int recordedFret in recordedFrets)
            {
                int distance = Mathf.Abs(fret - recordedFret);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestFret = recordedFret;
                }
            }

            if (sampleMap.TryGetValue((pickup, stringNum, closestFret), out var nearestClip))
            {
                Debug.Log($"[NoteSampleBank] Using nearest sample for string {stringNum}: fret {closestFret} instead of {fret}");
                return (nearestClip, false, closestFret);
            }
        }

        Debug.LogWarning($"[NoteSampleBank] No suitable sample found for string {stringNum}, fret {fret}");
        return (null, false, -1);
    }

    public AudioClip GetClipForNote(int midiNote, PickupType pickup)
    {
        // Этот метод оставлен для обратной совместимости
        // Находим подходящую струну для этой ноты
        for (int stringNum = 6; stringNum >= 1; stringNum--)
        {
            int openNote = GetOpenNoteForString(stringNum);
            int fret = midiNote - openNote;
            
            if (fret >= 0 && fret <= 22)
            {
                var (clip, _, _) = GetClipForStringAndFret(stringNum, fret, pickup);
                return clip;
            }
        }
        
        return null;
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
}
