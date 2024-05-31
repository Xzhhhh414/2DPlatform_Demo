using System;
using System.Collections;
using System.Collections.Generic;
using PropertyModification.SPs;
using UnityEngine;

namespace PropertyModification.SPs
{
    public class Attack : SingleProperty,IAdd<int>,IRate<int>,IGet<int>
    {
      
        public int Get()
        {
            return (this as IGet<int>).Auth((int)Mathf.Round(this._base*(_rate/10000.0f)),1,99999);
        }
   
        public override void Initialize()
        {
            _base = iniNum;
            _rate = 10000;
        }
   
        public void Add(int add)
        {
            _base += add;
        }

        public void AddRate(int add)
        {
            _rate += add;
        }
    }
}
