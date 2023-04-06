using ParrelSync.NonCore;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Meteor : NetworkBehaviour
{
    public List<NetworkObject> explosionParticles;
    public float speed = 10;
    public GameObject explosion;
    public int damage = 20;
    public AudioClip boom;

    Rigidbody2D rb;

    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.down * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GetComponent<Rigidbody2D>().simulated = false;
        if (collision.transform.CompareTag("Player") && IsServer)
            collision.transform.GetComponent<Health>().hp.Value -= damage;

        if(explosion != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }

        if (IsServer)
        {
            foreach (var particle in explosionParticles)
            {
                var position = transform.position + Random.Range(-1f, 1f) * (Vector3.right + Vector3.up);
                var p = Instantiate(particle, position, Quaternion.identity);
                p.Spawn(true);
                var direction = Random.Range(-1f, 1f) * Vector2.right + Vector2.up;
                p.GetComponent<Rigidbody2D>().AddForce(direction * 10, ForceMode2D.Impulse);
            }

            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraShake>().Shake();
            GetComponent<NetworkObject>().Despawn(true);
        }
    }
}
