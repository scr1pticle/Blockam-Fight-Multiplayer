using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class fallingPlatform : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private bool falling = false;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if(!falling)
                StartCoroutine(Fall());
        }
    }

    IEnumerator Fall()
    {
        falling = true;
        for (int i = 0; i < 3; i++)
        {
            sprite.DOColor(Color.blue, 0.2f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Flash);
            yield return new WaitForSeconds(.5f);
        }
        rb.constraints = RigidbodyConstraints2D.None;
        rb.AddForce(Vector3.down);
    }
}
