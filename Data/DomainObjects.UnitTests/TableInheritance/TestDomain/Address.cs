using System;

namespace Remotion.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_Address")]
  [DBTable ("TableInheritance_Address")]
  [Instantiable]
  [TableInheritanceTestDomain]
  public abstract class Address : DomainObject
  {
    public static Address NewObject ()
    {
      return NewObject<Address> ().With();
    }

    protected Address ()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Street { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 10)]
    public abstract string Zip { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string City { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Country { get; set; }

    [DBBidirectionalRelation ("Address", ContainsForeignKey = true)]
    [Mandatory]
    public abstract Person Person { get; set; }
  }
}