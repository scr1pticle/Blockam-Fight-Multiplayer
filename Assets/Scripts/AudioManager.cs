using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AudioManager : NetworkBehaviour
{
    public static AudioManager inst;

    private void Awake()
    {
        if(inst == null)
        {
            inst = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private AudioSource audio;
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    public void PlayOneShot(AudioClip clip)
    {
        audio.PlayOneShot(clip);
    }
}
