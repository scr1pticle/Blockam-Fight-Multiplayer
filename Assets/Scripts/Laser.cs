using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Laser : NetworkBehaviour
{
    public float lifetime = 0.2f;

    private void Start()
    {
        Invoke(nameof(SelfDestruct), lifetime);
    }
    private void SelfDestruct()
    {
        if(IsServer)
            GetComponent<NetworkObject>().Despawn();
    }
}
