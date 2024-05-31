using System;

namespace PropertyModification.SPs
{
    public interface IGet<T> where T:IComparable
    {
        public abstract T Get();
        public virtual T Auth(T oriNum,T min,T max)
        {
            return (oriNum as IComparable).CompareTo(min) == -1 ? min :
                (oriNum as IComparable).CompareTo(max) == 1 ? max : oriNum;
        }
    }
}