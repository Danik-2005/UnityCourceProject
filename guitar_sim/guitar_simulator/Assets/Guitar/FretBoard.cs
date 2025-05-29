/*
using UnityEngine;
using System.Collections.Generic;

public class FretBoard : MonoBehaviour
{
    public GameObject fretPrefab; // Префаб для лада (можно просто куб)
    public int numberOfFrets = 22;
    public float fretSpacing = 0.1f; // Расстояние между ладами
    public float stringSpacing = 0.02f; // Расстояние между струнами
    public Vector3 startPoint = Vector3.zero; // Начальная точка грифа

    private List<GameObject> frets = new List<GameObject>();
    private List<GuitarStringComponent> strings = new List<GuitarStringComponent>();
    private GuitarInstrument guitarInstrument;

    void Start()
    {
        guitarInstrument = GetComponent<GuitarInstrument>();
        CreateFrets();
        CreateStrings();
    }

    void CreateFrets()
    {
        for (int i = 0; i <= numberOfFrets; i++)
        {
            Vector3 fretPosition = startPoint + Vector3.forward * (i * fretSpacing);
            GameObject fret = Instantiate(fretPrefab, fretPosition, Quaternion.identity, transform);
            fret.name = $"Fret_{i}";
            
            // Настраиваем коллайдер для лада
            BoxCollider fretCollider = fret.AddComponent<BoxCollider>();
            fretCollider.isTrigger = true;
            fretCollider.size = new Vector3(0.15f, 0.01f, 0.02f); // Настройте размер под вашу модель
            
            frets.Add(fret);
        }
    }

    void CreateStrings()
    {
        for (int i = 0; i < 6; i++)
        {
            GameObject stringObj = new GameObject($"String_{i + 1}");
            stringObj.transform.parent = transform;
            
            GuitarStringComponent stringComponent = stringObj.AddComponent<GuitarStringComponent>();
            stringComponent.stringNumber = i + 1;
            
            // Вычисляем позиции начала и конца струны
            Vector3 stringStart = startPoint + Vector3.right * (i * stringSpacing);
            Vector3 stringEnd = stringStart + Vector3.forward * (numberOfFrets * fretSpacing);
            
            stringComponent.SetStringPoints(stringStart, stringEnd);
            strings.Add(stringComponent);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Левый клик мыши
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            
            GuitarStringComponent hitString = null;
            int fretNumber = -1;

            foreach (var hit in hits)
            {
                // Проверяем, попали ли мы в струну
                GuitarStringComponent stringComponent = hit.collider.GetComponent<GuitarStringComponent>();
                if (stringComponent != null)
                {
                    hitString = stringComponent;
                }

                // Проверяем, попали ли мы в лад
                if (hit.collider.gameObject.name.StartsWith("Fret_"))
                {
                    fretNumber = int.Parse(hit.collider.gameObject.name.Split('_')[1]);
                }
            }

            // Если попали и в струну, и в лад
            if (hitString != null && fretNumber != -1)
            {
                PlayNote(hitString.stringNumber, fretNumber);
                hitString.Highlight(true);
                StartCoroutine(UnhighlightStringAfterDelay(hitString, 0.1f));
            }
        }
    }

    IEnumerator UnhighlightStringAfterDelay(GuitarStringComponent stringComponent, float delay)
    {
        yield return new WaitForSeconds(delay);
        stringComponent.Highlight(false);
    }

    void PlayNote(int stringNumber, int fret)
    {
        if (guitarInstrument != null)
        {
            guitarInstrument.PlayNote(stringNumber, fret);
        }
    }
}
*/ 