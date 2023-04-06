using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Tutorials.Core;
using UnityEditor.EventSystems;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shooter : NetworkBehaviour
{
    private Gun gun;

    private List<Transform> gunsAround = new();

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && gun == null && IsLocalPlayer)
        {
            PickUpServerRpc();
        }

        if (Input.GetMouseButtonDown(0) && IsLocalPlayer)
            ShootServerRpc();

        if(Input.GetKeyDown(KeyCode.Q) && IsLocalPlayer)
            DropServerRpc();

        if(IsLocalPlayer)
            LookServerRpc(Input.mousePosition);
    }

    [ServerRpc]
    void DropServerRpc()
    {
        if(!gun) return;
        if (gun.ammo.Value <= 0) gun.Invoke(nameof(gun.Despawn), 3);
        gun.rb.simulated = true;
        gun.target = null;
        gun.transform.position += gun.transform.right * 1.5f;
        gun.GetComponent<Rigidbody2D>().AddForce(gun.transform.right * 5, ForceMode2D.Impulse);
        gun = null;
    }

    [ServerRpc]
    void ShootServerRpc()
    {
        if (gun == null || gun.ammo.Value <= 0) return;

        if(gun.type == "projectile")
            Instantiate(gun.bulletPb, gun.transform.position + gun.transform.right, gun.transform.rotation).Spawn(true);
        else if(gun.type == "hitscan")
        {
            var hit = Physics2D.Raycast(gun.transform.position + gun.transform.right, gun.transform.right);
            var laser = Instantiate(gun.laser, gun.transform.position + gun.transform.right, gun.transform.rotation);
            laser.Spawn(true);
            if (hit != false && hit.transform.CompareTag("Player"))
            {
                hit.transform.GetComponent<Player>().GetDamage(gun.damage);
            }
        }
        gun.PlayShootSFX();
        gun.ammo.Value--;
    }

    [ServerRpc]
    void LookServerRpc(Vector3 mousePos)
    {
        if (!gun) return;

        var worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.z = 0;

        var lookDir = worldPos - transform.position;

        gun.transform.right = lookDir;
        if (gun.transform.rotation.z > 0.7f  && gun.transform.localScale.y != -1)
            gun.transform.localScale = new Vector3(1, -1, 1);
        else if(gun.transform.rotation.z > -0.7f && gun.transform.rotation.z < 0.7f && gun.transform.localScale.y != 1)
            gun.transform.localScale = new Vector3(1, 1, 1);
    }

    [ServerRpc]
    void PickUpServerRpc()
    {
        if (gunsAround.Count == 0) return;
        var minDistance = float.MaxValue;
        foreach (var item in gunsAround)
        {
            var distance = Vector2.Distance(transform.position, item.position);
            if(distance < minDistance)
            {
                minDistance = distance;
                gun = item.GetComponent<Gun>();
            }
        }
        gun.name = "Picked gun: " + transform.name;
        gun.rb.simulated = false;
        gun.target = transform;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Gun"))
        {
            gunsAround.Add(collision.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Gun"))
        {
            gunsAround.Remove(collision.transform);
        }
    }
}
