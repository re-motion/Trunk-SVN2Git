// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Delete
{
  [TestFixture]
  public class DeleteDomainObjectWithOneToManyRelationTest : ClientTransactionBaseTest
  {
    private Employee _supervisor;
    private Employee _subordinate1;
    private Employee _subordinate2;
    private SequenceEventReceiver _eventReceiver;

    public override void SetUp ()
    {
      base.SetUp ();

      _supervisor = Employee.GetObject (DomainObjectIDs.Employee1);
      _subordinate1 = (Employee) _supervisor.Subordinates[0];
      _subordinate2 = (Employee) _supervisor.Subordinates[1];

      _eventReceiver = CreateEventReceiver ();
    }

    [Test]
    public void DeleteSupervisor ()
    {
      _supervisor.Delete ();

      ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_supervisor, "1. Deleting of supervisor"),
      new CollectionDeletionState (_supervisor.Subordinates, "2. Deleting of supervisor.Subordinates"),
      new RelationChangeState (_subordinate1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor", _supervisor, null, "3. Relation changing of subordinate1"),
      new RelationChangeState (_subordinate2, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor", _supervisor, null, "4. Relation changing of subordinate2"),
      new RelationChangeState (_subordinate2, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor", null, null, "5. Relation changed of subordinate2"),
      new RelationChangeState (_subordinate1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor", null, null, "6. Relation changed of subordinate1"),
      new CollectionDeletionState (_supervisor.Subordinates, "7. Deleted of supervisor.Subordinates"),
      new ObjectDeletionState (_supervisor, "8. Deleted of supervisor")
    };

      _eventReceiver.Check (expectedStates);
    }

    [Test]
    public void SupervisorCancelsDeleteEvent ()
    {
      _eventReceiver.CancelEventNumber = 1;

      try
      {
        _supervisor.Delete ();
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedStates = new ChangeState[] { new ObjectDeletionState (_supervisor, "1. Deleting of supervisor") };

        _eventReceiver.Check (expectedStates);
      }
    }

    [Test]
    public void SubordinateCancelsDeleteEvent ()
    {
      _eventReceiver.CancelEventNumber = 3;

      try
      {
        _supervisor.Delete ();
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        ChangeState[] expectedStates = new ChangeState[]
            {
              new ObjectDeletionState (_supervisor, "1. Deleting of supervisor"),
              new CollectionDeletionState (_supervisor.Subordinates, "2. Deleting of supervisor.Subordinates"),
              new RelationChangeState (_subordinate1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor", _supervisor, null, "3. Relation changing of subordinate1")
            };

        _eventReceiver.Check (expectedStates);
      }
    }

    [Test]
    public void Relations ()
    {
      int numberOfSubordinatesBeforeDelete = _supervisor.Subordinates.Count;

      _supervisor.Delete ();

      Assert.AreEqual (0, _supervisor.Subordinates.Count);
      Assert.IsNull (_subordinate1.Supervisor);
      Assert.IsNull (_subordinate2.Supervisor);
      Assert.IsNull (_subordinate1.Properties[typeof (Employee), "Supervisor"].GetRelatedObjectID ());
      Assert.IsNull (_subordinate2.Properties[typeof (Employee), "Supervisor"].GetRelatedObjectID ());
      Assert.AreEqual (StateType.Changed, _subordinate1.InternalDataContainer.State);
      Assert.AreEqual (StateType.Changed, _subordinate2.InternalDataContainer.State);
    }

    [Test]
    public void ChangePropertyBeforeDeletion ()
    {
      _supervisor.Subordinates.Clear ();
      _eventReceiver = CreateEventReceiver ();

      _supervisor.Delete ();

      ChangeState[] expectedStates = new ChangeState[]
    {
      new ObjectDeletionState (_supervisor, "1. Deleting of supervisor"),
      new CollectionDeletionState (_supervisor.Subordinates, "2. Deleting of supervisor.Subordinates"),
      new CollectionDeletionState (_supervisor.Subordinates, "3. Deleted of supervisor.Subordinates"),
      new ObjectDeletionState (_supervisor, "4. Deleted of supervisor"),
    };

      _eventReceiver.Check (expectedStates);
    }

    [Test]
    public void GetOriginalRelatedObjects ()
    {
      _supervisor.Delete ();
      DomainObjectCollection originalSubordinates = _supervisor.GetOriginalRelatedObjects ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Subordinates");

      Assert.IsNotNull (originalSubordinates);
      Assert.AreEqual (2, originalSubordinates.Count);
      Assert.IsNotNull (originalSubordinates[DomainObjectIDs.Employee4]);
      Assert.IsNotNull (originalSubordinates[DomainObjectIDs.Employee5]);
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void AddToRelatedObjectsOfDeletedObject ()
    {
      _supervisor.Delete ();

      _supervisor.Subordinates.Add (Employee.GetObject (DomainObjectIDs.Employee3));
    }

    [Test]
    [ExpectedException (typeof (ObjectDeletedException))]
    public void ReassignDeletedObject ()
    {
      _supervisor.Delete ();

      _subordinate1.Supervisor = _supervisor;
    }

    private SequenceEventReceiver CreateEventReceiver ()
    {
      return new SequenceEventReceiver (
          new DomainObject[] { _supervisor, _subordinate1, _subordinate2 },
          new DomainObjectCollection[] { _supervisor.Subordinates });
    }
  }
}
