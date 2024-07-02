using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BuffManager : MonoSingleton<BuffManager>
{
   public List<BuffSO> buffList=new List<BuffSO>();
   [HideInInspector]
   public Dictionary<string, Buff> Buffs;



   private void Awake()
   {
      Initialize();
   }

   private void FixedUpdate() {
      
   }

   public static void Initialize()
   {
      Instance.Buffs = new Dictionary<string, Buff>();
      foreach(var buff in Instance.buffList)
      {
         Instance.Buffs.TryAdd(buff.id,new Buff(buff));
      }
   }

   public static bool Get(string id,out Buff rst)
   {
      if(Instance.Buffs is null)
         Initialize();
      return Instance.Buffs.TryGetValue(id,out rst);
   }
}
