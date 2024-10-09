using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace NodeCanvas.Tasks.Conditions
{

    [Category("✫ XDT_Condition")]
    public class TownSpecialTimestamp : ConditionTask
    {

        [BlackboardOnly]
        public BBParameter<int> currentDate;
        public BBParameter<int> currentHour;
        public CompareMethod checkType = CompareMethod.EqualTo;
        public BBParameter<int> targetYear;
        public BBParameter<int> targetMonth;
        public BBParameter<int> targetDay;
        public BBParameter<int> targetHour;


        //[SliderField(0, 0.1f)]
        //public float differenceThreshold = 0.05f;

        protected override string info
        {
            get { return "小镇特殊时间戳" + OperationTools.GetCompareString(checkType) + targetYear + "年" + targetMonth + "月" + targetDay + "日" + targetHour + "点"; }
            
        }

        protected override bool OnCheck()
        {
            //Debug.Log("currentDate.value====" + currentDate.value);
            //Debug.Log("currentHour.value====" + currentHour.value);
            //Debug.Log("targetYear.value====" + targetYear.value);
            // Debug.Log("targetMonth.value====" + targetMonth.value);
            //Debug.Log("targetDay.value====" + targetDay.value);
            //Debug.Log("targetHour.value====" + targetHour.value);
            int current = (int)currentDate.value * 100 + (int)currentHour.value;
            int target = (int)targetYear.value * 1000000 + (int)targetMonth.value * 10000 + (int)targetDay.value * 100 + (int)targetHour.value;
            //Debug.Log("current====" + current);
            //Debug.Log("target====" + target);

            //return current < target;
            return OperationTools.Compare(current, target, checkType);
        }
    }
}