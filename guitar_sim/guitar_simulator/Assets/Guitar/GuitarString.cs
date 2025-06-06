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

        // Создаем ноты для всех ладов
        for (int fret = 0; fret <= 22; fret++)
        {
            int midiNote = openMidiNote + fret;
            var (clip, isExactMatch, baseFret) = NoteSampleBank.Instance.GetClipForStringAndFret(stringNumber, fret, PickupType.Neck);
            
            if (clip != null)
            {
                float pitchMultiplier = 1f;
                if (!isExactMatch)
                {
                    float fretDifference = fret - baseFret;
                    pitchMultiplier = Mathf.Pow(2f, fretDifference / 12f);
                }
                notes[midiNote] = new GuitarNote(midiNote, clip, midiNote, pitchMultiplier);
            }
        }
    }

    private int GetOpenNoteForString(int stringNum) =>
        stringNum switch
        {
            6 => 40, // E2
            5 => 45, // A2
            4 => 50, // D3
            3 => 55, // G3
            2 => 59, // B3
            1 => 64, // E4
            _ => 40  // Default to E2
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
