using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : NetworkBehaviour
{
    public float jumpHeight = 2;
    public float speed = 10;
    
    private Rigidbody2D rb;
    private Vector2 input;
    
    void Start()
    {
        if (!IsLocalPlayer)
            enabled = false;
        
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        //get input
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        var velocity = rb.velocity;
        velocity.x = input.x * speed;
        
        if(velocity.y == 0 && input.y > 0)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * Physics2D.gravity.y);
        
        //move
        rb.velocity = velocity;
    }
}
