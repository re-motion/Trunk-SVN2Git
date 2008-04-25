using System;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_AbstractBaseClassWithHierarchy")]
  [TableInheritanceTestDomain]
  public abstract class AbstractBaseClassWithHierarchy : DomainObject
  {
    protected AbstractBaseClassWithHierarchy ()
    {
    }

    public static AbstractBaseClassWithHierarchy GetObject (ObjectID id)
    {
      return DomainObject.GetObject<AbstractBaseClassWithHierarchy> (id);
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("ChildAbstractBaseClassesWithHierarchy")]
    public abstract AbstractBaseClassWithHierarchy ParentAbstractBaseClassWithHierarchy { get; set; }

    [DBBidirectionalRelation ("ParentAbstractBaseClassWithHierarchy", SortExpression = "Name DESC")]
    public abstract ObjectList<AbstractBaseClassWithHierarchy> ChildAbstractBaseClassesWithHierarchy { get;}

    public abstract Client ClientFromAbstractBaseClass { get; set; }

    public abstract FileSystemItem FileSystemItemFromAbstractBaseClass { get; set; }
  }
}