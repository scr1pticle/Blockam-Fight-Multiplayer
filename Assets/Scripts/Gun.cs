using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Gun : NetworkBehaviour
{
    public NetworkVariable<int> ammo = new();
    public int maxAmmo = 10;
    public NetworkObject bulletPb;
    [Range(0f, 100f)]public float chance = 100f;
    [HideInInspector]
    public float weight;
    public string type;
    public int damage;
    public NetworkObject laser;
    public AudioClip shoot;

    private AudioSource _audio;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Transform target;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _audio = GetComponent<AudioSource>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            ammo.Value = maxAmmo;
        else
            rb.simulated = false;
    }

    private void LateUpdate()
    {
        if(target != null && IsServer)
        {
            transform.position = target.position;
        }
    }

    public void Despawn()
    {
        GetComponent<NetworkObject>().Despawn(true);
    }

    public void PlayShootSFX()
    {
        _audio.PlayOneShot(shoot);
    }
}
