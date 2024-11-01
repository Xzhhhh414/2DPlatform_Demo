using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Conditions
{

    [Category("✫ XDT_Condition")]
    public class CurClockTime : ConditionTask
    {

        [BlackboardOnly]
        public BBParameter<int> currentHour;
        public BBParameter<int> currentMin;
        public CompareMethod checkType = CompareMethod.EqualTo;
        public BBParameter<int> targetHour;
        public BBParameter<int> targetMinUnit;


        //[SliderField(0, 0.1f)]
        //public float differenceThreshold = 0.05f;

        protected override string info
        {
            get { return "当前小时分钟" + OperationTools.GetCompareString(checkType) + targetHour + "点" + (targetMinUnit.value * 10) + "分"; }
            
        }

        protected override bool OnCheck()
        {

            return OperationTools.Compare((int)currentHour.value*100 + (int)currentMin.value, (int)targetHour.value * 100 + (int)targetMinUnit.value * 10, checkType);

            // 比较当前小时和目标小时
            //bool hourMatches = OperationTools.Compare((int)currentHour.value, (int)targetHour.value, checkType, 0);
            // 比较当前分钟单位和目标分钟单位*10
            //bool minMatches = OperationTools.Compare((int)currentMin.value , (int)targetMinUnit.value , checkType, 0);
            // 只有两个条件都满足时返回 true
            //return hourMatches && minMatches;
        }
    }
}