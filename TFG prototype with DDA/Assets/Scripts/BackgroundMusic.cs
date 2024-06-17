using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic instance;

    [Header("Background Music")]
    public AudioClip backgroundMusic;
    private AudioSource audioSource;
    private float originalPitch; // Store the original pitch of the audio source
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        originalPitch = audioSource.pitch; // Store the original pitch
    }
    public void PlayBackgroundMusic(float volume = 0.5f)
    {
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.pitch = originalPitch; // Ensure pitch is reset to original
        audioSource.Play();
    }

    public void StopBackgroundMusic()
    {
        audioSource.Stop();
    }

    public void SpeedUpBackgroundMusic(float factor)
    {
        audioSource.pitch *= factor; // Increase the pitch to speed up the music
    }

    public void RestoreBackgroundMusicSpeed()
    {
        audioSource.pitch = originalPitch; // Restore the original pitch
    }
}
