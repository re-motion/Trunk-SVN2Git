using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.Core.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Serialization
{
  [TestFixture]
  public class DataContainerTest : ClientTransactionBaseTest
  {
    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "Type 'Remotion.Data.DomainObjects.DataContainer' in Assembly "
        + ".* is not marked as serializable.", MatchType = MessageMatch.Regex)]
    public void DataContainerIsNotSerializable ()
    {
      ObjectID objectID = new ObjectID ("Customer", Guid.NewGuid ());
      DataContainer dataContainer = DataContainer.CreateNew (objectID);

      Serializer.SerializeAndDeserialize (dataContainer);
    }

    [Test]
    public void DataContainerIsFlattenedSerializable ()
    {
      ObjectID objectID = new ObjectID ("Customer", Guid.NewGuid ());
      DataContainer dataContainer = DataContainer.CreateNew (objectID);
      DataContainer deserializedDataContainer = FlattenedSerializer.SerializeAndDeserialize (dataContainer);
      Assert.AreNotSame (dataContainer, deserializedDataContainer);
      Assert.AreEqual (dataContainer.ID, deserializedDataContainer.ID);
    }

    [Test]
    public void DataContainer_Contents ()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);

      Computer computer = employee.Computer;
      computer.SerialNumber = "abc";

      DataContainer dataContainer = computer.InternalDataContainer;
      DataContainer deserializedDataContainer = FlattenedSerializer.SerializeAndDeserialize (dataContainer);

      Assert.AreEqual (dataContainer.ID, deserializedDataContainer.ID);
      Assert.AreSame (ClientTransactionMock, deserializedDataContainer.ClientTransaction);
      Assert.AreEqual (dataContainer.Timestamp, deserializedDataContainer.Timestamp);
      Assert.AreSame (dataContainer.DomainObject, deserializedDataContainer.DomainObject);
      Assert.AreEqual (StateType.Changed, deserializedDataContainer.State);
      Assert.AreEqual ("abc", deserializedDataContainer.PropertyValues[ReflectionUtility.GetPropertyName (typeof (Computer), "SerialNumber")].Value);
      Assert.AreEqual (employee.ID, deserializedDataContainer.PropertyValues[ReflectionUtility.GetPropertyName (typeof (Computer), "Employee")].Value);
    }

    [Test]
    public void DataContainer_MarkAsChanged_Contents()
    {
      Employee employee = Employee.GetObject (DomainObjectIDs.Employee3);

      Computer computer = employee.Computer;
      computer.MarkAsChanged ();

      DataContainer dataContainer = computer.InternalDataContainer;
      DataContainer deserializedDataContainer = FlattenedSerializer.SerializeAndDeserialize (dataContainer);

      Assert.AreEqual (dataContainer.ID, deserializedDataContainer.ID);
      Assert.AreEqual (StateType.Changed, deserializedDataContainer.State);
    }

    [Test]
    public void DataContainer_WithoutProperties_Contents ()
    {
      ObjectID objectID = new ObjectID (typeof (ClassWithoutProperties), Guid.NewGuid ());
      DataContainer dataContainer = DataContainer.CreateNew (objectID);
      DataContainer deserializedDataContainer = FlattenedSerializer.SerializeAndDeserialize (dataContainer);

      Assert.AreEqual (dataContainer.ID, deserializedDataContainer.ID);
      Assert.IsEmpty (deserializedDataContainer.PropertyValues);
    }

    [Test]
    public void DataContainer_Discarded_Contents ()
    {
      Computer computer = Computer.NewObject ();
      DataContainer dataContainer = computer.InternalDataContainer;
      computer.Delete ();
      Assert.IsTrue (dataContainer.IsDiscarded);

      DataContainer deserializedDataContainer = FlattenedSerializer.SerializeAndDeserialize (dataContainer);
      Assert.IsTrue (deserializedDataContainer.IsDiscarded);
      Assert.AreEqual (StateType.Discarded, deserializedDataContainer.State);
    }

    [Test]
    public void DataContainer_EventHandlers_Contents ()
    {
      Computer computer = Computer.NewObject ();

      DataContainer dataContainer = computer.InternalDataContainer;
      PropertyValueContainerEventReceiver eventReceiver = new PropertyValueContainerEventReceiver (dataContainer, false);

      DataContainer deserializedDataContainer = FlattenedSerializer.SerializeAndDeserialize (dataContainer);

      Assert.IsNull (eventReceiver.ChangingNewValue);
      Assert.IsNull (eventReceiver.ChangedNewValue);
      deserializedDataContainer.PropertyValues[ReflectionUtility.GetPropertyName (typeof (Computer), "SerialNumber")].Value = "1234";
      Assert.IsNotNull (eventReceiver.ChangingNewValue);
      Assert.IsNotNull (eventReceiver.ChangedNewValue);
    }
  }
}