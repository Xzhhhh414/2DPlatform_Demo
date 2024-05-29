using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        RecieveHealingIncrease=14
    }

    
    [Serializable]
    public class Property : MonoBehaviour
    {
        private static readonly int MAX_VALUE=9999999;

        [Serializable]
        public class Property2Json
        {
            public int maxHp;
            public int hpRecovery;
            public int attack;
            public int defense;
            public int armorLv;
            public int attackSpeed;
            public int criticalRate;
            public int criticalDmgMulti;
            public int walkSpeed;
            public int airSpeed;
            public int dmgRate;
            public int fragileRate;
            public int healRate;
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
            Property2Json characterData = JsonUtility.FromJson<Property2Json>(jsonData);
            this._maxHp = characterData.maxHp;
            this._hpRecovery = characterData.hpRecovery;
            this._attack = characterData.attack;
            this._defense = characterData.defense;
            this._armorLv = characterData.armorLv;
            this._attackSpeed = characterData.attackSpeed/100.0f;
            this._criticalRate = characterData.criticalRate;
            this._criticalDmg = characterData.criticalDmgMulti;
            this._speed = characterData.walkSpeed/100.0f;
            this._airSpeed = characterData.airSpeed/100.0f;
            this._dmgRate = characterData.dmgRate;
            this._fragileRate = characterData.fragileRate;
            this._healRate = characterData.healRate;
            this._currentHp = MaxHp;
        }
        //Initialize by Inspector
        public void Initialize()
        {
            this._currentHp = MaxHp;
        }

        #region Hp
        [SerializeField]
        private int _currentHp;
        public int CurrentHp
        {
            get
            {
                return _currentHp < 0 ? 0 : _currentHp > MAX_VALUE?MAX_VALUE:_currentHp;
            }
        }

        public void AddCurrentHp(int add)
        {
            _currentHp += add;
            OnHpChange();
        }
        
        [SerializeField]
        private int _maxHp;
        private int _maxHpAdd;
        //缩进两位
        private int _maxHpRate;
        private static readonly int _minMaxHp=1;
        public int MaxHp
        {
            get
            {
                int rst=(int)Mathf.Round((_maxHp + _maxHpAdd) * (1.0f+_maxHpRate/100.0f));
                return rst < _minMaxHp ? _minMaxHp : (rst > MAX_VALUE?MAX_VALUE:rst);
            }
        }

        public void AddMaxHp(int add)
        {
            _maxHpAdd+=add;
            OnHpChange();
        }

        public void AddMaxHpRate(int add)
        {
            _maxHpRate += add;
            OnHpChange();
        }

        public virtual void OnHpChange()
        {
            if (MaxHp < CurrentHp)
            {
                _currentHp = MaxHp;
            }
        }
        
        private int _hpRecovery;
        private int _hpRecoveryAdd;
        //缩进4位
        private int _hpRecoveryRate;
        private static readonly int _minHpRecovery=1;
        public int HpRecovery
        {
            get
            {
                int rst=(int)Mathf.Round((_hpRecovery + _hpRecoveryAdd) * (1.0f+_hpRecoveryRate/10000.0f));
                return rst < _minHpRecovery ? _minHpRecovery : (rst > MAX_VALUE?MAX_VALUE:rst);
            }
        }
        public void AddHpRecovery(int add)
        {
            _hpRecoveryAdd+=add;
        }

        public void AddHpRecoveryRate(int add)
        {
            _hpRecoveryRate += add;
        }
        #endregion
        #region Defense
        [SerializeField]
        private int _defense;
        private int _defenseAdd;
        //缩进4位
        private int _defenseRate;
        public int Defense
        {
            get
            {
                int rst=(int)Mathf.Round((_defense + _defenseAdd) * (1.0f+_defenseRate/10000.0f));
                return rst > MAX_VALUE?MAX_VALUE:rst;
            }
        }
        public void AddDefense(int add)
        {
            _defenseAdd+=add;
        }

        public void AddDefenseRate(int add)
        {
            _defenseRate += add;
        }
        #endregion

        #region Attack
        [SerializeField]
        private int _attack;
        private int _attackAdd;
        //缩进2位
        private int _attackRate;
        private static readonly int _minAttack=1;
        public int Attack
        {
            get
            {
                int rst=(int)Mathf.Round((_attack + _attackAdd) * (1.0f+_attackRate/10000.0f));
                return rst < _minAttack ? _minAttack : (rst > MAX_VALUE?MAX_VALUE:rst);
            }
        }
        public void AddAttack(int add)
        {
            _attackAdd+=add;
        }

        public void AddAttackRate(int add)
        {
            _attackRate += add;
        }

        #endregion


        #region ArmorLevel
        [SerializeField]
        private int _armorLv;
        private int _armorLvAdd;
        private static readonly int _minArmorLv=0;
        public int ArmorLv
        {
            get
            {
                int rst=(int)Mathf.Round(_armorLv + _armorLvAdd);
                return rst < _minArmorLv ? _minArmorLv : (rst > MAX_VALUE?MAX_VALUE:rst);
            }
        }
        public void AddArmor(int add)
        {
            _armorLvAdd+=add;
        }
        
        #endregion
        
        #region AttackSpeedRate
        //[SerializeField]
        private float _attackSpeed;
        //backward 4 demical bits
        private int _attackSpeedRate=10000;
        private static readonly float _minAttackSpeed=0.01f;
        public float AttackSpeed
        {
            get
            {
                float rst=_attackSpeed*(_attackSpeedRate/10000.0f);
                return rst < _minAttackSpeed ? _minAttackSpeed : (rst > MAX_VALUE?MAX_VALUE:rst);
            }
        }
        public void MultiAttackSpeed(int multi)
        {
            if (multi <= 0) return;
            _attackSpeedRate=(int)Mathf.Round(multi/10000.0f*_attackSpeedRate);
        }
        
        #endregion
        
        #region Crit
        //[SerializeField]
        private int _criticalRate;
        //backward 4 demical bits
        private int _criticalAdd;
        private static readonly int MinCriticalRate=0;
        public float CriticalRate
        {
            get
            {
                float rst=(_criticalRate+_criticalAdd)/10000.0f;
                return rst < MinCriticalRate ? MinCriticalRate : (rst > MAX_VALUE?MAX_VALUE:rst);
            }
        }
        public void AddCriticalRate(int add)
        {
            _criticalAdd+=add;
        }
        //[SerializeField]
        private int _criticalDmg;
        //backward 4 demical bits
        private int _criticalDmgAdd;
        private static readonly int MinCriticalDmg=0;
        public float CriticalDmgMulti
        {
            get
            {
                float rst=(_criticalDmg+_criticalDmgAdd)/10000.0f;
                return rst < MinCriticalDmg ? MinCriticalDmg : (rst > MAX_VALUE?MAX_VALUE:rst);
            }
        }
        public void AddCriticalDmgMulti(int add)
        {
            _criticalDmgAdd+=add;
        }
        #endregion
        
        #region Speed
        //[SerializeField]
        private float _speed;
        //backward 4 demical bits
        private int _speedRate=10000;
        private static readonly float MinSpeed=0.01f;
        public float LandSpeed
        {
            get
            {
                float rst=_speed*(_speedRate/10000.0f);
                return rst < MinSpeed ? MinSpeed : (rst > MAX_VALUE?MAX_VALUE:rst);
            }
        }
        public void MultiSpeed(int multi)
        {
            _speedRate = (int)Mathf.Round(_speedRate * (multi / 10000.0f));
        }
        //[SerializeField]
        private float _airSpeed;
        //backward 4 demical bits
        public float AirSpeed
        {
            get
            {
                float rst=_airSpeed*(_speedRate/10000.0f);
                return rst < MinSpeed ? MinSpeed : (rst > MAX_VALUE?MAX_VALUE:rst);
            }
        }
        
        #endregion

        #region jump
        [SerializeField]
        private int _maxAirJumps;
        private int _airJumpsAdd;
        private static readonly int MinAirJumps = 0;

        public int MaxAirJumps
        {
            get
            {
                int rst=_maxAirJumps+_airJumpsAdd;
                return rst < MinAirJumps ? MinAirJumps : (rst > MAX_VALUE?MAX_VALUE:rst);
            }
        }

        public void AddAirJumps(int add)
        {
            _maxAirJumps += add;
        }
        #endregion
        
        #region DmgRate
        //[SerializeField]
        private int _dmgRate=10000;
        //backward 4 demical bits
        private int _dmgRateMulti=10000;
        private static readonly float MinDmgRate=0.01f;
        public float DamageRate
        {
            get
            {
                float rst=(_dmgRate/10000.0f)*(_dmgRateMulti/10000.0f);
                return rst < MinDmgRate ? MinDmgRate : (rst > MAX_VALUE?MAX_VALUE:rst);
            }
        }
        public void MultiDmgRate(int multi)
        {
            _dmgRateMulti = (int)Mathf.Round(_dmgRateMulti * (multi / 10000.0f));
        }
        
        
        //[SerializeField]
        private int _fragileRate=10000;
        //backward 4 demical bits
        private int _fragileRateMulti=10000;
        private static readonly float MinFragileRate=0.01f;
        public float FragileRate
        {
            get
            {
                float rst=_fragileRate/10000.0f*(_fragileRateMulti/10000.0f);
                return rst < MinFragileRate ? MinFragileRate : (rst > MAX_VALUE?MAX_VALUE:rst);
            }
        }
        public void MultiFragileRate(int multi)
        {
            _fragileRateMulti = (int)Mathf.Round(_fragileRateMulti * (multi / 10000.0f));
        }
        
        
        
        //[SerializeField]
        private int _healRate=10000;
        //backward 4 demical bits
        private int _healRateMulti=10000;
        private static readonly float MinHealRate=0.01f;
        public float HealRate
        {
            get
            {
                float rst=_healRate/10000.0f*(_healRateMulti/10000.0f);
                return rst < MinHealRate ? MinHealRate : (rst > MAX_VALUE?MAX_VALUE:rst);
            }
        }
        public void MultiHealRate(int multi)
        {
            _healRateMulti = (int)Mathf.Round(_healRateMulti * (multi / 10000.0f));
        }
        
        #endregion

    }