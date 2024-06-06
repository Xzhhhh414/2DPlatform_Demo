using System;
using PropertyModification.SPs;
using UnityEngine;
using SO;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class Character : MonoBehaviour
{
    [SerializeField, Label("被击飞时的击退倍率")]
    protected float KnockBackRate = 1f;
   
    [SerializeField,Label("属性")]
    protected PropertySO so;
    [HideInInspector]
    protected Property prop;
    
    
    protected BoxCollider2D bCollider;
    protected Animator animator;
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
        walkspeed = Speed;
        airspeed = AirSpeed;
        defense = Defense;
        armorlv = ArmorLv;
    }
#endif
    
    
    
    protected virtual void OnHit(int damage, Vector2 knockback, int knockbackLevel, int armorLevel)
    {
        if (knockbackLevel >= armorLevel)
        {
            rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y) * KnockBackRate;
            //Debug.Log(rb.velocity);
        }
    }
    
    protected virtual void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        prop ??= new Property();
        prop.Initialize(so);
        prop.Add(PropertyType.CurrentHP.ToString(), MaxHp - CurrentHp);
       touchingDirections = GetComponent<TouchingDirections>();
#if UNITY_EDITOR 
        GUIUpdate();
#endif
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
    
    

    public bool Add(PropertyType type,int add)
    {
        var rst= prop is not null&&prop.Add(type.ToString(), add);
#if UNITY_EDITOR    
        if(rst)GUIUpdate();
#endif
        return rst;
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
    
    public float Speed
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
