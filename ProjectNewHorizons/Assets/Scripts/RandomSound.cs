using UnityEngine;
using UnityEngine.Events;

public class RandomSound : MonoBehaviour
{
    public AudioClip[] sounds;
    private AudioSource audioSource;
    public void GenerateRandomSound()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = sounds[Random.Range(0, sounds.Length)];
        audioSource.Play();
    }
}
