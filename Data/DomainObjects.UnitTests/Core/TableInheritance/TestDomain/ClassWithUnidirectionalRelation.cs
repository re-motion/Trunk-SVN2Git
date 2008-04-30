using System;

namespace Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain
{
  [ClassID ("TI_ClassWithUnidirectionalRelation")]
  [DBTable ("TableInheritance_TableWithUnidirectionalRelation")]
  [Instantiable]
  [TableInheritanceTestDomain]
  public abstract class ClassWithUnidirectionalRelation : DomainObject
  {
    public static ClassWithUnidirectionalRelation NewObject ()
    {
      return NewObject<ClassWithUnidirectionalRelation> ().With();
    }

    public static ClassWithUnidirectionalRelation GetObject (ObjectID id)
    {
      return GetObject<ClassWithUnidirectionalRelation> (id);
    }

    protected ClassWithUnidirectionalRelation ()
    {
    }

    public abstract DomainBase DomainBase { get; set; }
  }
}