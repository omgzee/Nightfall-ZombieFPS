using UnityEngine;

public class ZombieSoundController : MonoBehaviour
{
    public AudioClip[] idleSounds;
    public AudioClip[] chaseSounds;
    public AudioClip[] attackSounds;
    public AudioClip screamClip;
    private AudioSource audioSource;
    private float nextIdleTime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ScheduleNextIdle();
    }

    void Update()
    {
        // Play idle sounds occasionally
        if (Time.time >= nextIdleTime && !audioSource.isPlaying)
        {
            PlayIdleSound();
            ScheduleNextIdle();
        }
    }

    void ScheduleNextIdle()
    {
        nextIdleTime = Time.time + Random.Range(3f, 7f); // Play idle sounds every few seconds
    }

    void PlayIdleSound()
    {
        if (idleSounds.Length > 0)
        {
            audioSource.PlayOneShot(idleSounds[Random.Range(0, idleSounds.Length)]);
        }
    }

    public void PlayChaseSound()
    {
        if (chaseSounds.Length > 0)
            audioSource.PlayOneShot(chaseSounds[Random.Range(0, chaseSounds.Length)]);
    }

    public void PlayAttackSound()
    {
        if (attackSounds.Length > 0)
            audioSource.PlayOneShot(attackSounds[Random.Range(0, attackSounds.Length)]);
    }

    public void PlayScreamSound()
    {
        if (audioSource != null && screamClip != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f); // optional variation
            audioSource.PlayOneShot(screamClip);
        }
    }
}
