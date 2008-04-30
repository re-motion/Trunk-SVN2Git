using System;

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain
{
  [ClassID ("TI_DerivedClassWithEntityFromBaseClassWithHierarchy")]
  [Instantiable]
  public abstract class DerivedClassWithEntityFromBaseClassWithHierarchy: DerivedClassWithEntityWithHierarchy
  {
    public new static DerivedClassWithEntityFromBaseClassWithHierarchy NewObject ()
    {
      return NewObject<DerivedClassWithEntityFromBaseClassWithHierarchy> ().With ();
    }

    public new static DerivedClassWithEntityFromBaseClassWithHierarchy GetObject (ObjectID id)
    {
      return GetObject<DerivedClassWithEntityFromBaseClassWithHierarchy> (id);
    }

    protected DerivedClassWithEntityFromBaseClassWithHierarchy ()
    {
    }

    [DBBidirectionalRelation ("ChildDerivedClassesWithEntityFromBaseClassWithHierarchy")]
    public abstract DerivedClassWithEntityFromBaseClassWithHierarchy ParentDerivedClassWithEntityFromBaseClassWithHierarchy { get; set; }

    [DBBidirectionalRelation ("ParentDerivedClassWithEntityFromBaseClassWithHierarchy", SortExpression = "Name ASC")]
    public abstract ObjectList<DerivedClassWithEntityFromBaseClassWithHierarchy> ChildDerivedClassesWithEntityFromBaseClassWithHierarchy { get; }

    public abstract Client ClientFromDerivedClassWithEntityFromBaseClass { get; set; }

    public abstract FileSystemItem FileSystemItemFromDerivedClassWithEntityFromBaseClass { get; set; }
  }
}