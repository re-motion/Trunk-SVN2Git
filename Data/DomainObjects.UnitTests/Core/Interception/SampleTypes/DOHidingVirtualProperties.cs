using System;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Interception.SampleTypes
{
  [Serializable]
  public class DOHidingVirtualProperties : DOWithVirtualProperties
  {
    [DBColumn ("NewPropertyWithGetterAndSetter")]
    public new virtual int PropertyWithGetterAndSetter
    {
      get { return CurrentProperty.GetValue<int> (); }
      set { CurrentProperty.SetValue (value); }
    }
  }
}