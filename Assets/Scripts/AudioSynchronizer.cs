using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSynchronizer : MonoBehaviour
{
    AudioSource audioSource;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            audioSource.time = NetworkManager.Singleton.ServerTime.TimeAsFloat;
        };
    }
}
