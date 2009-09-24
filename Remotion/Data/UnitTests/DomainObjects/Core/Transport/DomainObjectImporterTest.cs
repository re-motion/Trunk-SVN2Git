// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Transport;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transport
{
  [TestFixture]
  public class DomainObjectImporterTest : StandardMappingTest
  {
    [Test]
    public void EmptyTransport ()
    {
      var imported = ImportObjects();
      Assert.That (imported, Is.Empty);
    }

    [Test]
    public void NonEmptyTransport ()
    {
      var loadedIDs = new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Company1 };
      var imported = ImportObjects (loadedIDs);

      Assert.IsNotEmpty (imported);
      List<ObjectID> ids = imported.ConvertAll (obj => obj.ID);
      Assert.That (ids, Is.EquivalentTo (loadedIDs));
    }

    [Test]
    public void NonEmptyTransport_ObjectsBoundToTransaction ()
    {
      var loadedIDs = new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Company1 };
      var data = GetBinaryDataFor (loadedIDs);

      TransportedDomainObjects transportedObjects = Import (data);
      foreach (DomainObject domainObject in transportedObjects.TransportedObjects)
      {
        Assert.IsTrue (domainObject.HasBindingTransaction);
        Assert.AreSame (transportedObjects.DataTransaction, domainObject.GetBindingTransaction());
      }
    }

    [Test]
    [ExpectedException (typeof (TransportationException),
        ExpectedMessage = "Invalid data specified: End of Stream encountered before parsing was completed.")]
    public void InvalidData ()
    {
      Import (new byte[] { 1, 2, 3 });
    }

    [Test]
    public void NonExistingObjects_New ()
    {
      byte[] binaryData = GetBinaryDataFor (DomainObjectIDs.ClassWithAllDataTypes1);
      ModifyDatabase (() => ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1).Delete());

      var imported = ImportObjects (binaryData);
      Assert.AreEqual (StateType.New, imported[0].State);
    }

    [Test]
    public void NonExistingObjects_ChangedBySource ()
    {
      byte[] binaryData = GetBinaryDataForChangedObject (
          DomainObjectIDs.ClassWithAllDataTypes1,
          MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (ClassWithAllDataTypes), "Int32Property"),
          12);
      ModifyDatabase (() => ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1).Delete());

      var imported = ImportObjects (binaryData);
      Assert.AreEqual (StateType.New, imported[0].State);
      Assert.AreEqual (12, ((ClassWithAllDataTypes) imported[0]).Int32Property);
    }

    [Test]
    public void NonExistingObjects_NewInSource ()
    {
      var transporter = new DomainObjectTransporter();
      var outerComputer = (Computer) transporter.LoadNew (typeof (Computer), ParamList.Empty);
      byte[] binaryData = GetBinaryDataFor (transporter);

      var imported = ImportObjects (binaryData);

      Assert.AreEqual (StateType.New, imported[0].State);
      Assert.AreEqual (outerComputer.ID, imported[0].ID);
    }

    [Test]
    public void ExistingObjects_Loaded ()
    {
      byte[] binaryData = GetBinaryDataFor (DomainObjectIDs.Order1);
      var imported = ImportObjects (binaryData);

      Assert.AreEqual (StateType.Unchanged, imported[0].State);
    }

    [Test]
    public void ExistingObjects_ChangedIfNecessary ()
    {
      byte[] binaryData = GetBinaryDataFor (DomainObjectIDs.Order1, DomainObjectIDs.Order2);
      ModifyDatabase (delegate { Order.GetObject (DomainObjectIDs.Order1).OrderNumber++; });

      var imported = ImportObjects (binaryData);

      Assert.AreEqual (StateType.Changed, imported[0].State);
      Assert.AreEqual (StateType.Unchanged, imported[1].State);
    }

    [Test]
    public void ExistingObjects_ChangedBySource ()
    {
      byte[] binaryData = GetBinaryDataForChangedObject (
          DomainObjectIDs.ClassWithAllDataTypes1,
          MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (ClassWithAllDataTypes), "Int32Property"),
          12);

      var imported = ImportObjects (binaryData);

      Assert.AreEqual (StateType.Changed, imported[0].State);
      Assert.AreEqual (12, ((ClassWithAllDataTypes) imported[0]).Int32Property);
    }

    [Test]
    public void SimplePropertyChanges ()
    {
      byte[] binaryData = GetBinaryDataFor (DomainObjectIDs.Order1);
      ModifyDatabase (delegate { Order.GetObject (DomainObjectIDs.Order1).OrderNumber = 13; });

      var imported = ImportObjects (binaryData);
      Assert.IsTrue (((Order) imported[0]).Properties[typeof (Order), "OrderNumber"].HasChanged);
      Assert.AreEqual (1, ((Order) imported[0]).OrderNumber);
      Assert.IsFalse (((Order) imported[0]).Properties[typeof (Order), "DeliveryDate"].HasChanged);
    }

    [Test]
    public void RelatedObjectChanges_RealSide ()
    {
      byte[] binaryData = GetBinaryDataFor (DomainObjectIDs.Computer1, DomainObjectIDs.Computer2, DomainObjectIDs.Computer3);
      ModifyDatabase (
          delegate
          {
            Computer.GetObject (DomainObjectIDs.Computer1).Employee = null;
            Computer.GetObject (DomainObjectIDs.Computer2).Employee = Employee.GetObject (DomainObjectIDs.Employee1);
          });

      var imported = ImportObjects (binaryData);
      var loadedObject1 = (Computer) imported[0];
      var loadedObject2 = (Computer) imported[1];
      var loadedObject3 = (Computer) imported[2];

      Assert.IsTrue (loadedObject1.Properties[typeof (Computer), "Employee"].HasChanged);
      Assert.IsTrue (loadedObject2.Properties[typeof (Computer), "Employee"].HasChanged);
      Assert.IsFalse (loadedObject3.Properties[typeof (Computer), "Employee"].HasChanged);

      using (loadedObject1.GetBindingTransaction().EnterNonDiscardingScope())
      {
        Assert.AreEqual (Employee.GetObject (DomainObjectIDs.Employee3), loadedObject1.Employee);
        Assert.AreEqual (Employee.GetObject (DomainObjectIDs.Employee4), loadedObject2.Employee);
        Assert.AreEqual (Employee.GetObject (DomainObjectIDs.Employee5), loadedObject3.Employee);
      }
    }

    [Test]
    public void RelatedObjectChanges_ToNull_RealSide ()
    {
      ModifyDatabase (delegate { Computer.GetObject (DomainObjectIDs.Computer1).Employee = null; });

      byte[] binaryData = GetBinaryDataFor (DomainObjectIDs.Computer1);
      ModifyDatabase (delegate { Computer.GetObject (DomainObjectIDs.Computer1).Employee = Employee.GetObject (DomainObjectIDs.Employee3); });

      var imported = ImportObjects (binaryData);
      var loadedObject1 = (Computer) imported[0];

      Assert.IsTrue (loadedObject1.Properties[typeof (Computer), "Employee"].HasChanged);
      Assert.IsNull (loadedObject1.Employee);
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException),
        ExpectedMessage = "Object 'Employee|3c4f3fc8-0db2-4c1f-aa00-ade72e9edb32|System.Guid' could not be found.")]
    public void RelatedObjectChanges_NonExistentObject_RealSide ()
    {
      byte[] binaryData = GetBinaryDataFor (DomainObjectIDs.Computer1);
      ModifyDatabase (() => Computer.GetObject (DomainObjectIDs.Computer1).Employee.Delete());

      Import (binaryData);

      Assert.Fail ("Expected exception");
    }

    [Test]
    public void RelatedObjectChanges_VirtualSide ()
    {
      byte[] binaryData = GetBinaryDataFor (DomainObjectIDs.Employee3);
      ModifyDatabase (delegate { Employee.GetObject (DomainObjectIDs.Employee3).Computer = null; });

      var imported = ImportObjects (binaryData);

      Assert.AreEqual (StateType.Unchanged, imported[0].State);
    }

    [Test]
    public void RelatedObjectCollection_OneSide ()
    {
      byte[] binaryData = GetBinaryDataFor (DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
      ModifyDatabase (delegate { OrderItem.GetObject (DomainObjectIDs.OrderItem1).Order = Order.GetObject (DomainObjectIDs.Order2); });

      var imported = ImportObjects (binaryData);
      var loadedObject1 = (OrderItem) imported[0];
      var loadedObject2 = (OrderItem) imported[1];

      Assert.IsTrue (loadedObject1.Properties[typeof (OrderItem), "Order"].HasChanged);
      Assert.IsFalse (loadedObject2.Properties[typeof (OrderItem), "Order"].HasChanged);

      using (loadedObject1.GetBindingTransaction().EnterNonDiscardingScope())
      {
        Assert.AreEqual (Order.GetObject (DomainObjectIDs.Order1), loadedObject1.Order);
        Assert.AreEqual (Order.GetObject (DomainObjectIDs.Order1), loadedObject2.Order);
      }
    }

    [Test]
    public void RelatedObjectCollection_ManySide ()
    {
      byte[] binaryData = GetBinaryDataFor (DomainObjectIDs.Order1);
      ModifyDatabase (delegate { Order.GetObject (DomainObjectIDs.Order1).OrderItems[0].Order = Order.GetObject (DomainObjectIDs.Order2); });

      var imported = ImportObjects (binaryData);

      Assert.IsFalse (((Order) imported[0]).Properties[typeof (Order), "OrderItems"].HasChanged);
    }

    [Test]
    public void ChangedBySource_PropertyValue ()
    {
      byte[] binaryData = GetBinaryDataForChangedObject (
          DomainObjectIDs.Order1, MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (Order), "OrderNumber"), 2);

      var imported = ImportObjects (binaryData);

      Assert.AreEqual (2, ((Order) imported[0]).OrderNumber);
    }

    [Test]
    public void ChangedBySource_RelatedObjectToExistingObject_RealSide ()
    {
      var transporter = new DomainObjectTransporter();
      transporter.Load (DomainObjectIDs.Computer1);
      transporter.Load (DomainObjectIDs.Computer2);
      transporter.Load (DomainObjectIDs.Employee3);
      transporter.Load (DomainObjectIDs.Employee4);
      var computer = (Computer) transporter.GetTransportedObject (DomainObjectIDs.Computer1);
      computer.Employee = (Employee) transporter.GetTransportedObject (DomainObjectIDs.Employee4);

      byte[] binaryData = GetBinaryDataFor (transporter);
      var imported = ImportObjects (binaryData);
      
      var loadedObject1 = (Computer) imported.Find (obj => obj.ID == DomainObjectIDs.Computer1);
      var loadedObject2 = (Employee) imported.Find (obj => obj.ID == DomainObjectIDs.Employee4);
      Assert.AreSame (loadedObject2, loadedObject1.Employee);
    }

    [Test]
    public void ChangedBySource_RelatedObjectToExistingObject_VirtualSide ()
    {
      var transporter = new DomainObjectTransporter();
      transporter.Load (DomainObjectIDs.Computer1);
      transporter.Load (DomainObjectIDs.Computer2);
      transporter.Load (DomainObjectIDs.Employee3);
      transporter.Load (DomainObjectIDs.Employee4);
      var employee = (Employee) transporter.GetTransportedObject (DomainObjectIDs.Employee3);
      employee.Computer = (Computer) transporter.GetTransportedObject (DomainObjectIDs.Computer2);

      byte[] binaryData = GetBinaryDataFor (transporter);
      var imported = ImportObjects (binaryData);
      var loadedObject1 = (Computer) imported.Find (obj => obj.ID == DomainObjectIDs.Computer2);
      var loadedObject2 = (Employee) imported.Find (obj => obj.ID == DomainObjectIDs.Employee3);
      Assert.AreSame (loadedObject1, loadedObject2.Computer);
    }

    [Test]
    public void ChangedBySource_RelatedObjectToNew ()
    {
      var transporter = new DomainObjectTransporter();
      var computer = (Computer) transporter.LoadNew (typeof (Computer), ParamList.Empty);
      var employee = (Employee) transporter.LoadNew (typeof (Employee), ParamList.Empty);

      computer.Employee = employee;

      byte[] binaryData = GetBinaryDataFor (transporter);
      var imported = ImportObjects (binaryData);

      var loadedObject1 = (Computer) imported.Find (obj => obj is Computer);
      var loadedObject2 = (Employee) imported.Find (obj => obj is Employee);
      Assert.AreSame (loadedObject2, loadedObject1.Employee);
    }

    [Test]
    public void SpecialStrategy ()
    {
      TransportItem[] items;
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        items = new[] { TransportItem.PackageDataContainer (Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer) };
      }
      
      var repository = new MockRepository();
      var strategyMock = repository.StrictMock<IImportStrategy>();
      var streamFake = repository.Stub<Stream> ();
      
      strategyMock.Expect (mock => mock.Import (streamFake)).Return (items);

      strategyMock.Replay();

      var importer = DomainObjectImporter.CreateImporterFromStream (streamFake, strategyMock);
      TransportedDomainObjects result = importer.GetImportedObjects();
      Assert.That (result.TransportedObjects, Is.EquivalentTo (result.DataTransaction.GetObjects<Order> (DomainObjectIDs.Order1)));

      strategyMock.VerifyAllExpectations();
    }

    private byte[] GetBinaryDataForChangedObject (ObjectID id, string propertyToTouch, object newValue)
    {
      var transporter = new DomainObjectTransporter();
      transporter.Load (id);
      DomainObject domainObject = transporter.GetTransportedObject (id);
      new PropertyIndexer (domainObject)[propertyToTouch].SetValueWithoutTypeCheck (newValue);
      return GetBinaryDataFor (transporter);
    }

    private byte[] GetBinaryDataFor (params ObjectID[] ids)
    {
      var transporter = new DomainObjectTransporter();
      foreach (ObjectID id in ids)
        transporter.Load (id);
      return GetBinaryDataFor (transporter);
    }

    private byte[] GetBinaryDataFor (DomainObjectTransporter transporter)
    {
      using (var stream = new MemoryStream())
      {
        transporter.Export (stream);
        return stream.ToArray();
      }
    }

    private List<DomainObject> ImportObjects (params ObjectID[] objectsToImport)
    {
      byte[] binaryData = GetBinaryDataFor (objectsToImport);
      return ImportObjects (binaryData);
    }

    private List<DomainObject> ImportObjects (byte[] binaryData)
    {
      TransportedDomainObjects transportedObjects = Import (binaryData);
      return new List<DomainObject> (transportedObjects.TransportedObjects);
    }

    private TransportedDomainObjects Import (byte[] binaryData)
    {
      using (var stream = new MemoryStream (binaryData))
      {
        return DomainObjectImporter.CreateImporterFromStream (stream, BinaryImportStrategy.Instance).GetImportedObjects();
      }
    }

    private void ModifyDatabase (Action changer)
    {
      SetDatabaseModifyable();
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        changer();
        ClientTransaction.Current.Commit();
      }
    }
  }
}