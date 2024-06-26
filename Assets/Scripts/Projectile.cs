using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore;
using UnityEngine.UI;

public class Projectile : Damageable
{
    [Serializable]
    public enum FlyTrack
    {
        Line,
        Gravity,
        Aiming
    }
    
    [Serializable,Flags]
    public enum Target
    {
        Wall=1<<0,
        Ground=1<<1,
    }

    public UnityEvent onHitOpposite;
    public UnityEvent onHitGround;
    
    protected Vector2 direction;
    [SerializeField]
    protected FlyTrack track;
    protected int _attackDamage;
    protected Rigidbody2D rb;
    [SerializeField]
    protected float lifeTime;
    [SerializeField,Label("启用渐出")] protected bool faded;
    [SerializeField, Label("渐出延迟")] protected float fadedDelayTime;
    [SerializeField, Label("生效目标")] protected Target target;
    protected float Timer;
    
    [HideInInspector]
    public ProjectileLauncher Launcher;
    [SerializeField,Label("启用限制追踪")]
    public bool constrainedAiming;

    [SerializeField, Label("启用锐角转弯追踪")] public bool acuteAngle;
    public int angleConstraint;
    public int step=5;
    [SerializeField,Label(" 重力影响")]
    public float mass;
    public GameObject hitEffect;
    
    
    private bool facingRight;
    private int stepCnt;
    private AimingZone aimingZone; 
    public Vector2 knockback = Vector2.zero;
    public int knockbackLevel;
    
    public override bool IsAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            if (!value)
            {
                damagebleDeath.Invoke();
            }
        }
    }
    
    public override bool IsInvincible
    {
        get
        {
            return _isInvincible;
        }
    }

    protected bool Face
    {
        get
        {
            return facingRight;
        }
        set
        {
            facingRight = value;
            transform.localScale = new Vector3(facingRight ? 1 : -1,1,1);
        }
    }

    private void OnEnable()
    {
        this.Timer = 0;
        damagebleDeath.AddListener(Death);
    }

    private void OnDisable()
    {
        damagebleDeath.RemoveListener(Death);
    }

    public void Spawn(Vector2 dir,bool face)
    {
        direction = dir;
        this.Timer = 0;
        Face = face;
        prop.Add(PropertyType.CurrentHP.ToString(), MaxHp - CurrentHp);
        stepCnt = 0;
        rb.velocity =(direction.x > 0 ? 1 : -1)*AirSpeed*this.transform.right.normalized;
        IsAlive = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
        rb = GetComponent<Rigidbody2D>();
        aimingZone = this.GetComponentInChildren<AimingZone>();
    }

    protected override void EarlyProcess()
    {
        base.EarlyProcess();
        switch (track)
        {
            case FlyTrack.Line:
            {
                break;
            }
            case FlyTrack.Gravity:
            {
                if (stepCnt == 0)
                {
                    direction=(direction+new Vector2(0, -mass*Time.deltaTime)).normalized;
                    float angle1 = Vector3.SignedAngle((Face ? 1 : -1)*transform.right, direction, Vector3.forward);
                    transform.Rotate(new (0,0,angle1),Space.Self);
                    rb.velocity =(Face ? 1 : -1)*AirSpeed*this.transform.right.normalized;
                }
                break;
            }
            case FlyTrack.Aiming:
            {
                if (stepCnt == 0)
                {
                    if (aimingZone.aim is not null)
                    {
                        Vector2 input = aimingZone.aim.transform.position-transform.position;
                        direction=input.normalized;                
                        float angle1 = Vector3.SignedAngle((Face ? 1 : -1)*transform.right, direction, Vector3.forward);
                        //将夹角坐标和世界坐标的取值和范围对齐
                        if(acuteAngle)angle1 =angle1<0?angle1+360:angle1;
                        if(constrainedAiming)
                            angle1 = Mathf.Abs(angle1) > angleConstraint ? angleConstraint * Mathf.Sign(angle1) : angle1;
                        transform.Rotate(new (0,0,angle1),Space.Self);
                        rb.velocity =AirSpeed*(Face?1:-1)*this.transform.right.normalized;
                    }
                }
                
                break;
            }
        }
        
    }
    public void SelfDamage(int damage)
    {
        if (IsAlive  && !IsInvincible)
        {
            prop.Add(PropertyType.CurrentHP.ToString(),-damage);
            healthChanged?.Invoke(CurrentHp,MaxHp);
            IsAlive = CurrentHp > 0;
        }
    }

    protected override void Process()
    {
        base.Process();
            this.Timer += Time.deltaTime;
            if (faded&&this.Timer > fadedDelayTime)
            {
                float newAlpha = originalColorOfSprite.a * (1-(Timer-fadedDelayTime) / (lifeTime-fadedDelayTime));
                sprite.color = new UnityEngine.Color(originalColorOfSprite.r, originalColorOfSprite.g, originalColorOfSprite.b, newAlpha);
            }
            if (Timer > lifeTime)
            {
                Death();
            }
        
        stepCnt++;
        stepCnt%=step;
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
            _attackDamage = Attack;
        }

        Damageable damageable = collision.GetComponentInParent<Damageable>();
    
        if (damageable != null)
        {
            Vector2 deliveredKnockback = transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);
            
            bool gotHit = damageable.Hit(_attackDamage, deliveredKnockback, knockbackLevel, hitEffect, transform.position);
            onHitOpposite.Invoke();
        }
        if ((target&Target.Ground)!=0&&collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            onHitGround.Invoke();
        }
    }
    
    public void PlayHitEffect(GameObject effect)
    {
        if (effect != null)
        {
            Vector3 nearestHitEffectPos = GetNearestHitEffectPos(this.transform.position);
            Instantiate(effect, nearestHitEffectPos, Quaternion.identity);
        }

    }

    private void Death()
    {
        Launcher.Pool.Release(this);
    }
}
