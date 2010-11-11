// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Resources;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class DataContainerTest : ClientTransactionBaseTest
  {
    private PropertyDefinition _nameDefinition;
    private PropertyValue _nameProperty;

    private DataContainer _newDataContainer;
    private DataContainer _existingDataContainer;
    private DataContainer _deletedDataContainer;
    private DataContainer _discardedDataContainer;
    private ObjectID _invalidObjectID;

    public override void SetUp ()
    {
      base.SetUp();

      var idValue = Guid.NewGuid();
      ReflectionBasedClassDefinition orderClass =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "Order", c_testDomainProviderID, typeof (Order), false);

      _newDataContainer = DataContainer.CreateNew (new ObjectID ("Order", idValue));
      _existingDataContainer = DataContainer.CreateForExisting (
          new ObjectID ("Order", idValue),
          null,
          propertyDefinition => propertyDefinition.DefaultValue);

      _deletedDataContainer = DataContainer.CreateForExisting (
          new ObjectID ("Order", idValue),
          null,
          propertyDefinition => propertyDefinition.DefaultValue);
      _deletedDataContainer.Delete();

      _invalidObjectID = new ObjectID ("Order", idValue);
      _discardedDataContainer = DataContainer.CreateNew (_invalidObjectID);
      _discardedDataContainer.Discard();

      _nameDefinition = ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (orderClass, "Name", "Name");
      _nameProperty = new PropertyValue (_nameDefinition, "Arthur Dent");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The ClassDefinition 'Remotion.Data.DomainObjects.Mapping.ReflectionBasedClassDefinition: Order' "
        + "of the ObjectID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid' is not part of the current mapping.\r\nParameter name: id")]
    public void ClassDefinitionNotInMapping ()
    {
      TestMappingConfiguration.Initialize();
      MappingConfiguration.SetCurrent (TestMappingConfiguration.Instance.GetMappingConfiguration());
      ObjectID id = new ObjectID (MappingConfiguration.Current.ClassDefinitions["Order"], new Guid ("5682f032-2f0b-494b-a31c-c97f02b89c36"));
      
      MappingConfiguration.SetCurrent (StandardConfiguration.Instance.GetMappingConfiguration());
      Assert.That (id.ClassDefinition, Is.Not.SameAs (MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order))));
      DataContainer.CreateNew (id);
    }

    [Test]
    public void NewDataContainerStates ()
    {
      _newDataContainer.PropertyValues.Add (_nameProperty);

      Assert.That (_newDataContainer.State, Is.EqualTo (StateType.New));
      Assert.That (_newDataContainer["Name"], Is.EqualTo ("Arthur Dent"));

      _newDataContainer["Name"] = "Zaphod Beeblebrox";

      Assert.That (_newDataContainer.State, Is.EqualTo (StateType.New));
      Assert.That (_newDataContainer["Name"], Is.EqualTo ("Zaphod Beeblebrox"));
    }

    [Test]
    public void ExistingDataContainerStates ()
    {
      _existingDataContainer.PropertyValues.Add (_nameProperty);

      Assert.That (_existingDataContainer.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (_existingDataContainer["Name"], Is.EqualTo ("Arthur Dent"));

      _existingDataContainer["Name"] = "Zaphod Beeblebrox";

      Assert.That (_existingDataContainer.State, Is.EqualTo (StateType.Changed));
      Assert.That (_existingDataContainer["Name"], Is.EqualTo ("Zaphod Beeblebrox"));
    }

    [Test]
    public void ExistingDataContainerEvents ()
    {
      var transaction = ClientTransaction.CreateRootTransaction ();

      _existingDataContainer.PropertyValues.Add (_nameProperty);
      ClientTransactionTestHelper.RegisterDataContainer (transaction, _existingDataContainer);

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (transaction);
      listenerMock
          .Expect (
              mock => mock.PropertyValueChanging (transaction, _existingDataContainer, _nameProperty, "Arthur Dent", "Zaphod Beeblebrox"))
          .WhenCalled (
              mi =>
              {
                Assert.That (_existingDataContainer.State, Is.EqualTo (StateType.Unchanged));
                Assert.That (_existingDataContainer["Name"], Is.EqualTo ("Arthur Dent"));
              });
      listenerMock
          .Expect (mock => mock.PropertyValueChanged (transaction, _existingDataContainer, _nameProperty, "Arthur Dent", "Zaphod Beeblebrox"))
          .WhenCalled (mi =>
          {
            Assert.That (_existingDataContainer.State, Is.EqualTo (StateType.Changed));
            Assert.That (_existingDataContainer["Name"], Is.EqualTo ("Zaphod Beeblebrox"));
          });
      listenerMock.Replay ();

      _existingDataContainer["Name"] = "Zaphod Beeblebrox";

      listenerMock.VerifyAllExpectations ();

      Assert.That (_existingDataContainer.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void ExistingDataContainerCancelEvents ()
    {
      var transaction = ClientTransaction.CreateRootTransaction ();

      _existingDataContainer.PropertyValues.Add (_nameProperty);
      ClientTransactionTestHelper.RegisterDataContainer (transaction, _existingDataContainer);

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (transaction);
      listenerMock
          .Expect (mock => mock.PropertyValueChanging (transaction, _existingDataContainer, _nameProperty, "Arthur Dent", "Zaphod Beeblebrox"))
          .Throw (new EventReceiverCancelException());
      listenerMock.Replay ();

      try
      {
        _existingDataContainer["Name"] = "Zaphod Beeblebrox";
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.That (_existingDataContainer.State, Is.EqualTo (StateType.Unchanged));
        Assert.That (_existingDataContainer["Name"], Is.EqualTo ("Arthur Dent"));
      }

      listenerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetObjectID ()
    {
      DataContainer dataContainer = TestDataContainerFactory.CreateOrder1DataContainer();
      var id = (ObjectID) dataContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer");
      Assert.That (id, Is.Not.Null);
    }

    [Test]
    public void GetNullObjectID ()
    {
      var id = new ObjectID ("Official", 1);
      DataContainer container = DataContainer.CreateNew (id);

      PropertyDefinition reportsToDefinition =
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (
              (ReflectionBasedClassDefinition) container.ClassDefinition, "ReportsTo", "ReportsTo");

      container.PropertyValues.Add (new PropertyValue (reportsToDefinition, null));

      Assert.That (container.GetValue ("ReportsTo"), Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "Property 'NonExistingPropertyName' does not exist.\r\nParameter name: propertyName")]
    public void GetObjectIDForNonExistingProperty ()
    {
      DataContainer container = TestDataContainerFactory.CreateOrder1DataContainer();
      container.GetValue ("NonExistingPropertyName");
    }

    [Test]
    public void ChangePropertyBackToOriginalValue ()
    {
      DataContainer container = TestDataContainerFactory.CreateOrder1DataContainer();

      container["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 42;
      Assert.That (container.State, Is.EqualTo (StateType.Changed));
      Assert.That (container.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"), Is.EqualTo (42));

      container["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 1;
      Assert.That (container.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (container.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"), Is.EqualTo (1));
    }

    [Test]
    public void SetValue ()
    {
      _existingDataContainer.PropertyValues.Add (_nameProperty);
      _existingDataContainer.SetValue ("Name", "Zaphod Beeblebrox");

      Assert.That (_existingDataContainer.GetValue ("Name"), Is.EqualTo ("Zaphod Beeblebrox"));
    }

    [Test]
    public void GetBytes ()
    {
      DataContainer dataContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer();

      ResourceManager.IsEqualToImage1 (
          (byte[]) dataContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"));
      Assert.That (dataContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"), Is.Null);
    }

    [Test]
    public void SetBytes ()
    {
      DataContainer dataContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer();

      dataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"] = new byte[0];
      ResourceManager.IsEmptyImage (
          (byte[]) dataContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"));

      dataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"] = null;
      Assert.That (dataContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"), Is.Null);
    }

    [Test]
    public void SetTimestamp ()
    {
      DataContainer dataContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer();
      dataContainer.SetTimestamp (10);

      Assert.That (dataContainer.Timestamp, Is.EqualTo (10));
    }

    [Test]
    public void Clone_SetsID ()
    {
      var original = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;

      var clone = original.Clone (DomainObjectIDs.Order2);
      Assert.That (clone.ID, Is.EqualTo (DomainObjectIDs.Order2));
    }

    [Test]
    public void Clone_CopiesState ()
    {
      var originalNew = DataContainer.CreateNew (DomainObjectIDs.Order1);
      Assert.That (originalNew.State, Is.EqualTo (StateType.New));

      var originalExisting = DataContainer.CreateForExisting (DomainObjectIDs.Order2, null, pd => pd.DefaultValue);
      Assert.That (originalExisting.State, Is.EqualTo (StateType.Unchanged));

      var clonedNew = originalNew.Clone (DomainObjectIDs.Order3);
      Assert.That (clonedNew.State, Is.EqualTo (StateType.New));

      var clonedExisting = originalExisting.Clone (DomainObjectIDs.Order4);
      Assert.That (clonedExisting.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void Clone_CopiesTimestamp ()
    {
      var original = DataContainer.CreateNew (DomainObjectIDs.Order1);
      original.SetTimestamp (12);

      var clone = original.Clone (DomainObjectIDs.Order2);
      Assert.That (clone.Timestamp, Is.EqualTo (12));
    }

    [Test]
    public void Clone_CopiesPropertyValues ()
    {
      var original = DataContainer.CreateForExisting (DomainObjectIDs.Order2, null, pd => pd.DefaultValue);

      var clone = original.Clone (DomainObjectIDs.Order2);

      Assert.That (
          clone.PropertyValues.Cast<PropertyValue>().Select (pv => pv.Definition).ToArray(),
          Is.EqualTo (original.PropertyValues.Cast<PropertyValue>().Select (pv => pv.Definition).ToArray()));
      Assert.That (
          clone.PropertyValues.Cast<PropertyValue>().Select (pv => pv.OriginalValue).ToArray(),
          Is.EqualTo (original.PropertyValues.Cast<PropertyValue>().Select (pv => pv.OriginalValue).ToArray()));
      Assert.That (
          clone.PropertyValues.Cast<PropertyValue>().Select (pv => pv.Value).ToArray(),
          Is.EqualTo (original.PropertyValues.Cast<PropertyValue>().Select (pv => pv.Value).ToArray()));
    }

    [Test]
    public void Clone_CopiesHasBeenMarkedChanged ()
    {
      var original = DataContainer.CreateForExisting (DomainObjectIDs.Order2, null, pd => pd.DefaultValue);
      original.MarkAsChanged();
      Assert.That (original.HasBeenMarkedChanged, Is.True);

      var clone = original.Clone (DomainObjectIDs.Order2);
      Assert.That (clone.HasBeenMarkedChanged, Is.True);
    }

    [Test]
    public void Clone_CopiesHasBeenChangedFlag ()
    {
      var original = DataContainer.CreateForExisting (DomainObjectIDs.Order2, null, pd => pd.DefaultValue);
      original.PropertyValues[typeof (Order) + ".OrderNumber"].Value = 10;
      Assert.That (original.State, Is.EqualTo (StateType.Changed));

      var clone = original.Clone (DomainObjectIDs.Order2);
      Assert.That (clone.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void Clone_DomainObjectEmpty ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var original = order.InternalDataContainer;
      Assert.That (original.DomainObject, Is.SameAs (order));

      var clone = original.Clone (DomainObjectIDs.Order1);
      Assert.That (PrivateInvoke.GetNonPublicField (clone, "_domainObject"), Is.Null);
    }

    [Test]
    public void Clone_TransactionEmpty ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      var original = order.InternalDataContainer;
      Assert.That (original.ClientTransaction, Is.SameAs (ClientTransactionMock));

      var clone = original.Clone (DomainObjectIDs.Order1);
      Assert.That (PrivateInvoke.GetNonPublicField (clone, "_clientTransaction"), Is.Null);
    }

    [Test]
    public void CommitState_SetsStateToExisting ()
    {
      _newDataContainer.CommitState ();

      Assert.That (_newDataContainer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void CommitState_RaisesStateUpdated ()
    {
      ClientTransactionTestHelper.RegisterDataContainer (ClientTransactionMock, _newDataContainer);

      CheckStateNotification (_newDataContainer, dc => dc.CommitState (), StateType.Unchanged);
    }

    [Test]
    public void CommitState_CommitsPropertyValues ()
    {
      var propertyValue = _existingDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"];
      propertyValue.Value = 10;

      Assert.That (propertyValue.HasChanged, Is.True);
      Assert.That (propertyValue.OriginalValue, Is.EqualTo (0));
      Assert.That (propertyValue.Value, Is.EqualTo (10));

      _existingDataContainer.CommitState ();

      Assert.That (propertyValue.HasChanged, Is.False);
      Assert.That (propertyValue.OriginalValue, Is.EqualTo (10));
      Assert.That (propertyValue.Value, Is.EqualTo (10));
    }

    [Test]
    public void CommitState_ResetsChangedFlag ()
    {
      var propertyValue = _existingDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"];
      propertyValue.Value = 10;

      Assert.That (_existingDataContainer.State, Is.EqualTo (StateType.Changed));

      _existingDataContainer.CommitState ();

      Assert.That (_existingDataContainer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void CommitState_ResetsMarkedChangedFlag ()
    {
      _existingDataContainer.MarkAsChanged ();
      Assert.That (_existingDataContainer.HasBeenMarkedChanged, Is.True);

      _existingDataContainer.CommitState ();

      Assert.That (_existingDataContainer.HasBeenMarkedChanged, Is.False);
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void CommitState_DiscardedDataContainer_Throws ()
    {
      _discardedDataContainer.CommitState ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "Deleted data containers cannot be committed, they have to be discarded.")]
    public void CommitState_DeletedDataContainer_Throws ()
    {
      _deletedDataContainer.CommitState ();
    }

    [Test]
    public void RollbackState_SetsStateToExisting ()
    {
      _deletedDataContainer.RollbackState ();

      Assert.That (_deletedDataContainer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void RollbackState_RaisesStateUpdated ()
    {
      ClientTransactionTestHelper.RegisterDataContainer (ClientTransactionMock, _existingDataContainer);

      CheckStateNotification (_existingDataContainer, dc => dc.RollbackState (), StateType.Unchanged);
    }

    [Test]
    public void RollbackState_RollsbackPropertyValues ()
    {
      var propertyValue = _existingDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"];
      propertyValue.Value = 10;

      Assert.That (propertyValue.HasChanged, Is.True);
      Assert.That (propertyValue.OriginalValue, Is.EqualTo (0));
      Assert.That (propertyValue.Value, Is.EqualTo (10));

      _existingDataContainer.RollbackState ();

      Assert.That (propertyValue.HasChanged, Is.False);
      Assert.That (propertyValue.OriginalValue, Is.EqualTo (0));
      Assert.That (propertyValue.Value, Is.EqualTo (0));
    }

    [Test]
    public void RollbackState_ResetsChangedFlag ()
    {
      var propertyValue = _existingDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"];
      propertyValue.Value = 10;

      Assert.That (_existingDataContainer.State, Is.EqualTo (StateType.Changed));

      _existingDataContainer.RollbackState ();

      Assert.That (_existingDataContainer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void RollbackState_ResetsMarkedChangedFlag ()
    {
      _existingDataContainer.MarkAsChanged ();
      Assert.That (_existingDataContainer.HasBeenMarkedChanged, Is.True);

      _existingDataContainer.RollbackState ();

      Assert.That (_existingDataContainer.HasBeenMarkedChanged, Is.False);
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void RollbackState_DiscardedDataContainer_Throws ()
    {
      _discardedDataContainer.RollbackState ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "New data containers cannot be rolled back, they have to be discarded.")]
    public void RollbackState_NewDataContainer_Throws ()
    {
      _newDataContainer.RollbackState ();
    }

    [Test]
    public void Delete_SetsStateToDeleted ()
    {
      _existingDataContainer.Delete ();

      Assert.That (_existingDataContainer.State, Is.EqualTo (StateType.Deleted));
    }

     [Test]
    public void Delete_RaisesStateUpdated ()
    {
      ClientTransactionTestHelper.RegisterDataContainer (ClientTransactionMock, _existingDataContainer);

      CheckStateNotification (_existingDataContainer, dc => dc.Delete(), StateType.Deleted);
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException))]
    public void Delete_DiscardedDataContainer_Throws ()
    {
      _discardedDataContainer.Delete ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "New data containers cannot be deleted, they have to be discarded.")]
    public void Delete_NewDataContainer_Throws ()
    {
      _newDataContainer.Delete ();
    }

    [Test]
    public void Discard_SetsDiscardedFlag ()
    {
      Assert.That (_newDataContainer.IsDiscarded, Is.False);

      _newDataContainer.Discard ();

      Assert.That (_newDataContainer.IsDiscarded, Is.True);
    }

    [Test]
    public void Discard_RaisesStateUpdated ()
    {
      ClientTransactionTestHelper.RegisterDataContainer (ClientTransactionMock, _newDataContainer);

      CheckStateNotification (_newDataContainer, dc => dc.Discard (), StateType.Invalid);
    }

    [Test]
    public void Discard_DiscardsPropertyValues ()
    {
      var propertyValue = _newDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"];
      Assert.That (propertyValue.IsDiscarded, Is.False);

      _newDataContainer.Discard ();

      Assert.That (propertyValue.IsDiscarded, Is.True);
    }

    [Test]
    public void SetPropertyValuesFrom_SetsValues ()
    {
      var sourceDataContainer = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      var newDataContainer = DataContainer.CreateNew (DomainObjectIDs.Order2);
      Assert.That (newDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"].Value, Is.Not.EqualTo (1));

      newDataContainer.SetPropertyValuesFrom (sourceDataContainer);

      Assert.That (newDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"].Value, Is.EqualTo (1));
    }

    [Test]
    public void SetPropertyValuesFrom_SetsForeignKeys ()
    {
      var sourceDataContainer = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1).InternalDataContainer;
      var newDataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket2);
      Assert.That (newDataContainer.PropertyValues[typeof (OrderTicket).FullName + ".Order"].Value, Is.Not.EqualTo (DomainObjectIDs.Order1));

      newDataContainer.SetPropertyValuesFrom (sourceDataContainer);

      Assert.That (newDataContainer.PropertyValues[typeof (OrderTicket).FullName + ".Order"].Value, Is.EqualTo (DomainObjectIDs.Order1));
    }

    [Test]
    public void SetPropertyValuesFrom_SetsChangedFlag_IfChanged ()
    {
      var sourceDataContainer = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      var existingDataContainer = Order.GetObject (DomainObjectIDs.Order2).InternalDataContainer;
      Assert.That (existingDataContainer.State, Is.EqualTo (StateType.Unchanged));

      existingDataContainer.SetPropertyValuesFrom (sourceDataContainer);

      Assert.That (existingDataContainer.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void SetPropertyValuesFrom_ResetsChangedFlag_IfUnchanged ()
    {
      var sourceDataContainer = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      var targetDataContainer = sourceDataContainer.Clone (DomainObjectIDs.Order1);
      targetDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"].Value = 10;
      Assert.That (targetDataContainer.State, Is.EqualTo (StateType.Changed));

      targetDataContainer.SetPropertyValuesFrom (sourceDataContainer);

      Assert.That (targetDataContainer.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void SetPropertyValuesFrom_RaisesStateUpdated_Changed ()
    {
      var sourceDataContainer = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      var targetDataContainer = Order.GetObject (DomainObjectIDs.Order2).InternalDataContainer;
      Assert.That (targetDataContainer.State, Is.EqualTo (StateType.Unchanged));

      CheckStateNotification (targetDataContainer, dc => dc.SetPropertyValuesFrom (sourceDataContainer), StateType.Changed);
    }

    [Test]
    public void SetPropertyValuesFrom_RaisesStateUpdated_Unchanged ()
    {
      var sourceDataContainer = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      var targetDataContainer = sourceDataContainer.Clone (DomainObjectIDs.Order2);
      targetDataContainer.PropertyValues[typeof (Order).FullName + ".OrderNumber"].Value = 10;
      Assert.That (targetDataContainer.State, Is.EqualTo (StateType.Changed));

      ClientTransactionTestHelper.RegisterDataContainer (ClientTransactionMock, targetDataContainer);
      CheckStateNotification (targetDataContainer, dc => dc.SetPropertyValuesFrom (sourceDataContainer), StateType.Unchanged);
    }

    [Test]
    public void SetPropertyValuesFrom_RaisesStateUpdated_OtherState ()
    {
      var sourceDataContainer = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      var targetDataContainer = sourceDataContainer.Clone (DomainObjectIDs.Order2);
      targetDataContainer.Delete ();
      Assert.That (targetDataContainer.State, Is.EqualTo (StateType.Deleted));

      ClientTransactionTestHelper.RegisterDataContainer (ClientTransactionMock, targetDataContainer);
      CheckStateNotification (targetDataContainer, dc => dc.SetPropertyValuesFrom (sourceDataContainer), StateType.Deleted);
    }

    [Test]
    public void SetPropertyValuesFrom_DoesntMarkAsChanged ()
    {
      var sourceDataContainer = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      var targetDataContainer = sourceDataContainer.Clone (DomainObjectIDs.Order1);
      sourceDataContainer.MarkAsChanged();
      Assert.That (sourceDataContainer.HasBeenMarkedChanged, Is.True);
      Assert.That (targetDataContainer.HasBeenMarkedChanged, Is.False);

      targetDataContainer.SetPropertyValuesFrom (sourceDataContainer);

      Assert.That (targetDataContainer.HasBeenMarkedChanged, Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Cannot set this data container's property values from 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'; the data containers do not "
        + "have the same class definition.\r\nParameter name: source")]
    public void SetPropertyValuesFrom_InvalidDefinition ()
    {
      var sourceDataContainer = Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer;
      var targetDataContainer = OrderTicket.GetObject (DomainObjectIDs.OrderTicket1).InternalDataContainer;

      targetDataContainer.SetPropertyValuesFrom (sourceDataContainer);
    }

    [Test]
    public void GetIDEvenPossibleWhenDiscarded ()
    {
      Assert.That (_discardedDataContainer.IsDiscarded, Is.True);
      Assert.That (_discardedDataContainer.ID, Is.EqualTo (_invalidObjectID));
    }

    [Test]
    public void GetDomainObjectEvenPossibleWhenDiscarded ()
    {
      var domainObject = Order.NewObject ();
      var dataContainerWithObject = domainObject.InternalDataContainer;
      dataContainerWithObject.Discard ();

      Assert.That (dataContainerWithObject.IsDiscarded, Is.True);
      Assert.That (dataContainerWithObject.DomainObject, Is.SameAs (domainObject));
    }

    [Test]
    public void MarkAsChanged ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      DataContainer dataContainer = order.InternalDataContainer;
      Assert.That (dataContainer.State, Is.EqualTo (StateType.Unchanged));
      dataContainer.MarkAsChanged();
      Assert.That (dataContainer.State, Is.EqualTo (StateType.Changed));

      ClientTransactionMock.Rollback();
      Assert.That (dataContainer.State, Is.EqualTo (StateType.Unchanged));

      SetDatabaseModifyable();

      dataContainer.MarkAsChanged();
      Assert.That (dataContainer.State, Is.EqualTo (StateType.Changed));

      ClientTransactionMock.Commit();
      Assert.That (dataContainer.State, Is.EqualTo (StateType.Unchanged));

      DataContainer clone = dataContainer.Clone (DomainObjectIDs.Order1);
      Assert.That (clone.State, Is.EqualTo (StateType.Unchanged));

      dataContainer.MarkAsChanged();
      Assert.That (dataContainer.State, Is.EqualTo (StateType.Changed));

      clone = dataContainer.Clone (DomainObjectIDs.Order1);
      Assert.That (clone.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void MarkAsChanged_WithoutClientTransaction ()
    {
      Assert.That (_existingDataContainer.State, Is.EqualTo (StateType.Unchanged));
      _existingDataContainer.MarkAsChanged ();
      Assert.That (_existingDataContainer.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void MarkAsChanged_RaisesStateUpdated ()
    {
      ClientTransactionTestHelper.RegisterDataContainer (ClientTransactionMock, _existingDataContainer);

      CheckStateNotification (_existingDataContainer, dc => dc.MarkAsChanged(), StateType.Changed);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Only existing DataContainers can be marked as changed.")]
    public void MarkAsChangedThrowsWhenNew ()
    {
      Order order = Order.NewObject();
      DataContainer dataContainer = order.InternalDataContainer;
      dataContainer.MarkAsChanged();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Only existing DataContainers can be marked as changed.")]
    public void MarkAsChangedThrowsWhenDeleted ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      order.Delete();
      DataContainer dataContainer = order.InternalDataContainer;
      dataContainer.MarkAsChanged();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "DataContainer has not been registered with a transaction.")]
    public void ErrorWhenNoClientTransaction ()
    {
      DataContainer dc = DataContainer.CreateNew (DomainObjectIDs.Order1);
      Dev.Null = dc.ClientTransaction;
    }

    [Test]
    public void CreateNew_DoesNotIncludesStorageClassNoneProperties ()
    {
      DataContainer dc = DataContainer.CreateNew (new ObjectID (typeof (ClassWithPropertiesHavingStorageClassAttribute), Guid.NewGuid()));
      Assert.That (dc.PropertyValues.Contains (GetStorageClassPropertyName ("None")), Is.False);
    }

    [Test]
    public void CreateNew_IncludesStorageClassPersistentProperties ()
    {
      DataContainer dc = DataContainer.CreateNew (new ObjectID (typeof (ClassWithPropertiesHavingStorageClassAttribute), Guid.NewGuid()));
      Assert.That (dc.PropertyValues.Contains (GetStorageClassPropertyName ("Persistent")), Is.True);
      Assert.That (dc.PropertyValues[GetStorageClassPropertyName ("Persistent")].Value, Is.EqualTo (0));
      Assert.That (dc.PropertyValues[GetStorageClassPropertyName ("Persistent")].OriginalValue, Is.EqualTo (0));
    }

    [Test]
    public void CreateNew_IncludesStorageClassTransactionProperties ()
    {
      DataContainer dc = DataContainer.CreateNew (new ObjectID (typeof (ClassWithPropertiesHavingStorageClassAttribute), Guid.NewGuid()));
      Assert.That (dc.PropertyValues.Contains (GetStorageClassPropertyName ("Transaction")), Is.True);
      Assert.That (dc.PropertyValues[GetStorageClassPropertyName ("Transaction")].Value, Is.EqualTo (0));
      Assert.That (dc.PropertyValues[GetStorageClassPropertyName ("Transaction")].OriginalValue, Is.EqualTo (0));
    }

    [Test]
    public void CreateNew_HasSamePropertyOrderAsClassDefinition ()
    {
      DataContainer dc = DataContainer.CreateNew (new ObjectID (typeof (ClassWithPropertiesHavingStorageClassAttribute), Guid.NewGuid()));

      int index = 0;
      foreach (PropertyDefinition propertyDefinition in dc.ClassDefinition.GetPropertyDefinitions())
      {
        if (propertyDefinition.StorageClass != StorageClass.None)
        {
          Assert.That (dc.PropertyValues[index].Definition, Is.SameAs (propertyDefinition));
          index++;
        }
      }
    }

    [Test]
    public void CreateForExisting_DoesNotIncludesStorageClassNoneProperties ()
    {
      DataContainer dc = DataContainer.CreateForExisting (
          new ObjectID (typeof (ClassWithPropertiesHavingStorageClassAttribute), Guid.NewGuid()),
          1,
          delegate { return 2; });
      Assert.That (dc.PropertyValues.Contains (GetStorageClassPropertyName ("None")), Is.False);
    }

    [Test]
    public void CreateForExisting_IncludesStorageClassPersistentProperties_WithPersistentValue ()
    {
      DataContainer dc = DataContainer.CreateForExisting (
          new ObjectID (typeof (ClassWithPropertiesHavingStorageClassAttribute), Guid.NewGuid()),
          1,
          delegate { return 2; });
      Assert.That (dc.PropertyValues.Contains (GetStorageClassPropertyName ("Persistent")), Is.True);
      Assert.That (dc.PropertyValues[GetStorageClassPropertyName ("Persistent")].Value, Is.EqualTo (2));
      Assert.That (dc.PropertyValues[GetStorageClassPropertyName ("Persistent")].OriginalValue, Is.EqualTo (2));
    }

    [Test]
    public void CreateForExisting_IncludesStorageClassTransactionProperties ()
    {
      DataContainer dc = DataContainer.CreateForExisting (
          new ObjectID (typeof (ClassWithPropertiesHavingStorageClassAttribute), Guid.NewGuid()),
          1,
          delegate { return 2; });
      Assert.That (dc.PropertyValues.Contains (GetStorageClassPropertyName ("Transaction")), Is.True);
      Assert.That (dc.PropertyValues[GetStorageClassPropertyName ("Transaction")].Value, Is.EqualTo (0));
      Assert.That (dc.PropertyValues[GetStorageClassPropertyName ("Transaction")].OriginalValue, Is.EqualTo (0));
    }

    [Test]
    public void CreateExisting_HasSamePropertyOrderAsClassDefinition ()
    {
      DataContainer dc = DataContainer.CreateForExisting (
          new ObjectID (typeof (ClassWithPropertiesHavingStorageClassAttribute), Guid.NewGuid()),
          1,
          delegate { return 2; });

      int index = 0;
      foreach (PropertyDefinition propertyDefinition in dc.ClassDefinition.GetPropertyDefinitions())
      {
        if (propertyDefinition.StorageClass != StorageClass.None)
        {
          Assert.That (dc.PropertyValues[index].Definition, Is.SameAs (propertyDefinition));
          index++;
        }
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This DataContainer has not been associated with a DomainObject yet.")]
    public void DomainObject_NoneSet ()
    {
      var dc = DataContainer.CreateNew (DomainObjectIDs.Order1);
      Dev.Null = dc.DomainObject;
    }

    [Test]
    public void SetDomainObject ()
    {
      var domainObject = Order.GetObject (DomainObjectIDs.Order1);

      var dc = DataContainer.CreateNew (DomainObjectIDs.Order1);
      dc.SetDomainObject (domainObject);

      Assert.That (dc.DomainObject, Is.SameAs (domainObject));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The given DomainObject has another ID than this DataContainer.\r\n"
                                                                      + "Parameter name: domainObject")]
    public void SetDomainObject_InvalidID ()
    {
      var domainObject = Order.GetObject (DomainObjectIDs.Order2);

      var dc = DataContainer.CreateNew (DomainObjectIDs.Order1);
      dc.SetDomainObject (domainObject);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This DataContainer has already been associated with a DomainObject.")]
    public void SetDomainObject_DomainObjectAlreadySet ()
    {
      var domainObject1 = Order.GetObject (DomainObjectIDs.Order1);
      var domainObject2 = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);

      var dc = DataContainer.CreateNew (DomainObjectIDs.Order1);
      dc.SetDomainObject (domainObject1);
      dc.SetDomainObject (domainObject2);
    }

    [Test]
    public void SetDomainObject_SameDomainObjectAlreadySet ()
    {
      var domainObject = Order.GetObject (DomainObjectIDs.Order1);

      var dc = DataContainer.CreateNew (DomainObjectIDs.Order1);
      dc.SetDomainObject (domainObject);
      dc.SetDomainObject (domainObject);

      Assert.That (dc.DomainObject, Is.SameAs (domainObject));
    }

    [Test]
    public void HasDomainObject_False ()
    {
      var dc = DataContainer.CreateNew (DomainObjectIDs.Order1);
      Assert.That (dc.HasDomainObject, Is.False);
    }

    [Test]
    public void HasDomainObject_True ()
    {
      var domainObject = Order.GetObject (DomainObjectIDs.Order1);

      var dc = DataContainer.CreateNew (DomainObjectIDs.Order1);
      dc.SetDomainObject (domainObject);

      Assert.That (dc.HasDomainObject, Is.True);
    }

    [Test]
    public void SetClientTransaction ()
    {
      var dc = DataContainer.CreateNew (DomainObjectIDs.Order1);

      PrivateInvoke.InvokeNonPublicMethod (dc, "SetClientTransaction", ClientTransactionMock);

      Assert.That (dc.ClientTransaction, Is.SameAs (ClientTransactionMock));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "This DataContainer has already been registered with a ClientTransaction.")]
    public void SetClientTransaction_Twice ()
    {
      var dc = DataContainer.CreateNew (DomainObjectIDs.Order1);
      PrivateInvoke.InvokeNonPublicMethod (dc, "SetClientTransaction", ClientTransactionMock);
      PrivateInvoke.InvokeNonPublicMethod (dc, "SetClientTransaction", ClientTransactionMock);
    }

    [Test]
    public void PropertyValueChanged_UpdatesState ()
    {
      var dc = DataContainer.CreateForExisting (DomainObjectIDs.Order1, "ts", pd => pd.DefaultValue);
      Assert.That (dc.State, Is.EqualTo (StateType.Unchanged));

      dc.PropertyValues[typeof (Order).FullName + ".OrderNumber"].Value = 5;

      Assert.That (dc.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void PropertyValueChanged_RaisesStateUpdated ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, "ts", pd => pd.DefaultValue);
      ClientTransactionTestHelper.RegisterDataContainer (ClientTransactionMock, dataContainer);

      CheckStateNotification (dataContainer, dc => dc.PropertyValues[typeof (Order).FullName + ".OrderNumber"].Value = 5, StateType.Changed);
      CheckStateNotification (dataContainer, dc => dc.PropertyValues[typeof (Order).FullName + ".OrderNumber"].Value = 0, StateType.Unchanged);
    }

    [Test]
    public void PropertyValueChanged_UpdatesState_After_PropertyValueEvent ()
    {
      var dc = DataContainer.CreateForExisting (DomainObjectIDs.Order1, "ts", pd => pd.DefaultValue);
      Assert.That (dc.State, Is.EqualTo (StateType.Unchanged));

      var propertyChangingCalled = false;
      var propertyChangedCalled = false;

      // It is documented that the DataContainer's state has not been updated in PropertyValueCollection.PropertyChanged:

      dc.PropertyValues.PropertyChanging += delegate
      {
        propertyChangingCalled = true;
        Assert.That (dc.State, Is.EqualTo (StateType.Unchanged));
      };
      dc.PropertyValues.PropertyChanged += delegate
      {
        propertyChangedCalled = true;
        Assert.That (dc.State, Is.EqualTo (StateType.Unchanged));
      };

      dc.PropertyValues[typeof (Order).FullName + ".OrderNumber"].Value = 5;

      Assert.That (dc.State, Is.EqualTo (StateType.Changed));
      Assert.That (propertyChangingCalled, Is.True);
      Assert.That (propertyChangedCalled, Is.True);
    }

    private string GetStorageClassPropertyName (string shortName)
    {
      return ReflectionMappingHelper.GetPropertyName (typeof (ClassWithPropertiesHavingStorageClassAttribute), shortName);
    }

    private void CheckStateNotification (DataContainer dataContainer, Action<DataContainer> action, StateType expectedState)
    {
      var listener = ClientTransactionTestHelper.CreateAndAddListenerMock (ClientTransactionMock);

      action (dataContainer);

      listener.AssertWasCalled (mock => mock.DataContainerStateUpdated (ClientTransactionMock, dataContainer, expectedState));
    }
  }
}