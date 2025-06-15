using System.Collections.Generic;
using UnityEngine;

public class GuitarString
{
    public int stringNumber;
    public int openMidiNote;
    public Dictionary<int, GuitarNote> notes = new();

    public GuitarString(int stringNumber)
    {
        this.stringNumber = stringNumber;
        openMidiNote = GetOpenNoteForString(stringNumber);

        // Создаем ноты для всех ладов
        for (int fret = 0; fret <= 22; fret++)
        {
            int midiNote = openMidiNote + fret;
            var (clip, isExactMatch, baseFret) = NoteSampleBank.Instance.GetClipForStringAndFret(stringNumber, fret, PickupType.Neck);
            
            if (clip != null)
            {
                float pitchMultiplier = 1f;
                if (!isExactMatch)
                {
                    float fretDifference = fret - baseFret;
                    pitchMultiplier = Mathf.Pow(2f, fretDifference / 12f);
                }
                notes[midiNote] = new GuitarNote(midiNote, clip, midiNote, pitchMultiplier);
            }
        }
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