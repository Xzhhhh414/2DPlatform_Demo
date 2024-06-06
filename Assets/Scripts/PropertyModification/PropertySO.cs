using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SO
{
   [CreateAssetMenu(fileName = "PropertySO",menuName = "SO/Property")]
   public class PropertySO : ScriptableObject
   {
      public List<PropertyType> propertyName=new List<PropertyType>();
      public List<int> propertyParam=new List<int>();
   }

}