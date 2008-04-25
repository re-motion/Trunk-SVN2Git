using System;

namespace Remotion.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_DerivedClassWithEntityWithHierarchy")]
  [DBTable ("TableInheritance_DerivedClassWithEntityWithHierarchy")]
  [Instantiable]
  public abstract class DerivedClassWithEntityWithHierarchy: AbstractBaseClassWithHierarchy
  {
    public static DerivedClassWithEntityWithHierarchy NewObject ()
    {
      return NewObject<DerivedClassWithEntityWithHierarchy> ().With ();
    }

    public new static DerivedClassWithEntityWithHierarchy GetObject (ObjectID id)
    {
      return DomainObject.GetObject<DerivedClassWithEntityWithHierarchy> (id);
    }

    protected DerivedClassWithEntityWithHierarchy ()
    {
    }

    [DBBidirectionalRelation ("ChildDerivedClassesWithEntityWithHierarchy")]
    public abstract DerivedClassWithEntityWithHierarchy ParentDerivedClassWithEntityWithHierarchy { get; set; }

    [DBBidirectionalRelation ("ParentDerivedClassWithEntityWithHierarchy", SortExpression = "Name ASC")]
    public abstract ObjectList<DerivedClassWithEntityWithHierarchy> ChildDerivedClassesWithEntityWithHierarchy { get; }

    public abstract Client ClientFromDerivedClassWithEntity { get; set; }

    public abstract FileSystemItem FileSystemItemFromDerivedClassWithEntity { get; set; }
  }
}