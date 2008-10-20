using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Interception.TestDomain
{
  [DBTable]
  internal class DirectlyInstantiableDO : DomainObject
  {
    protected override void PerformConstructorCheck ()
    {
    }
  }
}