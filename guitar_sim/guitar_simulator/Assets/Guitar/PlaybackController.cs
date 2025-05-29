using UnityEngine;
using UnityEngine.UI;

public class PlaybackController : MonoBehaviour
{
    public MidiPlayer midiPlayer;
    public Slider bpmSlider;
    public Button playButton;
    public Button stopButton;

    void Start()
    {
        if (bpmSlider != null)
        {
            bpmSlider.minValue = 0.1f;
            bpmSlider.maxValue = 2f;
            bpmSlider.value = 1f;
            bpmSlider.onValueChanged.AddListener(OnBPMChanged);
        }

        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayClick);
        }

        if (stopButton != null)
        {
            stopButton.onClick.AddListener(OnStopClick);
        }
    }

    void OnBPMChanged(float value)
    {
        if (midiPlayer != null)
        {
            midiPlayer.SetBPMMultiplier(value);
        }
    }

    void OnPlayClick()
    {
        if (midiPlayer != null)
        {
            midiPlayer.LoadAndPlayMidi();
        }
    }

    void OnStopClick()
    {
        if (midiPlayer != null)
        {
            midiPlayer.StopPlayback();
        }
    }

    void OnDestroy()
    {
        if (bpmSlider != null)
        {
            bpmSlider.onValueChanged.RemoveListener(OnBPMChanged);
        }

        if (playButton != null)
        {
            playButton.onClick.RemoveListener(OnPlayClick);
        }

        if (stopButton != null)
        {
            stopButton.onClick.RemoveListener(OnStopClick);
        }
    }
} 