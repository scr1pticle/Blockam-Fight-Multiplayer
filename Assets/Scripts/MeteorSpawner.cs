using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MeteorSpawner : NetworkBehaviour
{
    public float waitTime = 3f;
    public float delay = 5;
    public Vector2 bounds;

    public NetworkObject meteorPb;
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += () => 
        {
            if (IsServer) enabled = false;
            Invoke(nameof(StartSpawner), delay);
        };
        DontDestroyOnLoad(gameObject);
        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            StopAllCoroutines();
        };
    }

    private void Spawn()
    {
        var meteor = Instantiate(meteorPb, transform.position, Quaternion.identity);
        meteor.Spawn(true);
    }

    private void StartSpawner()
    {
        StartCoroutine(nameof(RandomSpawn));
    }

    IEnumerator RandomSpawn()
    {
        while (true)
        {
            var meteor = Instantiate(meteorPb, new Vector2(Random.Range(bounds.x, bounds.y), transform.position.y), Quaternion.identity);
            meteor.Spawn(true);
            yield return new WaitForSeconds(delay);
        }
    }
}
