

namespace PropertyModification.SPs
{
   public class CurrentHp : SingleProperty,IAdd<int>,IGet<int>
   {
      
      public int Get()
      {
         return (this as IGet<int>).Auth(this._base,0,99999);
      }
   
      public override void Initialize()
      {
         _base = iniNum;
      }
   
      public void Add(int add)
      {
         _base += add;
      }
   }
}

