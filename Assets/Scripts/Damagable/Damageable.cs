using System;
using System.Collections;
using System.Collections.Generic;
using PropertyModification.SPs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using SO;
public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2, int, int> damageableHit;
    public UnityEvent damagebleDeath;
    public UnityEvent<int, int> healthChanged;
   
    [SerializeField,Label("属性")]
    protected PropertySO so;
    protected Property prop;
    
    protected Animator animator;
    protected SpriteRenderer sprite;
    protected Color originalColorOfSprite;

    protected Coroutine coroutine;
    
    
    [SerializeField]
    protected bool _isAlive = true;
    [SerializeField]
    protected bool _isInvincible;
    public virtual bool IsAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            if (!value)
            {
                damagebleDeath.Invoke();
            }
        }
    }


    public virtual bool IsInvincible
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
    

    protected virtual void Initialize()
    {
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        prop ??= new Property();
        prop.Initialize(so);
        prop.Add(PropertyType.CurrentHP.ToString(), MaxHp - CurrentHp);
        originalColorOfSprite = sprite.color;
    }
    
    protected virtual void EarlyProcess()
    {
        
    }
    

    protected virtual void Process()
    {
    }
    
    private void Awake()
    {
        Initialize();
    }
    private void Update()
    {
        Process();
    }
    
    private void FixedUpdate()
    {
        EarlyProcess();
    }

    ///<summary>
    ///碰撞箱检测=>判断受击
    ///</summary>

    public virtual bool Hit(int damage, Vector2 knockback, int knockbackLevel, GameObject hitEffect,
        Vector3 hitPosition)
    {
        return false;
    }
 
    public bool Heal(int healthRestore)
    {
        if (IsAlive && CurrentHp < MaxHp)
        {
            int maxHeal = Mathf.Max(MaxHp - CurrentHp, 0);
            int actualHeal = Mathf.Min(maxHeal, healthRestore);
            prop.Add(PropertyType.CurrentHP.ToString(),actualHeal);

            //CharacterEvents.characterHealed(gameObject, actualHeal);
            EventManager.Instance.TriggerEvent<GameObject, int>(CustomEventType.CharacterHealed, gameObject, actualHeal);

            return true;
        }
        return false;
    }
    [SerializeField, Label("爆点特效位置")]
    protected GameObject[] hitEffectPosArray;
    protected virtual void PlayHitEffect(GameObject hitEffect, Vector3 hitPosition)
    {
        if (hitEffect != null)
        {
            Vector3 nearestHitEffectPos = GetNearestHitEffectPos(hitPosition);
            Instantiate(hitEffect, nearestHitEffectPos, Quaternion.identity);
        }

    }
    
    
    protected Vector3 GetNearestHitEffectPos(Vector3 hitPosition)
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

   

    
    
    public int MaxHp
    {
        get
        {
            if (prop is not null && prop.Get(PropertyType.MaxHP.ToString(), out int rst))
            {
                return rst;
            }

            return 1;
        }
    }
    
    public int CurrentHp
    {
        get
        {
            if (prop is not null && prop.Get(PropertyType.CurrentHP.ToString(), out int rst))
            {
                return rst;
            }

            return 0;
        }
    }
    
    public float WalkSpeed
    {
        get
        {
            if (prop is not null && prop.Get(PropertyType.WalkSpeed.ToString(), out float rst))
            {
                return rst;
            }

            return 0;
        }
    }
    
    public float AirSpeed
    {
        get
        {
            if (prop is not null && prop.Get(PropertyType.AirSpeed.ToString(), out float rst))
            {
                return rst;
            }

            return 0;
        }
    }
    
    public int MaxAirJumps
    {
        get
        {
            if (prop is not null && prop.Get(PropertyType.MaxAirJumps.ToString(), out int rst))
            {
                return rst;
            }

            return 0;
        }
    }
    
    public int Defense
    {
        get
        {
            if (prop is not null && prop.Get(PropertyType.Defense.ToString(), out int rst))
            {
                return rst;
            }

            return 0;
        }
    }
    
    public int ArmorLv
    {
        get
        {
            if (prop is not null && prop.Get(PropertyType.ArmorLv.ToString(), out int rst))
            {
                return rst;
            }

            return 0;
        }
    }
    
    public int Attack
    {
        get
        {
            if (prop is not null && prop.Get(PropertyType.Attack.ToString(), out int rst))
            {
                return rst;
            }

            return 0;
        }
    }


}
