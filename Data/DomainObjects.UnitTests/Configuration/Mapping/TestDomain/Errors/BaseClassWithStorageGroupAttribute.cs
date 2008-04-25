using System;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  [DBStorageGroup]
  public abstract class BaseClassWithStorageGroupAttribute: DomainObject
  {
    protected BaseClassWithStorageGroupAttribute ()
    {
    }
  }
}