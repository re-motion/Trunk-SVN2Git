using System;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Interception.SampleTypes
{
  public abstract class DOWithAbstractStorageClassNoneProperties : DomainObject
  {
    [StorageClassNone]
    public abstract int PropertyWithGetterAndSetter { get; set; }
  }
}