using UnityEngine;

[System.Serializable]
public class NoteSampleData
{
    public int midiNoteNumber;
    public AudioClip neckSample;
    public AudioClip middleSample;
    public AudioClip bridgeSample;

    public AudioClip GetClip(PickupType pickup)
    {
        return pickup switch
        {
            PickupType.Neck => neckSample,
            PickupType.Middle => middleSample,
            PickupType.Bridge => bridgeSample,
            _ => neckSample
        };
    }
}
