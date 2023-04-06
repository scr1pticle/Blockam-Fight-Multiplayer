using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GunSpawner : NetworkBehaviour
{
    public float waitTime = 3f;
    public float delay = 5;
    public Vector2 bounds;

    public List<Gun> guns;

    private float accumulatedWeights;

    System.Random rand = new System.Random();

    private void Awake()
    {
        CalculateWights();
    }
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            if (IsServer) enabled = false;
            Invoke(nameof(StartSpawner), delay);
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            CancelInvoke();
        };
        DontDestroyOnLoad(gameObject);
    }

    private void StartSpawner()
    {
        StartCoroutine(nameof(RandomSpawn));
    }
    IEnumerator RandomSpawn()
    {
        while (true)
        {
            NetworkObject gunPb = guns[GetRandomGunIndex()].GetComponent<NetworkObject>();
            Instantiate(gunPb, new Vector2(UnityEngine.Random.Range(bounds.x, bounds.y), transform.position.y), Quaternion.identity).Spawn(true);
            yield return new WaitForSeconds(delay);
        }
    }
    private int GetRandomGunIndex()
    {
        double r = rand.NextDouble() * accumulatedWeights;

        for (int i = 0; i < guns.Count; i++)
        {
            print(r + " " + guns[i].weight);
            if (guns[i].weight >= r)
                return i;
        }
        return 0;
    }

    private void CalculateWights()
    {
        accumulatedWeights = 0f;
        foreach (Gun item in guns)
        {
            accumulatedWeights += item.chance;
            item.weight = accumulatedWeights;
        }
    }
}
