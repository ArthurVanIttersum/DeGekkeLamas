using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider volumeSlider;

    void Start()
    {
        float volume;
        mixer.GetFloat("MasterVolume", out volume);
        volumeSlider.value = Mathf.Pow(10, volume / 20); // Convert dB to linear

        float savedVolume = PlayerPrefs.GetFloat("GlobalVolume", 1f); // default to full volume
        mixer.SetFloat("MasterVolume", Mathf.Log10(savedVolume) * 20);

        if (volumeSlider != null)
            volumeSlider.value = savedVolume;
    }

    public void SetVolume(float value)
    {
        if (value <= 0.0001f)
            value = 0.0001f; // prevent log(0)

        mixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("GlobalVolume", value);
        PlayerPrefs.Save();
    }
}
