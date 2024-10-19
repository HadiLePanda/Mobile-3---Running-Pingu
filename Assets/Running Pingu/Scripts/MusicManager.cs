using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambientSource;

    [Header("Settings")]
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip gameAmbient;

    private Coroutine musicFadeRoutine;
    private Coroutine ambientFadeRoutine;

    public static MusicManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Play Ambient
    public void PlayAmbient(float targetVolume = 1f, float fadeDuration = 0.5f)
    {
        if (ambientFadeRoutine != null)
            StopCoroutine(ambientFadeRoutine);
        ambientFadeRoutine = StartCoroutine(AnimateSourceCrossfade(ambientSource, gameAmbient, targetVolume, fadeDuration));
    }

    // Play Music
    public void PlayMusic(float targetVolume = 1f, float fadeDuration = 0.5f)
    {
        if (musicFadeRoutine != null)
            StopCoroutine(musicFadeRoutine);
        musicFadeRoutine = StartCoroutine(AnimateSourceCrossfade(musicSource, gameMusic, targetVolume, fadeDuration));
    }
    public void PlayMusicInstant(float targetVolume = 1f, float fadeDuration = 0.5f)
    {
        musicSource.clip = gameMusic;
        musicSource.Play();
    }
    public void StopMusic() => musicSource.Stop();
    public void PauseMusic() => musicSource.Pause();
    public void ResumeMusic() => musicSource.UnPause();
    public void SetMusicVolume(float volume) => musicSource.volume = volume;

    IEnumerator AnimateSourceCrossfade(AudioSource audioSource, AudioClip nextTrack, float targetVolume = 1f, float fadeDuration = 0.5f)
    {
        float percent = 0;
        float startingVolume = audioSource.volume;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            audioSource.volume = Mathf.Lerp(startingVolume, 0, percent);
            yield return null;
        }

        audioSource.clip = nextTrack;
        audioSource.Play();

        percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            audioSource.volume = Mathf.Lerp(0, targetVolume, percent);
            yield return null;
        }
    }
}
