using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithBothEndPointsOnSameClass: DomainObject
  {
    protected ClassWithBothEndPointsOnSameClass()
    {
    }

    [DBBidirectionalRelation ("Children")]
    public abstract ClassWithBothEndPointsOnSameClass Parent { get; set; }

    [DBBidirectionalRelation ("Parent")]
    public abstract ObjectList<ClassWithBothEndPointsOnSameClass> Children { get; }
  }
}