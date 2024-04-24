using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Attack : MonoBehaviour
{
    public int attackDamage;
    public Vector2 knockback = Vector2.zero;


    public float stunRatio = 0.1f;
    private Animator animator;
    private Dictionary<Damageable, Coroutine> damageableCoroutines = new Dictionary<Damageable, Coroutine>();
    public bool canClearCooldown;
    public UnityEvent ClearCooldown;
    private float lagDuration = 0.2f;//加个计时，命中多个目标只算1次
    private float lagLeftTime;
    private bool hitValid = true;


    private void Start()
    {

        animator = GetComponentInParent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("OnTriggerEnter2D!!!!!");
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable != null)
        {

            Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

            bool gotHit = damageable.Hit(attackDamage, deliveredKnockback);

            if (gotHit)
            {
                if (damageableCoroutines.ContainsKey(damageable) && damageableCoroutines[damageable] != null)
                {
                    StopCoroutine(damageableCoroutines[damageable]);
                }

                damageableCoroutines[damageable] = StartCoroutine(ChangAnimationSpeed(0.01f, stunRatio, animator, collision.GetComponent<Animator>()));
                //Debug.Log(collision.name + "hit for" + attackDamage);

                if (canClearCooldown && hitValid)
                {
                    Debug.Log("ClearCooldown Invoke");
                    hitValid = false;
                    lagLeftTime = lagDuration;
                    ClearCooldown.Invoke();
                }
            }
            else
            {
                // Debug.Log("No Damage");
            }
        }
    }

    IEnumerator ChangAnimationSpeed(float newSpeed, float duration, Animator myAnimator, Animator otherAnimator)
    {
        myAnimator.speed = newSpeed;
        otherAnimator.speed = newSpeed;
        yield return new WaitForSeconds(duration);
        otherAnimator.speed = 1;
        myAnimator.speed = 1;
    }

    private void Update()
    {
        if (!hitValid)
        {
            if (lagLeftTime > 0)
            {
                lagLeftTime -= Time.deltaTime;
            }
            else
            {
                hitValid = true;
                lagLeftTime = lagDuration;
            }
        }

    }

}
