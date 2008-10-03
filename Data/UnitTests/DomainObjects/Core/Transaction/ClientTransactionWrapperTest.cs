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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  [TestFixture]
  public class ClientTransactionAsITransactionTest : ClientTransactionBaseTest
  {
    private ITransaction _transaction;

    public override void SetUp()
    {
 	     base.SetUp();
     
      _transaction = new ClientTransactionWrapper (ClientTransactionMock);
    }

    [Test]
    public void CanCreateChild ()
    {
      Assert.IsTrue (_transaction.CanCreateChild);
    }

    [Test]
    public void CreateChild ()
    {
      ITransaction child = _transaction.CreateChild ();
      Assert.IsNotNull (child);
      Assert.IsInstanceOfType (typeof (ClientTransactionWrapper), child);
      Assert.IsInstanceOfType (typeof (SubClientTransaction), ((ClientTransactionWrapper)child).WrappedInstance);
    }

    [Test]
    public void IsChild ()
    {
      ITransaction child = _transaction.CreateChild ();
      Assert.IsTrue (child.IsChild);
      Assert.IsFalse (_transaction.IsChild);
      Assert.IsTrue (child.CreateChild().IsChild);
    }

    [Test]
    public void Parent ()
    {
      ITransaction child = _transaction.CreateChild ();
      Assert.AreSame (((ClientTransactionWrapper)_transaction).WrappedInstance, ((ClientTransactionWrapper)child.Parent).WrappedInstance);
      Assert.AreSame (((ClientTransactionWrapper) child).WrappedInstance, ((ClientTransactionWrapper) child.CreateChild ().Parent).WrappedInstance);
    }

    [Test]
    public void Release ()
    {
      ITransaction child = _transaction.CreateChild ();
      Assert.IsTrue (((ClientTransactionWrapper) _transaction).WrappedInstance.IsReadOnly);
      Assert.IsFalse (((ClientTransactionWrapper)child).WrappedInstance.IsDiscarded);
      child.Release ();
      Assert.IsFalse (((ClientTransactionWrapper) _transaction).WrappedInstance.IsReadOnly);
      Assert.IsTrue (((ClientTransactionWrapper) child).WrappedInstance.IsDiscarded);
    }

    [Test]
    public void EnterScope ()
    {
      ITransaction transaction = new ClientTransactionWrapper(ClientTransaction.CreateRootTransaction ());

      ClientTransactionScope.ResetActiveScope ();
      Assert.That (ClientTransactionScope.ActiveScope, Is.Null);

      ITransactionScope transactionScope = transaction.EnterScope ();

      Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (transactionScope));
      Assert.That (ClientTransactionScope.ActiveScope.ScopedTransaction, Is.SameAs (((ClientTransactionWrapper) transaction).WrappedInstance));
      Assert.That (ClientTransactionScope.ActiveScope.AutoRollbackBehavior, Is.EqualTo (AutoRollbackBehavior.None));
      ClientTransactionScope.ResetActiveScope ();
    }
  }
}
