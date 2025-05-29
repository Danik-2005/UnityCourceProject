using UnityEngine;
using System.Collections.Generic;

public class AudioSourcePool : MonoBehaviour
{
    public static AudioSourcePool Instance { get; private set; }

    public AudioSource audioSourcePrefab;
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

        for (int i = 0; i < poolSize; i++)
        {
            var source = Instantiate(audioSourcePrefab, transform);
            source.playOnAwake = false;
            source.gameObject.SetActive(false);
            pool.Enqueue(source);
        }
    }

    public AudioSource GetSource()
    {
        AudioSource source = pool.Dequeue();
        source.gameObject.SetActive(true);
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
}
