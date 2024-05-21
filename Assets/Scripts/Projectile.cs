using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10;
    public Vector2 movespeed = new Vector2(3f, 0);
    public Vector2 knockback = Vector2.zero;

    private GameObject hitEffect;
    private Vector3 hitPosition;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = new Vector2(movespeed.x * transform.localScale.x, movespeed.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null)
        {
            Vector2 deliveredKnockback = transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

            //Vector2 deliveredKnockback = new Vector2(0, 0);
            bool gotHit = damageable.Hit(damage, deliveredKnockback, hitEffect, hitPosition);

            if (gotHit)
                Debug.Log(collision.name + "hit for" + damage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }

    }
}
