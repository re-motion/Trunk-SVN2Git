using System;

namespace Remotion.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_Person")]
  [DBTable ("TableInheritance_Person")]
  [Instantiable]
  public abstract class Person: DomainBase
  {
    public static Person NewObject ()
    {
      return NewObject<Person> ().With ();
    }

    public static Person GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Person> (id);
    }

    protected Person()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string FirstName { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string LastName { get; set; }

    public abstract DateTime DateOfBirth { get; set; }

    [DBBidirectionalRelation ("Person")]
    public abstract Address Address { get; }

    [BinaryProperty]
    public abstract byte[] Photo { get; set; }
  }
}