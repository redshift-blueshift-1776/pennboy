using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource effectsSource;

    [Header("Audio Clips")]
    public AudioClip[] musicClips;
    public AudioClip[] effectClips;

    private void Awake()
    {
        PlayMusic(0);
    }

    public void PlayMusic(int clipIndex)
    {
        if (clipIndex >= 0 && clipIndex < musicClips.Length)
        {
            musicSource.clip = musicClips[clipIndex];
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Invalid music clip index.");
        }
    }

    public void PlayEffect(int clipIndex)
    {
        if (clipIndex >= 0 && clipIndex < effectClips.Length)
        {
            effectsSource.PlayOneShot(effectClips[clipIndex]);
        }
        else
        {
            Debug.LogWarning("Invalid effect clip index.");
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetEffectsVolume(float volume)
    {
        effectsSource.volume = Mathf.Clamp01(volume);
    }
}