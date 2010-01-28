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
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Transport;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_List = Rhino.Mocks.Constraints.List;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transport
{
  [TestFixture]
  public class TransportedDomainObjectsTest : StandardMappingTest
  {
    [Test]
    public void EmptyTransport ()
    {
      ClientTransaction dataTransaction = ClientTransaction.CreateRootTransaction();
      var transportedObjects = new TransportedDomainObjects (dataTransaction, new List<DomainObject>());

      Assert.That (transportedObjects.DataTransaction, Is.Not.Null);
      Assert.That (transportedObjects.DataTransaction, Is.SameAs (dataTransaction));
      Assert.IsEmpty (GetTransportedObjects (transportedObjects));
    }

    [Test]
    public void TransportedObjectsStayConstant_WhenTransactionIsManipulated ()
    {
      var transportedObjects = new TransportedDomainObjects (ClientTransaction.CreateRootTransaction(), new List<DomainObject>());

      Assert.IsEmpty (GetTransportedObjects (transportedObjects));

      using (transportedObjects.DataTransaction.EnterNonDiscardingScope())
      {
        Order.GetObject (DomainObjectIDs.Order1);
      }

      Assert.IsEmpty (GetTransportedObjects (transportedObjects));
    }

    [Test]
    public void NonEmptyTransport ()
    {
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction();
      var transportedObjectList = new List<DomainObject>();
      using (newTransaction.EnterNonDiscardingScope())
      {
        transportedObjectList.Add (Order.GetObject (DomainObjectIDs.Order1));
        transportedObjectList.Add (Order.GetObject (DomainObjectIDs.Order2));
        transportedObjectList.Add (Company.GetObject (DomainObjectIDs.Company1));
      }

      var transportedObjects = new TransportedDomainObjects (newTransaction, transportedObjectList);

      Assert.That (transportedObjects.DataTransaction, Is.Not.Null);
      Assert.IsNotEmpty (GetTransportedObjects (transportedObjects));
      List<ObjectID> ids = GetTransportedObjects (transportedObjects).ConvertAll (obj => obj.ID);
      Assert.That (ids, Is.EquivalentTo (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Company1 }));
    }

    [Test]
    public void FinishTransport_CallsCommit ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects (
          DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);
      var mockRepository = new MockRepository();
      var mock = mockRepository.StrictMock<IClientTransactionExtension>();

      mock.Committing (null, null);
      LastCall.Constraints (Mocks_Is.Same (transportedObjects.DataTransaction), Mocks_List.Equal (GetTransportedObjects (transportedObjects)));
      mock.Committed (null, null);
      LastCall.Constraints (Mocks_Is.Same (transportedObjects.DataTransaction), Mocks_List.Equal (GetTransportedObjects (transportedObjects)));

      mockRepository.ReplayAll();

      transportedObjects.DataTransaction.Extensions.Add ("mock", mock);
      transportedObjects.FinishTransport();

      mockRepository.VerifyAll();
    }

    [Test]
    public void FinishTransport_FilterCalledForEachChangedObject ()
    {
      var transporter = new DomainObjectTransporter();
      transporter.Load (DomainObjectIDs.ClassWithAllDataTypes1);
      transporter.Load (DomainObjectIDs.ClassWithAllDataTypes2);
      transporter.Load (DomainObjectIDs.Order1);

      ModifyDatabase (
          delegate
          {
            ClassWithAllDataTypes c1 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
            ClassWithAllDataTypes c2 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2);
            c1.Delete();
            c2.Delete();
          });

      TransportedDomainObjects transportedObjects = Transport (transporter);

      var expectedObjects = new List<DomainObject>();
      using (transportedObjects.DataTransaction.EnterNonDiscardingScope())
      {
        expectedObjects.Add (ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1));
        expectedObjects.Add (ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2));
      }

      var filteredObjects = new List<DomainObject>();
      transportedObjects.FinishTransport (
          delegate (DomainObject domainObject)
          {
            filteredObjects.Add (domainObject);
            return true;
          });
      Assert.That (filteredObjects, Is.EquivalentTo (expectedObjects));
    }

    [Test]
    public void FinishTransport_WithoutFilter ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects (
          DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);

      transportedObjects.FinishTransport();

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        ClassWithAllDataTypes c3 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
        ClassWithAllDataTypes c4 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2);

        Assert.That (c3.Int32Property, Is.EqualTo (2147483647));
        Assert.That (c4.Int32Property, Is.EqualTo (-2147483647));
      }
    }

    [Test]
    public void FinishTransport_FilterNew ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects (
          DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);

      transportedObjects.FinishTransport (transportedObject => 
          { 
            Assert.That (transportedObject.State == StateType.New); 
            return ((ClassWithAllDataTypes) transportedObject).Int32Property < 0; 
          });

      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        try
        {
          ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
          Assert.Fail ("Expected ObjectNotFoundException");
        }
        catch (ObjectNotFoundException)
        {
          // ok
        }

        ClassWithAllDataTypes c2 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2);
        Assert.That (c2.Int32Property, Is.EqualTo (-2147483647));
      }
    }

    [Test]
    public void FinishTransport_FilterExisting ()
    {

      TransportedDomainObjects transportedObjects = TransportAndChangeObjects (
          typeof (ClassWithAllDataTypes).FullName + ".Int32Property",
          42,
          DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);

      transportedObjects.FinishTransport (transportedObject =>
      {
        Assert.That (transportedObject.State == StateType.Changed);
        return ((ClassWithAllDataTypes) transportedObject).BooleanProperty;
      });

      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        var c1 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
        Assert.That (c1.Int32Property, Is.EqualTo (42));

        var c2 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2);
        Assert.That (c2.Int32Property, Is.Not.EqualTo (42));
      }
    }

    [Test]
    public void FinishTransport_ClearsTransportedObjects ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects (
          DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);

      transportedObjects.FinishTransport (delegate { return false; });
      Assert.That (transportedObjects.DataTransaction, Is.Null);
      Assert.That (transportedObjects.TransportedObjects, Is.Null);
    }

    [Test]
    public void FinishTransport_DiscardsTransaction ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects (
          DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);

      var transaction = transportedObjects.DataTransaction;
      transportedObjects.FinishTransport ();

      Assert.That (transaction.IsDiscarded, Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "FinishTransport can only be called once.")]
    public void FinishTransport_Twice ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects (
          DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);

      transportedObjects.FinishTransport (delegate { return false; });
      transportedObjects.FinishTransport();
    }

    private TransportedDomainObjects TransportAndDeleteObjects (params ObjectID[] objectsToLoadAndDelete)
    {
      var transporter = new DomainObjectTransporter();
      foreach (ObjectID id in objectsToLoadAndDelete)
        transporter.Load (id);

      ModifyDatabase (
          delegate
          {
            foreach (var id in objectsToLoadAndDelete)
            {
              var domainObject = LifetimeService.GetObject (ClientTransaction.Current, id, false);
              LifetimeService.DeleteObject (ClientTransaction.Current, domainObject);
            }
          });

      return Transport (transporter);
    }

    private TransportedDomainObjects TransportAndChangeObjects (string propertyName, object newValue, params ObjectID[] objectsToLoadAndDelete)
    {
      var transporter = new DomainObjectTransporter ();
      foreach (ObjectID id in objectsToLoadAndDelete)
        transporter.Load (id);

      ModifyDatabase (
          delegate
          {
            foreach (var id in objectsToLoadAndDelete)
            {
              var domainObject = LifetimeService.GetObject (ClientTransaction.Current, id, false);
              new PropertyIndexer (domainObject)[propertyName].SetValueWithoutTypeCheck (newValue);
            }
          });

      return Transport (transporter);
    }

    private List<DomainObject> GetTransportedObjects (TransportedDomainObjects transportedObjects)
    {
      return new List<DomainObject> (transportedObjects.TransportedObjects);
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

    private TransportedDomainObjects Transport (DomainObjectTransporter transporter)
    {
      using (var stream = new MemoryStream())
      {
        transporter.Export (stream);
        stream.Seek (0, SeekOrigin.Begin);

        return DomainObjectTransporter.LoadTransportData (stream);
      }
    }
  }
}
