/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Transport;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Transport
{
  [TestFixture]
  public class DomainObjectImporterTest : StandardMappingTest
  {
    [Test]
    public void EmptyTransport ()
    {
      CheckImport (delegate (List<DomainObject> importedObjects)
      {
        Assert.IsEmpty (importedObjects);
      });
    }

    [Test]
    public void NonEmptyTransport ()
    {
      ObjectID[] loadedObjects = new ObjectID[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Company1};

      CheckImport (delegate (List<DomainObject> importedObjects)
      {
        Assert.IsNotEmpty (importedObjects);
        List<ObjectID> ids = importedObjects.ConvertAll<ObjectID> (delegate (DomainObject obj) { return obj.ID; });
        Assert.That (ids, Is.EquivalentTo (loadedObjects));
      }, loadedObjects);
    }

    [Test]
    public void NonEmptyTransport_ObjectsBoundToTransaction ()
    {
      ObjectID[] loadedObjects = new ObjectID[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Company1 };
      DomainObjectTransporter transporter = new DomainObjectTransporter();
      foreach (ObjectID id in loadedObjects)
        transporter.Load (id);

      TransportedDomainObjects transportedObjects = new DomainObjectImporter (transporter.GetBinaryTransportData(), BinaryImportStrategy.Instance).GetImportedObjects();
      foreach (DomainObject domainObject in transportedObjects.TransportedObjects)
      {
        Assert.IsTrue (domainObject.IsBoundToSpecificTransaction);
        Assert.AreSame (transportedObjects.DataTransaction, domainObject.ClientTransaction);
      }
    }

    [Test]
    [ExpectedException (typeof (TransportationException), ExpectedMessage = "Invalid data specified: End of Stream encountered before parsing was completed.")]
    public void InvalidData ()
    {
      byte[] data = new byte[] { 1, 2, 3 };
      new DomainObjectImporter (data, BinaryImportStrategy.Instance);
    }

    [Test]
    public void NonExistingObjects_New ()
    {
      byte[] binaryData = GetBinaryDataFor (DomainObjectIDs.ClassWithAllDataTypes1);
      ModifyDatabase (delegate { ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1).Delete(); });

      CheckImport (delegate (List<DomainObject> importedObjects)
      {
        DomainObject loadedObject1 = importedObjects[0];
        Assert.AreEqual (StateType.New, loadedObject1.State);
      }, binaryData);
    }

    [Test]
    public void NonExistingObjects_ChangedBySource ()
    {
      byte[] binaryData = GetBinaryDataForChangedObject (DomainObjectIDs.ClassWithAllDataTypes1, 
          ReflectionUtility.GetPropertyName (typeof (ClassWithAllDataTypes), "Int32Property"), 12);
      ModifyDatabase (delegate { ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1).Delete (); });

      CheckImport (delegate (List<DomainObject> importedObjects)
      {
        ClassWithAllDataTypes loadedObject1 = (ClassWithAllDataTypes) importedObjects[0];
        Assert.AreEqual (StateType.New, loadedObject1.State);
        Assert.AreEqual (12, loadedObject1.Int32Property);
      }, binaryData);
    }

    [Test]
    public void NonExistingObjects_NewInSource ()
    {
      DomainObjectTransporter transporter = new DomainObjectTransporter ();
      Computer outerComputer = (Computer) transporter.LoadNew (typeof (Computer));
      byte[] binaryData = transporter.GetBinaryTransportData ();

      CheckImport (delegate (List<DomainObject> importedObjects)
      {
        Computer loadedObject1 = (Computer) importedObjects[0];
        Assert.AreEqual (StateType.New, loadedObject1.State);
        Assert.AreEqual (outerComputer.ID, loadedObject1.ID);
      }, binaryData);
    }

    [Test]
    public void ExistingObjects_Loaded ()
    {
      byte[] binaryData = GetBinaryDataFor (DomainObjectIDs.Order1);

      CheckImport (delegate (List<DomainObject> importedObjects)
      {
        DomainObject loadedObject1 = importedObjects[0];
        Assert.AreEqual (StateType.Unchanged, loadedObject1.State);
      }, binaryData);
    }

    [Test]
    public void ExistingObjects_ChangedIfNecessary ()
    {
      byte[] binaryData = GetBinaryDataFor (DomainObjectIDs.Order1, DomainObjectIDs.Order2);
      ModifyDatabase (delegate { Order.GetObject (DomainObjectIDs.Order1).OrderNumber++; });

      CheckImport (delegate (List<DomainObject> importedObjects)
      {
        DomainObject loadedObject1 = importedObjects[0];
        Assert.AreEqual (StateType.Changed, loadedObject1.State);
        DomainObject loadedObject2 = importedObjects[1];
        Assert.AreEqual (StateType.Unchanged, loadedObject2.State);
      }, binaryData);
    }

    [Test]
    public void ExistingObjects_ChangedBySource ()
    {
      byte[] binaryData = GetBinaryDataForChangedObject (DomainObjectIDs.ClassWithAllDataTypes1,
          ReflectionUtility.GetPropertyName (typeof (ClassWithAllDataTypes), "Int32Property"), 12);

      CheckImport (delegate (List<DomainObject> importedObjects)
      {
        ClassWithAllDataTypes loadedObject1 = (ClassWithAllDataTypes) importedObjects[0];
        Assert.AreEqual (StateType.Changed, loadedObject1.State);
        Assert.AreEqual (12, loadedObject1.Int32Property);
      }, binaryData);
    }

    [Test]
    public void SimplePropertyChanges ()
    {
      byte[] binaryData = GetBinaryDataFor (DomainObjectIDs.Order1);
      ModifyDatabase (delegate { Order.GetObject (DomainObjectIDs.Order1).OrderNumber = 13; });

      CheckImport (delegate (List<DomainObject> importedObjects)
      {
        Order loadedObject1 = (Order) importedObjects[0];
        Assert.IsTrue (loadedObject1.Properties[typeof (Order), "OrderNumber"].HasChanged);
        Assert.AreEqual (1, loadedObject1.OrderNumber);
        Assert.IsFalse (loadedObject1.Properties[typeof (Order), "DeliveryDate"].HasChanged);
      }, binaryData);
    }

    [Test]
    public void RelatedObjectChanges_RealSide ()
    {
      byte[] binaryData = GetBinaryDataFor (DomainObjectIDs.Computer1, DomainObjectIDs.Computer2, DomainObjectIDs.Computer3);
      ModifyDatabase (delegate
      {
        Computer.GetObject (DomainObjectIDs.Computer1).Employee = null;
        Computer.GetObject (DomainObjectIDs.Computer2).Employee = Employee.GetObject(DomainObjectIDs.Employee1);
      });

      CheckImport (delegate (List<DomainObject> importedObjects)
      {
        Computer loadedObject1 = (Computer) importedObjects[0];
        Computer loadedObject2 = (Computer) importedObjects[1];
        Computer loadedObject3 = (Computer) importedObjects[2];
        
        Assert.IsTrue (loadedObject1.Properties[typeof (Computer), "Employee"].HasChanged);
        Assert.IsTrue (loadedObject2.Properties[typeof (Computer), "Employee"].HasChanged);
        Assert.IsFalse (loadedObject3.Properties[typeof (Computer), "Employee"].HasChanged);

        using (loadedObject1.ClientTransaction.EnterNonDiscardingScope ())
        {
          Assert.AreEqual (Employee.GetObject (DomainObjectIDs.Employee3), loadedObject1.Employee);
          Assert.AreEqual (Employee.GetObject (DomainObjectIDs.Employee4), loadedObject2.Employee);
          Assert.AreEqual (Employee.GetObject (DomainObjectIDs.Employee5), loadedObject3.Employee);
        }
      }, binaryData);
    }

    [Test]
    public void RelatedObjectChanges_ToNull_RealSide ()
    {
      ModifyDatabase (delegate
      {
        Computer.GetObject (DomainObjectIDs.Computer1).Employee = null;
      });

      byte[] binaryData = GetBinaryDataFor (DomainObjectIDs.Computer1);
      ModifyDatabase (delegate
      {
        Computer.GetObject (DomainObjectIDs.Computer1).Employee = Employee.GetObject (DomainObjectIDs.Employee3);
      });

      CheckImport (delegate (List<DomainObject> importedObjects)
      {
        Computer loadedObject1 = (Computer) importedObjects[0];

        Assert.IsTrue (loadedObject1.Properties[typeof (Computer), "Employee"].HasChanged);

        Assert.IsNull (loadedObject1.Employee);
      }, binaryData);
    }

    [Test]
    [ExpectedException (typeof (ObjectNotFoundException),
        ExpectedMessage = "Object 'Employee|3c4f3fc8-0db2-4c1f-aa00-ade72e9edb32|System.Guid' could not be found.")]
    public void RelatedObjectChanges_NonExistentObject_RealSide ()
    {
      byte[] binaryData = GetBinaryDataFor (DomainObjectIDs.Computer1);
      ModifyDatabase (delegate
      {
        Computer.GetObject (DomainObjectIDs.Computer1).Employee.Delete ();
      });

      Import (binaryData);

      Assert.Fail ("Expected exception");
    }

    [Test]
    public void RelatedObjectChanges_VirtualSide ()
    {
      byte[] binaryData = GetBinaryDataFor (DomainObjectIDs.Employee3);
      ModifyDatabase (delegate
      {
        Employee.GetObject (DomainObjectIDs.Employee3).Computer = null;
      });

      CheckImport (delegate (List<DomainObject> importedObjects)
      {
        Employee loadedObject1 = (Employee) importedObjects[0];

        Assert.AreEqual (StateType.Unchanged, loadedObject1.State);
      }, binaryData);
    }

    [Test]
    public void RelatedObjectCollection_OneSide ()
    {
      byte[] binaryData = GetBinaryDataFor (DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2);
      ModifyDatabase (delegate
      {
        OrderItem.GetObject (DomainObjectIDs.OrderItem1).Order = Order.GetObject (DomainObjectIDs.Order2);
      });

      CheckImport (delegate (List<DomainObject> importedObjects)
      {
        OrderItem loadedObject1 = (OrderItem) importedObjects[0];
        OrderItem loadedObject2 = (OrderItem) importedObjects[1];

        Assert.IsTrue (loadedObject1.Properties[typeof (OrderItem), "Order"].HasChanged);
        Assert.IsFalse (loadedObject2.Properties[typeof (OrderItem), "Order"].HasChanged);

        using (loadedObject1.ClientTransaction.EnterNonDiscardingScope ())
        {
          Assert.AreEqual (Order.GetObject (DomainObjectIDs.Order1), loadedObject1.Order);
          Assert.AreEqual (Order.GetObject (DomainObjectIDs.Order1), loadedObject2.Order);
        }
      }, binaryData);
    }

    [Test]
    public void RelatedObjectCollection_ManySide ()
    {
      byte[] binaryData = GetBinaryDataFor (DomainObjectIDs.Order1);
      ModifyDatabase (delegate
      {
        Order.GetObject (DomainObjectIDs.Order1).OrderItems[0].Order = Order.GetObject (DomainObjectIDs.Order2);
      });

      CheckImport (delegate (List<DomainObject> importedObjects)
      {
        Order loadedObject1 = (Order) importedObjects[0];

        Assert.IsFalse (loadedObject1.Properties[typeof (Order), "OrderItems"].HasChanged);
      }, binaryData);
    }

    [Test]
    public void ChangedBySource_PropertyValue ()
    {
      byte[] binaryData = GetBinaryDataForChangedObject (DomainObjectIDs.Order1, ReflectionUtility.GetPropertyName (typeof (Order), "OrderNumber"), 2);
      CheckImport (delegate (List<DomainObject> importedObjects)
      {
        Order loadedObject1 = (Order) importedObjects[0];

        Assert.AreEqual (2, loadedObject1.OrderNumber);
      }, binaryData);
    }

    [Test]
    public void ChangedBySource_RelatedObjectToExistingObject_RealSide ()
    {
      DomainObjectTransporter transporter = new DomainObjectTransporter ();
      transporter.Load (DomainObjectIDs.Computer1);
      transporter.Load (DomainObjectIDs.Computer2);
      transporter.Load (DomainObjectIDs.Employee3);
      transporter.Load (DomainObjectIDs.Employee4);
      Computer computer = (Computer) transporter.GetTransportedObject (DomainObjectIDs.Computer1);
      computer.Employee = (Employee) transporter.GetTransportedObject (DomainObjectIDs.Employee4);

      byte[] binaryData = transporter.GetBinaryTransportData ();
      CheckImport (delegate (List<DomainObject> importedObjects)
      {
        Computer loadedObject1 = (Computer) importedObjects.Find (delegate (DomainObject obj) { return obj.ID == DomainObjectIDs.Computer1; });
        Employee loadedObject2 = (Employee) importedObjects.Find (delegate (DomainObject obj) { return obj.ID == DomainObjectIDs.Employee4; });
        Assert.AreSame (loadedObject2, loadedObject1.Employee);
      }, binaryData);
    }

    [Test]
    public void ChangedBySource_RelatedObjectToExistingObject_VirtualSide ()
    {
      DomainObjectTransporter transporter = new DomainObjectTransporter ();
      transporter.Load (DomainObjectIDs.Computer1);
      transporter.Load (DomainObjectIDs.Computer2);
      transporter.Load (DomainObjectIDs.Employee3);
      transporter.Load (DomainObjectIDs.Employee4);
      Employee employee = (Employee) transporter.GetTransportedObject (DomainObjectIDs.Employee3);
      employee.Computer = (Computer) transporter.GetTransportedObject (DomainObjectIDs.Computer2);

      byte[] binaryData = transporter.GetBinaryTransportData ();
      CheckImport (delegate (List<DomainObject> importedObjects)
      {
        Computer loadedObject1 = (Computer) importedObjects.Find (delegate (DomainObject obj) { return obj.ID == DomainObjectIDs.Computer2; });
        Employee loadedObject2 = (Employee) importedObjects.Find (delegate (DomainObject obj) { return obj.ID == DomainObjectIDs.Employee3; });
        Assert.AreSame (loadedObject1, loadedObject2.Computer);
      }, binaryData);
    }

    [Test]
    public void ChangedBySource_RelatedObjectToNew ()
    {
      DomainObjectTransporter transporter = new DomainObjectTransporter ();
      Computer computer = (Computer) transporter.LoadNew (typeof (Computer));
      Employee employee = (Employee) transporter.LoadNew (typeof (Employee));

      computer.Employee = employee;

      byte[] binaryData = transporter.GetBinaryTransportData ();
      CheckImport (delegate (List<DomainObject> importedObjects)
      {
        Computer loadedObject1 = (Computer) importedObjects.Find (delegate (DomainObject obj) { return obj is Computer; });
        Employee loadedObject2 = (Employee) importedObjects.Find (delegate (DomainObject obj) { return obj is Employee; });
        Assert.AreSame (loadedObject2, loadedObject1.Employee);
      }, binaryData);
    }

    [Test]
    public void SpecialStrategy ()
    {
      MockRepository repository = new MockRepository ();
      IImportStrategy mockStrategy = repository.CreateMock<IImportStrategy> ();
      byte[] data = new byte[] { 1, 2, 3 };
      TransportItem[] items;
      using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope ())
      {
        items = new TransportItem[] { TransportItem.PackageDataContainer (Order.GetObject (DomainObjectIDs.Order1).InternalDataContainer) };
      }

      Expect.Call (mockStrategy.Import (data)).Return (items);

      repository.ReplayAll ();

      DomainObjectImporter importer = new DomainObjectImporter (data, mockStrategy);
      TransportedDomainObjects result = importer.GetImportedObjects ();
      Assert.That (result.TransportedObjects, Is.EquivalentTo (result.DataTransaction.GetObjects<Order> (DomainObjectIDs.Order1)));

      repository.VerifyAll ();
    }
    
    private byte[] GetBinaryDataForChangedObject (ObjectID id, string propertyToTouch, object newValue)
    {
      DomainObjectTransporter transporter = new DomainObjectTransporter ();
      transporter.Load (id);
      DomainObject domainObject = transporter.GetTransportedObject (id);
      new PropertyIndexer (domainObject)[propertyToTouch].SetValueWithoutTypeCheck (newValue);
      return transporter.GetBinaryTransportData();
    }

    private byte[] GetBinaryDataFor (params ObjectID[] ids)
    {
      DomainObjectTransporter transporter = new DomainObjectTransporter ();
      foreach (ObjectID id in ids)
        transporter.Load (id);
      return transporter.GetBinaryTransportData ();
    }

    private void CheckImport (Proc<List<DomainObject>> checker, params ObjectID[] objectsToImport)
    {
      byte[] binaryData = GetBinaryDataFor (objectsToImport);
      CheckImport (checker, binaryData);
    }

    private void CheckImport (Proc<List<DomainObject>> checker, byte[] binaryData)
    {
      TransportedDomainObjects transportedObjects = Import(binaryData);
      List<DomainObject> domainObjects = new List<DomainObject> (transportedObjects.TransportedObjects);
      checker (domainObjects);
    }

    private TransportedDomainObjects Import (byte[] binaryData)
    {
      return new DomainObjectImporter (binaryData, BinaryImportStrategy.Instance).GetImportedObjects ();
    }

    private void ModifyDatabase (Proc changer)
    {
      SetDatabaseModifyable ();
      using (ClientTransaction.NewRootTransaction ().EnterNonDiscardingScope())
      {
        changer();
        ClientTransaction.Current.Commit ();
      }
    }
  }
}
