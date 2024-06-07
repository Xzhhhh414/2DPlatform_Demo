using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : Damageable
{
    [Serializable]
    enum FlyTrack
    {
        Line,
        Gravity,
        Aiming
    }
    public int damage;
    public Vector2 movespeed = new Vector2(3f, 0);
    public Vector2 knockback = Vector2.zero;
    public int knockbackLevel; //����ȼ�
    int _attackDamage;

    private GameObject hitEffect;
    private Vector3 hitPosition;

    Rigidbody2D rb;

    protected override void Initialize()
    {
        base.Initialize();
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(movespeed.x * transform.localScale.x, movespeed.y);
    }
    

    // Start is called before the first frame update
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

        Damageable damageable = collision.GetComponentInParent<Damageable>();

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
