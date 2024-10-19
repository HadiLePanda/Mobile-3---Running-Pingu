using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource musicSource;

    [Header("Settings")]
    [SerializeField] private AudioClip gameMusic;

    private Coroutine musicFadeRoutine;

    public static MusicManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Play Music
    public void PlayMusic(float targetVolume = 1f, float fadeDuration = 0.5f)
    {
        if (musicFadeRoutine != null)
            StopCoroutine(musicFadeRoutine);
        musicFadeRoutine = StartCoroutine(AnimateMusicCrossfade(gameMusic, targetVolume, fadeDuration));
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

    IEnumerator AnimateMusicCrossfade(AudioClip nextTrack, float targetVolume = 1f, float fadeDuration = 0.5f)
    {
        float percent = 0;
        float startingVolume = musicSource.volume;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(startingVolume, 0, percent);
            yield return null;
        }

        musicSource.clip = nextTrack;
        musicSource.Play();

        percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(0, targetVolume, percent);
            yield return null;
        }
    }
}
