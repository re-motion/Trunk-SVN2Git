using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Computer : TestDomainBase
  {
    public static Computer NewObject ()
    {
      return NewObject<Computer> ().With();
    }

    public new static Computer GetObject (ObjectID id)
    {
      return DomainObject.GetObject<Computer> (id);
    }

    protected Computer ()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 20)]
    public abstract string SerialNumber { get; set; }

    [DBBidirectionalRelation ("Computer", ContainsForeignKey = true)]
    public abstract Employee Employee { get; set; }
  }
}