using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    public float lifeTime = 15;
    public float speed = 10;
    public int damage = 10;
    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.right * speed;

        Invoke(nameof(SelfDestruct), lifeTime);
    }

    private void SelfDestruct()
    {
        if(IsServer)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
