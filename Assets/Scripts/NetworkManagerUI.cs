using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NetworkManagerUI : NetworkBehaviour
{
    public TMP_InputField ip;
    public TMP_InputField port;
    public GameObject host;
    public GameObject join;
    public GameObject disconnect;
    public UnityEvent shuttingDown;
    public void Host()
    {
        NetworkManager.Singleton.StartHost();
        /*host.SetActive(false);
        join.SetActive(false);
        ip.gameObject.SetActive(false);
        port.gameObject.SetActive(false);
        disconnect.SetActive(true);
        disconnect.GetComponent<Button>().onClick.AddListener(StopServer);*/
    } 

    private void Start()
    {
        GetPublicIp();
        disconnect.SetActive(false);
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if (NetworkManager.Singleton.LocalClientId != id) return;
            host.SetActive(false);
            join.SetActive(false);
            ip.gameObject.SetActive(false);
            port.gameObject.SetActive(false);
            disconnect.SetActive(true);
            disconnect.GetComponent<Button>().onClick.AddListener(Disconnect);
        };
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            if (!IsHost) return;
            host.SetActive(false);
            join.SetActive(false);
            ip.gameObject.SetActive(false);
            port.gameObject.SetActive(false);
            disconnect.SetActive(true);
            disconnect.GetComponent<Button>().onClick.AddListener(StopServer);
        };
    }
    public void Join()
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip.text, ushort.Parse(port.text));
        NetworkManager.Singleton.StartClient();
    }

    public void Disconnect()
    {
        DisconnectServerRpc(NetworkManager.LocalClientId);
        host.SetActive(true);
        join.SetActive(true);
        ip.gameObject.SetActive(true);
        port.gameObject.SetActive(true);
        disconnect.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DisconnectServerRpc(ulong id)
    {
        NetworkManager.Singleton.DisconnectClient(id);
    }

    public void StopServer()
    {
        shuttingDown.Invoke();
        NetworkManager.Singleton.Shutdown();
        host.SetActive(true);
        join.SetActive(true);
        ip.gameObject.SetActive(true);
        port.gameObject.SetActive(true);
        disconnect.SetActive(false);
        
    }

    public void GetPublicIp()
    {
        StartCoroutine(GetPublicIpCoroutine());
    }

    IEnumerator GetPublicIpCoroutine()
    {
        var www = new WWW("https://api.my-ip.io/ip");
        yield return www;
        ip.text = www.text;
    }
}
