using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource sfxSource;

    public static AudioManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // SFX
    public void PlaySound2DOneShot(AudioClip clip, float volume = 1.0f, float pitchVariation = 0f)
    {
        if (clip == null)
            return;

        // randomize sound values and play it
        sfxSource.pitch = Random.Range(1 - pitchVariation, 1 + pitchVariation);

        // play sound
        sfxSource.PlayOneShot(clip, volume);
        
        // reset default values
        sfxSource.pitch = 1f;
    }
    public void PlaySound2DOneShotWithDelay(AudioClip clip, float volume = 1.0f, float pitchVariation = 0f, float delay = 0f)
    {
        if (clip == null)
            return;

        StartCoroutine(TriggerSound2DOneShotAfterDelay(clip, volume, pitchVariation, delay));
    }
    private IEnumerator TriggerSound2DOneShotAfterDelay(AudioClip clip, float volume = 1.0f, float pitchVariation = 0f, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        PlaySound2DOneShot(clip, volume, pitchVariation);
    }
}
