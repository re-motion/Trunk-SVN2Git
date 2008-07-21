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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.DomainObjects.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.DomainObjects
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
      Assert.AreEqual (_newClient.ID, _location.InternalDataContainer["Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client"]);
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
      new RelationChangeState (_location, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client", _oldClient, _newClient, "1. Changing event of location"),
      new RelationChangeState (_location, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client", null, null, "2. Changed event of location")
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
      Assert.AreSame (_oldClient, _location.GetRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client"));
    }

    [Test]
    public void GetOriginalRelatedObject ()
    {
      Assert.AreSame (_oldClient, _location.GetOriginalRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client"));

      _location.Client = _newClient;

      Assert.AreSame (_oldClient, _location.GetOriginalRelatedObject ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client"));
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
      new RelationChangeState (location, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client", null, client1, "1. Changing event of location"),
      new RelationChangeState (location, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Location.Client", null, null, "2. Changed event of location")
    };

      eventReceiver.Check (expectedStates);

      ClientTransactionMock.Commit ();

      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
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
    [ExpectedException (typeof (ObjectNotFoundException),
        ExpectedMessage = "Object 'Client|.*|System.Guid' could not be found.", MatchType = MessageMatch.Regex)]
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

      using (ClientTransaction.NewRootTransaction().EnterDiscardingScope())
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
