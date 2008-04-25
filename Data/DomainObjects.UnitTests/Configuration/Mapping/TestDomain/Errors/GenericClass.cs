using System;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  public abstract class GenericClass<T>: DomainObject
  {
    protected GenericClass ()
    {
    }
  }
}