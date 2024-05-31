using System;
using UnityEngine;
using SO;

public class Character : MonoBehaviour
{
    //[SerializeField, Label("被击飞时的击退倍率")]
    //protected float KnockBackRate = 1f;

    //protected Rigidbody2D rb;

    //protected virtual void OnHit(int damage, Vector2 knockback, int knockbackLevel, int armorLevel)
    //{
    //    if (knockbackLevel >= armorLevel)
    //    {
    //        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y) * KnockBackRate;
    //        //Debug.Log(rb.velocity);
    //    }
    //}
    [SerializeField,Label("属性")]
    protected PropertySO so;
    [HideInInspector]
    public Property prop;
    
    protected virtual void Initialize()
    {
        
        prop ??= new Property();
        prop.Initialize(so);
       
    }
    private void Awake()
    {
        Initialize();
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

            return 1;
        }
    }
    
    public float Speed
    {
        get
        {
            if (prop is not null && prop.Get(PropertyType.SpeedRate.ToString(), out float rst))
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
