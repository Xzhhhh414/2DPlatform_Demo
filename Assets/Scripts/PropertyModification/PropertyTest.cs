using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class PropertyTest : MonoBehaviour
{
    private TextMeshProUGUI text;
    public Property.Property2Json p2j=new Property.Property2Json();

    private Property prop;
    
    // Start is called before the first frame update
    void Start()
    {
        text = this.GetComponent<TextMeshProUGUI>();
        prop = this.GetComponent<Property>();
        prop.Initialize(".\\Assets\\jsonInfo.json");
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "CurrentHp:" + prop.CurrentHp.ToString() + '\n' +
               "MaxHp:" + prop.MaxHp.ToString() + '\n' +
               "HpRecovery:" + prop.HpRecovery.ToString() + '\n' +
               "Attack:" + prop.Attack.ToString() + '\n' +
               "Defense:" + prop.Defense.ToString() + '\n' +
               "ArmorLv:" + prop.ArmorLv.ToString() + '\n' +
               "LandSpeed:" + prop.LandSpeed.ToString("f4") + '\n' +
               "AirSpeed:" + prop.AirSpeed.ToString("f4") + '\n' +
               "AttackSpeed:" + prop.AttackSpeed.ToString("f4") + '\n' +
               "CriticalRate:" + prop.CriticalRate.ToString("f4") + '\n' +
               "DamageRate:" + prop.DamageRate.ToString("f4") + '\n' +
               "FragileRate:" + prop.FragileRate.ToString("f4") + '\n' +
               "HealRate:" + prop.HealRate.ToString("f4") + '\n' +
               "CriticalDmgMulti:" + prop.CriticalDmgMulti.ToString("f4") + '\n' +
               "MaxAirJumps:" + prop.MaxAirJumps.ToString() + '\n';
    }

    public void Save()
    {
        string js = JsonUtility.ToJson(p2j);
        //获取到项目路径
        string fileUrl =  ".\\Assets\\jsonInfo.json";
        //打开或者新建文档
        using (StreamWriter sw =new StreamWriter(fileUrl))
        {
            //保存数据
            sw.WriteLine(js);
            //关闭文档
            sw.Close();
            sw.Dispose();
        }
    }
}
