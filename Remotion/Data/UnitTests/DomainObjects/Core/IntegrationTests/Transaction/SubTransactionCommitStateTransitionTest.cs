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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  [TestFixture]
  public class SubTransactionCommitStateTransitionTest : ClientTransactionStateTransitionBaseTest
  {
    [Test]
    public void CommitRootChangedSubChanged ()
    {
      Order obj = GetChangedThroughPropertyValue ();
      Assert.AreEqual (StateType.Changed, obj.State);
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
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
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        obj.EnsureDataAvailable ();
        Assert.AreEqual (StateType.Unchanged, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void CommitRootChangedSubNotLoaded ()
    {
      Order obj = GetChangedThroughPropertyValue ();
      Assert.AreEqual (StateType.Changed, obj.State);
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.NotLoadedYet, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.AreEqual (StateType.Changed, obj.State);
    }

    [Test]
    public void CommitRootChangedSubDeleted ()
    {
      Order obj = GetChangedThroughPropertyValue ();
      Assert.AreEqual (StateType.Changed, obj.State);
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        FullyDeleteOrder (obj);
        Assert.AreEqual (StateType.Deleted, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit();
        Assert.IsTrue (obj.IsInvalid);
      }
      Assert.AreEqual (StateType.Deleted, obj.State);
    }

    [Test]
    public void CommitRootUnchangedSubChanged ()
    {
      Order obj = GetUnchanged();
      Assert.AreEqual (StateType.Unchanged, obj.State);
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
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
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        obj.EnsureDataAvailable ();
        Assert.AreEqual (StateType.Unchanged, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.AreEqual (StateType.Unchanged, obj.State);
    }

    [Test]
    public void CommitRootUnchangedSubNotLoaded ()
    {
      Order obj = GetUnchanged ();
      Assert.AreEqual (StateType.Unchanged, obj.State);
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.NotLoadedYet, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.AreEqual (StateType.Unchanged, obj.State);
    }

    [Test]
    public void CommitRootUnchangedSubDeleted ()
    {
      Order obj = GetUnchanged();
      Assert.AreEqual (StateType.Unchanged, obj.State);
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
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
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
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
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        obj.EnsureDataAvailable ();
        Assert.AreEqual (StateType.Unchanged, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.AreEqual (StateType.New, obj.State);
    }

    [Test]
    public void CommitRootNewSubNotLoaded ()
    {
      ClassWithAllDataTypes obj = GetNewUnchanged ();
      Assert.AreEqual (StateType.New, obj.State);
      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (StateType.NotLoadedYet, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.AreEqual (StateType.New, obj.State);
    }

    [Test]
    public void CommitRootNewSubDeleted ()
    {
      ClassWithAllDataTypes obj = GetNewUnchanged ();
      Assert.AreEqual (StateType.New, obj.State);
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        obj.Delete();
        Assert.AreEqual (StateType.Deleted, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.IsTrue (obj.IsInvalid);
    }

    [Test]
    public void CommitRootDeletedSubDiscarded ()
    {
      Order obj = GetDeleted();
      Assert.AreEqual (StateType.Deleted, obj.State);
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.IsTrue (obj.IsInvalid);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.AreEqual (StateType.Deleted, obj.State);
    }

    [Test]
    public void CommitRootDiscardedSubDiscarded ()
    {
      Order obj = GetInvalid();
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.IsTrue (obj.IsInvalid);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.IsTrue (obj.IsInvalid);
    }

    [Test]
    public void CommitRootUnknownSubChanged ()
    {
      Order obj;
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
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
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        obj = GetUnchanged();
        Assert.AreEqual (StateType.Unchanged, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.IsNotNull (TestableClientTransaction.DataManager.DataContainers[obj.ID]);
      Assert.AreEqual (StateType.Unchanged, obj.State);
    }

    [Test]
    public void CommitRootUnknownSubNew ()
    {
      ClassWithAllDataTypes obj;
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        obj = GetNewUnchanged();
        Assert.AreEqual (StateType.New, obj.State);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.AreEqual (StateType.New, obj.State);
    }

    [Test]
    public void CommitRootUnknown_SubNew_SubSubAlsoNew ()
    {
      ClassWithAllDataTypes objectCreatedInSub;
      ClassWithAllDataTypes objectCreatedInSubSub;

      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        objectCreatedInSub = GetNewUnchanged ();
        Assert.AreEqual (StateType.New, objectCreatedInSub.State);

        using (ClientTransactionScope.CurrentTransaction.CreateSubTransaction ().EnterDiscardingScope ())
        {
          objectCreatedInSubSub = GetNewUnchanged ();

          Assert.AreEqual (StateType.NotLoadedYet, objectCreatedInSub.State);
          Assert.AreEqual (StateType.New, objectCreatedInSubSub.State);

          ClientTransactionScope.CurrentTransaction.Commit ();

          Assert.AreEqual (StateType.NotLoadedYet, objectCreatedInSub.State);
          Assert.AreEqual (StateType.Unchanged, objectCreatedInSubSub.State);
        }

        Assert.AreEqual (StateType.New, objectCreatedInSub.State);
        Assert.AreEqual (StateType.New, objectCreatedInSubSub.State);
        
        ClientTransactionScope.CurrentTransaction.Commit ();

        Assert.AreEqual (StateType.Unchanged, objectCreatedInSub.State);
        Assert.AreEqual (StateType.Unchanged, objectCreatedInSubSub.State);
      }

      Assert.AreEqual (StateType.New, objectCreatedInSub.State);
      Assert.AreEqual (StateType.New, objectCreatedInSubSub.State);
    }

    [Test]
    public void CommitRootUnknownSubDeleted ()
    {
      Order obj;
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
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
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        obj = GetInvalid();
        Assert.IsTrue (obj.IsInvalid);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.IsNull (TestableClientTransaction.DataManager.DataContainers[obj.ID]);
    }

    [Test]
    public void CommitRootChangedSubMakesUnchanged ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      ++order.OrderNumber;
      Assert.That (order.State, Is.EqualTo (StateType.Changed));

      using (TestableClientTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        --order.OrderNumber;
        ClientTransaction.Current.Commit ();
      }

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
    }
  }
}
