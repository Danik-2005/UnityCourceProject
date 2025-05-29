using System.Collections.Generic;
using UnityEngine;

public class GuitarInstrument : MonoBehaviour
{
    public AudioSource audioSourcePrefab;
    public PickupType pickup = PickupType.Neck;

    private Dictionary<int, GuitarString> strings = new();

    void Start()
    {
        for (int s = 1; s <= 6; s++)
        {
            strings[s] = new GuitarString(s);
        }
    }

    public void PlayNote(int stringNum, int fret)
    {
        if (!strings.TryGetValue(stringNum, out var guitarString)) return;

        int midi = guitarString.openMidiNote + fret;
        if (!guitarString.notes.TryGetValue(midi, out var note)) return;

        var source = Instantiate(audioSourcePrefab, transform);
        source.clip = note.sample;
        source.pitch = note.pitchMultiplier;
        source.Play();
        Destroy(source.gameObject, note.sample.length / source.pitch + 0.2f);
    }
}
