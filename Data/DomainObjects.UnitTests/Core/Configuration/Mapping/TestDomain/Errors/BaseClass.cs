using System;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.TestDomain.Errors
{
  public abstract class BaseClass: DomainObject
  {
    protected BaseClass ()
    {
    }

    public abstract int Int32 { get; set; }
  }
}