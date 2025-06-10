using UnityEngine;

public class GuitarInputController : MonoBehaviour
{
    [Header("References")]
    public GuitarKnobsController knobsController;
    public TremoloController tremoloController;

    [Header("Knob Control Settings")]
    public float knobSensitivity = 0.1f;
    public KeyCode volumeKnobKey = KeyCode.Alpha1;
    public KeyCode toneKnobKey = KeyCode.Alpha2;
    public KeyCode bassKnobKey = KeyCode.Alpha3;
    public KeyCode tremoloKey = KeyCode.T;

    private bool isVolumeSelected = false;
    private bool isToneSelected = false;
    private bool isBassSelected = false;

    private void Start()
    {
        if (knobsController == null)
        {
            knobsController = FindObjectOfType<GuitarKnobsController>();
            if (knobsController == null)
                Debug.LogError("GuitarKnobsController not found!");
        }

        if (tremoloController == null)
        {
            tremoloController = FindObjectOfType<TremoloController>();
            if (tremoloController == null)
                Debug.LogError("TremoloController not found!");
        }
    }

    private void Update()
    {
        // Обработка выбора крутилок
        HandleKnobSelection();

        // Обработка вращения крутилок
        HandleKnobRotation();

        // Обработка тремоло
        HandleTremolo();
    }

    private void HandleKnobSelection()
    {
        // Сброс выбора при нажатии ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isVolumeSelected = isToneSelected = isBassSelected = false;
            return;
        }

        // Выбор крутилок
        if (Input.GetKeyDown(volumeKnobKey))
        {
            isVolumeSelected = true;
            isToneSelected = isBassSelected = false;
        }
        else if (Input.GetKeyDown(toneKnobKey))
        {
            isToneSelected = true;
            isVolumeSelected = isBassSelected = false;
        }
        else if (Input.GetKeyDown(bassKnobKey))
        {
            isBassSelected = true;
            isVolumeSelected = isToneSelected = false;
        }
    }

    private void HandleKnobRotation()
    {
        if (knobsController == null) return;

        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta != 0)
        {
            if (isVolumeSelected)
            {
                float newValue = knobsController.volumeKnob + scrollDelta * knobSensitivity;
                knobsController.SetVolume(Mathf.Clamp01(newValue));
            }
            else if (isToneSelected)
            {
                float newValue = knobsController.toneKnob + scrollDelta * knobSensitivity;
                knobsController.SetTone(Mathf.Clamp01(newValue));
            }
            else if (isBassSelected)
            {
                float newValue = knobsController.bassKnob + scrollDelta * knobSensitivity;
                knobsController.SetBass(Mathf.Clamp01(newValue));
            }
        }
    }

    private void HandleTremolo()
    {
        if (tremoloController == null) return;

        // Активация тремоло
        if (Input.GetKeyDown(tremoloKey))
        {
            tremoloController.StartTremolo();
        }
        else if (Input.GetKeyUp(tremoloKey))
        {
            tremoloController.StopTremolo();
        }
    }
} 