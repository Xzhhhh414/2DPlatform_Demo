using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public enum Coherence
{
    Stack,
    Override,
}
[Serializable]
public enum Exit
{
    Unstack,
    Clear,
}
public class Buff
{
    public string id;
    public string sortID;
    protected bool isFromItem;
    protected int stacknum;
    protected int maxStack;
    public string typeID;
    protected Coherence coherence;
    protected int lastTimeBase;
    protected int lastTimeStack;
    protected Exit exitMethod;
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
    
    protected float lastTime
    {
        get
        {
            return lastTimeBase+stack*lastTimeStack;
        }
    }
    

    public Buff(BuffSO buff)
    {
        id=buff.id;
        sortID=buff.sortID;
        isFromItem=buff.isFromItem;
        stacknum=buff.stacknum;
        maxStack=buff.maxStack;
        typeID=buff.typeID;
        coherence=buff.coherence;
        lastTimeBase=buff.lastTimeBase;
        lastTimeStack=buff.lastTimeStack;
        exitMethod=buff.exitMethod;
        isDeathClear=buff.isDeathClear;
    }

    public void Initialize(){
        stack=stacknum;
        _timestamp= Time.time+lastTime;

    }

    public bool Stack(Buff buff)
    {
        switch (coherence)
        {
            case Coherence.Stack:
            {
                stack= Mathf.Min(stack+buff.StackNum,maxStack);
                _timestamp = Time.time+lastTime;
                break;
            }
            case Coherence.Override:
            {
                return buff.StackNum >= this.stack;
            }
        }

        return false;
    }

    public bool OnExit()
    {
        switch(exitMethod){
            case Exit.Unstack:
            {
                this.stack--;
                this._timestamp=Time.time+lastTime;
                return false;
            }
            case Exit.Clear:
            {
                return true;
            }
        }
        return false;
    }
    
    
    
}
public static class BuffParser{

    public static List<int> Check(List<string> param){
        var rst=new List<int>();
        switch(param[0]){
            case "100":
            case "105":{
                for(int i=1;i<=5;i++){
                    if(!int.TryParse(param[i],out int varInt))
                        rst.Add(i);
                }
                break;
            }
            case "101":
            case "102":
            case "103":
            case "104":{
                for(int i=1;i<=6;i++){
                    if(!int.TryParse(param[i],out int varInt))
                        rst.Add(i);
                }
                break;
            }
            case "200":{
                if(!System.Enum.TryParse(typeof( PropertyType ), param[1],out object varRst))
                    rst.Add(1);
                for(int i=2;i<=4;i++){
                    if(!int.TryParse(param[i],out int varInt))
                        rst.Add(i);
                }
                break;
            }
            case "300":{
                for(int i=1;i<=3;i++){
                    if(!int.TryParse(param[i],out int varInt))
                        rst.Add(i);
                }
                break;
            }
            case "400":
            case "401":{
                for(int i=1;i<=4;i++){
                    if(!int.TryParse(param[i],out int varInt))
                        rst.Add(i);
                }
                break;
            }
            default:{
                rst.Add(0);
                break;
            }
        }

        return rst;
    }
}