using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Core.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.Core.Resources;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Core.DataManagement
{
  [TestFixture]
  public class DataContainerTest : ClientTransactionBaseTest
  {
    private PropertyDefinition _nameDefinition;
    private PropertyValue _nameProperty;
    private DataContainer _newDataContainer;
    private DataContainer _existingDataContainer;

    public override void SetUp ()
    {
      base.SetUp ();

      Guid idValue = Guid.NewGuid ();
      ReflectionBasedClassDefinition orderClass =
          new ReflectionBasedClassDefinition ("Order", "Order", c_testDomainProviderID, typeof (Order), false, new List<Type> ());

      _newDataContainer = DataContainer.CreateNew (new ObjectID ("Order", idValue));
      _existingDataContainer = DataContainer.CreateForExisting (new ObjectID ("Order", idValue), null);

      ClientTransactionMock.SetClientTransaction (_existingDataContainer);
      ClientTransactionMock.SetClientTransaction (_newDataContainer);

      _nameDefinition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(orderClass, "Name", "Name", typeof (string), 100);
      _nameProperty = new PropertyValue (_nameDefinition, "Arthur Dent");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The ClassDefinition 'Remotion.Data.DomainObjects.Mapping."
        + "ReflectionBasedClassDefinition: Order' of the ObjectID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is not part of the current"
        + " mapping.\r\nParameter name: id")]
    public void ClassDefinitionNotInMapping ()
    {
      ObjectID id = DomainObjectIDs.Order1;
      MappingConfiguration.SetCurrent (null);
      Assert.AreNotSame (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order)), id.ClassDefinition);
      DataContainer.CreateNew (id);
    }

    [Test]
    public void NewDataContainerStates ()
    {
      _newDataContainer.PropertyValues.Add (_nameProperty);

      Assert.AreEqual (StateType.New, _newDataContainer.State);
      Assert.AreEqual ("Arthur Dent", _newDataContainer["Name"]);

      _newDataContainer["Name"] = "Zaphod Beeblebrox";

      Assert.AreEqual (StateType.New, _newDataContainer.State);
      Assert.AreEqual ("Zaphod Beeblebrox", _newDataContainer["Name"]);
    }

    [Test]
    public void ExistingDataContainerStates ()
    {
      _existingDataContainer.PropertyValues.Add (_nameProperty);

      Assert.AreEqual (StateType.Unchanged, _existingDataContainer.State);
      Assert.AreEqual ("Arthur Dent", _existingDataContainer["Name"]);

      _existingDataContainer["Name"] = "Zaphod Beeblebrox";

      Assert.AreEqual (StateType.Changed, _existingDataContainer.State);
      Assert.AreEqual ("Zaphod Beeblebrox", _existingDataContainer["Name"]);
    }

    [Test]
    public void NewDataContainerEvents ()
    {
      _newDataContainer.PropertyValues.Add (_nameProperty);

      PropertyValueContainerEventReceiver eventReceiver = new PropertyValueContainerEventReceiver (
          _newDataContainer, false);

      _newDataContainer["Name"] = "Zaphod Beeblebrox";

      Assert.AreEqual (StateType.New, _newDataContainer.State);
      Assert.AreEqual ("Zaphod Beeblebrox", _newDataContainer["Name"]);
      Assert.AreSame (_nameProperty, eventReceiver.ChangingPropertyValue);
      Assert.AreEqual ("Arthur Dent", eventReceiver.ChangingOldValue);
      Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.ChangingNewValue);

      Assert.AreSame (_nameProperty, eventReceiver.ChangedPropertyValue);
      Assert.AreEqual ("Arthur Dent", eventReceiver.ChangedOldValue);
      Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.ChangedNewValue);
    }

    [Test]
    public void NewDataContainerCancelEvents ()
    {
      _newDataContainer.PropertyValues.Add (_nameProperty);

      PropertyValueContainerEventReceiver eventReceiver = new PropertyValueContainerEventReceiver (
          _newDataContainer, true);

      try
      {
        _newDataContainer["Name"] = "Zaphod Beeblebrox";
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.AreEqual (StateType.New, _newDataContainer.State);
        Assert.AreEqual ("Arthur Dent", _newDataContainer["Name"]);
        Assert.AreSame (_nameProperty, eventReceiver.ChangingPropertyValue);
        Assert.AreEqual ("Arthur Dent", eventReceiver.ChangingOldValue);
        Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.ChangingNewValue);
        Assert.AreSame (null, eventReceiver.ChangedPropertyValue);
      }
    }

    [Test]
    public void ExistingDataContainerEvents ()
    {
      _existingDataContainer.PropertyValues.Add (_nameProperty);

      PropertyValueContainerEventReceiver eventReceiver = new PropertyValueContainerEventReceiver (
          _existingDataContainer, false);

      _existingDataContainer["Name"] = "Zaphod Beeblebrox";

      Assert.AreEqual (StateType.Changed, _existingDataContainer.State);
      Assert.AreEqual ("Zaphod Beeblebrox", _existingDataContainer["Name"]);

      Assert.AreSame (_nameProperty, eventReceiver.ChangingPropertyValue);
      Assert.AreEqual ("Arthur Dent", eventReceiver.ChangingOldValue);
      Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.ChangingNewValue);

      Assert.AreSame (_nameProperty, eventReceiver.ChangedPropertyValue);
      Assert.AreEqual ("Arthur Dent", eventReceiver.ChangedOldValue);
      Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.ChangedNewValue);
    }

    [Test]
    public void ExistingDataContainerCancelEvents ()
    {
      _existingDataContainer.PropertyValues.Add (_nameProperty);

      PropertyValueContainerEventReceiver eventReceiver = new PropertyValueContainerEventReceiver (
          _existingDataContainer, true);

      try
      {
        _existingDataContainer["Name"] = "Zaphod Beeblebrox";
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.AreEqual (StateType.Unchanged, _existingDataContainer.State);
        Assert.AreEqual ("Arthur Dent", _existingDataContainer["Name"]);
        Assert.AreSame (_nameProperty, eventReceiver.ChangingPropertyValue);
        Assert.AreEqual ("Arthur Dent", eventReceiver.ChangingOldValue);
        Assert.AreEqual ("Zaphod Beeblebrox", eventReceiver.ChangingNewValue);
        Assert.AreSame (null, eventReceiver.ChangedPropertyValue);
      }
    }

    [Test]
    public void GetObjectID ()
    {
      DataContainer dataContainer = TestDataContainerFactory.CreateOrder1DataContainer ();
      ObjectID id = (ObjectID) dataContainer.GetValue ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer");
      Assert.IsNotNull (id);
    }

    [Test]
    public void GetNullObjectID ()
    {
      ObjectID id = new ObjectID ("Official", 1);
      DataContainer container = DataContainer.CreateNew (id);

      PropertyDefinition reportsToDefinition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition((ReflectionBasedClassDefinition) container.ClassDefinition, "ReportsTo", "ReportsTo", typeof (string), true, 100);

      container.PropertyValues.Add (new PropertyValue (reportsToDefinition, null));

      Assert.IsNull (container.GetValue ("ReportsTo"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), 
        ExpectedMessage = "Property 'NonExistingPropertyName' does not exist.\r\nParameter name: propertyName")]
    public void GetObjectIDForNonExistingProperty ()
    {
      DataContainer container = TestDataContainerFactory.CreateOrder1DataContainer ();
      container.GetValue ("NonExistingPropertyName");
    }

    [Test]
    public void ChangePropertyBackToOriginalValue ()
    {
      DataContainer container = TestDataContainerFactory.CreateOrder1DataContainer ();

      container["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"] = 42;
      Assert.AreEqual (StateType.Changed, container.State);
      Assert.AreEqual (42, container.GetValue ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"));

      container["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"] = 1;
      Assert.AreEqual (StateType.Unchanged, container.State);
      Assert.AreEqual (1, container.GetValue ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"));
    }

    [Test]
    public void SetValue ()
    {
      _existingDataContainer.PropertyValues.Add (_nameProperty);
      _existingDataContainer.SetValue ("Name", "Zaphod Beeblebrox");

      Assert.AreEqual ("Zaphod Beeblebrox", _existingDataContainer.GetValue ("Name"));
    }

    [Test]
    public void GetBytes ()
    {
      DataContainer dataContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer ();

      ResourceManager.IsEqualToImage1 ((byte[]) dataContainer.GetValue ("Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty"));
      Assert.IsNull (dataContainer.GetValue ("Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"));
    }

    [Test]
    public void SetBytes ()
    {
      DataContainer dataContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer ();

      dataContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty"] = new byte[0];
      ResourceManager.IsEmptyImage ((byte[]) dataContainer.GetValue ("Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty"));

      dataContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"] = null;
      Assert.IsNull (dataContainer.GetValue ("Remotion.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"));
    }

    private void CheckIfDataContainersAreEqual (DataContainer expected, DataContainer actual, bool checkID)
    {
      ArgumentUtility.CheckNotNull ("expected", expected);
      ArgumentUtility.CheckNotNull ("actual", actual);

      Assert.AreNotSame (expected, actual);
      
      if (checkID)
        Assert.AreEqual (expected.ID, actual.ID);

      Assert.AreSame (expected.ClassDefinition, actual.ClassDefinition);
      Assert.AreSame (expected.DomainObject, actual.DomainObject);
      Assert.AreSame (expected.DomainObjectType, actual.DomainObjectType);
      Assert.AreEqual (expected.IsDiscarded, actual.IsDiscarded);
      Assert.AreEqual (expected.PropertyValues.Count, actual.PropertyValues.Count);

      for (int i = 0; i < actual.PropertyValues.Count; ++i)
      {
        Assert.AreSame (expected.PropertyValues[i].Definition, actual.PropertyValues[i].Definition);
        Assert.AreEqual (expected.PropertyValues[i].HasChanged, actual.PropertyValues[i].HasChanged);
        Assert.AreEqual (expected.PropertyValues[i].HasBeenTouched, actual.PropertyValues[i].HasBeenTouched);
        Assert.AreEqual (expected.PropertyValues[i].IsDiscarded, actual.PropertyValues[i].IsDiscarded);
        Assert.AreEqual (expected.PropertyValues[i].OriginalValue, actual.PropertyValues[i].OriginalValue);
        Assert.AreEqual (expected.PropertyValues[i].Value, actual.PropertyValues[i].Value);
      }
      
      Assert.AreEqual (expected.State, actual.State);
      Assert.AreSame (expected.Timestamp, actual.Timestamp);
    }

    private void CheckIfClientTransactionIsNull (DataContainer dataContainer)
    {
      Assert.IsNull (PrivateInvoke.GetNonPublicField (dataContainer, "_clientTransaction"));
    }

    private void SetClientTransaction (DataContainer dataContainer, ClientTransaction transaction)
    {
      PrivateInvoke.InvokeNonPublicMethod (dataContainer, "SetClientTransaction", transaction);
    }

    [Test]
    public void CloneLoadedUnchanged ()
    {
      DataContainer original = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      Assert.IsNotNull (original);
      Assert.AreEqual (DomainObjectIDs.Order1, original.ID);
      Assert.IsNotNull (original.ClassDefinition);
      Assert.AreSame (ClientTransactionMock, original.ClientTransaction);
      Assert.AreSame (Order.GetObject (DomainObjectIDs.Order1), original.DomainObject);
      Assert.AreSame (typeof (Order), original.DomainObjectType);
      Assert.IsFalse (original.IsDiscarded);
      Assert.AreEqual (4, original.PropertyValues.Count);
      Assert.IsNotNull (original.PropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].Definition);
      Assert.IsFalse (original.PropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].HasChanged);
      Assert.IsFalse (original.PropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].HasBeenTouched);
      Assert.IsFalse (original.PropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].IsDiscarded);
      Assert.AreEqual (1, original.PropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].OriginalValue);
      Assert.AreEqual (1, original.PropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].Value);
      Assert.AreEqual (StateType.Unchanged, original.State);
      Assert.IsNotNull (original.Timestamp);

      DataContainer clone = original.Clone ();

      Assert.IsNotNull (clone);
      CheckIfClientTransactionIsNull (clone);
      SetClientTransaction (clone, original.ClientTransaction);

      CheckIfDataContainersAreEqual (original, clone, true);
    }

    [Test]
    public void CloneLoadedChanged ()
    {
      DataContainer original = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      original.PropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].Value = 75;

      Assert.IsNotNull (original);
      Assert.AreEqual (DomainObjectIDs.Order1, original.ID);
      Assert.IsNotNull (original.ClassDefinition);
      Assert.AreSame (ClientTransactionMock, original.ClientTransaction);
      Assert.AreSame (Order.GetObject (DomainObjectIDs.Order1), original.DomainObject);
      Assert.AreSame (typeof (Order), original.DomainObjectType);
      Assert.IsFalse (original.IsDiscarded);
      Assert.AreEqual (4, original.PropertyValues.Count);
      Assert.IsNotNull (original.PropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].Definition);
      Assert.IsTrue (original.PropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].HasChanged);
      Assert.IsTrue (original.PropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].HasBeenTouched);
      Assert.IsFalse (original.PropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].IsDiscarded);
      Assert.AreEqual (1, original.PropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].OriginalValue);
      Assert.AreEqual (75, original.PropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].Value);
      Assert.AreEqual (StateType.Changed, original.State);
      Assert.IsNotNull (original.Timestamp);

      DataContainer clone = original.Clone ();

      Assert.IsNotNull (clone);
      CheckIfClientTransactionIsNull (clone);
      SetClientTransaction (clone, original.ClientTransaction);

      CheckIfDataContainersAreEqual (original, clone, true);
    }

    [Test]
    public void CloneNew ()
    {
      Order order = Order.NewObject ();
      DataContainer original = order.InternalDataContainer;

      Assert.IsNotNull (original);
      Assert.AreEqual (order.ID, original.ID);
      Assert.IsNotNull (original.ClassDefinition);
      Assert.AreSame (ClientTransactionMock, original.ClientTransaction);
      Assert.AreSame (order, original.DomainObject);
      Assert.AreSame (typeof (Order), original.DomainObjectType);
      Assert.IsFalse (original.IsDiscarded);
      Assert.AreEqual (4, original.PropertyValues.Count);
      Assert.IsNotNull (original.PropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].Definition);
      Assert.IsFalse (original.PropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].HasChanged);
      Assert.IsFalse (original.PropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].HasBeenTouched);
      Assert.IsFalse (original.PropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].IsDiscarded);
      Assert.AreEqual (0, original.PropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].OriginalValue);
      Assert.AreEqual (0, original.PropertyValues["Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"].Value);
      Assert.AreEqual (StateType.New, original.State);
      Assert.IsNull (original.Timestamp);

      DataContainer clone = original.Clone ();

      Assert.IsNotNull (clone);
      CheckIfClientTransactionIsNull (clone);
      SetClientTransaction (clone, original.ClientTransaction);

      CheckIfDataContainersAreEqual (original, clone, true);
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException), ExpectedMessage = "Object 'Order.*' is already discarded.", MatchType = MessageMatch.Regex)]
    public void CloneDeleted ()
    {
      Order order = Order.NewObject ();
      DataContainer original = order.InternalDataContainer;
      order.Delete ();

      Assert.IsTrue (original.IsDiscarded);

      original.Clone ();
    }

    [Test]
    public void CloneDataContainerWithoutClientTransaction ()
    {
      Order order = Order.NewObject ();
      DataContainer containerWithoutClientTransaction = order.InternalDataContainer.Clone ();
      CheckIfClientTransactionIsNull (containerWithoutClientTransaction);

      DataContainer clone = containerWithoutClientTransaction.Clone ();

      Assert.IsNotNull (clone);
      CheckIfClientTransactionIsNull (clone);
      SetClientTransaction (clone, ClientTransactionScope.CurrentTransaction);

      CheckIfDataContainersAreEqual (containerWithoutClientTransaction, clone, true);
    }

    [Test]
    public void CreateAndCopyState ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      DataContainer original = order.InternalDataContainer;
      ObjectID newID = new ObjectID (order.ID.ClassDefinition, Guid.NewGuid());

      DataContainer cloneWithDifferentID = DataContainer.CreateAndCopyState (newID, original);
      Assert.AreNotEqual (original.ID, cloneWithDifferentID.ID);
      Assert.AreEqual (newID, cloneWithDifferentID.ID);

      CheckIfClientTransactionIsNull (cloneWithDifferentID);
      SetClientTransaction (cloneWithDifferentID, original.ClientTransaction);

      CheckIfDataContainersAreEqual (original, cloneWithDifferentID, false);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The ID parameter specifies class 'Remotion.Data.DomainObjects.Mapping."
        + "ReflectionBasedClassDefinition: Official', but the state source is of class 'Remotion.Data.DomainObjects.Mapping."
        + "ReflectionBasedClassDefinition: Order'.\r\nParameter name: stateSource")]
    public void CreateAndCopyStateThrowsWhenWrongClassDefinition ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      DataContainer original = order.InternalDataContainer;
      ObjectID newID = new ObjectID (DomainObjectIDs.Official1.ClassDefinition, Guid.NewGuid ());
      DataContainer.CreateAndCopyState (newID, original);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "A DataContainer cannot be discarded while it doesn't have an "
        + "associated DomainObject.")]
    public void DiscardWithoutDomainObjectThrows ()
    {
      DataContainer dataContainerWithoutDomainObject = DataContainer.CreateNew (DomainObjectIDs.Order1);
      ClientTransactionMock.SetClientTransaction (dataContainerWithoutDomainObject);
      PrivateInvoke.InvokeNonPublicMethod (dataContainerWithoutDomainObject, "Delete");
      Assert.Fail ("Expected exception");
    }

    [Test]
    public void GetIDEvenPossibleWhenDiscarded ()
    {
      Order order = Order.NewObject ();
      DataContainer dataContainer = order.InternalDataContainer;
      order.Delete ();
      Assert.IsTrue (dataContainer.IsDiscarded);
      Assert.AreEqual (order.ID, dataContainer.ID);
    }

    [Test]
    public void GetDomainObjectEvenPossibleWhenDiscarded ()
    {
      Order order = Order.NewObject ();
      DataContainer dataContainer = order.InternalDataContainer;
      order.Delete ();
      Assert.IsTrue (dataContainer.IsDiscarded);
      Assert.AreSame (order, dataContainer.DomainObject);
    }

    [Test]
    public void MarkAsChanged ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      DataContainer dataContainer = order.InternalDataContainer;
      Assert.AreEqual(StateType.Unchanged, dataContainer.State);
      dataContainer.MarkAsChanged ();
      Assert.AreEqual (StateType.Changed, dataContainer.State);

      ClientTransactionMock.Rollback ();
      Assert.AreEqual (StateType.Unchanged, dataContainer.State);

      SetDatabaseModifyable ();

      dataContainer.MarkAsChanged ();
      Assert.AreEqual (StateType.Changed, dataContainer.State);
      
      ClientTransactionMock.Commit ();
      Assert.AreEqual (StateType.Unchanged, dataContainer.State);

      DataContainer clone = dataContainer.Clone ();
      Assert.AreEqual (StateType.Unchanged, clone.State);

      dataContainer.MarkAsChanged();
      Assert.AreEqual (StateType.Changed, dataContainer.State);

      clone = dataContainer.Clone ();
      Assert.AreEqual (StateType.Changed, clone.State);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Only existing DataContainers can be marked as changed.")]
    public void MarkAsChangedThrowsWhenNew ()
    {
      Order order = Order.NewObject ();
      DataContainer dataContainer = order.InternalDataContainer;
      dataContainer.MarkAsChanged ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Only existing DataContainers can be marked as changed.")]
    public void MarkAsChangedThrowsWhenDeleted ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.Delete ();
      DataContainer dataContainer = order.InternalDataContainer;
      dataContainer.MarkAsChanged ();
    }

    [Test]
    [ExpectedException (typeof (DomainObjectException), ExpectedMessage = "Internal error: ClientTransaction of DataContainer is not set.")]
    public void ErrorWhenNoClientTransaction ()
    {
      DataContainer dc = DataContainer.CreateNew (DomainObjectIDs.Order1);
      Dev.Null = dc.ClientTransaction;
    }
  }
}
