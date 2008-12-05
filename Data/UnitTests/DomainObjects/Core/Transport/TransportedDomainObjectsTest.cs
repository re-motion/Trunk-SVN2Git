// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
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
      ClientTransaction dataTransaction = ClientTransaction.CreateRootTransaction ();
      TransportedDomainObjects transportedObjects = new TransportedDomainObjects (dataTransaction, new List<DomainObject>());

      Assert.IsNotNull (transportedObjects.DataTransaction);
      Assert.AreSame (dataTransaction, transportedObjects.DataTransaction);
      Assert.IsEmpty ((ICollection) GetTransportedObjects (transportedObjects));
    }

    [Test]
    public void TransportedObjectsStayConstant_WhenTransactionIsManipulated ()
    {
      TransportedDomainObjects transportedObjects = new TransportedDomainObjects (ClientTransaction.CreateRootTransaction (), new List<DomainObject> ());

      Assert.IsEmpty ((ICollection) GetTransportedObjects (transportedObjects));

      using (transportedObjects.DataTransaction.EnterNonDiscardingScope ())
      {
        Order.GetObject (DomainObjectIDs.Order1);
      }

      Assert.IsEmpty ((ICollection) GetTransportedObjects (transportedObjects));
    }

    [Test]
    public void NonEmptyTransport ()
    {
      ClientTransaction newTransaction = ClientTransaction.CreateRootTransaction ();
      List<DomainObject> transportedObjectList = new List<DomainObject>  ();
      using (newTransaction.EnterNonDiscardingScope ())
      {
        transportedObjectList.Add (Order.GetObject (DomainObjectIDs.Order1));
        transportedObjectList.Add (Order.GetObject (DomainObjectIDs.Order2));
        transportedObjectList.Add (Company.GetObject (DomainObjectIDs.Company1));
      }

      TransportedDomainObjects transportedObjects = new TransportedDomainObjects (newTransaction, transportedObjectList);

      Assert.IsNotNull (transportedObjects.DataTransaction);
      Assert.IsNotEmpty ((ICollection) GetTransportedObjects (transportedObjects));
      List<ObjectID> ids = GetTransportedObjects (transportedObjects).ConvertAll<ObjectID> (delegate (DomainObject obj) { return obj.ID; });
      Assert.That (ids, Is.EquivalentTo (new ObjectID[] { DomainObjectIDs.Order1, DomainObjectIDs.Order2, DomainObjectIDs.Company1 }));
    }

    [Test]
    public void FinishTransport_CallsCommit ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects (DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);
      MockRepository mockRepository = new MockRepository ();
      IClientTransactionExtension mock = mockRepository.StrictMock<IClientTransactionExtension> ();

      mock.Committing (null, null);
      LastCall.Constraints (Mocks_Is.Same (transportedObjects.DataTransaction), Mocks_List.Equal (GetTransportedObjects (transportedObjects)));
      mock.Committed (null, null);
      LastCall.Constraints (Mocks_Is.Same (transportedObjects.DataTransaction), Mocks_List.Equal (GetTransportedObjects (transportedObjects)));

      mockRepository.ReplayAll ();

      transportedObjects.DataTransaction.Extensions.Add ("mock", mock);
      transportedObjects.FinishTransport ();

      mockRepository.VerifyAll ();
    }

    [Test]
    public void FinishTransport_FilterCalledForEachChangedObject ()
    {
      DomainObjectTransporter transporter = new DomainObjectTransporter ();
      transporter.Load (DomainObjectIDs.ClassWithAllDataTypes1);
      transporter.Load (DomainObjectIDs.ClassWithAllDataTypes2);
      transporter.Load (DomainObjectIDs.Order1);

      ModifyDatabase (delegate
      {
        ClassWithAllDataTypes c1 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
        ClassWithAllDataTypes c2 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2);
        c1.Delete ();
        c2.Delete ();
      });

      byte[] transportData = transporter.GetBinaryTransportData ();

      TransportedDomainObjects transportedObjects = DomainObjectTransporter.LoadTransportData (transportData);
      List<DomainObject> expectedObjects = new List<DomainObject> ();
      using (transportedObjects.DataTransaction.EnterNonDiscardingScope ())
      {
        expectedObjects.Add (ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1));
        expectedObjects.Add (ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2));
      }

      List<DomainObject> filteredObjects = new List<DomainObject> ();
      transportedObjects.FinishTransport (delegate (DomainObject domainObject) { filteredObjects.Add (domainObject); return true; });
      Assert.That (filteredObjects, Is.EquivalentTo (expectedObjects));
    }

    [Test]
    public void FinishTransport_All ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects (DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);

      transportedObjects.FinishTransport ();

      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        ClassWithAllDataTypes c3 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
        ClassWithAllDataTypes c4 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2);

        Assert.AreEqual (2147483647, c3.Int32Property);
        Assert.AreEqual (-2147483647, c4.Int32Property);
      }
    }

    [Test]
    public void FinishTransport_Some ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects (DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);

      transportedObjects.FinishTransport (delegate (DomainObject transportedObject)
      {
        return ((ClassWithAllDataTypes) transportedObject).Int32Property < 0;
      });

      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
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

        ClassWithAllDataTypes c4 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2);
        Assert.AreEqual (-2147483647, c4.Int32Property);
      }
    }

    [Test]
    public void FinishTransport_ClearsTransportedObjects ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects (DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);

      transportedObjects.FinishTransport (delegate { return false; });
      Assert.IsNull (transportedObjects.DataTransaction);
      Assert.IsNull (transportedObjects.TransportedObjects);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "FinishTransport can only be called once.")]
    public void FinishTransport_Twice ()
    {
      TransportedDomainObjects transportedObjects = TransportAndDeleteObjects (DomainObjectIDs.ClassWithAllDataTypes1,
          DomainObjectIDs.ClassWithAllDataTypes2);

      transportedObjects.FinishTransport (delegate { return false; });
      transportedObjects.FinishTransport ();
    }

    private TransportedDomainObjects TransportAndDeleteObjects (params ObjectID[] objectsToLoadAndDelete)
    {
      DomainObjectTransporter transporter = new DomainObjectTransporter ();
      foreach (ObjectID id in objectsToLoadAndDelete)
        transporter.Load (id);

      ModifyDatabase (delegate
      {
        ClassWithAllDataTypes c1 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
        ClassWithAllDataTypes c2 = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2);

        Assert.AreEqual (2147483647, c1.Int32Property);
        Assert.AreEqual (-2147483647, c2.Int32Property);

        c1.Delete ();
        c2.Delete ();
      });

      byte[] transportData = transporter.GetBinaryTransportData ();

      return DomainObjectTransporter.LoadTransportData (transportData);
    }

    private System.Collections.Generic.List<DomainObject> GetTransportedObjects (TransportedDomainObjects transportedObjects)
    {
      return new List<DomainObject> (transportedObjects.TransportedObjects);
    }

    private void ModifyDatabase (Action changer)
    {
      SetDatabaseModifyable ();
      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        changer ();
        ClientTransaction.Current.Commit ();
      }
    }
  }
}
