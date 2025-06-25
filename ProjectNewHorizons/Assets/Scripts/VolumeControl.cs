using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider volumeSlider;

    void Start()
    {
        // Set the slider's value to match the current volume
        float currentVolume;
        mixer.GetFloat("MasterVolume", out currentVolume);
        volumeSlider.value = Mathf.Pow(10f, currentVolume / 20f); // convert dB to 0–1
    }

    public void SetVolume(float sliderValue)
    {
        // Convert slider (0–1) to dB scale (-80 to 0)
        float dB = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20f;
        mixer.SetFloat("MasterVolume", dB);
    }
}
