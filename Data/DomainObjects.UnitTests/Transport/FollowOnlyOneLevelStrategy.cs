using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.UnitTests.Transport
{
  public class FollowOnlyOneLevelStrategy : IGraphTraversalStrategy
  {
    public bool ShouldProcessObject (DomainObject domainObject)
    {
      return true;
    }

    public bool ShouldFollowLink (DomainObject root, DomainObject currentObject, int currentDepth, PropertyAccessor linkProperty)
    {
      return currentDepth == 0;
    }
  }
}