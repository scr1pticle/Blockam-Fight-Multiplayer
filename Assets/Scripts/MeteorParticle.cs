using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MeteorParticle : NetworkBehaviour
{
    public int damage = 10;
    public float despawnTime = 7;
    private float time;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player") && IsServer)
        {
            collision.transform.GetComponent<Health>().hp.Value -= damage;
            GetComponent<NetworkObject>().Despawn(true);
        }
            
    }

    private void Start()
    {
        time = Time.time;
    }
    private void Update()
    {
        if(Time.time >= time + despawnTime && IsServer)
            GetComponent<NetworkObject>().Despawn(true);
    }
}
