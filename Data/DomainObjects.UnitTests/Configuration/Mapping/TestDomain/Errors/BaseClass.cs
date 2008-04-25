using System;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  public abstract class BaseClass: DomainObject
  {
    protected BaseClass ()
    {
    }

    public abstract int Int32 { get; set; }
  }
}