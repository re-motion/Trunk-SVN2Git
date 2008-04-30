using System;

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain
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
      return GetObject<AbstractBaseClassWithHierarchy> (id);
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