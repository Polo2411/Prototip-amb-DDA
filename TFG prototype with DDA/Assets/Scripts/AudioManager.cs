using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Sound Effects")]
    public AudioClip dashSound;
    public AudioClip shootSound;
    public AudioClip damageSound;
    public AudioClip hitSound;
    public AudioClip defeatSound;
    public AudioClip deathSound;
    public AudioClip chargingSound;
    public AudioClip chargedShootSound;
    public AudioClip ObjectTook;
    public AudioClip FireMonster;

    private AudioSource audioSource;

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
    }

    public void PlaySound(AudioClip clip, float volume = 1.0f, float duration = -1f)
    {
        audioSource.PlayOneShot(clip, volume);

        if (duration > 0)
        {
            StartCoroutine(StopSoundAfterDuration(duration));
        }
    }

    private IEnumerator StopSoundAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        audioSource.Stop();
    }
}
