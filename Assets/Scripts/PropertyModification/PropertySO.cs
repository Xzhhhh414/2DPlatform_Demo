using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SO
{
   [CreateAssetMenu(fileName = "PropertySO",menuName = "SO/Property")]
   public class PropertySO : ScriptableObject
   {
      public List<PropertyType> propertyName;
      public List<int> propertyParam;
   }

}