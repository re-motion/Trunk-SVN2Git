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
using NUnit.Framework.Constraints;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  [TestFixture]
  public class NotFoundObjectTest : ClientTransactionBaseTest
  {
    private ObjectID _nonExistingObjectID;
    private ObjectID _nonExistingObjectIDForSubtransaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _nonExistingObjectID = new ObjectID (typeof (Order), Guid.NewGuid());

      var classWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      classWithAllDataTypes.Delete();
      _nonExistingObjectIDForSubtransaction = classWithAllDataTypes.ID;
    }

    [Test]
    [Ignore ("TODO 4920")]
    public void GetObject_True_ShouldThrowObjectInvalidException_AndMarkObjectNotFound ()
    {
      Assert.That (() => Order.GetObject (_nonExistingObjectID, true), ThrowsObjectInvalidExceptionForNotFoundObject (_nonExistingObjectID));

      CheckObjectIsMarkedInvalid (_nonExistingObjectID);

      // After the object has been marked invalid
      Assert.That (() => Order.GetObject (_nonExistingObjectID, true), ThrowsObjectInvalidException (_nonExistingObjectID));
    }

    [Test]
    [Ignore ("TODO 4920")]
    public void GetObject_False_ShouldThrowObjectInvalidException_AndMarkObjectNotFound ()
    {
      Assert.That (() => Order.GetObject (_nonExistingObjectID, false), ThrowsObjectInvalidExceptionForNotFoundObject (_nonExistingObjectID));

      CheckObjectIsMarkedInvalid (_nonExistingObjectID);

      // After the object has been marked invalid
      Assert.That (() => Order.GetObject (_nonExistingObjectID, true), ThrowsObjectInvalidException (_nonExistingObjectID));
    }

    [Test]
    public void GetObject_Subtransaction ()
    {
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (
            () => Order.GetObject (_nonExistingObjectIDForSubtransaction, true),
            ThrowsObjectInvalidException (_nonExistingObjectIDForSubtransaction));

        CheckObjectIsMarkedInvalid (_nonExistingObjectIDForSubtransaction);
      }
    }

    [Test]
    public void GetObjectReference_ShouldGiveNotLoadedYetObject ()
    {
      var instance = LifetimeService.GetObjectReference (TestableClientTransaction, _nonExistingObjectID);
      Assert.That (instance.State, Is.EqualTo (StateType.NotLoadedYet));
    }

    [Test]
    [Ignore ("TODO 4920")]
    public void EnsureDataAvailable_ShouldThrow_AndMarkObjectNotFound ()
    {
      var instance = LifetimeService.GetObjectReference (TestableClientTransaction, _nonExistingObjectID);
      Assert.That (instance.State, Is.EqualTo (StateType.NotLoadedYet));

      Assert.That (() => instance.EnsureDataAvailable(), ThrowsObjectInvalidExceptionForNotFoundObject (_nonExistingObjectID));

      CheckObjectIsMarkedInvalid (instance.ID);

      // After the object has been marked invalid
      Assert.That (() => instance.EnsureDataAvailable (), ThrowsObjectInvalidException (_nonExistingObjectID));
    }

    [Test]
    public void EnsureDataAvailable_Subtransaction ()
    {
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var instance = LifetimeService.GetObjectReference (ClientTransaction.Current, _nonExistingObjectIDForSubtransaction);
        CheckObjectIsMarkedInvalid (instance.ID);
        
        Assert.That (() => instance.EnsureDataAvailable (), ThrowsObjectInvalidException (_nonExistingObjectIDForSubtransaction));
      }
    }

    [Test]
    [Ignore ("TODO 4920")]
    public void EnsureDataAvailable_MultipleObjects_ShouldThrow_AndMarkObjectNotFound ()
    {
      Assert.That (
          () => TestableClientTransaction.EnsureDataAvailable (new[] { _nonExistingObjectID, DomainObjectIDs.Order1 }),
          ThrowsObjectInvalidExceptionForNotFoundObject (_nonExistingObjectID));

      CheckObjectIsMarkedInvalid (_nonExistingObjectID);

      // After the object has been marked invalid
      Assert.That (
          () => TestableClientTransaction.EnsureDataAvailable (new[] { _nonExistingObjectID, DomainObjectIDs.Order1 }), 
          ThrowsObjectInvalidException (_nonExistingObjectID));
    }

    [Test]
    public void EnsureDataAvailable_MultipleObjects_Subtransaction ()
    {
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        CheckObjectIsMarkedInvalid (_nonExistingObjectIDForSubtransaction);
        Assert.That (
            () => ClientTransaction.Current.EnsureDataAvailable (new[] { _nonExistingObjectIDForSubtransaction, DomainObjectIDs.Order1 }),
            ThrowsObjectInvalidException (_nonExistingObjectIDForSubtransaction));
      }
    }

    [Test]
    [Ignore ("TODO 4920")]
    public void TryEnsureDataAvailable_ShouldReturnFalse_AndMarkObjectNotFound ()
    {
      var instance = LifetimeService.GetObjectReference (TestableClientTransaction, _nonExistingObjectID);
      Assert.That (instance.State, Is.EqualTo (StateType.NotLoadedYet));

      var result = instance.TryEnsureDataAvailable ();

      Assert.That (result, Is.False);
      CheckObjectIsMarkedInvalid (instance.ID);

      // After the object has been marked invalid
      Assert.That (() => instance.TryEnsureDataAvailable (), ThrowsObjectInvalidException (_nonExistingObjectID));
    }

    [Test]
    public void TryEnsureDataAvailable_Subtransaction ()
    {
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var instance = LifetimeService.GetObjectReference (ClientTransaction.Current, _nonExistingObjectIDForSubtransaction);
        CheckObjectIsMarkedInvalid (instance.ID);

        Assert.That (() => instance.TryEnsureDataAvailable (), ThrowsObjectInvalidException (_nonExistingObjectIDForSubtransaction));
      }
    }

    [Test]
    [Ignore ("TODO 4920")]
    public void TryEnsureDataAvailable_MultipleObjects_ShouldReturnFalse_AndMarkObjectNotFound ()
    {
      var result = TestableClientTransaction.TryEnsureDataAvailable (new[] { _nonExistingObjectID, DomainObjectIDs.Order1 });
      Assert.That (result, Is.False);
      CheckObjectIsMarkedInvalid (_nonExistingObjectID);

      // After the object has been marked invalid
      Assert.That (
          () => TestableClientTransaction.TryEnsureDataAvailable (new[] { _nonExistingObjectID, DomainObjectIDs.Order1 }),
          ThrowsObjectInvalidException (_nonExistingObjectID));
    }

    [Test]
    public void TryEnsureDataAvailable_MultipleObjects_Subtransaction ()
    {
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        CheckObjectIsMarkedInvalid (_nonExistingObjectIDForSubtransaction);
        Assert.That (
            () => ClientTransaction.Current.TryEnsureDataAvailable (new[] { _nonExistingObjectIDForSubtransaction, DomainObjectIDs.Order1 }),
            ThrowsObjectInvalidException (_nonExistingObjectIDForSubtransaction));
      }
    }

    [Test]
    [Ignore ("TODO 4920")]
    public void PropertyAccess_ShouldThrowObjectInvalidException_ValueProperty ()
    {
      var instance = (Order) LifetimeService.GetObjectReference (TestableClientTransaction, _nonExistingObjectID);
      Assert.That (instance.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (() => instance.OrderNumber, ThrowsObjectInvalidExceptionForNotFoundObject (_nonExistingObjectID));
      CheckObjectIsMarkedInvalid (instance.ID);

      // After the object has been marked invalid
      Assert.That (() => instance.OrderNumber, ThrowsObjectInvalidException (_nonExistingObjectID));
    }

    [Test]
    [Ignore ("TODO 4920")]
    public void PropertyAccess_ShouldThrowObjectInvalidException_VirtualRelationProperty ()
    {
      var instance = (Order) LifetimeService.GetObjectReference (TestableClientTransaction, _nonExistingObjectID);
      Assert.That (instance.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (() => instance.OrderTicket, ThrowsObjectInvalidExceptionForNotFoundObject (_nonExistingObjectID));
      CheckObjectIsMarkedInvalid (instance.ID);

      // After the object has been marked invalid
      Assert.That (() => instance.OrderTicket, ThrowsObjectInvalidException (_nonExistingObjectID));
    }

    [Test]
    [Ignore ("TODO 4920")]
    public void PropertyAccess_ShouldThrowObjectInvalidException_ForeignKeyRelationProperty ()
    {
      var instance = (Order) LifetimeService.GetObjectReference (TestableClientTransaction, _nonExistingObjectID);
      Assert.That (instance.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (() => instance.Customer, ThrowsObjectInvalidExceptionForNotFoundObject (_nonExistingObjectID));
      CheckObjectIsMarkedInvalid (instance.ID);

      // After the object has been marked invalid
      Assert.That (() => instance.Customer, ThrowsObjectInvalidException (_nonExistingObjectID));
    }

    [Test]
    [Ignore ("TODO 4920")]
    public void PropertyAccess_ShouldThrowObjectInvalidException_CollectionRelationProperty ()
    {
      var instance = (Order) LifetimeService.GetObjectReference (TestableClientTransaction, _nonExistingObjectID);
      Assert.That (instance.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (() => instance.OrderItems, ThrowsObjectInvalidExceptionForNotFoundObject (_nonExistingObjectID));
      CheckObjectIsMarkedInvalid (instance.ID);

      // After the object has been marked invalid
      Assert.That (() => instance.OrderItems, ThrowsObjectInvalidException (_nonExistingObjectID));
    }

    [Test]
    public void PropertyAccess_Subtransaction ()
    {
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var instance = (ClassWithAllDataTypes) LifetimeService.GetObjectReference (ClientTransaction.Current, _nonExistingObjectIDForSubtransaction);
        Assert.That (instance.State, Is.EqualTo (StateType.Invalid));
        Assert.That (() => instance.StringProperty, ThrowsObjectInvalidException (_nonExistingObjectIDForSubtransaction));
      }
    }

    [Test]
    [Ignore ("TODO 4920")]
    public void UnidirectionalRelationProperty_ShouldReturnInvalidObject ()
    {
      SetDatabaseModifyable ();

      // Need to disable the foreign key constraints so that the property is allowed to point to an invalid ID in the database
      var clientTable = (TableDefinition) GetTypeDefinition (typeof (Client)).StorageEntityDefinition;
      DisableConstraints (clientTable);

      ObjectID clientID = null;
      try
      {
        clientID = CreateClientWithNonExistingParentClient();
        
        var client = Client.GetObject (clientID);
        Client instance = null;
        Assert.That (() => instance = client.ParentClient, Throws.Nothing);
        CheckObjectIsMarkedInvalid (instance.ID);
      }
      finally
      {
        if (clientID != null)
          CleanupClientWithNonExistingParentClient(clientID);

        EnableConstraints (clientTable);
      }
    }

    [Test]
    public void UnidirectionalRelationProperty_Subtransaction ()
    {
      var newClient = Client.NewObject ();
      newClient.ParentClient = Client.GetObject (DomainObjectIDs.Client3);
      newClient.ParentClient.Delete ();

      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Client instance = null;
        Assert.That (() => instance = newClient.ParentClient, Throws.Nothing);
        CheckObjectIsMarkedInvalid (instance.ID);
      }
    }

    private void EnableConstraints (TableDefinition tableDefinition)
    {
      var commandText = string.Format ("ALTER TABLE [{0}] WITH CHECK CHECK CONSTRAINT all", tableDefinition.TableName.EntityName);
      DatabaseAgent.ExecuteCommand (commandText);
    }

    private void DisableConstraints (TableDefinition tableDefinition)
    {
      var commandText = string.Format ("ALTER TABLE [{0}] NOCHECK CONSTRAINT all", tableDefinition.TableName.EntityName);
      DatabaseAgent.ExecuteCommand (commandText);
    }

    private void CheckObjectIsMarkedInvalid (ObjectID objectID)
    {
      var instance = LifetimeService.GetObjectReference (ClientTransaction.Current, objectID);
      Assert.That (instance.State, Is.EqualTo (StateType.Invalid));
    }

    private IResolveConstraint ThrowsObjectInvalidExceptionForNotFoundObject (ObjectID objectID)
    {
      var expected = string.Format ("Object '{0}' is invalid in this transaction. Reason: The object could not be found.", objectID);
      return Throws.TypeOf<ObjectInvalidException> ().With.Message.EqualTo (expected);
    }

    private IResolveConstraint ThrowsObjectInvalidException (ObjectID objectID)
    {
      var expected = string.Format ("Object '{0}' is invalid in this transaction.", objectID);
      return Throws.TypeOf<ObjectInvalidException> ().With.Message.EqualTo (expected);
    }

    private ObjectID CreateClientWithNonExistingParentClient ()
    {
      ObjectID newClientID;
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var newClient = Client.NewObject ();
        newClientID = newClient.ID;
        newClient.ParentClient = Client.GetObject (DomainObjectIDs.Client3);
        newClient.ParentClient.Delete ();
        ClientTransaction.Current.Commit ();
        Assert.That (newClient.ParentClient.State, Is.EqualTo (StateType.Invalid));
      }
      return newClientID;
    }

    private void CleanupClientWithNonExistingParentClient (ObjectID clientID)
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var client = Client.GetObject (clientID);
        client.Delete ();
        ClientTransaction.Current.Commit ();
      }
    }
  }
}