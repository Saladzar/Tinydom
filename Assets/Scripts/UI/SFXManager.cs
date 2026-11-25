using System;
using Unity.VisualScripting;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [SerializeField] private AudioSource sfxSource, musicSource;
    [SerializeField] private AudioClip startSound, enterSound, confirmSound, backSound, gameOverSound, placeSound, errorSound;
    [SerializeField] private AudioClip mainMenuMusic, gameMusic, combatMusic, deathMusic;

    void Awake()
    {
        Instance = this;
    }

    public void PlayStartSound() { sfxSource.PlayOneShot(startSound); }

    public void PlayEnterSound() { sfxSource.PlayOneShot(enterSound); }

    public void PlayConfirmSound() { sfxSource.PlayOneShot(confirmSound); }

    public void PlayBackSound() { sfxSource.PlayOneShot(backSound); }

    public void PlayGameOverSound() { sfxSource.PlayOneShot(gameOverSound); }

    public void PlayPlaceSound() { sfxSource.PlayOneShot(placeSound); }

    public void PlayErrorSound() { sfxSource.PlayOneShot(errorSound); }

    public void PlayMainMenuMusic()
    {
        musicSource.Stop();
        musicSource.clip = mainMenuMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayGameMusic()
    {
        musicSource.Stop();
        musicSource.clip = gameMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayCombatMusic()
    {
        musicSource.Stop();
        musicSource.clip = combatMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlayDeathMusic()
    {
        musicSource.Stop();
        musicSource.clip = deathMusic;
        musicSource.loop = false;
        musicSource.Play();
    }
}
