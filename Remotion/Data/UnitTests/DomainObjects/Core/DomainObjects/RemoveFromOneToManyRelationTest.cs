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
using System.Collections;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DomainObjects
{
  [TestFixture]
  public class RemoveFromOneToManyRelationTest : ClientTransactionBaseTest
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
      _subordinate = Employee.GetObject (DomainObjectIDs.Employee4);

      _supervisorEventReceiver = new DomainObjectEventReceiver (_supervisor);
      _subordinateEventReceiver = new DomainObjectEventReceiver (_subordinate);
      _subordinateCollectionEventReceiver = new DomainObjectCollectionEventReceiver (_supervisor.Subordinates);
    }

    [Test]
    public void ChangeEvents ()
    {
      _subordinateEventReceiver.Cancel = false;
      _subordinateCollectionEventReceiver.Cancel = false;
      _supervisorEventReceiver.Cancel = false;

      _supervisor.Subordinates.Remove (_subordinate.ID);

      Assert.AreEqual (true, _subordinateEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.AreEqual (true, _subordinateEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor", _subordinateEventReceiver.ChangedRelationPropertyName);
      Assert.AreSame (_supervisor, _subordinateEventReceiver.ChangingOldRelatedObject);
      Assert.IsNull (_subordinateEventReceiver.ChangingNewRelatedObject);
      Assert.AreSame (_supervisor, _subordinateEventReceiver.ChangedOldRelatedObject);
      Assert.IsNull (_subordinateEventReceiver.ChangedNewRelatedObject);

      Assert.IsTrue (_subordinateCollectionEventReceiver.HasRemovingEventBeenCalled);
      Assert.IsTrue (_subordinateCollectionEventReceiver.HasRemovedEventBeenCalled);
      Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.RemovingDomainObjects[0]);
      Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.RemovedDomainObjects[0]);

      Assert.AreEqual (true, _supervisorEventReceiver.HasRelationChangingEventBeenCalled);
      Assert.AreEqual (true, _supervisorEventReceiver.HasRelationChangedEventBeenCalled);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Subordinates", _supervisorEventReceiver.ChangingRelationPropertyName);
      Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Subordinates", _supervisorEventReceiver.ChangedRelationPropertyName);
      Assert.AreSame (_subordinate, _supervisorEventReceiver.ChangingOldRelatedObject);
      Assert.IsNull (_supervisorEventReceiver.ChangingNewRelatedObject);
      Assert.AreSame (_subordinate, _supervisorEventReceiver.ChangedOldRelatedObject);
      Assert.IsNull (_supervisorEventReceiver.ChangedNewRelatedObject);

      Assert.AreEqual (StateType.Changed, _subordinate.State);
      Assert.AreEqual (StateType.Changed, _supervisor.State);
      Assert.IsNull (_supervisor.Subordinates[_subordinate.ID]);
      Assert.IsNull (_subordinate.Supervisor);
    }

    [Test]
    public void SubordinateCancelsChangeEvent ()
    {
      _subordinateEventReceiver.Cancel = true;
      _subordinateCollectionEventReceiver.Cancel = false;
      _supervisorEventReceiver.Cancel = false;

      try
      {
        _supervisor.Subordinates.Remove (_subordinate);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.IsTrue (_subordinateEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.IsFalse (_subordinateEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_subordinateEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_supervisor, _subordinateEventReceiver.ChangingOldRelatedObject);
        Assert.IsNull (_subordinateEventReceiver.ChangingNewRelatedObject);

        Assert.IsFalse (_subordinateCollectionEventReceiver.HasRemovingEventBeenCalled);
        Assert.IsFalse (_subordinateCollectionEventReceiver.HasRemovedEventBeenCalled);
        Assert.AreEqual (0, _subordinateCollectionEventReceiver.RemovingDomainObjects.Count);
        Assert.AreEqual (0, _subordinateCollectionEventReceiver.RemovedDomainObjects.Count);

        Assert.IsFalse (_supervisorEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.IsFalse (_supervisorEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.IsNull (_supervisorEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_supervisorEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_supervisorEventReceiver.ChangingOldRelatedObject);
        Assert.IsNull (_supervisorEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (StateType.Unchanged, _subordinate.State);
        Assert.AreEqual (StateType.Unchanged, _supervisor.State);
        Assert.IsNotNull (_supervisor.Subordinates[_subordinate.ID]);
        Assert.AreSame (_supervisor, _subordinate.Supervisor);
      }
    }


    [Test]
    public void SubordinateCollectionCancelsChangeEvent ()
    {
      _subordinateEventReceiver.Cancel = false;
      _subordinateCollectionEventReceiver.Cancel = true;
      _supervisorEventReceiver.Cancel = false;

      try
      {
        _supervisor.Subordinates.Remove (_subordinate);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.IsTrue (_subordinateEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.IsFalse (_subordinateEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_subordinateEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_supervisor, _subordinateEventReceiver.ChangingOldRelatedObject);
        Assert.IsNull (_subordinateEventReceiver.ChangingNewRelatedObject);

        Assert.IsTrue (_subordinateCollectionEventReceiver.HasRemovingEventBeenCalled);
        Assert.IsFalse (_subordinateCollectionEventReceiver.HasRemovedEventBeenCalled);
        Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.RemovingDomainObjects[0]);
        Assert.AreEqual (0, _subordinateCollectionEventReceiver.RemovedDomainObjects.Count);

        Assert.IsFalse (_supervisorEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.IsFalse (_supervisorEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.IsNull (_supervisorEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_supervisorEventReceiver.ChangedRelationPropertyName);
        Assert.IsNull (_supervisorEventReceiver.ChangingOldRelatedObject);
        Assert.IsNull (_supervisorEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (StateType.Unchanged, _subordinate.State);
        Assert.AreEqual (StateType.Unchanged, _supervisor.State);
        Assert.IsNotNull (_supervisor.Subordinates[_subordinate.ID]);
        Assert.AreSame (_supervisor, _subordinate.Supervisor);
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
        _supervisor.Subordinates.Remove (_subordinate);
        Assert.Fail ("EventReceiverCancelException should be raised.");
      }
      catch (EventReceiverCancelException)
      {
        Assert.IsTrue (_subordinateEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.IsFalse (_subordinateEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Supervisor", _subordinateEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_subordinateEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_supervisor, _subordinateEventReceiver.ChangingOldRelatedObject);
        Assert.IsNull (_subordinateEventReceiver.ChangingNewRelatedObject);

        Assert.IsTrue (_subordinateCollectionEventReceiver.HasRemovingEventBeenCalled);
        Assert.IsFalse (_subordinateCollectionEventReceiver.HasRemovedEventBeenCalled);
        Assert.AreSame (_subordinate, _subordinateCollectionEventReceiver.RemovingDomainObjects[0]);
        Assert.AreEqual (0, _subordinateCollectionEventReceiver.RemovedDomainObjects.Count);

        Assert.AreEqual (true, _supervisorEventReceiver.HasRelationChangingEventBeenCalled);
        Assert.IsFalse (_supervisorEventReceiver.HasRelationChangedEventBeenCalled);
        Assert.AreEqual ("Remotion.Data.UnitTests.DomainObjects.TestDomain.Employee.Subordinates", _supervisorEventReceiver.ChangingRelationPropertyName);
        Assert.IsNull (_supervisorEventReceiver.ChangedRelationPropertyName);
        Assert.AreSame (_subordinate, _supervisorEventReceiver.ChangingOldRelatedObject);
        Assert.IsNull (_supervisorEventReceiver.ChangingNewRelatedObject);

        Assert.AreEqual (StateType.Unchanged, _subordinate.State);
        Assert.AreEqual (StateType.Unchanged, _supervisor.State);
        Assert.IsNotNull (_supervisor.Subordinates[_subordinate.ID]);
        Assert.AreSame (_supervisor, _subordinate.Supervisor);
      }
    }

    [Test]
    public void StateTracking ()
    {
      Assert.AreEqual (StateType.Unchanged, _supervisor.State);
      Assert.AreEqual (StateType.Unchanged, _subordinate.State);

      _supervisor.Subordinates.Remove (_subordinate);

      Assert.AreEqual (StateType.Changed, _supervisor.State);
      Assert.AreEqual (StateType.Changed, _subordinate.State);
    }

    [Test]
    public void RelationEndPointMap ()
    {
      _supervisor.Subordinates.Remove (_subordinate.ID);

      Assert.IsNull (_subordinate.Supervisor);
    }

    [Test]
    public void SetPropertyValue ()
    {
      _supervisor.Subordinates.Remove (_subordinate);

      Assert.IsNull (_subordinate.Properties[typeof (Employee), "Supervisor"].GetRelatedObjectID ());
    }

    [Test]
    public void SetSupervisorNull ()
    {
      _subordinate.Supervisor = null;

      Assert.IsNull (_subordinate.Supervisor);
      Assert.AreEqual (1, _supervisor.Subordinates.Count);
      Assert.IsNull (_supervisor.Subordinates[_subordinate.ID]);
    }

    [Test]
    public void SetNumericIndexerOfSupervisorNull ()
    {
      Employee employeeToRemove = (Employee) _supervisor.Subordinates[0];
      Employee unaffectedEmployee = (Employee) _supervisor.Subordinates[1];

      _supervisor.Subordinates[0] = null;

      Assert.AreEqual (1, _supervisor.Subordinates.Count);
      Assert.IsTrue (_supervisor.Subordinates.ContainsObject (unaffectedEmployee));
      Assert.IsFalse (_supervisor.Subordinates.ContainsObject (employeeToRemove));

      Assert.AreSame (_supervisor, unaffectedEmployee.Supervisor);
      Assert.IsNull (employeeToRemove.Supervisor);
    }

    [Test]
    public void SetNumericIListIndexerOfSupervisorNull ()
    {
      Employee employeeToRemove = (Employee) _supervisor.Subordinates[0];
      Employee unaffectedEmployee = (Employee) _supervisor.Subordinates[1];

      IList list = _supervisor.Subordinates;
      list[0] = null;

      Assert.AreEqual (1, _supervisor.Subordinates.Count);
      Assert.IsTrue (_supervisor.Subordinates.ContainsObject (unaffectedEmployee));
      Assert.IsFalse (_supervisor.Subordinates.ContainsObject (employeeToRemove));

      Assert.AreSame (_supervisor, unaffectedEmployee.Supervisor);
      Assert.IsNull (employeeToRemove.Supervisor);
    }
  }
}
