using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    //Here add more sounds by placing them inside the Resources/Sounds folder
    public enum Sound
    {
        attack,

    }

    private AudioSource audioSource;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }
    public void PlaySound()
    {
        audioSource.PlayOneShot(Resources.Load<AudioClip>(Sound.attack.ToString()));

    }
}
