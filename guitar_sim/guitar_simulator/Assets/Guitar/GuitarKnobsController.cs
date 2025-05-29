using UnityEngine;
using UnityEngine.Audio;

public class GuitarKnobsController : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer guitarMixer;

    [Header("Mixer Parameters")]
    [SerializeField] private string volumeParameter = "GuitarVolume";
    [SerializeField] private string toneParameter = "GuitarToneLPF";
    [SerializeField] private string bassParameter = "GuitarBassHPF";

    [Header("Current Values")]
    [Range(0f, 1f)]
    public float volumeKnob = 1f;
    [Range(0f, 1f)]
    public float toneKnob = 1f;
    [Range(0f, 1f)]
    public float bassKnob = 0.5f;

    [Header("Filter Ranges")]
    public float minToneFreq = 350f;   // Минимальная частота для тона (Low Pass Filter)
    public float maxToneFreq = 22000f;  // Максимальная частота для тона
    public float minBassFreq = 20f;    // Минимальная частота для баса (High Pass Filter)
    public float maxBassFreq = 2000f;   // Максимальная частота для баса

    private bool parametersVerified = false;

    private void Start()
    {
        if (guitarMixer == null)
        {
            Debug.LogError("Guitar AudioMixer not assigned!");
            return;
        }

        // Проверяем существование параметров
        VerifyMixerParameters();

        // Применяем начальные значения
        UpdateAllParameters();

        // Выводим текущие значения для проверки
        PrintCurrentMixerValues();
    }

    private void VerifyMixerParameters()
    {
        if (guitarMixer == null) return;

        float testValue;
        bool volumeExists = guitarMixer.GetFloat(volumeParameter, out testValue);
        bool toneExists = guitarMixer.GetFloat(toneParameter, out testValue);
        bool bassExists = guitarMixer.GetFloat(bassParameter, out testValue);

        if (!volumeExists)
            Debug.LogError($"Parameter '{volumeParameter}' not found in AudioMixer! Check if it's exposed.");
        if (!toneExists)
            Debug.LogError($"Parameter '{toneParameter}' not found in AudioMixer! Check if it's exposed.");
        if (!bassExists)
            Debug.LogError($"Parameter '{bassParameter}' not found in AudioMixer! Check if it's exposed.");

        parametersVerified = volumeExists && toneExists && bassExists;
        
        if (parametersVerified)
            Debug.Log("All AudioMixer parameters verified successfully!");
        else
            Debug.LogError("Some AudioMixer parameters are missing! Check if they are exposed in the AudioMixer.");
    }

    private void PrintCurrentMixerValues()
    {
        if (!parametersVerified) return;

        float currentVolume, currentTone, currentBass;
        guitarMixer.GetFloat(volumeParameter, out currentVolume);
        guitarMixer.GetFloat(toneParameter, out currentTone);
        guitarMixer.GetFloat(bassParameter, out currentBass);

        Debug.Log($"Current Mixer Values:\n" +
                 $"Volume: {currentVolume:F1}dB\n" +
                 $"Tone LPF: {currentTone:F0}Hz\n" +
                 $"Bass HPF: {currentBass:F0}Hz");
    }

    public void UpdateAllParameters()
    {
        if (guitarMixer == null || !parametersVerified) return;

        try
        {
            // Volume: преобразуем линейное значение (0-1) в децибелы (-80 до 0)
            float volumeDb = Mathf.Lerp(-80f, 0f, volumeKnob);
            if (!guitarMixer.SetFloat(volumeParameter, volumeDb))
                Debug.LogError($"Failed to set {volumeParameter}");

            // Tone: управляем частотой среза Low Pass Filter
            float toneFreq = Mathf.Lerp(minToneFreq, maxToneFreq, toneKnob);
            if (!guitarMixer.SetFloat(toneParameter, toneFreq))
                Debug.LogError($"Failed to set {toneParameter}");

            // Bass: управляем частотой среза High Pass Filter
            float bassFreq = Mathf.Lerp(maxBassFreq, minBassFreq, bassKnob);
            if (!guitarMixer.SetFloat(bassParameter, bassFreq))
                Debug.LogError($"Failed to set {bassParameter}");

            Debug.Log($"Updated mixer - Vol: {volumeDb:F1}dB, Tone: {toneFreq:F0}Hz, Bass: {bassFreq:F0}Hz");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error updating mixer parameters: {e.Message}");
        }
    }

    // Методы для изменения значений крутилок
    public void SetVolume(float value)
    {
        volumeKnob = Mathf.Clamp01(value);
        UpdateAllParameters();
    }

    public void SetTone(float value)
    {
        toneKnob = Mathf.Clamp01(value);
        UpdateAllParameters();
    }

    public void SetBass(float value)
    {
        bassKnob = Mathf.Clamp01(value);
        UpdateAllParameters();
    }
} 