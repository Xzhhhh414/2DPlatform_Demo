using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffSO",menuName = "SO/Buff")]
public class BuffSO : SerializedScriptableObject
{
    public string id;
    [InfoBox("sortID不正确","CheckSortID",InfoMessageType =InfoMessageType.Error)]
    public string sortID;
    [Label("是否来源于道具")]
    public bool isFromItem;
    [Label("基础Buff层数")]
    public int stacknum;
    [Label("最大堆叠层数")]
    public int maxStack;
    [Label("分组ID（0为特殊规则）")]
    public string typeID;
    [Label("共存规则")]
    public Coherence coherence;
    [Label("基础持续时间（-1为无限时间）")]
    [SuffixLabel("ms", Overlay = true)]
    public int lastTimeBase;
    [HideIf("@this.lastTimeBase==-1")]
    [Label("每层附加持续时间")]
    [SuffixLabel("ms", Overlay = true)]
    public int lastTimeStack;
    [Label("持续时间结束后清除方式")]
    public Exit exitMethod;
    [Label("死亡后自动清空")]
    public bool isDeathClear;
    [Title("功能模块")]
    [HideIf("CheckSortID")]
    [ValidateInput("CheckParam")]
    public List<string> param;

    private bool CheckSortID(){
       var check=new List<string>();
        check.Add(sortID);
        check.AddRange(param);
        var rst=BuffParser.Check(check);
        if (rst.Count>0&&rst[0]==0){
            return true;
        }
        return false;
    }

    private bool CheckParam(List<string> param,ref string errorMessage)
    {
        if (param == null) return true;
        var check=new List<string>();
        check.Add(sortID);
        check.AddRange(param);
        var rst=BuffParser.Check(check);
        errorMessage="";
        if (rst.Count>0){
            if(rst[0]==0)
                errorMessage+= "sortID"+"(\""+check[0]+"\") 没有对应的功能模块";
            else{
                foreach(var err in rst)
                    errorMessage+= "参数"+err+"(\""+check[err]+"\") 不符合要求";//如果设置消息，则默认消息会被覆盖
            }
            return false;
        }

    
        return true;
    }
}

