using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

[RequireComponent(typeof(Property))]
public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2, int, int> damageableHit;
    public UnityEvent damagebleDeath;
    public UnityEvent<int, int> healthChanged;
    private Property prop;
    
    private Animator animator;
    private SpriteRenderer sprite;
    Color originalColorOfSprite;

    Coroutine coroutine;
    
    public int MaxHealth
    {
        get
        {
            if(prop!=null)
                return prop.MaxHp;
            return 1;
        }
    }
    public int Health
    {
        get
        {
            if(prop!=null)
                return prop.CurrentHp;
            return 0;
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
            //Debug.Log("IsAlive set " + value);
            if (!value)
            {
                damagebleDeath.Invoke();
            }
        }
    }


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
        prop = this.GetComponent<Property>();
        prop.Initialize();
        originalColorOfSprite = sprite.color;
    }

    [SerializeField]
    private bool hitInterval = false;
    public float hitIntervalTime = 0.25f;

    private void Update()
    {

        //if (hitInterval)
        //{
        //    if (timeSinceHit > hitIntervalTime)
        //    {
        //        hitInterval = false;
        //        timeSinceHit = 0;
        //    }

        //    timeSinceHit += Time.deltaTime;
        //}

    }

    ///<summary>
    ///碰撞箱检测=>判断受击
    ///</summary>
    public bool Hit(int damage, Vector2 knockback,int knockbackLevel, GameObject hitEffect, Vector3 hitPosition)
    {
        if (IsAlive && !hitInterval && !IsBlocking && !IsInvincible)
        {
            //受到伤害
            damage -= prop.Defense;
            if (damage <= 1)
            {
                damage = 1;
            } 
            
            prop.AddCurrentHp(-damage);
            healthChanged?.Invoke(Health,MaxHealth);
            IsAlive = prop.CurrentHp > 0;
            //hitInterval = true;
            if (knockbackLevel >= prop.ArmorLv)
            {
                animator.SetTrigger(AnimationStrings.hitTrigger);
            }
            
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(ChangeColorTemp(sprite, originalColorOfSprite, hurtColor));
            //LockVelocity = true;
            damageableHit?.Invoke(damage, knockback, knockbackLevel, prop.ArmorLv);

            EventManager.Instance.TriggerEvent<GameObject, int>(CustomEventType.CharacterDamaged, gameObject, damage);
            PlayHitEffect(hitEffect, hitPosition);

            return true;
        }
        else if (IsBlocking)
        {
            animator.SetTrigger(AnimationStrings.skill01CounterAtk);
        }

        return false;
    }
 
    public bool Heal(int healthRestore)
    {
        if (IsAlive && Health < MaxHealth)
        {
            int maxHeal = Mathf.Max(MaxHealth - Health, 0);
            int actualHeal = Mathf.Min(maxHeal, healthRestore);
            prop.AddCurrentHp(actualHeal);

            //CharacterEvents.characterHealed(gameObject, actualHeal);
            EventManager.Instance.TriggerEvent<GameObject, int>(CustomEventType.CharacterHealed, gameObject, actualHeal);

            return true;
        }
        return false;
    }

    [SerializeField, Label("受击颜色时长")]
    private float changeColorTime = 0.7f;
    [SerializeField, Label("受击颜色")]
    private Color hurtColor = Color.red;
    IEnumerator ChangeColorTemp(SpriteRenderer sprite, Color oriColor, Color newColor)
    {

        sprite.color = newColor;

        yield return new WaitForSeconds(changeColorTime);

        sprite.color = oriColor;
    }


    [SerializeField, Label("爆点特效位置")]
    private GameObject[] hitEffectPosArray;
    private void PlayHitEffect(GameObject hitEffect, Vector3 hitPosition)
    {
        if (hitEffect != null)
        {
            Vector3 nearestHitEffectPos = GetNearestHitEffectPos(hitPosition);
            Instantiate(hitEffect, nearestHitEffectPos, Quaternion.identity);

        }

    }
    private Vector3 GetNearestHitEffectPos(Vector3 hitPosition)
    {
        Vector3 nearestHitEffectPos = Vector3.zero;
        float minDistance = float.MaxValue;
        foreach (GameObject hitEffectPosObj in hitEffectPosArray)
        {
            Vector3 hitEffectPos = hitEffectPosObj.transform.position;
            float distance = Vector3.Distance(hitPosition, hitEffectPos);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestHitEffectPos = hitEffectPos;
            }
        }
        //Debug.Log("nearestHitEffectPos===="+ nearestHitEffectPos);

        if (nearestHitEffectPos == Vector3.zero)
        {
            return hitPosition; //如果没配，就在打击位置播
        }
        else
        {
            return nearestHitEffectPos;
        }

    }

}
