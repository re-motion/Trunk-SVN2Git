using Remotion.Data.DomainObjects;

namespace Remotion.Development.UnitTests.Data.UnitTesting.DomainObjects.TestDomain
{
  [Instantiable]
  [DBTable]
  [DBStorageGroup]
  public abstract class SimpleDomainObject:DomainObject
  {
    public abstract int Value { get; set; }
  }
}