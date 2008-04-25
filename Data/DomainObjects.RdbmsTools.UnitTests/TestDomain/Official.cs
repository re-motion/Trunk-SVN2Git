using System;
using System.Reflection;

namespace Remotion.Data.DomainObjects.RdbmsTools.UnitTests.TestDomain
{
  [DBTable]
  [SecondStorageGroupAttribute]
  [Instantiable]
  public abstract class Official : DomainObject
  {
    public static Official NewObject()
    {
      return NewObject<Official>().With();
    }

    public static Official GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Official> (id);
    }

    protected Official()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    public abstract OrderPriority ResponsibleForOrderPriority { get; set; }

    public abstract CustomerType ResponsibleForCustomerType { get; set; }

    [DBBidirectionalRelation ("Official")]
    public abstract ObjectList<Order> Orders { get; }
  }
}