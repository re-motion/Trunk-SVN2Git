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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  [TestFixture]
  public class SubTransactionCommitRollbackTest : ClientTransactionBaseTest
  {
    private ClientTransaction _subTransaction;

    public override void SetUp ()
    {
      base.SetUp ();
      _subTransaction = ClientTransactionMock.CreateSubTransaction ();
    }

    [Test]
    public void DiscardReturnsTrue ()
    {
      Assert.AreEqual (true, _subTransaction.Discard ());
    }

    [Test]
    public void DiscardMakesParentWriteable ()
    {
      Assert.IsTrue (_subTransaction.ParentTransaction.IsReadOnly);
      Assert.IsFalse (_subTransaction.IsDiscarded);
      _subTransaction.Discard ();
      Assert.IsTrue (_subTransaction.IsDiscarded);
      Assert.IsFalse (_subTransaction.ParentTransaction.IsReadOnly);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The transaction can no longer be used because it has been discarded.")]
    public void DiscardRendersSubTransactionUnusable ()
    {
      _subTransaction.Discard ();
      using (_subTransaction.EnterDiscardingScope ())
      {
        Order.NewObject ();
      }
    }

    [Test]
    public void SubTransactionCanContinueToBeUsedAfterRollback ()
    {
      _subTransaction.Rollback ();
      Assert.IsFalse (_subTransaction.IsDiscarded);
      using (_subTransaction.EnterDiscardingScope ())
      {
        Order order = Order.NewObject ();
        Assert.IsNotNull (order);
      }
    }

    [Test]
    public void SubTransactionCanContinueToBeUsedAfterCommit ()
    {
      _subTransaction.Commit ();
      Assert.IsFalse (_subTransaction.IsDiscarded);
      using (_subTransaction.EnterDiscardingScope ())
      {
        Order order = Order.NewObject ();
        Assert.IsNotNull (order);
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException), ExpectedMessage = "Object 'Order.*' is already discarded.", MatchType = MessageMatch.Regex)]
    public void RollbackResetsNewedObjects ()
    {
      using (_subTransaction.EnterDiscardingScope ())
      {
        Order order = Order.NewObject();
        _subTransaction.Rollback();
        int i = order.OrderNumber;
      }
    }

    [Test]
    public void RollbackResetsLoadedObjects ()
    {
      using (_subTransaction.EnterDiscardingScope ())
      {
        Order order = Order.GetObject (DomainObjectIDs.Order1);
        order.OrderNumber = 5;

        _subTransaction.Rollback ();

        Assert.AreNotEqual (5, order.OrderNumber);
      }
    }

    [Test]
    public void SubRollbackDoesNotRollbackParent ()
    {
      _subTransaction.Discard ();
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.AreEqual (1, order.OrderNumber);
      order.OrderNumber = 3;
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order.OrderNumber = 5;
        ClientTransactionScope.CurrentTransaction.Rollback ();
        Assert.AreEqual (3, order.OrderNumber);
      }
      Assert.AreEqual (3, order.OrderNumber);
      ClientTransactionMock.Rollback ();
      Assert.AreEqual (1, order.OrderNumber);
    }


    [Test]
    public void ParentTransactionStillReadOnlyAfterCommit ()
    {
      using (_subTransaction.EnterDiscardingScope ())
      {
        Assert.IsTrue (ClientTransactionMock.IsReadOnly);
        ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject ();
        Assert.AreNotEqual (7, classWithAllDataTypes.Int32Property);
        classWithAllDataTypes.Int32Property = 7;
        _subTransaction.Commit ();
        Assert.IsTrue (ClientTransactionMock.IsReadOnly);
      }
    }
    
    [Test]
    public void CommitPropagatesNewObjectsToParentTransaction ()
    {
      ClassWithAllDataTypes classWithAllDataTypes;
      using (_subTransaction.EnterDiscardingScope ())
      {
        classWithAllDataTypes = ClassWithAllDataTypes.NewObject ();
        Assert.AreNotEqual (7, classWithAllDataTypes.Int32Property);
        classWithAllDataTypes.Int32Property = 7;
        _subTransaction.Commit ();
        Assert.AreEqual (7, classWithAllDataTypes.Int32Property);
      }
      Assert.IsNotNull (classWithAllDataTypes);
      Assert.AreEqual (7, classWithAllDataTypes.Int32Property);
    }

    [Test]
    public void CommitPropagatesChangedObjectsToParentTransaction ()
    {
      Order order;
      using (_subTransaction.EnterDiscardingScope ())
      {
        order = Order.GetObject (DomainObjectIDs.Order1);
        order.OrderNumber = 5;

        _subTransaction.Commit ();

        Assert.AreEqual (5, order.OrderNumber);
      }

      Assert.IsNotNull (order);
      Assert.AreEqual (5, order.OrderNumber);
    }

    [Test]
    public void SubCommitDoesNotCommitParent ()
    {
      _subTransaction.Discard ();
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        order.OrderNumber = 5;
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.AreEqual (5, order.OrderNumber);
      ClientTransactionMock.Rollback ();
      Assert.AreEqual (1, order.OrderNumber);
    }

    [Test]
    public void SubCommit_OfDeletedObject_DoesNotRaiseDeletedEvent ()
    {
      using (_subTransaction.EnterDiscardingScope ())
      {
        ClassWithAllDataTypes domainObject = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);

        MockRepository repository = new MockRepository();

        IClientTransactionExtension extensionMock = repository.CreateMock<IClientTransactionExtension>();
        _subTransaction.Extensions.Add ("Mock", extensionMock);

        extensionMock.ObjectDeleting (_subTransaction, domainObject);
        extensionMock.ObjectDeleted (_subTransaction, domainObject);

        repository.ReplayAll();
        domainObject.Delete();
        repository.VerifyAll();

        repository.BackToRecordAll();
        extensionMock.Committing (null, null);
        LastCall.IgnoreArguments();
        extensionMock.Committed (null, null);
        LastCall.IgnoreArguments ();
        repository.ReplayAll ();
       
        _subTransaction.Commit();
        repository.VerifyAll ();
      }
    }

    [Test]
    public void SubCommit_OfDeletedObject_DoesNotRaiseDeletedEvent_WithRelations ()
    {
      using (_subTransaction.EnterDiscardingScope ())
      {
        Order domainObject = Order.GetObject (DomainObjectIDs.Order1);
        domainObject.OrderItems[0].Delete();

        MockRepository repository = new MockRepository ();

        IClientTransactionExtension extensionMock = repository.CreateMock<IClientTransactionExtension> ();
        _subTransaction.Extensions.Add ("Mock", extensionMock);

        using (repository.Ordered ())
        {
          extensionMock.Committing (null, null);
          LastCall.IgnoreArguments ();
          extensionMock.Committed (null, null);
          LastCall.IgnoreArguments ();
        }

        repository.ReplayAll ();

        _subTransaction.Commit ();
        
        repository.VerifyAll ();
      }
    }
  }
}
