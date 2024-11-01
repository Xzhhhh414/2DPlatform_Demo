using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;


namespace NodeCanvas.Tasks.Conditions
{

    [Category("✫ XDT_Condition")]
    public class DuringPeriod : ConditionTask
    {

        [BlackboardOnly]
        public BBParameter<int> currentPeriod;
        public CompareMethod checkType = CompareMethod.EqualTo;
        public BBParameter<int> targetPeriod;

        //[SliderField(0, 0.1f)]
        //public float differenceThreshold = 0.05f;

        protected override string info
        {
            get { return currentPeriod + OperationTools.GetCompareString(checkType) + targetPeriod; }
        }

        protected override bool OnCheck()
        {
            return OperationTools.Compare((int)currentPeriod.value, (int)targetPeriod.value, checkType);
        }
    }
}