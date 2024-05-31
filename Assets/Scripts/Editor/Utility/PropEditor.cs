using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using NUnit.Framework;
using PlasticGui;
using PropertyModification.SPs;
using SO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;





[CanEditMultipleObjects,CustomEditor(typeof(PropertySO),true)] //特性：自定义编辑类 ChinarInspector
public class PropEditor : Editor      //需要继承 Editor
{
    private PropertySO prop; // 类对象
    private PropertyType newtype=PropertyType.MaxHP;
    private int toBeDel=-1;

    
    private void OnEnable()
    {
        prop = (PropertySO) target; //获取当前编辑自定义Inspector的对象（官方解释授检查的对象）
    }


    //通过对该方法的重写，可以对 Inspector 面板重新绘制，添加自己自定义GUI
    //既然说的自定义UI，就会覆盖我们关联脚本本身在面板的绘制
    public override void OnInspectorGUI()
    {
        // 更新显示
        this.serializedObject.Update();
        newtype=(PropertyType)EditorGUILayout.EnumPopup("属性类型",newtype);
        if (GUILayout.Button("添加该属性"))
        {
            if(!prop.propertyName.Contains(newtype))
            { prop.propertyName.Add(newtype);
                prop.propertyParam.Add(0);}
        }
        
        //不写任何代码，ChinarInspector的面板上不出现任何属性
        EditorGUILayout.LabelField("属性信息","该角色拥有"+prop.propertyName.Count+"个属性");
        for(int i=0;i<prop.propertyName.Count;i++)
        {
            string text=(ParseEnum(prop.propertyName[i]) is IGet<int>)?"":"（实为所填值0.01%的浮点数）";
            prop.propertyParam[i]= EditorGUILayout.IntField(prop.propertyName[i]+text, prop.propertyParam[i]);
            
            if (GUILayout.Button("删除该属性",GUILayout.Width(100)))
            {
                toBeDel = i;
            }
        }

        if (toBeDel!=-1)
        {
            prop.propertyName.RemoveAt(toBeDel);
            prop.propertyParam.RemoveAt(toBeDel);
            toBeDel = -1;
        }
        
        if (GUILayout.Button("Save"))
        {
            AssetDatabase.SaveAssets();
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
        this.serializedObject.ApplyModifiedProperties();
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

