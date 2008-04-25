using System;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  [DBStorageGroup]
  public abstract class Derived2ClassWithStorageGroupAttribute : Derived1ClassWithStorageGroupAttribute
  {
    protected Derived2ClassWithStorageGroupAttribute ()
    {
    }
  }
}