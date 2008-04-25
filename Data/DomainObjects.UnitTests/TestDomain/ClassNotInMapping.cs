using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [DBTable]
  [TestDomain]
  public class ClassNotInMapping : DomainObject
  {
    protected ClassNotInMapping ()
    {
    }
  }
}
