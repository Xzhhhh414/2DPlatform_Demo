using System;
using System.Collections;
using PropertyModification.SPs;
using UnityEngine;
using SO;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class Character : Damageable
{
    [SerializeField, Label("被击飞时的击退倍率")]
    protected float KnockBackRate = 1f;
    
    
    protected BoxCollider2D bCollider;
    protected Rigidbody2D rb;
    protected TouchingDirections touchingDirections;

#if UNITY_EDITOR
    [Label("生命值")]
    public int currenthp;
    [Label("最大生命值")]
    public int maxhp;
    [Label("攻击力")]
    public int attack;
    [Label("陆地速度")]
    public float walkspeed;
    [Label("空中速度")]
    public float airspeed;
    [Label("防御")]
    public int defense;
    [Label("护甲等级")]
    public int armorlv;

    protected void GUIUpdate()
    {
        currenthp = CurrentHp;
        maxhp = MaxHp;
        attack = Attack;
        walkspeed = WalkSpeed;
        airspeed = AirSpeed;
        defense = Defense;
        armorlv = ArmorLv;
    }
#endif
    
    
    
    public virtual void OnHit(int damage, Vector2 knockback, int knockbackLevel, int armorLevel)
    {
        if (knockbackLevel >= armorLevel)
        {
            rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y) * KnockBackRate;
            //Debug.Log(rb.velocity);
        }
    }
    
    public override bool Hit(int damage, Vector2 knockback,int knockbackLevel, GameObject hitEffect, Vector3 hitPosition)
    {
        if (IsAlive && !IsBlocking && !IsInvincible)
        {
            //受到伤害
            damage = Mathf.Max(1,damage-Defense);
            
            
            prop.Add(PropertyType.CurrentHP.ToString(),-damage);
            healthChanged?.Invoke(CurrentHp,MaxHp);
            IsAlive = CurrentHp > 0;
            //hitInterval = true;
            if (knockbackLevel >= ArmorLv)
            {
                animator.SetTrigger(AnimationStrings.hitTrigger);
            }
            
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(ChangeColorTemp(sprite, originalColorOfSprite, hurtColor));
            //LockVelocity = true;
            damageableHit?.Invoke(damage, knockback, knockbackLevel, ArmorLv);

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
    protected override void Initialize()
    {
        base.Initialize();
        rb = GetComponent<Rigidbody2D>();
       touchingDirections = GetComponent<TouchingDirections>();
#if UNITY_EDITOR
        changedSpeedRate = 1.0f;
        GUIUpdate();
#endif
    }

    
    protected override void EarlyProcess()
    {
        base.EarlyProcess();
    }
    

    protected override void Process()
    {
        base.Process();
    #if UNITY_EDITOR    
            GUIUpdate();
    #endif
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
    
    

    
    private float changedSpeedRate;
    private bool isListen;
    protected void AnimMultiSpeedRate(float rate)
    {
        changedSpeedRate *= rate;
        prop.Multi(PropertyType.WalkSpeed.ToString(),Mathf.CeilToInt(10000*rate));
        prop.Multi(PropertyType.AirSpeed.ToString(),Mathf.CeilToInt(10000*rate));
        if(!isListen)
            StartCoroutine(ListenToAnimator());
    }

    protected void AnimSetSpeedRate(float rate)
    {
        prop.Multi(PropertyType.WalkSpeed.ToString(),Mathf.CeilToInt(10000*rate/changedSpeedRate));
        prop.Multi(PropertyType.AirSpeed.ToString(),Mathf.CeilToInt(10000*rate/changedSpeedRate));
        changedSpeedRate = rate;
        if(!isListen)
            StartCoroutine(ListenToAnimator());
    }

    IEnumerator ListenToAnimator()
    {
        isListen = true;
        AnimatorStateInfo LastFrameAnim=animator.GetCurrentAnimatorStateInfo(0);
        while (LastFrameAnim.shortNameHash==animator.GetCurrentAnimatorStateInfo(0).shortNameHash&&LastFrameAnim.normalizedTime<1.0f)
        {
            yield return null;
        }

        prop.Multi(PropertyType.WalkSpeed.ToString(), Mathf.CeilToInt(10000 / changedSpeedRate));
        prop.Multi(PropertyType.AirSpeed.ToString(), Mathf.CeilToInt(10000 / changedSpeedRate));
        changedSpeedRate = 1.0f;
        isListen = false;
    }
    
    

}


