using System;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip startSound, enterSound, confirmSound, backSound, gameOverSound, placeSound, errorSound;

    void Awake()
    {
        Instance = this;
    }

    public void PlayStartSound() { audioSource.PlayOneShot(startSound); }

    public void PlayEnterSound() { audioSource.PlayOneShot(enterSound); }

    public void PlayConfirmSound() { audioSource.PlayOneShot(confirmSound); }

    public void PlayBackSound() { audioSource.PlayOneShot(backSound); }

    public void PlayGameOverSound() { audioSource.PlayOneShot(gameOverSound); }

    public void PlayPlaceSound() { audioSource.PlayOneShot(placeSound); }

    public void PlayErrorSound() { audioSource.PlayOneShot(errorSound); }
}
