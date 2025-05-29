using UnityEngine;

public class GuitarNote
{
    public int midiNote;
    public AudioClip sample;
    public float pitchMultiplier;

    public GuitarNote(int midiNote, AudioClip sample, int baseMidiNote)
    {
        this.midiNote = midiNote;
        this.sample = sample;
        this.pitchMultiplier = Mathf.Pow(2f, (midiNote - baseMidiNote) / 12f);
    }
}
