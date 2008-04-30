using System;
using System.Runtime.Serialization;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [Instantiable]
  public abstract class Customer : Company, IDeserializationCallback
  {
    [NonSerialized]
    public bool OnDeserializationCalled;
    [NonSerialized]
    public bool OnDeserializingAttributeCalled;
    [NonSerialized]
    public bool OnDeserializedAttributeCalled;

    public enum CustomerType
    {
      Standard = 0,
      Premium = 1,
      Gold = 2
    }

    public new static Customer NewObject ()
    {
      return NewObject<Customer> ().With ();
    }

    public new static Customer GetObject (ObjectID id)
    {
      return GetObject<Customer> (id);
    }

    protected Customer ()
    {
    }

    public abstract DateTime? CustomerSince { get; set; }

    [DBColumn ("CustomerType")]
    public abstract CustomerType Type { get; set; }

    [DBBidirectionalRelation ("Customer", SortExpression = "OrderNo asc")]
    public abstract OrderCollection Orders { get; }

    public void OnDeserialization (object sender)
    {
      OnDeserializationCalled = true;
    }

    [OnDeserialized]
    private void OnDeserializedAttribute (StreamingContext context)
    {
      OnDeserializedAttributeCalled = true;
    }

    [OnDeserializing]
    private void OnOnDeserializingAttribute (StreamingContext context)
    {
      OnDeserializingAttributeCalled = true;
    }
  }
}
