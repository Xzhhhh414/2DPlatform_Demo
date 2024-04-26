using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2> damageableHit;
    public UnityEvent damagebleDeath;
    public UnityEvent<int, int> healthChanged;

    Animator animator;
    SpriteRenderer sprite;
    Color originalColorOfSprite;

    Coroutine coroutine;

    [SerializeField]
    private int _maxHealth = 100;
    [SerializeField, Label("ÊÜ»÷ÊÇ·ñÓÐ»÷ÍË")]
    private bool isLockVelocityOnHit = true;
    public int MaxHealth
    {
        get
        {
            return _maxHealth;
        }
        set
        {
            _maxHealth = value;
        }
    }

    [SerializeField]
    private int _health = 100;
    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            healthChanged?.Invoke(_health, MaxHealth);

            if (_health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    [SerializeField]
    private bool _isAlive = true;
    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log("IsAlive set " + value);

            if (value == false)
            {
                damagebleDeath.Invoke();
            }
        }
    }

    [SerializeField]
    private bool hitInterval = false;

    private float timeSinceHit = 0;
    public float hitIntervalTime = 0.25f;

    public bool IsInvincible
    {
        get
        {
            return animator.GetBool(AnimationStrings.isInvincible);
        }
    }

    public bool LockVelocity
    {
        get
        {
            return animator.GetBool(AnimationStrings.lockVelocity);
        }
        set
        {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }
    }

    public bool IsBlocking
    {
        get
        {
            return animator.GetBool(AnimationStrings.isBlocking);
        }
    }




    private void Awake()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        originalColorOfSprite = sprite.color;
    }


    private void Update()
    {

        if (hitInterval)
        {
            if (timeSinceHit > hitIntervalTime)
            {
                hitInterval = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }
        Debug.Log(LockVelocity);

    }

    public bool Hit(int damage, Vector2 knockback)
    {
        if (IsAlive && !hitInterval && !IsBlocking && !IsInvincible)
        {
            Health -= damage;
            hitInterval = true;

            animator.SetTrigger(AnimationStrings.hitTrigger);
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(ChangeColorTemp(sprite, originalColorOfSprite, Color.red));
            //LockVelocity = true;
            damageableHit?.Invoke(damage, knockback);
            CharacterEvents.characterDamaged.Invoke(gameObject, damage);



            return true;
        }
        if (IsBlocking)
        {
            animator.SetTrigger(AnimationStrings.skill01CounterAtk);
            return false;
        }

        return false;
    }

    public bool Heal(int healthRestore)
    {
        if (IsAlive && Health < MaxHealth)
        {
            int maxHeal = Mathf.Max(MaxHealth - Health, 0);
            int actualHeal = Mathf.Min(maxHeal, healthRestore);
            Health += actualHeal;

            CharacterEvents.characterHealed(gameObject, actualHeal);

            return true;
        }
        return false;
    }

    IEnumerator ChangeColorTemp(SpriteRenderer sprite, Color oriColor, Color newColor)
    {

        sprite.color = newColor;

        yield return new WaitForSeconds(0.7f);

        sprite.color = oriColor;
    }
}
