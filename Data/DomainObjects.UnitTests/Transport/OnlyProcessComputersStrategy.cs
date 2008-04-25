using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Transport
{
  public class OnlyProcessComputersStrategy : IGraphTraversalStrategy
  {
    public bool ShouldProcessObject (DomainObject domainObject)
    {
      return domainObject is Computer;
    }

    public bool ShouldFollowLink (DomainObject root, DomainObject currentObject, int currentDepth, PropertyAccessor linkProperty)
    {
      return true;
    }
  }
}