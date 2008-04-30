using System;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Interception.SampleTypes
{
  [Instantiable]
  public abstract class DOImplementingAbstractPropertyAccessors : DOWithAbstractProperties
  {
    private int _value;

    public override int PropertyWithGetterAndSetter
    {
      get { return _value; }
      set { _value = value + 7; }
    }
  }
}