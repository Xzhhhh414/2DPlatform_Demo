using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class SingleProperty
{
    
    public int iniNum;
    
    protected int _base;

    protected int _rate;
    
    public abstract void Initialize();

}
