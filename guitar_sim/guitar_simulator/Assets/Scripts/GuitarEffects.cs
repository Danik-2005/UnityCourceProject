using UnityEngine;
using UnityEngine.Audio;

public class GuitarEffects : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer audioMixer;
    
    [Range(0f, 1f)]
    public float volumeKnob = 1f;
    
    [Range(0f, 1f)]
    public float toneKnob = 0.5f;
    
    [Range(0f, 1f)]
    public float distortionKnob = 0f;

    private void Start()
    {
        // Initialize effects with default values
        UpdateEffects();
    }

    public void UpdateEffects()
    {
        // Volume control (-80dB to 0dB)
        float volumeDB = Mathf.Lerp(-80f, 0f, volumeKnob);
        audioMixer.SetFloat("Volume", volumeDB);

        // Tone control (low-pass filter cutoff frequency)
        float toneCutoff = Mathf.Lerp(500f, 22000f, toneKnob);
        audioMixer.SetFloat("LowPassCutoff", toneCutoff);

        // Distortion control
        audioMixer.SetFloat("Distortion", distortionKnob);
    }

    // Methods to control individual knobs
    public void SetVolume(float value)
    {
        volumeKnob = Mathf.Clamp01(value);
        UpdateEffects();
    }

    public void SetTone(float value)
    {
        toneKnob = Mathf.Clamp01(value);
        UpdateEffects();
    }

    public void SetDistortion(float value)
    {
        distortionKnob = Mathf.Clamp01(value);
        UpdateEffects();
    }
} 