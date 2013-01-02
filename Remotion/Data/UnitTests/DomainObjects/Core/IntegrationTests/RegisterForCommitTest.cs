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
using System.Collections.ObjectModel;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;
using Remotion.Data.UnitTests.UnitTesting;
using System.Linq;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  [Ignore ("TODO 1961")]
  public class RegisterForCommitTest : ClientTransactionBaseTest
  {
    [Test]
    public void NewObject_NoOp ()
    {
      var domainObject = ClassWithAllDataTypes.NewObject();
      Assert.That (domainObject.State, Is.EqualTo (StateType.New));

      domainObject.RegisterForCommit();

      Assert.That (domainObject.State, Is.EqualTo (StateType.New));
      CheckNotMarkedAsChanged (domainObject);
    }

    [Test]
    public void ChangedObject_RemembersRegistration_EvenWhenChangedBack ()
    {
      var domainObject = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      ++domainObject.Int32Property;
      Assert.That (domainObject.State, Is.EqualTo (StateType.Changed));

      domainObject.RegisterForCommit ();

      Assert.That (domainObject.State, Is.EqualTo (StateType.Changed));
      CheckMarkedAsChanged (domainObject);

      --domainObject.Int32Property;
      Assert.That (domainObject.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void UnchangedObject ()
    {
      var domainObject = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.That (domainObject.State, Is.EqualTo (StateType.Unchanged));

      domainObject.RegisterForCommit ();

      Assert.That (domainObject.State, Is.EqualTo (StateType.Changed));
      CheckMarkedAsChanged (domainObject);

      ++domainObject.Int32Property;
      --domainObject.Int32Property;

      Assert.That (domainObject.State, Is.EqualTo (StateType.Changed));
    }

    [Test]
    public void DeletedObject ()
    {
      var domainObject = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      domainObject.Delete();
      Assert.That (domainObject.State, Is.EqualTo (StateType.Deleted));

      Assert.That (() => domainObject.RegisterForCommit (), Throws.TypeOf<ObjectDeletedException>());

      Assert.That (domainObject.State, Is.EqualTo (StateType.Deleted));
      CheckNotMarkedAsChanged (domainObject);
    }

    [Test]
    public void InvalidObject ()
    {
      var domainObject = ClassWithAllDataTypes.NewObject();
      domainObject.Delete ();
      Assert.That (domainObject.State, Is.EqualTo (StateType.Invalid));

      Assert.That (() => domainObject.RegisterForCommit (), Throws.TypeOf<ObjectInvalidException> ());

      Assert.That (domainObject.State, Is.EqualTo (StateType.Invalid));

      ResurrectionService.ResurrectInvalidObject (TestableClientTransaction, domainObject.ID);

      Assert.That (domainObject.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    public void NotLoadedYetObject_LoadedToUnchanged ()
    {
      var domainObject = (ClassWithAllDataTypes) LifetimeService.GetObjectReference (TestableClientTransaction, DomainObjectIDs.ClassWithAllDataTypes1);
      Assert.That (TestableClientTransaction.DataManager.DataContainers[domainObject.ID], Is.Null);
      Assert.That (domainObject.State, Is.EqualTo (StateType.NotLoadedYet));

      domainObject.RegisterForCommit();

      Assert.That (domainObject.State, Is.EqualTo (StateType.Changed));
      Assert.That (TestableClientTransaction.DataManager.DataContainers[domainObject.ID], Is.Not.Null);
      CheckMarkedAsChanged (domainObject);
    }

    [Test]
    public void NotLoadedYetObject_LoadedToChanged ()
    {
      var domainObject = (ClassWithAllDataTypes) LifetimeService.GetObjectReference (TestableClientTransaction, DomainObjectIDs.ClassWithAllDataTypes1);
      domainObject.ProtectedLoaded += (sender, args) => ++domainObject.Int32Property;
      Assert.That (TestableClientTransaction.DataManager.DataContainers[domainObject.ID], Is.Null);
      Assert.That (domainObject.State, Is.EqualTo (StateType.NotLoadedYet));

      domainObject.RegisterForCommit ();

      Assert.That (domainObject.State, Is.EqualTo (StateType.Changed));
      Assert.That (TestableClientTransaction.DataManager.DataContainers[domainObject.ID], Is.Not.Null);
      CheckMarkedAsChanged (domainObject);
    }

    [Test]
    public void CommitRoot ()
    {
      var newObject = ClassWithAllDataTypes.NewObject ();
      newObject.DateTimeProperty = new DateTime (2012, 12, 12);
      newObject.DateProperty = new DateTime (2012, 12, 12);
      Assert.That (newObject.State, Is.EqualTo (StateType.New));

      var changedObject = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      ++changedObject.Int32Property;
      Assert.That (changedObject.State, Is.EqualTo (StateType.Changed));

      var unchangedObject = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2);
      Assert.That (unchangedObject.State, Is.EqualTo (StateType.Unchanged));

      newObject.RegisterForCommit();
      changedObject.RegisterForCommit();
      unchangedObject.RegisterForCommit();

      Assert.That (newObject.State, Is.EqualTo (StateType.New));
      CheckNotMarkedAsChanged (newObject);
      Assert.That (changedObject.State, Is.EqualTo (StateType.Changed));
      CheckMarkedAsChanged (changedObject);
      Assert.That (unchangedObject.State, Is.EqualTo (StateType.Changed));
      CheckMarkedAsChanged (unchangedObject);

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (TestableClientTransaction);
      listenerMock.Expect (
          mock => mock.TransactionCommitting (
              Arg.Is (TestableClientTransaction),
              Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new[] { newObject, changedObject, unchangedObject }),
              Arg<ICommittingEventRegistrar>.Is.Anything));
      listenerMock.Expect (
          mock => mock.TransactionCommitValidate (
              Arg.Is (TestableClientTransaction),
              Arg<ReadOnlyCollection<PersistableData>>.Matches (
                  x => x.Select (d => d.DomainObject).SetEquals (new[] { newObject, changedObject, unchangedObject }))));

      SetDatabaseModifyable ();
      CommitTransactionAndCheckTimestamps (newObject, changedObject, unchangedObject);

      listenerMock.VerifyAllExpectations();

      Assert.That (newObject.State, Is.EqualTo (StateType.Unchanged));
      CheckNotMarkedAsChanged (newObject);
      Assert.That (changedObject.State, Is.EqualTo (StateType.Unchanged));
      CheckNotMarkedAsChanged (changedObject);
      Assert.That (unchangedObject.State, Is.EqualTo (StateType.Unchanged));
      CheckNotMarkedAsChanged (unchangedObject);
    }

    [Test]
    public void CommitRoot_RegisterForUnchanged_LeadsToConcurrencyCheck ()
    {
      var unchangedObject = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2);
      unchangedObject.RegisterForCommit ();

      SetDatabaseModifyable();
      ModifyAndCommitInOtherTransaction (unchangedObject.ID);

      Assert.That (() => TestableClientTransaction.Commit(), Throws.TypeOf<ConcurrencyViolationException>());
    }

    [Test]
    public void CommitSub ()
    {
      ClassWithAllDataTypes newObject;
      ClassWithAllDataTypes changedObject;
      ClassWithAllDataTypes unchangedObject;

      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        newObject = ClassWithAllDataTypes.NewObject();
        Assert.That (newObject.State, Is.EqualTo (StateType.New));

        changedObject = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
        ++changedObject.Int32Property;
        Assert.That (changedObject.State, Is.EqualTo (StateType.Changed));

        unchangedObject = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2);
        Assert.That (unchangedObject.State, Is.EqualTo (StateType.Unchanged));

        newObject.RegisterForCommit();
        changedObject.RegisterForCommit();
        unchangedObject.RegisterForCommit();

        Assert.That (newObject.State, Is.EqualTo (StateType.New));
        CheckNotMarkedAsChanged (newObject);
        Assert.That (changedObject.State, Is.EqualTo (StateType.Changed));
        CheckMarkedAsChanged (changedObject);
        Assert.That (unchangedObject.State, Is.EqualTo (StateType.Changed));
        CheckMarkedAsChanged (unchangedObject);

        var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (ClientTransaction.Current);
        try
        {
          listenerMock.Expect (
              mock => mock.TransactionCommitting (
                  Arg.Is (ClientTransaction.Current),
                  Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new[] { newObject, changedObject, unchangedObject }),
                  Arg<ICommittingEventRegistrar>.Is.Anything));
          listenerMock.Expect (
              mock => mock.TransactionCommitValidate (
                  Arg.Is (ClientTransaction.Current),
                  Arg<ReadOnlyCollection<PersistableData>>.Matches (
                      x => x.Select (d => d.DomainObject).SetEquals (new[] { newObject, changedObject, unchangedObject }))));

          ClientTransaction.Current.Commit();
        }
        finally
        {
          ClientTransactionTestHelper.RemoveListener (ClientTransaction.Current, listenerMock);
        }

        listenerMock.VerifyAllExpectations();

        Assert.That (newObject.State, Is.EqualTo (StateType.Unchanged));
        CheckNotMarkedAsChanged (newObject);
        Assert.That (changedObject.State, Is.EqualTo (StateType.Unchanged));
        CheckNotMarkedAsChanged (changedObject);
        Assert.That (unchangedObject.State, Is.EqualTo (StateType.Unchanged));
        CheckNotMarkedAsChanged (unchangedObject);
      }

      Assert.That (newObject.State, Is.EqualTo (StateType.New));
      CheckNotMarkedAsChanged (newObject);
      Assert.That (changedObject.State, Is.EqualTo (StateType.Changed));
      CheckMarkedAsChanged (changedObject);
      Assert.That (unchangedObject.State, Is.EqualTo (StateType.Changed));
      CheckMarkedAsChanged (unchangedObject);
    }

    [Test]
    public void Rollback ()
    {
      var newObject = ClassWithAllDataTypes.NewObject ();
      Assert.That (newObject.State, Is.EqualTo (StateType.New));

      var changedObject = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      ++changedObject.Int32Property;
      Assert.That (changedObject.State, Is.EqualTo (StateType.Changed));

      var unchangedObject = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2);
      Assert.That (unchangedObject.State, Is.EqualTo (StateType.Unchanged));

      newObject.RegisterForCommit ();
      changedObject.RegisterForCommit ();
      unchangedObject.RegisterForCommit ();

      Assert.That (newObject.State, Is.EqualTo (StateType.New));
      CheckNotMarkedAsChanged (newObject);
      Assert.That (changedObject.State, Is.EqualTo (StateType.Changed));
      CheckMarkedAsChanged (changedObject);
      Assert.That (unchangedObject.State, Is.EqualTo (StateType.Changed));
      CheckMarkedAsChanged (unchangedObject);

      var listenerMock = ClientTransactionTestHelper.CreateAndAddListenerMock (TestableClientTransaction);
      listenerMock.Expect (
          mock => mock.TransactionRollingBack (
              Arg.Is (TestableClientTransaction),
              Arg<ReadOnlyCollection<DomainObject>>.List.Equivalent (new[] { newObject, changedObject, unchangedObject })));

      TestableClientTransaction.Rollback();

      listenerMock.VerifyAllExpectations ();

      Assert.That (newObject.State, Is.EqualTo (StateType.Invalid));
      Assert.That (changedObject.State, Is.EqualTo (StateType.Unchanged));
      CheckNotMarkedAsChanged (changedObject);
      Assert.That (unchangedObject.State, Is.EqualTo (StateType.Unchanged));
      CheckNotMarkedAsChanged (unchangedObject);
    }

    private void CommitTransactionAndCheckTimestamps (params DomainObject[] domainObjects)
    {
      var timestampsBefore = domainObjects.Select (obj => obj.Timestamp).ToArray();
      TestableClientTransaction.Commit();
      var timestampsAfter = domainObjects.Select (obj => obj.Timestamp).ToArray ();
      Assert.That (timestampsBefore, Is.Not.EqualTo (timestampsAfter));
    }

    private void ModifyAndCommitInOtherTransaction (ObjectID objectID)
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var domainObject = ClassWithAllDataTypes.GetObject (objectID);
        ++domainObject.Int32Property;
        ClientTransaction.Current.Commit();
      }
    }
    
    private void CheckMarkedAsChanged (ClassWithAllDataTypes domainObject)
    {
      Assert.That (domainObject.InternalDataContainer.HasBeenMarkedChanged, Is.True);
    }

    private void CheckNotMarkedAsChanged (ClassWithAllDataTypes domainObject)
    {
      Assert.That (domainObject.InternalDataContainer.HasBeenMarkedChanged, Is.False);
    }
  }

  public static class Temp
  {
    public static void RegisterForCommit (this DomainObject domainObject)
    {
      // TODO 1961
      //domainObject.EnsureDataAvailable ();
      //if (domainObject.State == StateType.Deleted)
      //  throw new ObjectDeletedException (domainObject.ID);

      //if (domainObject.State != StateType.New)
      //  domainObject.MarkAsChanged();
    }
  }
}