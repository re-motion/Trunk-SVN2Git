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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Transaction
{
  [TestFixture]
  public class SubTransactionCommitStateTransitionTest : ClientTransactionStateTransitionBaseTest
  {
    [Test]
    public void CommitRootChangedSubChanged ()
    {
      Order obj = GetChangedThroughPropertyValue ();
      Assert.AreEqual (StateType.Changed, obj.State);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        ++obj.OrderNumber;
        Assert.AreEqual (StateType.Changed, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void CommitRootChangedSubUnchanged ()
    {
      Order obj = GetChangedThroughPropertyValue ();
      Assert.AreEqual (StateType.Changed, obj.State);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void CommitRootChangedSubDeleted ()
    {
      Order obj = GetChangedThroughPropertyValue ();
      Assert.AreEqual (StateType.Changed, obj.State);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        FullyDeleteOrder (obj);
        Assert.AreEqual (StateType.Deleted, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.IsTrue (obj.IsDiscarded);
      }
      Assert.AreEqual (StateType.Deleted, obj.State);
    }

    [Test]
    public void CommitRootUnchangedSubChanged ()
    {
      Order obj = GetUnchanged();
      Assert.AreEqual (StateType.Unchanged, obj.State);
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        ++obj.OrderNumber;
        Assert.AreEqual (StateType.Changed, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void CommitRootUnchangedSubUnchanged ()
    {
      Order obj = GetUnchanged();
      Assert.AreEqual (StateType.Unchanged, obj.State);
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.AreEqual (StateType.Unchanged, obj.State);
    }

    [Test]
    public void CommitRootUnchangedSubDeleted ()
    {
      Order obj = GetUnchanged();
      Assert.AreEqual (StateType.Unchanged, obj.State);
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        FullyDeleteOrder (obj);
        Assert.AreEqual (StateType.Deleted, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.AreEqual (StateType.Deleted, obj.State);
    }

    [Test]
    public void CommitRootNewSubChanged ()
    {
      ClassWithAllDataTypes obj = GetNewUnchanged();
      Assert.AreEqual (StateType.New, obj.State);
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        ++obj.Int32Property;
        Assert.AreEqual (StateType.Changed, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.AreEqual (StateType.New, obj.State);
    }

    [Test]
    public void CommitRootNewSubUnchanged ()
    {
      ClassWithAllDataTypes obj = GetNewUnchanged ();
      Assert.AreEqual (StateType.New, obj.State);
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.AreEqual (StateType.Unchanged, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.AreEqual (StateType.New, obj.State);
    }

    [Test]
    public void CommitRootNewSubDeleted ()
    {
      ClassWithAllDataTypes obj = GetNewUnchanged ();
      Assert.AreEqual (StateType.New, obj.State);
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        obj.Delete();
        Assert.AreEqual (StateType.Deleted, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.IsTrue (obj.IsDiscarded);
    }

    [Test]
    public void CommitRootDeletedSubDiscarded ()
    {
      Order obj = GetDeleted();
      Assert.AreEqual (StateType.Deleted, obj.State);
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.IsTrue (obj.IsDiscarded);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.AreEqual (StateType.Deleted, obj.State);
    }

    [Test]
    public void CommitRootDiscardedSubDiscarded ()
    {
      Order obj = GetDiscarded();
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.IsTrue (obj.IsDiscarded);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.IsTrue (obj.IsDiscarded);
    }

    [Test]
    public void CommitRootUnknownSubChanged ()
    {
      Order obj;
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        obj = GetChangedThroughPropertyValue();
        Assert.AreEqual (StateType.Changed, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void CommitRootUnknownSubUnchanged ()
    {
      Order obj;
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        obj = GetUnchanged();
        Assert.AreEqual (StateType.Unchanged, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.IsNotNull (ClientTransactionMock.DataManager.DataContainerMap[obj.ID]);
      Assert.AreEqual (StateType.Unchanged, obj.State);
    }

    [Test]
    public void CommitRootUnknownSubNew ()
    {
      ClassWithAllDataTypes obj;
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        obj = GetNewUnchanged();
        Assert.AreEqual (StateType.New, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.AreEqual (StateType.New, obj.State);
    }

    [Test]
    public void CommitRootUnknownSubDeleted ()
    {
      Order obj;
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        obj = GetDeleted();
        Assert.AreEqual (StateType.Deleted, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.AreEqual (StateType.Deleted, obj.State);
    }

    [Test]
    public void CommitRootUnknownSubDiscarded ()
    {
      Order obj;
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        obj = GetDiscarded();
        Assert.IsTrue (obj.IsDiscarded);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.IsNull (ClientTransactionMock.DataManager.DataContainerMap[obj.ID]);
    }
  }
}
