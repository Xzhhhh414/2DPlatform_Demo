using System.Collections;
using System.Collections.Generic;
using PropertyModification.SPs;
using UnityEngine;

namespace PropertyModification.SPs
{
    public class CriticalDamage : SingleProperty,IAdd<int>,IGet<int>
    {
      
        public int Get()
        {
            return (this as IGet<int>).Auth(this._base,0,999999);
        }
   
        public override void Initialize()
        {
            _base = iniNum;
        }
   
        public void Add(int add)
        {
            _base += add;
        }
        
    }
}
