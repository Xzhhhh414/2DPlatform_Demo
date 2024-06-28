using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Buff:ScriptableObject
{
    [SerializeField]
    public string id;
    [SerializeField]
    public string sortID;
    [SerializeField,Label("是否来源于道具")]
    protected bool isFromItem;
    [SerializeField,Label("基础Buff层数")]
    protected int stacknum;
    [SerializeField,Label("最大堆叠层数")]
    protected int maxStack;
    [SerializeField,Label("分组ID（0为特殊规则）")]
    public string typeID;

    [Serializable]
    protected enum Coherence
    {
        Stack,
        Override,
    }
    [SerializeField,Label("共存规则")]
    protected Coherence coherence;
    [SerializeField,Label("基础持续时间（-1为无限时间）")]
    protected int lastTimeBase;
    [SerializeField,Label("每层附加持续时间")]
    protected int lastTimeStack;
    [SerializeField,Label("持续时间结束后清除方式")]
    protected Exit exitMethod;
    [Serializable]
    protected enum Exit
    {
        UnStack,
        Clear,
    }
    [SerializeField,Label("死亡后自动清空")]
    protected bool isDeathClear;

    private float _timestamp;
    private int stack;

    protected int StackNum
    {
        get
        {
            int rst = isFromItem ? 1 : stacknum;
            //TODO:道具系统不完善
            return rst;
        }
    }

    public List<PropertyType> types = new List<PropertyType>();
    
    

    public void Initialize()
    {
        
    }

    public bool Stack(Buff buff)
    {
        switch (coherence)
        {
            case Coherence.Stack:
            {
                stack= Mathf.Min(stack+buff.StackNum,maxStack);
                _timestamp = Time.time;
                break;
            }
            case Coherence.Override:
            {
                return buff.StackNum >= this.stack;
            }
        }

        return false;
    }
    
    
    
}
