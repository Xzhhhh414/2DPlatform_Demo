using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField, Label("被击飞时的击退倍率")]
    protected float KnockBackRate = 1f;

    protected Rigidbody2D rb;

    protected virtual void OnHit(int damage, Vector2 knockback, int knockbackLevel, int armorLevel)
    {
        if (knockbackLevel >= armorLevel)
        {
            rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y) * KnockBackRate;
            Debug.Log(rb.velocity);
        }
    }
}
