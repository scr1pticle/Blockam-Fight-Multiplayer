using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager inst;

    [HideInInspector]
    public List<Player> players = new();
    
    private int activePlayerCount = 0;
    
    private void Awake()
    {
        if (inst == null)
        {
            inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            enabled = false;
        }
    }

    void Start()
    {
        if (!IsServer)
            enabled = false;

        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            var player = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(id).GetComponent<Player>();
            players.Add(player);
            activePlayerCount++;
        };

        NetworkManager.OnServerStarted += () =>
        {
            NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
        };
        
    }

    void OnSceneEvent(SceneEvent sceneEvent)
    {
        print(sceneEvent.SceneEventType);
        activePlayerCount = players.Count;
        print("PLAYERS: " + activePlayerCount);

        if(sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted)
        {
            var spawnPoint = GameObject.Find("SpawnPoint");

            if(spawnPoint == null)
            {
                Debug.LogError("No spawnpoint found");
                return;
            }

            var pos = spawnPoint.transform.position;

            foreach (var item in players)
            {
                print(item.name);
                item.transform.position = pos;
                print(item.name + " position: " + item.transform.position);
                pos += Vector3.right;

                var health = item.GetComponent<Health>();
                health.hp.Value = health.maxHp;

                item.isVisible.Value = true;
            }
        }
    }

    public void PlayerDied(Player player)
    {
        activePlayerCount--;
        
        players.Remove(player);
        player.isVisible.Value = false;
        print(player.name + " isivisible" + player.isVisible.Value);
        player.roundWinner.Value = false;
        players.Add(player);
        
        if(activePlayerCount == 1)
        {
            players[0].roundWinner.Value = true;

            Invoke(nameof(LoadNextScene), 3);
        }
            
    }

    void LoadNextScene()
    {
        NetworkManager.Singleton.SceneManager.LoadScene("Level" + (SceneManager.GetActiveScene().buildIndex + 1), LoadSceneMode.Single);
    }
    
}
