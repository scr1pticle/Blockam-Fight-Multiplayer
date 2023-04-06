using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Chat : NetworkBehaviour
{
    public Transform layout;
    public List<Sprite> emojis;
    public Image popup;
    
    Transform target;
    
    void Start()
    {
        foreach (var e in emojis)
        {
            //create game object
            var go = new GameObject(e.name);
            
            //add image and set sprite
            var image = go.AddComponent<Image>();
            image.sprite = e;
            
            //set size to 50x50
            image.rectTransform.sizeDelta = new Vector2(50, 50);
            
            //add button and subscribe to click event
            var button = go.AddComponent<Button>();
            button.onClick.AddListener(() =>
            {
                if (Player.localPlayer != null)
                {
                    DistributeChatServerRpc(emojis.IndexOf(e), PlayerManager.inst.players.IndexOf(Player.localPlayer));
                }
            });
            
            //add to canvas
            go.transform.SetParent(layout);
        }
        
        layout.gameObject.SetActive(false);
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            layout.gameObject.SetActive(!layout.gameObject.activeSelf);
        }

        if (target != null)
        {
            //get target pos and convert to screen
            var pos = Camera.main.WorldToScreenPoint(target.position);
            popup.transform.position = pos;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void DistributeChatServerRpc(int imageId, int id)
    {
        RecieveChatClientRpc(imageId, id);
    }

    [ClientRpc]
    void RecieveChatClientRpc(int imageId, int id)
    {
        var player = PlayerManager.inst.players[id];
        
        if(player == null)
            print("No player");
        
        var pos = player.transform.position;
        
        layout.gameObject.SetActive(false);
        
        //convert position to screen point
        pos = Camera.main.WorldToScreenPoint(pos);
        
        popup.transform.position = pos;
        target = player.transform;
        
        //set sprite
        popup.sprite = emojis[imageId];
        
        //show popup
        popup.gameObject.SetActive(true);
        
        //hide popup after 2 seconds
        Invoke(nameof(HidePopup), 2);
    }

    void HidePopup()
    {
        popup.gameObject.SetActive(false);
        target = null;
    }
}
