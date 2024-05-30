using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public Vector2 movespeed = new Vector2(3f, 0);
    public Vector2 knockback = Vector2.zero;
    public int knockbackLevel; //³å»÷µÈ¼¶
    int _attackDamage;

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
        if (BattleTestManager.Instance.isMinDamage)
        {
            _attackDamage = 1;
        }
        else
        {
            _attackDamage = damage;
        }

        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null)
        {
            Vector2 deliveredKnockback = transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

            //Vector2 deliveredKnockback = new Vector2(0, 0);
            bool gotHit = damageable.Hit(_attackDamage, deliveredKnockback, knockbackLevel, hitEffect, hitPosition);

            if (gotHit)
            //Debug.Log(collision.name + "hit for" + damage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }

    }
}
