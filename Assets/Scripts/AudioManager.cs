using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("----- Audio Source -----")]
    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource SFXSource;

    [Header("----- Audio Clip -----")]
    public AudioClip background;
    public AudioClip open_door;
    public AudioClip close_door;
    public AudioClip coin_collect;
    public AudioClip knocking;
    public AudioClip escape;

    [Header("----- Volume Settings -----")]
    [Range(0f, 1f)]
    public float defaultSFXVolume = 1.0f;

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    // New method to play SFX with a specific volume
    public void PlaySFXWithVolume(AudioClip clip, float volume)
    {
        SFXSource.PlayOneShot(clip, volume);
    }

    // Additional helper methods
    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void PauseMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        SFXSource.volume = Mathf.Clamp01(volume);
    }
}