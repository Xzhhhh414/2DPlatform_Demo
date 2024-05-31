using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PropertyModification.SPs;
using SO;
using UnityEngine;


    
   
[Serializable]
public enum PropertyType
{
    CurrentHP=1,
    MaxHP=2,
    HPRecovery=3,
    Attack=4,
    Defense=5,
    ArmorLv=6,
    AttackSpeedRate=7,
    CriticalRate=8,
    CritDamageMulti=9,
    SpeedRate=10,
    MaxAirJumps=11,
    DamageIncrease=12,
    RecieveDmgIncrease=13,
    RecieveHealingIncrease=14,
    AirSpeed=15
}
   

    
    [Serializable]
    public class Property 
    {
        private Dictionary<String, SingleProperty> _propDictionary=new Dictionary<string, SingleProperty>();

        public Dictionary<String, SingleProperty> PropDictionary
        {
            get
            {
                return this._propDictionary;
            }
        }
        /// <summary>
        /// 试图对属性进行固定加成
        /// </summary>
        /// <param name="name"></param>
        /// <param name="add"></param>
        /// <returns></returns>

        public bool Add(string name,int add)
        {
            if (_propDictionary.TryGetValue(name,out var tmp))
            {
                if (tmp is IAdd<int>)
                {
                    (tmp as IAdd<int>).Add(add);
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// 尝试以对应格式获取属性的值，结果由rst传出，return为成功与否
        /// </summary>
        /// <param name="name"></param>
        /// <param name="rst"></param>
        /// <returns></returns>
        public bool Get(string name,out float rst)
        {
            if (_propDictionary.TryGetValue(name,out SingleProperty tmp))
            {
                if (tmp is IGet<float>)
                {
                    rst = (tmp as IGet<float>).Get();
                    return true;
                }
            }
            rst = 0;
            return false;
        }
        public bool Get(string name,out int rst)
        {
            if (_propDictionary.TryGetValue(name,out SingleProperty tmp))
            {
                if (tmp is IGet<int>)
                {
                    rst = (tmp as IGet<int>).Get();
                    return true;
                }
            }
            rst = 0;
            return false;
        }
        /// <summary>
        /// 试图对属性倍率进行乘法操作
        /// </summary>
        /// <param name="name"></param>
        /// <param name="multi"></param>
        /// <returns></returns>
        public bool Multi(string name, int multi)
        {
            if (_propDictionary.TryGetValue(name,out var tmp))
            {
                if (tmp is IMulti<int>)
                {
                    (tmp as IMulti<int>).Multi(multi);
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// 试图对属性倍率进行加法操作
        /// </summary>
        /// <param name="name"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        public bool Rate(string name, int add)
        {
            if (_propDictionary.TryGetValue(name,out var tmp))
            {
                if (tmp is IRate<int>)
                {
                    (tmp as IRate<int>).AddRate(add);
                    return true;
                }
            }

            return false;
        }
        

        
        
        //Initialize from json
        public void Initialize(String path)
        {
            string jsonData;
            //读取文件
            using (StreamReader sr =File.OpenText(path))
            {
                //数据保存
                jsonData = sr.ReadToEnd();
                sr.Close();
            }
            
        }
        //Initialize from So
        public void Initialize(PropertySO so)
        {
            if (so is null) return;
            for (int i = 0; i < so.propertyName.Count; i++)
            {
                _propDictionary.TryAdd(so.propertyName[i].ToString(), ParseEnum(so.propertyName[i]));
                _propDictionary[so.propertyName[i].ToString()].iniNum = so.propertyParam[i];
            }
            foreach (var property in _propDictionary)
            {
                property.Value.Initialize();
            }
        }
        
        private SingleProperty ParseEnum(PropertyType type)
        {
            switch (type)
            {
                case PropertyType.Attack:
                {
                    return new PropertyModification.SPs.Attack();
                }
                case PropertyType.Defense:
                {
                    return new Defense();
                }
                case PropertyType.ArmorLv:
                {
                    return new ArmorLv();
                }
                case PropertyType.CriticalRate:
                {
                    return new CriticalRate();
                }
                case PropertyType.DamageIncrease:
                {
                    return new DamageIncrease();
                }
                case PropertyType.SpeedRate:
                {
                    return new Speed();
                }
                case PropertyType.AttackSpeedRate:
                {
                    return new AttackSpeed();
                }
                case PropertyType.CritDamageMulti:
                {
                    return new CriticalDamage();
                }
                case PropertyType.CurrentHP:
                {
                    return new CurrentHp();
                }
                case PropertyType.HPRecovery:
                {
                    return new HpRecovery();
                }
                case PropertyType.MaxAirJumps:
                {
                    return new AirJumps();
                }
                case PropertyType.MaxHP:
                {
                    return new MaxHp();
                }
                case PropertyType.RecieveDmgIncrease:
                {
                    return new HurtIncrease();
                }
                case PropertyType.RecieveHealingIncrease:
                {
                    return new HealIncrease();
                }
                case PropertyType.AirSpeed:
                {
                    return new AirSpeed();
                }
            }

            return new CurrentHp();
        }
        

    }
    