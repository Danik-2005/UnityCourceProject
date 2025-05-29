using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class NoteSampleBank : MonoBehaviour
{
    public static NoteSampleBank Instance { get; private set; }

    private Dictionary<(PickupType, int, int), AudioClip> sampleMap = new();

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
            Debug.Log($"Added clip {clip.name} for pickup:{pickup}, string:{stringNum}, fret:{fret}");
        }
        Debug.Log($"Total clips in sampleMap: {sampleMap.Count}");
    }

    public Dictionary<PickupType, Dictionary<int, (AudioClip clip, int baseMidiNote)>> GetSamplesForString(int stringNum)
    {
        var result = new Dictionary<PickupType, Dictionary<int, (AudioClip, int)>>();

        foreach (var pickup in System.Enum.GetValues(typeof(PickupType)))
        {
            var p = (PickupType)pickup;
            result[p] = new();

            for (int fret = 0; fret <= 21; fret++)
            {
                if (sampleMap.TryGetValue((p, stringNum, fret), out var clip))
                {
                    int midi = GetOpenNoteForString(stringNum) + fret;
                    result[p][fret] = (clip, midi);
                }
            }
        }

        return result;
    }

    private int GetOpenNoteForString(int stringNum) =>
        stringNum switch
        {
            6 => 40,
            5 => 45,
            4 => 50,
            3 => 55,
            2 => 59,
            1 => 64,
            _ => 40
        };

    public AudioClip GetClipForNote(int midiNote, PickupType pickup)
    {
        // Find the best string to play this note on
        for (int stringNum = 6; stringNum >= 1; stringNum--)
        {
            int openNote = GetOpenNoteForString(stringNum);
            int fret = midiNote - openNote;
            
            // Check if the note is playable on this string (fret 0-21)
            if (fret >= 0 && fret <= 21)
            {
                // Try to get the exact sample
                if (sampleMap.TryGetValue((pickup, stringNum, fret), out var clip))
                {
                    return clip;
                }

                // If no exact sample, find the nearest recorded fret
                int[] recordedFrets = new[] { 0, 7, 14, 21 };
                int nearestFret = recordedFrets[0];
                int minDistance = Mathf.Abs(fret - recordedFrets[0]);

                foreach (int recordedFret in recordedFrets)
                {
                    int distance = Mathf.Abs(fret - recordedFret);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestFret = recordedFret;
                    }
                }

                if (sampleMap.TryGetValue((pickup, stringNum, nearestFret), out var nearestClip))
                {
                    return nearestClip;
                }
            }
        }

        return null;
    }
}
