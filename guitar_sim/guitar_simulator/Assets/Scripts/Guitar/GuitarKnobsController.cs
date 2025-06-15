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
    [SerializeField] private string pitchParameter = "GuitarPitch"; // Параметр для тремоло

    [Header("Current Values")]
    [Range(0f, 1f)]
    public float volumeKnob = 1f;
    [Range(0f, 1f)]
    public float toneKnob = 1f;
    [Range(0f, 1f)]
    public float bassKnob = 0.5f;
    [Range(0.5f, 2f)]
    public float currentPitch = 1f; // Текущий питч (1 = нормальный, <1 = ниже, >1 = выше)

    [Header("Filter Ranges")]
    public float minToneFreq = 350f;   // Минимальная частота для тона (Low Pass Filter)
    public float maxToneFreq = 22000f;  // Максимальная частота для тона
    public float minBassFreq = 20f;    // Минимальная частота для баса (High Pass Filter)
    public float maxBassFreq = 2000f;   // Максимальная частота для баса

    [Header("Tremolo Settings")]
    public float minPitch = 0.5f;     // Минимальный питч (октава вниз)
    public float maxPitch = 2f;       // Максимальный питч (октава вверх)

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

    }

    private void VerifyMixerParameters()
    {
        if (guitarMixer == null) return;

        float testValue;
        bool volumeExists = guitarMixer.GetFloat(volumeParameter, out testValue);
        bool toneExists = guitarMixer.GetFloat(toneParameter, out testValue);
        bool bassExists = guitarMixer.GetFloat(bassParameter, out testValue);
        bool pitchExists = guitarMixer.GetFloat(pitchParameter, out testValue);

        if (!volumeExists)
            Debug.LogError($"Parameter '{volumeParameter}' not found in AudioMixer! Check if it's exposed.");
        if (!toneExists)
            Debug.LogError($"Parameter '{toneParameter}' not found in AudioMixer! Check if it's exposed.");
        if (!bassExists)
            Debug.LogError($"Parameter '{bassParameter}' not found in AudioMixer! Check if it's exposed.");
        if (!pitchExists)
            Debug.LogError($"Parameter '{pitchParameter}' not found in AudioMixer! Check if it's exposed.");

        parametersVerified = volumeExists && toneExists && bassExists && pitchExists;
        
        if (parametersVerified)
            Debug.Log("All AudioMixer parameters verified successfully!");
        else
            Debug.LogError("Some AudioMixer parameters are missing! Check if they are exposed in the AudioMixer.");
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

            // Pitch: устанавливаем текущий питч
            if (!guitarMixer.SetFloat(pitchParameter, currentPitch))
                Debug.LogError($"Failed to set {pitchParameter}");

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

    // Метод для управления тремоло
    public void SetPitch(float value)
    {
        currentPitch = Mathf.Clamp(value, minPitch, maxPitch);
        UpdateAllParameters();
    }
} 