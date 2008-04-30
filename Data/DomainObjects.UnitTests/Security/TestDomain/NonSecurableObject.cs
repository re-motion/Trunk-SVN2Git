using System;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Security.TestDomain
{
  [Instantiable]
  [DBTable]
  public abstract class NonSecurableObject : DomainObject
  {
    public static NonSecurableObject NewObject (ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterNonDiscardingScope())
      {
        return NewObject<NonSecurableObject>().With();
      }
    }

    protected NonSecurableObject ()
    {
    }

    public DataContainer GetDataContainer ()
    {
      return (DataContainer) PrivateInvoke.InvokeNonPublicMethod (this, typeof (DomainObject), "GetDataContainerForTransaction", ClientTransaction);
    }

    public abstract string StringProperty { get; set; }

    [DBBidirectionalRelation ("Children")]
    public abstract NonSecurableObject Parent { get; set; }

    [DBBidirectionalRelation ("Parent")]
    public abstract ObjectList<NonSecurableObject> Children { get; }
  }
}