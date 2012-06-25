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
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class AddToOneToManyRelationTest : ClientTransactionBaseTest
  {
    private Employee _supervisor;
    private Employee _subordinate;

    private DomainObjectEventReceiver _supervisorEventReceiver;
    private DomainObjectEventReceiver _subordinateEventReceiver;
    private DomainObjectCollectionEventReceiver _subordinateCollectionEventReceiver;

    public override void SetUp ()
    {
      base.SetUp ();

      _supervisor = Employee.GetObject (DomainObjectIDs.Employee1);
      _subordinate = Employee.GetObject (DomainObjectIDs.Employee2);

      _supervisorEventReceiver = new DomainObjectEventReceiver (_supervisor);
      _subordinateEventReceiver = new DomainObjectEventReceiver (_subordinate);
      _subordinateCollectionEventReceiver = new DomainObjectCollectionEventReceiver (_supervisor.Subordinates);
    }

    [Test]
    public void AddEvents ()
    {
      _subordinateEventReceiver.Cancel = false;
      _subordinateCollectionEventReceiver.Cancel = false;
      _supervisorEventReceiver.Cancel = false;

      _supervisor.Subordinates.Add (_subordinate);

      Assert.AreEqual (true, _subordinateEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.AreEqual (true, _subordinateEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor", _subordinateEventReceiver.ChangedRelationPropertyName);
      Assert.IsNull (_subordinateEventReceiver.ChangingOldRelatedObject);
      Assert.AreSame (_supervisor, _subordinateEventReceiver.ChangingNewRelatedObject);
      Assert.IsNull (_subordinateEventReceiver.ChangedOldRelatedObject);
      Assert.AreSame (_supervisor, _subordinateEventReceiver.ChangedNewRelatedObject);

      Assert.IsTrue (_subordinateCollectionEventReceiver.HasAddingEventBeenCalled);
      Assert.IsTrue (_subordinateCollectionEventReceiver.HasAddedEventBeenCalled);
      Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.AddingDomainObject);
      Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.AddedDomainObject);

      Assert.AreEqual (true, _supervisorEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.AreEqual (true, _supervisorEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Subordinates", _supervisorEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Subordinates", _supervisorEventReceiver.ChangedRelationPropertyName);
      Assert.IsNull (_supervisorEventReceiver.ChangingOldRelatedObject);
      Assert.AreSame (_subordinate, _supervisorEventReceiver.ChangingNewRelatedObject);
      Assert.IsNull (_supervisorEventReceiver.ChangedOldRelatedObject);
      Assert.AreSame (_subordinate, _supervisorEventReceiver.ChangedNewRelatedObject);

      Assert.AreEqual (StateType.Changed, _subordinate.State);
      Assert.AreEqual (StateType.Changed, _supervisor.State);
      Assert.IsNotNull (_supervisor.Subordinates[_subordinate.ID]);
      Assert.AreEqual (_supervisor.Subordinates.Count - 1, _supervisor.Subordinates.IndexOf (_subordinate));
      Assert.AreSame (_supervisor, _subordinate.Supervisor);
    }


    [Test]
    public void SubordinateCancelsChangeEvent ()
    {
      _subordinateEventReceiver.Cancel = true;
      _subordinateCollectionEventReceiver.Cancel = false;
      _supervisorEventReceiver.Cancel = false;

      try
      {
        _supervisor.Subordinates.Add (_subordinate);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.IsTrue (_subordinateEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.IsFalse (_subordinateEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_subordinateEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_subordinateEventReceiver.ChangingOldRelatedObject);
        Assert.AreSame (_supervisor, _subordinateEventReceiver.ChangingNewRelatedObject);

        Assert.IsFalse (_subordinateCollectionEventReceiver.HasAddingEventBeenCalled);
        Assert.IsFalse (_subordinateCollectionEventReceiver.HasAddedEventBeenCalled);
        Assert.IsNull (_subordinateCollectionEventReceiver.AddingDomainObject);
        Assert.IsNull (_subordinateCollectionEventReceiver.AddedDomainObject);

        Assert.IsFalse (_supervisorEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.IsFalse (_supervisorEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.IsNull (_supervisorEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_supervisorEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_supervisorEventReceiver.ChangingOldRelatedObject);
        Assert.IsNull (_supervisorEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (StateType.Unchanged, _subordinate.State);
        Assert.AreEqual (StateType.Unchanged, _supervisor.State);
        Assert.AreEqual (2, _supervisor.Subordinates.Count);
        Assert.IsNull (_subordinate.Supervisor);
      }
    }


    [Test]
    public void SubOrdinateCollectionCancelsChangeEvent ()
    {
      _subordinateEventReceiver.Cancel = false;
      _subordinateCollectionEventReceiver.Cancel = true;
      _supervisorEventReceiver.Cancel = false;

      try
      {
        _supervisor.Subordinates.Add (_subordinate);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.IsTrue (_subordinateEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.IsFalse (_subordinateEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_subordinateEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_subordinateEventReceiver.ChangingOldRelatedObject);
        Assert.AreSame (_supervisor, _subordinateEventReceiver.ChangingNewRelatedObject);

        Assert.IsTrue (_subordinateCollectionEventReceiver.HasAddingEventBeenCalled);
        Assert.IsFalse (_subordinateCollectionEventReceiver.HasAddedEventBeenCalled);
        Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.AddingDomainObject);
        Assert.IsNull (_subordinateCollectionEventReceiver.AddedDomainObject);

        Assert.IsFalse (_supervisorEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.IsFalse (_supervisorEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.IsNull (_supervisorEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_supervisorEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_supervisorEventReceiver.ChangingOldRelatedObject);
        Assert.IsNull (_supervisorEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (StateType.Unchanged, _subordinate.State);
        Assert.AreEqual (StateType.Unchanged, _supervisor.State);
        Assert.AreEqual (2, _supervisor.Subordinates.Count);
        Assert.IsNull (_subordinate.Supervisor);
      }
    }

    [Test]
    public void SupervisorCancelsChangeEvent ()
    {
      _subordinateEventReceiver.Cancel = false;
      _subordinateCollectionEventReceiver.Cancel = false;
      _supervisorEventReceiver.Cancel = true;

      try
      {
        _supervisor.Subordinates.Add (_subordinate);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.IsTrue (_subordinateEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.IsFalse (_subordinateEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_subordinateEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_subordinateEventReceiver.ChangingOldRelatedObject);
        Assert.AreSame (_supervisor, _subordinateEventReceiver.ChangingNewRelatedObject);

        Assert.IsTrue (_subordinateCollectionEventReceiver.HasAddingEventBeenCalled);
        Assert.IsFalse (_subordinateCollectionEventReceiver.HasAddedEventBeenCalled);
        Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.AddingDomainObject);
        Assert.IsNull (_subordinateCollectionEventReceiver.AddedDomainObject);

        Assert.AreEqual (true, _supervisorEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.IsFalse (_supervisorEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Subordinates", _supervisorEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_supervisorEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_supervisorEventReceiver.ChangingOldRelatedObject);
        Assert.AreSame (_subordinate, _supervisorEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (StateType.Unchanged, _subordinate.State);
        Assert.AreEqual (StateType.Unchanged, _supervisor.State);
        Assert.AreEqual (2, _supervisor.Subordinates.Count);
        Assert.IsNull (_subordinate.Supervisor);
      }
    }

    [Test]
    public void StateTracking ()
    {
      Assert.AreEqual (StateType.Unchanged, _supervisor.State);
      Assert.AreEqual (StateType.Unchanged, _subordinate.State);

      _supervisor.Subordinates.Add (_subordinate);

      Assert.AreEqual (StateType.Changed, _supervisor.State);
      Assert.AreEqual (StateType.Changed, _subordinate.State);
    }

    [Test]
    public void RelationEndPointMap ()
    {
      _supervisor.Subordinates.Add (_subordinate);

      Assert.AreSame (_supervisor, _subordinate.Supervisor);
    }

    [Test]
    public void SetPropertyValue ()
    {
      _supervisor.Subordinates.Add (_subordinate);

      Assert.AreEqual (_supervisor.ID, _subordinate.Properties[typeof (Employee), "Supervisor"].GetRelatedObjectID());
    }

    [Test]
    public void SetSupervisor ()
    {
      _subordinate.Supervisor = _supervisor;

      Assert.AreSame (_supervisor, _subordinate.Supervisor);
      Assert.AreEqual (3, _supervisor.Subordinates.Count);
      Assert.IsNotNull (_supervisor.Subordinates[_subordinate.ID]);
    }

    [Test]
    public void SetSameSupervisor ()
    {
      Employee employeeWithSupervisor = _supervisor.Subordinates[DomainObjectIDs.Employee4];
      employeeWithSupervisor.Supervisor = _supervisor;

      Assert.AreEqual (StateType.Unchanged, _supervisor.State);
      Assert.AreEqual (StateType.Unchanged, employeeWithSupervisor.State);
    }

    [Test]
    public void SetSupervisorNull ()
    {
      _subordinate.Supervisor = null;

      // expectation: no exception
    }

    [Test]
    public void InsertEvents ()
    {
      _subordinateEventReceiver.Cancel = false;
      _subordinateCollectionEventReceiver.Cancel = false;
      _supervisorEventReceiver.Cancel = false;

      _supervisor.Subordinates.Insert (0, _subordinate);

      Assert.AreEqual (0, _supervisor.Subordinates.IndexOf (_subordinate));
      Assert.AreEqual (true, _subordinateEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.AreEqual (true, _subordinateEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor", _subordinateEventReceiver.ChangedRelationPropertyName);
      Assert.IsNull (_subordinateEventReceiver.ChangingOldRelatedObject);
      Assert.AreSame (_supervisor, _subordinateEventReceiver.ChangingNewRelatedObject);
      Assert.IsNull (_subordinateEventReceiver.ChangedOldRelatedObject);
      Assert.AreSame (_supervisor, _subordinateEventReceiver.ChangedNewRelatedObject);

      Assert.IsTrue (_subordinateCollectionEventReceiver.HasAddingEventBeenCalled);
      Assert.IsTrue (_subordinateCollectionEventReceiver.HasAddedEventBeenCalled);
      Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.AddingDomainObject);
      Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.AddedDomainObject);

      Assert.AreEqual (true, _supervisorEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.AreEqual (true, _supervisorEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Subordinates", _supervisorEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Subordinates", _supervisorEventReceiver.ChangedRelationPropertyName);
      Assert.IsNull (_supervisorEventReceiver.ChangingOldRelatedObject);
      Assert.AreSame (_subordinate, _supervisorEventReceiver.ChangingNewRelatedObject);
      Assert.IsNull (_supervisorEventReceiver.ChangedOldRelatedObject);
      Assert.AreSame (_subordinate, _supervisorEventReceiver.ChangedNewRelatedObject);

      Assert.AreEqual (StateType.Changed, _subordinate.State);
      Assert.AreEqual (StateType.Changed, _supervisor.State);
      Assert.IsNotNull (_supervisor.Subordinates[_subordinate.ID]);
      Assert.AreSame (_supervisor, _subordinate.Supervisor);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The collection already contains an object with ID 'Employee|c3b2bbc3-e083-4974-bac7-9cee1fb85a5e|System.Guid'.\r\n"
        + "Parameter name: domainObject")]
    public void AddObjectAlreadyInCollection ()
    {
      _supervisor.Subordinates.Add (_subordinate);
      _supervisor.Subordinates.Add (_subordinate);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The collection already contains an object with ID 'Employee|c3b2bbc3-e083-4974-bac7-9cee1fb85a5e|System.Guid'.\r\n"
        + "Parameter name: domainObject")]
    public void InsertObjectAlreadyInCollection ()
    {
      _supervisor.Subordinates.Insert (0, _subordinate);
      _supervisor.Subordinates.Insert (0, _subordinate);
    }

    [Test]
    public void AddSubordinateWithOldSupervisor ()
    {
      Employee subordinate = Employee.GetObject (DomainObjectIDs.Employee3);
      Employee oldSupervisorOfSubordinate = Employee.GetObject (DomainObjectIDs.Employee2);

      var subordinateEventReceiver = new DomainObjectEventReceiver (subordinate);
      subordinateEventReceiver.Cancel = false;

      var oldSupervisorEventReceiver = new DomainObjectEventReceiver (oldSupervisorOfSubordinate);
      oldSupervisorEventReceiver.Cancel = false;

      var oldSupervisorSubordinateCollectionEventReceiver = new DomainObjectCollectionEventReceiver (oldSupervisorOfSubordinate.Subordinates);
      oldSupervisorSubordinateCollectionEventReceiver.Cancel = false;

      _subordinateCollectionEventReceiver.Cancel = false;
      _supervisorEventReceiver.Cancel = false;

      _supervisor.Subordinates.Add (subordinate);

      Assert.IsTrue (oldSupervisorEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsTrue (oldSupervisorEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Subordinates", oldSupervisorEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Subordinates", oldSupervisorEventReceiver.ChangedRelationPropertyName);

      Assert.IsTrue (oldSupervisorSubordinateCollectionEventReceiver.HasRemovingEventBeenCalled);
      Assert.IsTrue (oldSupervisorSubordinateCollectionEventReceiver.HasRemovedEventBeenCalled);
      Assert.AreEqual (1, oldSupervisorSubordinateCollectionEventReceiver.RemovingDomainObjects.Count);
      Assert.AreSame (subordinate, oldSupervisorSubordinateCollectionEventReceiver.RemovingDomainObjects[0]);
      Assert.AreEqual (1, oldSupervisorSubordinateCollectionEventReceiver.RemovingDomainObjects.Count);
      Assert.AreSame (subordinate, oldSupervisorSubordinateCollectionEventReceiver.RemovingDomainObjects[0]);


      Assert.IsTrue (subordinateEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsTrue (subordinateEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor", subordinateEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor", subordinateEventReceiver.ChangedRelationPropertyName);
      Assert.AreSame (oldSupervisorOfSubordinate, subordinateEventReceiver.ChangingOldRelatedObject);
      Assert.AreSame (_supervisor, subordinateEventReceiver.ChangingNewRelatedObject);
      Assert.AreSame (oldSupervisorOfSubordinate, subordinateEventReceiver.ChangedOldRelatedObject);
      Assert.AreSame (_supervisor, subordinateEventReceiver.ChangedNewRelatedObject);

      Assert.IsTrue (_subordinateCollectionEventReceiver.HasAddingEventBeenCalled);
      Assert.IsTrue (_subordinateCollectionEventReceiver.HasAddedEventBeenCalled);
      Assert.AreSame (subordinate, _subordinateCollectionEventReceiver.AddingDomainObject);
      Assert.AreSame (subordinate, _subordinateCollectionEventReceiver.AddedDomainObject);

      Assert.IsTrue (_supervisorEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.IsTrue (_supervisorEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Subordinates", _supervisorEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Subordinates", _supervisorEventReceiver.ChangedRelationPropertyName);
      Assert.IsNull (_supervisorEventReceiver.ChangingOldRelatedObject);
      Assert.AreSame (subordinate, _supervisorEventReceiver.ChangingNewRelatedObject);
      Assert.IsNull (_supervisorEventReceiver.ChangedOldRelatedObject);
      Assert.AreSame (subordinate, _supervisorEventReceiver.ChangedNewRelatedObject);

      Assert.AreEqual (StateType.Changed, subordinate.State);
      Assert.AreEqual (StateType.Changed, _supervisor.State);
      Assert.AreEqual (StateType.Changed, oldSupervisorOfSubordinate.State);

      Assert.IsNotNull (_supervisor.Subordinates[subordinate.ID]);
      Assert.AreEqual (_supervisor.Subordinates.Count - 1, _supervisor.Subordinates.IndexOf (subordinate));
      Assert.IsFalse (oldSupervisorOfSubordinate.Subordinates.ContainsObject (subordinate));
      Assert.AreSame (_supervisor, subordinate.Supervisor);
    }
  }
}
