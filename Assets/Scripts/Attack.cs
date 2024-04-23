using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Events;

public class Attack : MonoBehaviour
{
    public int attackDamage;
    public Vector2 knockback = Vector2.zero;

    public bool canClearCooldown;
    public UnityEvent ClearCooldown;
    public float stunRatio = 0.1f;
    private Animator animator;
    Coroutine coroutine1;
    Coroutine coroutine2;

    private void Start()
    {

        animator = GetComponentInParent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("OnTriggerEnter2D!!!!!");
        Damageable damageable = collision.GetComponent<Damageable>();
        if (coroutine1 != null) StopCoroutine(coroutine1);
        if (coroutine2 != null) StopCoroutine(coroutine2);
        if (damageable != null)
        {
            Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

            bool gotHit = damageable.Hit(attackDamage, deliveredKnockback);

            if (gotHit)
            {

                coroutine1 = StartCoroutine(ChangAnimationSpeed(0.01f, stunRatio, animator));
                coroutine2 = StartCoroutine(ChangAnimationSpeed(0.01f, stunRatio, collision.transform.GetComponent<Animator>()));
                //Debug.Log(collision.name + "hit for" + attackDamage);
                if (canClearCooldown)
                {
                    //Debug.Log("ClearCooldown Invoke");
                    ClearCooldown.Invoke();

                }
            }
            else
            {
                // Debug.Log("No Damage");
            }


        }
    }

    IEnumerator ChangAnimationSpeed(float newSpeed, float duration, Animator animator)
    {
        animator.speed = newSpeed;
        yield return new WaitForSeconds(duration);
        animator.speed = 1;
    }


}
