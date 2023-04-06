using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public void TakeDamage(int hp)
    {
        var x = Mathf.Clamp(hp / 100f, 0, 1);
        transform.localScale = new Vector3(x, 1, 1);
        
    }
}
