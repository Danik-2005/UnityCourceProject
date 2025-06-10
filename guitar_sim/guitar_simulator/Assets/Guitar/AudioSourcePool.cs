using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Linq;

public class AudioSourcePool : MonoBehaviour
{
    public static AudioSourcePool Instance { get; private set; }

    [Header("Audio Settings")]
    public AudioSource audioSourcePrefab;
    public AudioMixerGroup guitarMixerGroup; // Ссылка на группу Guitar в миксере
    public int poolSize = 32;

    private Queue<AudioSource> inactiveSources = new Queue<AudioSource>();
    private HashSet<AudioSource> activeSources = new HashSet<AudioSource>();

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
            inactiveSources.Enqueue(source);
        }
    }

    private void Update()
    {
        // Проверяем и возвращаем в пул неактивные источники
        foreach (var source in new HashSet<AudioSource>(activeSources))
        {
            if (!source.isPlaying)
            {
                ReturnToPool(source);
            }
        }
    }

    private void ConfigureAudioSource(AudioSource source)
    {
        source.playOnAwake = false;
        source.outputAudioMixerGroup = guitarMixerGroup;
        source.spatialBlend = 0f; // Делаем звук 2D для лучшей слышимости эффектов
        source.gameObject.SetActive(false);
    }

    public AudioSource GetSource()
    {
        AudioSource source;
        
        // Если в пуле нет свободных источников, ищем неактивный среди активных
        if (inactiveSources.Count == 0)
        {
            source = activeSources.FirstOrDefault(s => !s.isPlaying);
            if (source != null)
            {
                activeSources.Remove(source);
            }
            else
            {
                Debug.LogWarning("No available audio sources in pool!");
                return null;
            }
        }
        else
        {
            source = inactiveSources.Dequeue();
        }

        source.gameObject.SetActive(true);
        
        // Проверяем настройки
        if (source.outputAudioMixerGroup != guitarMixerGroup)
        {
            source.outputAudioMixerGroup = guitarMixerGroup;
        }
        
        activeSources.Add(source);
        return source;
    }

    public void ReturnToPool(AudioSource source)
    {
        if (source == null) return;

        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
        activeSources.Remove(source);
        inactiveSources.Enqueue(source);
    }

    // Alias for ReturnToPool for better API consistency
    public void ReturnSource(AudioSource source) => ReturnToPool(source);

    public void StopAllSources()
    {
        foreach (var source in activeSources)
        {
            if (source != null)
            {
                ReturnToPool(source);
            }
        }
    }

    // Метод для проверки и исправления настроек всех источников
    public void ValidateAllSources()
    {
        foreach (var source in activeSources)
        {
            if (source != null && source.outputAudioMixerGroup != guitarMixerGroup)
            {
                source.outputAudioMixerGroup = guitarMixerGroup;
            }
        }
    }

    private void OnDisable()
    {
        StopAllSources();
    }
}
