using System;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Interception.SampleTypes
{
  public class DOWithIndirectlySealedPropertyAccessors : DOWithVirtualProperties
  {
    public sealed override int PropertyWithGetterAndSetter
    {
      get { return base.PropertyWithGetterAndSetter; }
      set { base.PropertyWithGetterAndSetter = value; }
    }
  }
}