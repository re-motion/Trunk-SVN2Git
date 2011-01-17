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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class UnidirectionalRelationTest : RelationChangeBaseTest
  {
    private Client _oldClient;
    private Client _newClient;
    private Location _location;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp ();
      SetDatabaseModifyable ();
    }

    public override void SetUp ()
    {
      base.SetUp ();

      _oldClient = Client.GetObject (DomainObjectIDs.Client1);
      _newClient = Client.GetObject (DomainObjectIDs.Client2);
      _location = Location.GetObject (DomainObjectIDs.Location1);
    }

    [Test]
    public void SetRelatedObject ()
    {
      _location.Client = _newClient;

      Assert.AreSame (_newClient, _location.Client);
      Assert.AreEqual (_newClient.ID, _location.InternalDataContainer["Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client"]);
      Assert.AreEqual (StateType.Changed, _location.State);
      Assert.AreEqual (StateType.Unchanged, _oldClient.State);
      Assert.AreEqual (StateType.Unchanged, _newClient.State);
    }

    [Test]
    public void EventsForSetRelatedObject ()
    {
      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (new DomainObject[] { _location, _oldClient, _newClient }, new DomainObjectCollection[0]);

      _location.Client = _newClient;

      ChangeState[] expectedStates = new ChangeState[]
    {
      new RelationChangeState (_location, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client", _oldClient, _newClient, "1. Changing event of location"),
      new RelationChangeState (_location, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client", null, null, "2. Changed event of location")
    };

      eventReceiver.Check (expectedStates);
    }

    [Test]
    public void SetRelatedObjectWithSameOldAndNewObject ()
    {
      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (new DomainObject[] { _location, _oldClient, _newClient }, new DomainObjectCollection[0]);

      _location.Client = _oldClient;

      eventReceiver.Check (new ChangeState[0]);
      Assert.AreEqual (StateType.Unchanged, _location.State);
    }

    [Test]
    public void GetRelatedObject ()
    {
      Assert.AreSame (_oldClient, _location.GetRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client"));
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      Assert.AreSame (_oldClient, _location.GetOriginalRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client"));

      _location.Client = _newClient;

      Assert.AreSame (_oldClient, _location.GetOriginalRelatedObject ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client"));
    }

    [Test]
    public void CreateObjectsAndCommit ()
    {
      Client client1 = Client.NewObject ();
      Client client2 = Client.NewObject ();
      Location location = Location.NewObject();

      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (new DomainObject[] { location, client1, client2 }, new DomainObjectCollection[0]);

      location.Client = client1;

      Assert.AreEqual (StateType.New, client1.State);
      Assert.AreEqual (StateType.New, client2.State);
      Assert.AreEqual (StateType.New, location.State);

      ObjectID clientID1 = client1.ID;
      ObjectID clientID2 = client2.ID;
      ObjectID locationID = location.ID;


      ChangeState[] expectedStates = new ChangeState[]
    {
      new RelationChangeState (location, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client", null, client1, "1. Changing event of location"),
      new RelationChangeState (location, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Location.Client", null, null, "2. Changed event of location")
    };

      eventReceiver.Check (expectedStates);

      ClientTransactionMock.Commit ();

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        client1 = Client.GetObject (clientID1);
        client2 = Client.GetObject (clientID2);
        location = Location.GetObject (locationID);

        Assert.IsNotNull (client1);
        Assert.IsNotNull (client2);
        Assert.IsNotNull (location);
        Assert.AreSame (client1, location.Client);
      }
    }

    [Test]
    public void DeleteLocationAndCommit ()
    {
      SequenceEventReceiver eventReceiver = new SequenceEventReceiver (new DomainObject[] { _location, _oldClient, _newClient }, new DomainObjectCollection[0]);

      _location.Delete ();
      ClientTransactionMock.Commit ();

      ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_location, "1. Deleting event of location"),
      new ObjectDeletionState (_location, "2. Deleted event of location")
    };

      eventReceiver.Check (expectedStates);
    }

    [Test]
    public void DeleteMultipleObjectsAndCommit ()
    {
      _location.Delete ();
      _oldClient.Delete ();
      _newClient.Delete ();

      Client client3 = Client.GetObject (DomainObjectIDs.Client3);
      client3.Delete ();

      Location location2 = Location.GetObject (DomainObjectIDs.Location2);
      location2.Delete ();

      Location location3 = Location.GetObject (DomainObjectIDs.Location3);
      location3.Delete ();

      ClientTransactionMock.Commit ();
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException),
        ExpectedMessage = "Object 'Client|1627ade8-125f-4819-8e33-ce567c42b00c|System.Guid' is already deleted.")]
    public void IndirectAccessToDeletedLoadedThrows ()
    {
      _location.Client.Delete ();
      Client client = _location.Client;
    }

    [Test]
    [ExpectedException (typeof (ObjectInvalidException),
        ExpectedMessage = @"Object 'Client\|.*\|System.Guid' is invalid in this transaction.", MatchType = MessageMatch.Regex)]
    public void IndirectAccessToDeletedNewThrows ()
    {
      _location.Client = Client.NewObject ();
      _location.Client.Delete ();
      Client client = _location.Client;
    }

    [Test]
    [Ignore ("TODO: Discuss whether to implement consistency checks in OPF.")]
    public void DeleteClientAndCommit ()
    {
      _location.Client.Delete ();
      ClientTransactionMock.Commit ();
      Assert.IsNull (_location.Client);
    }

    [Test]
    public void ResettingDeletedLoadedWorks ()
    {
      _location.Client.Delete ();
      Client newClient = Client.NewObject ();
      _location.Client = newClient;
      Assert.AreSame (newClient, _location.Client);
    }

    [Test]
    public void ResettingDeletedNewWorks ()
    {
      _location.Client = Client.NewObject ();
      _location.Client.Delete ();
      Client newClient = Client.NewObject ();
      _location.Client = newClient;
      Assert.AreSame (newClient, _location.Client);
    }

    [Test]
    public void StateRemainsUnchangedWhenDeletingRelatedObject ()
    {
      Assert.AreEqual (StateType.Unchanged, _location.State);
      _location.Client.Delete ();
      Assert.AreEqual (StateType.Unchanged, _location.State);
    }

    [Test]
    public void Rollback ()
    {
      _location.Delete ();
      Location newLocation = Location.NewObject();
      newLocation.Client = _newClient;

      ClientTransactionMock.Rollback ();

      Assert.AreEqual (StateType.Unchanged, _location.State);
    }

    [Test]
    public void CreateHierarchy ()
    {
      Client newClient1 = Client.NewObject ();
      Client newClient2 = Client.NewObject ();
      newClient2.ParentClient = newClient1;

      ObjectID newClientID1 = newClient1.ID;
      ObjectID newClientID2 = newClient2.ID;

      ClientTransactionMock.Commit ();

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        newClient1 = Client.GetObject (newClientID1);
        newClient2 = Client.GetObject (newClientID2);

        Assert.IsNotNull (newClient1);
        Assert.IsNotNull (newClient2);
        Assert.AreSame (newClient1, newClient2.ParentClient);
      }
    }

    [Test]
    public void HasBeenTouched ()
    {
      CheckTouching (delegate { _location.Client = _newClient; }, _location, "Client",
          new RelationEndPointID (_location.ID, typeof (Location).FullName + ".Client"));
    }

    [Test]
    public void HasBeenTouched_OriginalValue ()
    {
      CheckTouching (delegate { _location.Client = _location.Client; }, _location, "Client",
          new RelationEndPointID (_location.ID, typeof (Location).FullName + ".Client"));
    }
  }
}
