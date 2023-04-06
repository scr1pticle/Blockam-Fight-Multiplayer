using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : NetworkBehaviour
{
    public static Player localPlayer;
    
    public GameObject crown;
    private Health health;
    SpriteRenderer spriteRenderer;

    public NetworkVariable<Color> color = new(readPerm:NetworkVariableReadPermission.Everyone, writePerm:NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> roundWinner = new();

    Collider2D lava;

    public NetworkVariable<bool> isVisible = new();

    public override void OnNetworkSpawn()
    {
        if (IsLocalPlayer)
            localPlayer = this;
        
        if(IsServer)
            color.Value = Random.ColorHSV(0, 1, 0.7f, 0.7f, 1, 1);
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();
        spriteRenderer.color = color.Value;
        transform.name = NetworkObjectId.ToString();

        isVisible.Value = true;
        roundWinner.OnValueChanged += (old, current) =>
        {
            crown.SetActive(current);
        };

        isVisible.OnValueChanged += (old, current) =>
        {
            gameObject.SetActive(current);
            print(this.name + " current is " + current);
        };
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        lava = other.transform.GetComponent<Collider2D>();
        if (other.gameObject.CompareTag("Lava") && IsServer)
        {
            //health.hp.Value -= 10;
            Invoke(nameof(LavaDamage), 3f);
        }

        if(other.gameObject.CompareTag("Bullet") && IsServer)
        {
            health.hp.Value -= other.transform.GetComponent<Bullet>().damage;
            other.transform.GetComponent<NetworkObject>().Despawn(true);
        }

        if (other.gameObject.CompareTag("Boundry") && IsServer)
        {
            transform.position += Vector3.one;
            print("Boundry");
            health.hp.Value = 0;
        }
            
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Lava") && IsServer)
        {
            CancelInvoke(nameof(LavaDamage));
        }
    }

    private void LavaDamage()
    {
        if (GetComponent<Collider2D>().IsTouching(lava))
        {
            health.hp.Value -= 10;
            Invoke(nameof(LavaDamage), 1.5f);
        }
    }
    public void Die()
    {
        PlayerManager.inst.PlayerDied(this);
    }

    public void GetDamage(int dmg)
    {
        health.hp.Value -= dmg;
    }
}
