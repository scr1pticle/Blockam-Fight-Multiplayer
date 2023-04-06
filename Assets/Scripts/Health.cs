using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Health : NetworkBehaviour
{
    public int maxHp = 100;

    public UnityEvent<int> onDamage;
    public UnityEvent onDeath;

    public NetworkVariable<int> hp = new();

    private void Start()
    {
        if(IsServer)
            hp.Value = maxHp;

        hp.OnValueChanged += (old, current) =>
        {
            onDamage.Invoke(current);
            
            print($"{transform.name} took {old - current} damage");
            
            if (current <= 0)
            {
                onDeath.Invoke();
                return;
            }
        };
    }
}
