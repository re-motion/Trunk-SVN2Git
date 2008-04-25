using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Serialization
{
  [TestFixture]
  public class DomainObjectTest : ClientTransactionBaseTest
  {
    [Test]
    public void Serializable ()
    {
      ClassDerivedFromSimpleDomainObject instance = ClassDerivedFromSimpleDomainObject.NewObject ().With ();
      instance.IntProperty = 6;
      Tuple<ClientTransaction, ClassDerivedFromSimpleDomainObject> deserializedData =
          Serializer.SerializeAndDeserialize (Tuple.NewTuple (ClientTransaction.Current, instance));
      ClassDerivedFromSimpleDomainObject deserializedInstance = deserializedData.B;
      using (deserializedData.A.EnterNonDiscardingScope ())
      {
        Assert.AreNotSame (instance, deserializedInstance);
        Assert.AreEqual (6, deserializedInstance.IntProperty);
      }
    }

    [Test]
    public void Serializable_WithISerializable ()
    {
      ClassWithAllDataTypes instance = ClassWithAllDataTypes.NewObject ();
      instance.Int32Property = 5;
      Tuple<ClientTransaction, ClassWithAllDataTypes> deserializedData =
          Serializer.SerializeAndDeserialize (Tuple.NewTuple (ClientTransaction.Current, instance));
      ClassWithAllDataTypes deserializedInstance = deserializedData.B;
      using (deserializedData.A.EnterNonDiscardingScope ())
      {
        Assert.AreNotSame (instance, deserializedInstance);
        Assert.AreEqual (5, deserializedInstance.Int32Property);
      }
    }

    [Test]
    public void Serializable_WithISerializable_AndBindingTransaction ()
    {
      ClassWithAllDataTypes instance;
      
      using (ClientTransaction.NewBindingTransaction ().EnterNonDiscardingScope ())
      {
        instance = ClassWithAllDataTypes.NewObject();
      }

      instance.Int32Property = 5;
      Tuple<ClientTransaction, ClassWithAllDataTypes> deserializedData =
          Serializer.SerializeAndDeserialize (Tuple.NewTuple (instance.ClientTransaction, instance));
      
      ClassWithAllDataTypes deserializedInstance = deserializedData.B;
      Assert.IsTrue (deserializedInstance.IsBoundToSpecificTransaction);
      Assert.AreSame (deserializedData.A, deserializedInstance.ClientTransaction);
      Assert.AreEqual (5, deserializedInstance.Int32Property);
    }

    [Test]
    public void Serializable_LoadCount ()
    {
      ClassWithAllDataTypes instance = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.IsTrue (instance.OnLoadedHasBeenCalled);
      Assert.AreEqual (LoadMode.WholeDomainObjectInitialized, instance.OnLoadedLoadMode);

      Tuple<ClientTransaction, ClassWithAllDataTypes> deserializedData =
          Serializer.SerializeAndDeserialize (Tuple.NewTuple (ClientTransaction.Current, instance));

      using (deserializedData.A.EnterNonDiscardingScope ())
      {
        ClassWithAllDataTypes deserializedInstance = deserializedData.B;
        Assert.IsFalse (deserializedInstance.OnLoadedHasBeenCalled);
        using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
        {
          ClientTransaction.Current.EnlistDomainObject (deserializedInstance);
          deserializedInstance.Int32Property = 15;
          Assert.IsTrue (deserializedInstance.OnLoadedHasBeenCalled);
          Assert.AreEqual (LoadMode.DataContainerLoadedOnly, deserializedInstance.OnLoadedLoadMode);
        }
      }
    }
  }
}