using System;

namespace Remotion.Data.DomainObjects.UnitTests.Interception.SampleTypes
{
  public abstract class DOWithAbstractStorageClassNoneProperties : DomainObject
  {
    [StorageClassNone]
    public abstract int PropertyWithGetterAndSetter { get; set; }
  }
}