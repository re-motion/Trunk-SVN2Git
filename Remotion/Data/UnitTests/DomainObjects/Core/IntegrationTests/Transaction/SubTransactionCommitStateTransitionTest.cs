// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
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
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
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
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
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
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
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
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
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
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
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
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
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
    public void CommitRootUnknown_SubNew_SubSubAlsoNew ()
    {
      ClassWithAllDataTypes objectCreatedInSub;
      ClassWithAllDataTypes objectCreatedInSubSub;

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
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
        obj = GetInvalid();
        Assert.IsTrue (obj.IsInvalid);
        ClientTransactionScope.CurrentTransaction.Commit();
      }
      Assert.IsNull (ClientTransactionMock.DataManager.DataContainerMap[obj.ID]);
    }

    [Test]
    public void CommitRootChangedSubMakesUnchanged ()
    {
      var order = Order.GetObject (DomainObjectIDs.Order1);
      ++order.OrderNumber;
      Assert.That (order.State, Is.EqualTo (StateType.Changed));

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        --order.OrderNumber;
        ClientTransaction.Current.Commit ();
      }

      Assert.That (order.State, Is.EqualTo (StateType.Unchanged));
    }

    [Test]
    public void CommitObjectsMarkedAsChanged ()
    {
      var instanceNewInParent = ClassWithAllDataTypes.NewObject ();
      var instanceChangedInParent = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      instanceChangedInParent.Int32Property++;
      var instanceUnchangedInParent = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        instanceNewInParent.MarkAsChanged ();
        instanceChangedInParent.MarkAsChanged ();
        instanceUnchangedInParent.MarkAsChanged ();
        ClientTransaction.Current.Commit ();
      }

      Assert.That (instanceNewInParent.State, Is.EqualTo (StateType.New));
      Assert.That (instanceChangedInParent.State, Is.EqualTo (StateType.Changed));
      Assert.That (instanceUnchangedInParent.State, Is.EqualTo (StateType.Changed));

      Assert.That (instanceNewInParent.InternalDataContainer.HasBeenMarkedChanged, Is.False);
      Assert.That (instanceChangedInParent.InternalDataContainer.HasBeenMarkedChanged, Is.True);
      Assert.That (instanceUnchangedInParent.InternalDataContainer.HasBeenMarkedChanged, Is.True);
    }

    [Test]
    public void CommitObjectsNotMarkedAsChanged ()
    {
      var instanceChangedInSub = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      var instanceUnchangedInSub = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes2);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        instanceChangedInSub.Int32Property++;
        ClientTransaction.Current.Commit ();
      }

      Assert.That (instanceChangedInSub.State, Is.EqualTo (StateType.Changed));
      Assert.That (instanceUnchangedInSub.State, Is.EqualTo (StateType.Unchanged));

      Assert.That (instanceChangedInSub.InternalDataContainer.HasBeenMarkedChanged, Is.False);
      Assert.That (instanceUnchangedInSub.InternalDataContainer.HasBeenMarkedChanged, Is.False);
    }
  }
}
