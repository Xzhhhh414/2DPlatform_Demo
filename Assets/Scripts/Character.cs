using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField, Label("被击飞的系数")]
    protected float KnockBackRate = 1f;

    protected Rigidbody2D rb;

    protected virtual void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y) * KnockBackRate;
    }
}
