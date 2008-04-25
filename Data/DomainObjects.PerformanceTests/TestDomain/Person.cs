using System;

namespace Remotion.Data.DomainObjects.PerformanceTests.TestDomain
{
  [Instantiable]
  [DBTable]
  public abstract class Person: ClientBoundBaseClass
  {
    public static Person NewObject()
    {
      return NewObject<Person>().With();
    }

    protected Person()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string FirstName { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string LastName { get; set; }
  }
}