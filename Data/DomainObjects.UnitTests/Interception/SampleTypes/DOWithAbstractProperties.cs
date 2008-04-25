using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.UnitTests.Interception.SampleTypes
{
  [DBTable]
  [Instantiable]
  public abstract class DOWithAbstractProperties : DomainObject
  {
    public abstract int PropertyWithGetterAndSetter { get; set; }
    public abstract string PropertyWithGetterOnly { get; }
    public abstract DateTime PropertyWithSetterOnly { set; }
    protected abstract int ProtectedProperty { get; set; }

    [StorageClassNone]
    public new PropertyIndexer Properties
    {
      get { return base.Properties; }
    }

    public new string GetAndCheckCurrentPropertyName ()
    {
      return base.GetAndCheckCurrentPropertyName();
    }
  }
}