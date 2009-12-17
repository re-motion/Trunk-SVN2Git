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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.Core.Resources;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
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
      base.SetUp();

      Guid idValue = Guid.NewGuid();
      ReflectionBasedClassDefinition orderClass =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition ("Order", "Order", c_testDomainProviderID, typeof (Order), false);

      _newDataContainer = DataContainer.CreateNew (new ObjectID ("Order", idValue));
      _existingDataContainer = DataContainer.CreateForExisting (
          new ObjectID ("Order", idValue),
          null,
          propertyDefinition => propertyDefinition.DefaultValue);

      _nameDefinition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
          orderClass, "Name", "Name", typeof (string), 100);
      _nameProperty = new PropertyValue (_nameDefinition, "Arthur Dent");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The ClassDefinition 'Remotion.Data.DomainObjects.Mapping."
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

      var eventReceiver = new PropertyValueContainerEventReceiver (_newDataContainer, false);

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

      var eventReceiver = new PropertyValueContainerEventReceiver (_newDataContainer, true);

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

      var eventReceiver = new PropertyValueContainerEventReceiver (_existingDataContainer, false);

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

      var eventReceiver = new PropertyValueContainerEventReceiver (_existingDataContainer, true);

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
      DataContainer dataContainer = TestDataContainerFactory.CreateOrder1DataContainer();
      var id = (ObjectID) dataContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Customer");
      Assert.IsNotNull (id);
    }

    [Test]
    public void GetNullObjectID ()
    {
      var id = new ObjectID ("Official", 1);
      DataContainer container = DataContainer.CreateNew (id);

      PropertyDefinition reportsToDefinition =
          ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition (
              (ReflectionBasedClassDefinition) container.ClassDefinition, "ReportsTo", "ReportsTo", typeof (string), true, 100);

      container.PropertyValues.Add (new PropertyValue (reportsToDefinition, null));

      Assert.IsNull (container.GetValue ("ReportsTo"));
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
      Assert.AreEqual (StateType.Changed, container.State);
      Assert.AreEqual (42, container.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"));

      container["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"] = 1;
      Assert.AreEqual (StateType.Unchanged, container.State);
      Assert.AreEqual (1, container.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber"));
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
      DataContainer dataContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer();

      ResourceManager.IsEqualToImage1 (
          (byte[]) dataContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"));
      Assert.IsNull (dataContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"));
    }

    [Test]
    public void SetBytes ()
    {
      DataContainer dataContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer();

      dataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"] = new byte[0];
      ResourceManager.IsEmptyImage (
          (byte[]) dataContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.BinaryProperty"));

      dataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"] = null;
      Assert.IsNull (dataContainer.GetValue ("Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"));
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
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "A DataContainer cannot be discarded while it doesn't have an "
                                                                              + "associated DomainObject.")]
    public void DiscardWithoutDomainObjectThrows ()
    {
      DataContainer dataContainerWithoutDomainObject = DataContainer.CreateNew (DomainObjectIDs.Order1);
      dataContainerWithoutDomainObject.RegisterNewDataContainer (ClientTransactionMock);

      PrivateInvoke.InvokeNonPublicMethod (dataContainerWithoutDomainObject, "Delete");
      Assert.Fail ("Expected exception");
    }

    [Test]
    public void GetIDEvenPossibleWhenDiscarded ()
    {
      Order order = Order.NewObject();
      DataContainer dataContainer = order.InternalDataContainer;
      order.Delete();
      Assert.IsTrue (dataContainer.IsDiscarded);
      Assert.AreEqual (order.ID, dataContainer.ID);
    }

    [Test]
    public void GetDomainObjectEvenPossibleWhenDiscarded ()
    {
      Order order = Order.NewObject();
      DataContainer dataContainer = order.InternalDataContainer;
      order.Delete();
      Assert.IsTrue (dataContainer.IsDiscarded);
      Assert.AreSame (order, dataContainer.DomainObject);
    }

    [Test]
    public void MarkAsChanged ()
    {
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      DataContainer dataContainer = order.InternalDataContainer;
      Assert.AreEqual (StateType.Unchanged, dataContainer.State);
      dataContainer.MarkAsChanged();
      Assert.AreEqual (StateType.Changed, dataContainer.State);

      ClientTransactionMock.Rollback();
      Assert.AreEqual (StateType.Unchanged, dataContainer.State);

      SetDatabaseModifyable();

      dataContainer.MarkAsChanged();
      Assert.AreEqual (StateType.Changed, dataContainer.State);

      ClientTransactionMock.Commit();
      Assert.AreEqual (StateType.Unchanged, dataContainer.State);

      DataContainer clone = dataContainer.Clone (DomainObjectIDs.Order1);
      Assert.AreEqual (StateType.Unchanged, clone.State);

      dataContainer.MarkAsChanged();
      Assert.AreEqual (StateType.Changed, dataContainer.State);

      clone = dataContainer.Clone (DomainObjectIDs.Order1);
      Assert.AreEqual (StateType.Changed, clone.State);
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
      var domainObject2 = new ClientTransactionMock().GetObject (DomainObjectIDs.Order1);

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
    public void RegisterNewDataContainer ()
    {
      var dc = DataContainer.CreateNew (DomainObjectIDs.Order1);

      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);
      var collectionEndPointID = new RelationEndPointID (DomainObjectIDs.Order1, typeof (Order).FullName + ".OrderItems");
      var realEndPointID = new RelationEndPointID (DomainObjectIDs.Order1, typeof (Order).FullName + ".Customer");
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[collectionEndPointID], Is.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[realEndPointID], Is.Null);

      dc.RegisterNewDataContainer (ClientTransactionMock);

      Assert.That (dc.ClientTransaction, Is.SameAs (ClientTransactionMock));
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1], Is.SameAs (dc));

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[collectionEndPointID].ObjectID, Is.EqualTo (dc.ID));
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[realEndPointID].ObjectID, Is.EqualTo (dc.ID));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This DataContainer has not been associated with a DomainObject yet.")]
    public void RegisterNewDataContainer_DoesNotSetDomainObject ()
    {
      var dc = DataContainer.CreateNew (DomainObjectIDs.Order1);

      dc.RegisterNewDataContainer (ClientTransactionMock);

      Dev.Null = dc.DomainObject;
    }


    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "This DataContainer has already been registered with a ClientTransaction.")]
    public void RegisterNewDataContainer_Twice ()
    {
      var dc = DataContainer.CreateNew (DomainObjectIDs.Order1);
      dc.RegisterNewDataContainer (ClientTransactionMock);
      dc.RegisterNewDataContainer (ClientTransactionMock);
    }

    [Test]
    public void RegisterLoadedDataContainer ()
    {
      var dc = DataContainer.CreateForExisting (DomainObjectIDs.Order1, "ts", pd => pd.DefaultValue);

      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1], Is.Null);
      var collectionEndPointID = new RelationEndPointID (DomainObjectIDs.Order1, typeof (Order).FullName + ".OrderItems");
      var realEndPointID = new RelationEndPointID (DomainObjectIDs.Order1, typeof (Order).FullName + ".Customer");
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[collectionEndPointID], Is.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[realEndPointID], Is.Null);

      dc.RegisterLoadedDataContainer (ClientTransactionMock);

      Assert.That (dc.ClientTransaction, Is.SameAs (ClientTransactionMock));
      Assert.That (ClientTransactionMock.DataManager.DataContainerMap[DomainObjectIDs.Order1], Is.SameAs (dc));

      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[collectionEndPointID], Is.Null);
      Assert.That (ClientTransactionMock.DataManager.RelationEndPointMap[realEndPointID].ObjectID, Is.EqualTo (dc.ID));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This DataContainer has not been associated with a DomainObject yet.")]
    public void RegisterLoadedDataContainer_DoesNotSetDomainObject ()
    {
      var dc = DataContainer.CreateForExisting (DomainObjectIDs.Order1, "ts", pd => pd.DefaultValue);

      dc.RegisterLoadedDataContainer (ClientTransactionMock);

      Dev.Null = dc.DomainObject;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "This DataContainer has already been registered with a ClientTransaction.")]
    public void RegisterLoadedDataContainer_Twice ()
    {
      var dc = DataContainer.CreateForExisting (DomainObjectIDs.Order1, "ts", pd => pd.DefaultValue);
      dc.RegisterLoadedDataContainer (ClientTransactionMock);
      dc.RegisterLoadedDataContainer (ClientTransactionMock);
    }

    [Test]
    public void State_Changed_InOnPropertyChanged ()
    {
      var dc = DataContainer.CreateForExisting (DomainObjectIDs.Order1, "ts", pd => pd.DefaultValue);
      Assert.That (dc.State, Is.EqualTo (StateType.Unchanged));

      var propertyChangingCalled = false;
      var propertyChangedCalled = false;

      dc.PropertyChanging += delegate
      {
        propertyChangingCalled = true;
        Assert.That (dc.State, Is.EqualTo (StateType.Unchanged));
      };
      dc.PropertyChanged += delegate
      {
        propertyChangedCalled = true;
        Assert.That (dc.State, Is.EqualTo (StateType.Changed));
      };

      dc.PropertyValues[typeof (Order).FullName + ".OrderNumber"].Value = 5;

      Assert.That (dc.State, Is.EqualTo (StateType.Changed));
      Assert.That (propertyChangingCalled, Is.True);
      Assert.That (propertyChangedCalled, Is.True);
    }

    [Test]
    public void State_Changed_InPropertyValueOnChanged ()
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
      return Configuration.NameResolver.GetPropertyName (typeof (ClassWithPropertiesHavingStorageClassAttribute), shortName);
    }
  }
}