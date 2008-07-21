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
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Transaction
{
  [TestFixture]
  public class ClientTransactionAsITransactionTest : ClientTransactionBaseTest
  {
    private ITransaction _transaction;

    public override void SetUp()
    {
 	     base.SetUp();
     
      _transaction = ClientTransactionMock;
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
      Assert.IsInstanceOfType (typeof (SubClientTransaction), child);
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
      Assert.AreSame (_transaction, child.Parent);
      Assert.AreSame (child, child.CreateChild ().Parent);
    }

    [Test]
    public void Release ()
    {
      ITransaction child = _transaction.CreateChild ();
      Assert.IsTrue (((ClientTransaction) _transaction).IsReadOnly);
      Assert.IsFalse (((ClientTransaction) child).IsDiscarded);
      child.Release ();
      Assert.IsFalse (((ClientTransaction) _transaction).IsReadOnly);
      Assert.IsTrue (((ClientTransaction) child).IsDiscarded);
    }
  }
}
