using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Conditions
{

    [Category("✫ XDT_Condition")]
    public class CurrentWeekday : ConditionTask
    {

        [BlackboardOnly]
        public BBParameter<int> currentDay;
        public CompareMethod checkType = CompareMethod.EqualTo;
        public BBParameter<int> targetDay;

        //[SliderField(0, 0.1f)]
        //public float differenceThreshold = 0.05f;

        protected override string info
        {
            get { return currentDay + OperationTools.GetCompareString(checkType) + targetDay; }
        }

        protected override bool OnCheck()
        {
            return OperationTools.Compare((int)currentDay.value, (int)targetDay.value, checkType);
        }
    }
}