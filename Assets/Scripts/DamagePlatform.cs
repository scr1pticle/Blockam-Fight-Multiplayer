using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using DG.Tweening;

public class DamagePlatform : NetworkBehaviour
{
    public Vector2 bounds;
    private Vector3 _target;
    public float speed = 5f;

    private void Start()
    {
        _target = new Vector3(transform.position.x, bounds.x, transform.position.z);
        transform.DOMoveY(bounds.x, speed).SetLoops(-1, LoopType.Yoyo);
    }
    void Update()
    {
        /*if(transform.position != _target)
        {
            transform.position = Vector3.Lerp(transform.position, _target, speed);
        }

        if(transform.position.y == _target.y && _target.y == bounds.x)
        {
            _target = new Vector3(transform.position.x, bounds.y, transform.position.z);
        }
        else if(transform.position.y == _target.y && _target.y == bounds.y)
        {
            _target = new Vector3(transform.position.x, bounds.x, transform.position.z);
        }*/
        
    }
}
