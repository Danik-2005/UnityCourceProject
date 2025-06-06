using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class TestSoundButton : MonoBehaviour
{
    [Header("References")]
    public GuitarEffects guitarEffects;
    public AudioSource audioSource;

    [Header("Test Settings")]
    public AudioClip testSound;
    public float testDuration = 1f;
    public float[] pitchValues = { 0.5f, 0.75f, 1f, 1.5f, 2f };

    private Button button;
    private bool isPlaying = false;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(StartSoundTest);

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void StartSoundTest()
    {
        if (!isPlaying)
        {
            StartCoroutine(PlayTestSequence());
        }
    }

    private IEnumerator PlayTestSequence()
    {
        isPlaying = true;
        button.interactable = false;

        // Сохраняем оригинальные значения
        float originalVolume = guitarEffects.volumeKnob;
        float originalTone = guitarEffects.toneKnob;
        float originalDistortion = guitarEffects.distortionKnob;

        // Тестируем разные питчи
        foreach (float pitch in pitchValues)
        {
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(testSound);
            Debug.Log($"Testing pitch: {pitch}");
            yield return new WaitForSeconds(testDuration);
        }

        // Тестируем громкость
        audioSource.pitch = 1f;
        for (float volume = 0; volume <= 1; volume += 0.25f)
        {
            guitarEffects.SetVolume(volume);
            audioSource.PlayOneShot(testSound);
            Debug.Log($"Testing volume: {volume}");
            yield return new WaitForSeconds(testDuration);
        }

        // Тестируем искажение
        guitarEffects.SetVolume(0.75f);
        for (float distortion = 0; distortion <= 1; distortion += 0.25f)
        {
            guitarEffects.SetDistortion(distortion);
            audioSource.PlayOneShot(testSound);
            Debug.Log($"Testing distortion: {distortion}");
            yield return new WaitForSeconds(testDuration);
        }

        // Возвращаем оригинальные настройки
        guitarEffects.volumeKnob = originalVolume;
        guitarEffects.toneKnob = originalTone;
        guitarEffects.distortionKnob = originalDistortion;
        guitarEffects.UpdateEffects();

        button.interactable = true;
        isPlaying = false;
    }
} 