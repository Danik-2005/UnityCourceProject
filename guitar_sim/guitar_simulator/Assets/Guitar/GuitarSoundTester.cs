using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GuitarSoundTester : MonoBehaviour
{
    [Header("Test Parameters")]
    [Range(1, 6)]
    public int stringNumber = 1;
    [Range(0, 22)]
    public int fretNumber = 0;

    [Header("UI References (Optional)")]
    public TMP_InputField stringInput;
    public TMP_InputField fretInput;
    public Button playButton;

    private void Start()
    {
        // Подключаем обработчики к UI элементам, если они есть
        if (stringInput != null)
        {
            stringInput.onValueChanged.AddListener(OnStringInputChanged);
            stringInput.text = stringNumber.ToString();
        }

        if (fretInput != null)
        {
            fretInput.onValueChanged.AddListener(OnFretInputChanged);
            fretInput.text = fretNumber.ToString();
        }

        if (playButton != null)
        {
            playButton.onClick.AddListener(PlayTestNote);
        }
    }

    public void PlayTestNote()
    {
        if (GuitarSoundSystem.Instance == null)
        {
            Debug.LogError("GuitarSoundSystem not found!");
            return;
        }

        Debug.Log($"Playing test note: String {stringNumber}, Fret {fretNumber}");
        int midiNote = GuitarSoundSystem.Instance.GetMIDINoteFromStringAndFret(stringNumber, fretNumber);
        Debug.Log($"MIDI Note: {midiNote}");

        GuitarSoundSystem.Instance.PlayNote(stringNumber, fretNumber);
    }

    private void OnStringInputChanged(string value)
    {
        if (int.TryParse(value, out int newValue))
        {
            stringNumber = Mathf.Clamp(newValue, 1, 6);
            if (stringInput != null && int.Parse(value) != stringNumber)
            {
                stringInput.text = stringNumber.ToString();
            }
        }
    }

    private void OnFretInputChanged(string value)
    {
        if (int.TryParse(value, out int newValue))
        {
            fretNumber = Mathf.Clamp(newValue, 0, 22);
            if (fretInput != null && int.Parse(value) != fretNumber)
            {
                fretInput.text = fretNumber.ToString();
            }
        }
    }

    private void OnDestroy()
    {
        // Отключаем обработчики
        if (stringInput != null)
        {
            stringInput.onValueChanged.RemoveListener(OnStringInputChanged);
        }

        if (fretInput != null)
        {
            fretInput.onValueChanged.RemoveListener(OnFretInputChanged);
        }

        if (playButton != null)
        {
            playButton.onClick.RemoveListener(PlayTestNote);
        }
    }
} 