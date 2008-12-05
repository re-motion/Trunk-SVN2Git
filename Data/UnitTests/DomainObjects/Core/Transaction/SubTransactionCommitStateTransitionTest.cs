// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
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
