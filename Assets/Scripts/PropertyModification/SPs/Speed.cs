using System;
using System.Collections;
using System.Collections.Generic;
using PropertyModification.SPs;
using UnityEngine;

namespace PropertyModification.SPs
{
    [Serializable]
    public class Speed : SingleProperty,IMulti<int>,IGet<float>
    {
        public float Get()
        {
            return (this as IGet<float>).Auth((this._base/100.0f)*(_rate/10000.0f),0,1000.0f);
        }
   
        public override void Initialize()
        {
            _base = iniNum;
            _rate = 10000;
        }

        public void Multi(int add)
        {
            _rate = (int)Mathf.Round(this._rate / 10000.0f * (add / 10000.0f));
        }
    }
}
