using System.Collections.Generic;
using UnityEngine;

public class GuitarString
{
    public int stringNumber;
    public int openMidiNote;
    public Dictionary<int, GuitarNote> notes = new();

    public GuitarString(int stringNumber)
    {
        this.stringNumber = stringNumber;
        openMidiNote = GetOpenNoteForString(stringNumber);

        var samplesByPickup = NoteSampleBank.Instance.GetSamplesForString(stringNumber);

        foreach (var kvp in samplesByPickup)
        {
            foreach (var (fret, sampleData) in kvp.Value)
            {
                int midi = openMidiNote + fret;
                notes[midi] = new GuitarNote(midi, sampleData.clip, midi);
            }
        }

        for (int fret = 0; fret <= 21; fret++)
        {
            int midi = openMidiNote + fret;
            if (!notes.ContainsKey(midi))
            {
                var nearest = FindNearestSample(fret, samplesByPickup);
                if (nearest != null)
                    notes[midi] = new GuitarNote(midi, nearest.Value.clip, nearest.Value.baseMidiNote);
            }
        }
    }

    private (AudioClip clip, int baseMidiNote)? FindNearestSample(int fret, Dictionary<PickupType, Dictionary<int, (AudioClip clip, int baseMidiNote)>> pickups)
    {
        foreach (var pickup in pickups.Values)
        {
            int minDist = int.MaxValue;
            (AudioClip, int)? best = null;
            foreach (var (f, data) in pickup)
            {
                int d = Mathf.Abs(f - fret);
                if (d < minDist)
                {
                    minDist = d;
                    best = data;
                }
            }
            if (best.HasValue) return best.Value;
        }
        return null;
    }

    private int GetOpenNoteForString(int stringNum) =>
        stringNum switch
        {
            6 => 40,
            5 => 45,
            4 => 50,
            3 => 55,
            2 => 59,
            1 => 64,
            _ => 40
        };
}

/*
public class GuitarStringComponent : MonoBehaviour
{
    public int stringNumber; // Номер струны (1-6)
    public Material normalMaterial;
    public Material highlightMaterial;
    private LineRenderer lineRenderer;
    private BoxCollider stringCollider;

    void Start()
    {
        // Создаем LineRenderer для визуализации струны
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = normalMaterial;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.positionCount = 2;

        // Создаем коллайдер для струны
        stringCollider = gameObject.AddComponent<BoxCollider>();
        stringCollider.isTrigger = true;
        stringCollider.size = new Vector3(0.02f, 0.02f, 1f); // Настройте размер под вашу модель
    }

    public void SetStringPoints(Vector3 start, Vector3 end)
    {
        // Устанавливаем точки для LineRenderer
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        // Обновляем позицию и поворот коллайдера
        Vector3 center = (start + end) / 2f;
        transform.position = center;
        transform.LookAt(end);
        stringCollider.center = Vector3.zero;
        stringCollider.size = new Vector3(0.02f, 0.02f, Vector3.Distance(start, end));
    }

    public void Highlight(bool isHighlighted)
    {
        lineRenderer.material = isHighlighted ? highlightMaterial : normalMaterial;
    }
}
*/
