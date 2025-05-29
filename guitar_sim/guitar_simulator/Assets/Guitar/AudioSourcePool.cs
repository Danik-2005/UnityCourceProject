using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioSourcePool : MonoBehaviour
{
    public static AudioSourcePool Instance { get; private set; }

    [Header("Audio Settings")]
    public AudioSource audioSourcePrefab;
    public AudioMixerGroup guitarMixerGroup; // Ссылка на группу Guitar в миксере
    public int poolSize = 32;

    private Queue<AudioSource> pool = new Queue<AudioSource>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (guitarMixerGroup == null)
        {
            Debug.LogError("Guitar Mixer Group not assigned in AudioSourcePool!");
            return;
        }

        for (int i = 0; i < poolSize; i++)
        {
            var source = Instantiate(audioSourcePrefab, transform);
            ConfigureAudioSource(source);
            pool.Enqueue(source);
        }

        Debug.Log($"AudioSourcePool initialized with {poolSize} sources, using mixer group: {guitarMixerGroup.name}");
    }

    private void ConfigureAudioSource(AudioSource source)
    {
        source.playOnAwake = false;
        source.outputAudioMixerGroup = guitarMixerGroup;
        source.spatialBlend = 0f; // Делаем звук 2D для лучшей слышимости эффектов
        source.gameObject.SetActive(false);
        Debug.Log($"Configured AudioSource: {source.name} with mixer group: {guitarMixerGroup.name}");
    }

    public AudioSource GetSource()
    {
        AudioSource source = pool.Dequeue();
        source.gameObject.SetActive(true);
        
        // На всякий случай проверяем настройки
        if (source.outputAudioMixerGroup != guitarMixerGroup)
        {
            source.outputAudioMixerGroup = guitarMixerGroup;
            Debug.Log($"Restored mixer group for source: {source.name}");
        }
        
        pool.Enqueue(source);
        return source;
    }

    public void StopAllSources()
    {
        foreach (var source in pool)
        {
            if (source != null)
            {
                source.Stop();
                source.gameObject.SetActive(false);
            }
        }
    }

    // Метод для проверки и исправления настроек всех источников
    public void ValidateAllSources()
    {
        foreach (var source in pool)
        {
            if (source != null && source.outputAudioMixerGroup != guitarMixerGroup)
            {
                source.outputAudioMixerGroup = guitarMixerGroup;
                Debug.Log($"Fixed mixer group for source: {source.name}");
            }
        }
    }
}
