using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("BGM")]
    public AudioSource bgmSource;
    public List<AudioClip> musics;
    [Header("Effects")]
    public AudioSource effectSource;
    public List<AudioClip> hittingSounds;
    public AudioClip shootingSound;
    public AudioClip teleposrtSound;
    public AudioClip waringSound;
    /*     public AudioClip reflectSound; */


    private static AudioManager instance;
    public static AudioManager Instance => instance;
    private void Awake()
    {
        if (instance == null) instance = this;
    }
    private int currentMusicIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (musics != null && musics.Count > 0)
        {
            PlayNextMusic();
        }
    }

    private void PlayNextMusic()
    {
        if (musics.Count == 0) return;
        bgmSource.clip = musics[currentMusicIndex];
        bgmSource.Play();
        Invoke(nameof(HandleMusicEnd), bgmSource.clip.length);
    }

    private void HandleMusicEnd()
    {
        currentMusicIndex = (currentMusicIndex + 1) % musics.Count;
        PlayNextMusic();
    }

    public void PlayHittingSound()
    {
        if (hittingSounds != null && hittingSounds.Count > 0)
        {
            int rand = Random.Range(0, hittingSounds.Count);
            effectSource.PlayOneShot(hittingSounds[rand]);
        }
    }
    public void PlayShootingSound()
    {
        if (shootingSound != null)
        {
            effectSource.PlayOneShot(shootingSound);
        }
    }
    public void PlayTelepostSound()
    {
        if (teleposrtSound != null)
        {
            effectSource.PlayOneShot(teleposrtSound);
        }
    }
    public void PlayWarningSound()
    {
        if (waringSound != null)
        {
            effectSource.PlayOneShot(waringSound, 1f);
        }
    }
    /*     public void PlayReflectSound()
        {
            if (reflectSound != null)
            {
                effectSource.PlayOneShot(reflectSound);
            }
        } */
}
