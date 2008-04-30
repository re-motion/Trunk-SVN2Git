using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Transport
{
  public class FollowAllProcessNoneStrategy : IGraphTraversalStrategy
  {
    public bool ShouldProcessObject (DomainObject domainObject)
    {
      return false;
    }

    public bool ShouldFollowLink (DomainObject root, DomainObject currentObject, int currentDepth, PropertyAccessor linkProperty)
    {
      return true;
    }
  }
}