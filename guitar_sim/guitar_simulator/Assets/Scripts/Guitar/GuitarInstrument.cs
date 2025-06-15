using System.Collections.Generic;
using UnityEngine;

public class GuitarInstrument : MonoBehaviour
{
    private void Start()
    {
        if (GuitarSoundSystem.Instance == null)
        {
            Debug.LogError("GuitarSoundSystem not found!");
            return;
        }
    }

    public void PlayNote(int stringNum, int fret)
    {
        GuitarSoundSystem.Instance.PlayNote(stringNum, fret);
    }
}
