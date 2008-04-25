using System;

namespace Remotion.Data.DomainObjects.PerformanceTests.TestDomain
{
  [Instantiable]
  [DBTable]
  public abstract class File : DomainObject
  {
    public static File NewObject()
    {
      return NewObject<File>().With();
    }

    protected File()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Number { get; set; }

    [DBBidirectionalRelation ("Files")]
    [Mandatory]
    public abstract Client Client { get; set; }
  }
}