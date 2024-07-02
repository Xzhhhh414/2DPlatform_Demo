using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;


   [CreateAssetMenu(fileName = "PropertySO",menuName = "SO/Property")]
   public class PropertySO : SerializedScriptableObject
   {
      
      [DictionaryDrawerSettings()]
      [ShowInInspector]
      public Dictionary<PropertyType,int> prop=new Dictionary<PropertyType, int>(){
         {PropertyType.MaxHP,1}
      };

   }

