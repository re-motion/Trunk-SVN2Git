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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests.Transaction
{
  [TestFixture]
  public class SubTransactionTest : ClientTransactionBaseTest
  {
    [Test]
    public void CreateSubTransaction ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction();
      Assert.That (subTransaction, Is.Not.Null);
      Assert.That (ClientTransactionTestHelper.GetPersistenceStrategy (subTransaction), Is.TypeOf (typeof (SubPersistenceStrategy)));
    }

    [Test]
    public void CreateSubTransaction_OfSubTransaction ()
    {
      ClientTransaction subTransaction1 = TestableClientTransaction.CreateSubTransaction();
      ClientTransaction subTransaction2 = subTransaction1.CreateSubTransaction();
      Assert.That (ClientTransactionTestHelper.GetPersistenceStrategy (subTransaction2), Is.TypeOf (typeof (SubPersistenceStrategy)));
    }

    [Test]
    public void CreateSubTransaction_SetsParentReadonly ()
    {
      Assert.That (TestableClientTransaction.IsReadOnly, Is.False);
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction();
      Assert.That (TestableClientTransaction.IsReadOnly, Is.True);
      Assert.That (subTransaction.IsReadOnly, Is.False);

      ClientTransaction subTransaction2 = subTransaction.CreateSubTransaction();
      Assert.That (subTransaction.IsReadOnly, Is.True);
      Assert.That (subTransaction2.IsReadOnly, Is.False);
    }

    [Test]
    public void ParentTransaction ()
    {
      ClientTransaction subTransaction1 = TestableClientTransaction.CreateSubTransaction ();
      Assert.That (subTransaction1.ParentTransaction, Is.SameAs (TestableClientTransaction));

      ClientTransaction subTransaction2 = subTransaction1.CreateSubTransaction ();
      Assert.That (subTransaction2.ParentTransaction, Is.SameAs (subTransaction1));
    }

    [Test]
    public void ActiveSubTansaction ()
    {
      ClientTransaction subTransaction1 = TestableClientTransaction.CreateSubTransaction ();
      Assert.That (TestableClientTransaction.SubTransaction, Is.SameAs (subTransaction1));

      ClientTransaction subTransaction2 = subTransaction1.CreateSubTransaction ();
      Assert.That (subTransaction1.SubTransaction, Is.SameAs (subTransaction2));
      Assert.That (subTransaction2.SubTransaction, Is.Null);

      subTransaction2.Discard();

      Assert.That (subTransaction1.SubTransaction, Is.Null);
      Assert.That (TestableClientTransaction.SubTransaction, Is.SameAs (subTransaction1));

      subTransaction1.Discard();
      Assert.That (TestableClientTransaction.SubTransaction, Is.Null);
    }

    [Test]
    public void RootTransaction ()
    {
      ClientTransaction subTransaction1 = TestableClientTransaction.CreateSubTransaction ();
      ClientTransaction subTransaction2 = subTransaction1.CreateSubTransaction ();

      Assert.That (TestableClientTransaction.RootTransaction, Is.SameAs (TestableClientTransaction));
      Assert.That (subTransaction1.RootTransaction, Is.SameAs (TestableClientTransaction));
      Assert.That (subTransaction2.RootTransaction, Is.SameAs (TestableClientTransaction));
    }

    [Test]
    public void LeafTransaction ()
    {
      ClientTransaction subTransaction1 = TestableClientTransaction.CreateSubTransaction ();
      ClientTransaction subTransaction2 = subTransaction1.CreateSubTransaction ();

      Assert.That (TestableClientTransaction.LeafTransaction, Is.SameAs (subTransaction2));
      Assert.That (subTransaction1.LeafTransaction, Is.SameAs (subTransaction2));
      Assert.That (subTransaction2.LeafTransaction, Is.SameAs (subTransaction2));

      subTransaction2.Discard();

      Assert.That (TestableClientTransaction.LeafTransaction, Is.SameAs (subTransaction1));
      Assert.That (subTransaction1.LeafTransaction, Is.SameAs (subTransaction1));

      subTransaction1.Discard ();

      Assert.That (TestableClientTransaction.LeafTransaction, Is.SameAs (TestableClientTransaction));
    }

    [Test]
    [Obsolete ("CreateEmptyTransactionOfSameType will be removed in the near future. (1.13.138)", false)]
    public void CreateEmptyTransactionOfSameType ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      subTransaction.Discard();

      var newSubTransaction = subTransaction.CreateEmptyTransactionOfSameType (false);

      Assert.That (newSubTransaction, Is.Not.SameAs (subTransaction));
      Assert.That (newSubTransaction.GetType (), Is.EqualTo (subTransaction.GetType ()));

      Assert.That (newSubTransaction.ParentTransaction, Is.SameAs (TestableClientTransaction));
      Assert.That (TestableClientTransaction.SubTransaction, Is.SameAs (newSubTransaction));
    }

    [Test]
    [Obsolete ("CreateEmptyTransactionOfSameType will be removed in the near future. (1.13.138)", false)]
    public void CreateEmptyTransactionOfSameType_CopyInvalidObjectInformation_False ()
    {
      var objectInvalidInParent = Order.NewObject();
      objectInvalidInParent.Delete();
      Assert.That (objectInvalidInParent.State, Is.EqualTo (StateType.Invalid));

      var objectDeletedInParent = Order.GetObject (DomainObjectIDs.Order1);
      objectDeletedInParent.Delete ();
      Assert.That (objectDeletedInParent.State, Is.EqualTo (StateType.Deleted));
      
      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      var objectInvalidInSub = subTransaction.Execute (() => Order.NewObject());
      subTransaction.Execute (objectInvalidInSub.Delete);

      Assert.That (subTransaction.IsInvalid (objectInvalidInParent.ID), Is.True);
      Assert.That (subTransaction.IsInvalid (objectDeletedInParent.ID), Is.True);
      Assert.That (subTransaction.IsInvalid (objectInvalidInSub.ID), Is.True);
      
      subTransaction.Discard ();

      var newSubTransaction = subTransaction.CreateEmptyTransactionOfSameType (false);

      // The Invalid state is copied from the parent transaction to the new subtransaction, so all invalid objects stay invalid.
      Assert.That (newSubTransaction.IsEnlisted (objectInvalidInParent), Is.True);
      Assert.That (newSubTransaction.IsInvalid (objectInvalidInParent.ID), Is.True);

      Assert.That (newSubTransaction.IsEnlisted (objectDeletedInParent), Is.True);
      Assert.That (newSubTransaction.IsInvalid (objectDeletedInParent.ID), Is.True);

      Assert.That (newSubTransaction.IsEnlisted (objectInvalidInSub), Is.True);
      Assert.That (newSubTransaction.IsInvalid (objectInvalidInSub.ID), Is.True);
    }

    [Test]
    [Obsolete ("CreateEmptyTransactionOfSameType will be removed in the near future. (1.13.138)", false)]
    public void CreateEmptyTransactionOfSameType_CopyInvalidObjectInformation_True ()
    {
      var objectInvalidInParent = Order.NewObject ();
      objectInvalidInParent.Delete ();
      Assert.That (objectInvalidInParent.State, Is.EqualTo (StateType.Invalid));

      var objectDeletedInParent = Order.GetObject (DomainObjectIDs.Order1);
      objectDeletedInParent.Delete ();
      Assert.That (objectDeletedInParent.State, Is.EqualTo (StateType.Deleted));

      var subTransaction = TestableClientTransaction.CreateSubTransaction ();
      var objectInvalidInSub = subTransaction.Execute (() => Order.NewObject ());
      subTransaction.Execute (objectInvalidInSub.Delete);

      Assert.That (subTransaction.IsInvalid (objectInvalidInParent.ID), Is.True);
      Assert.That (subTransaction.IsInvalid (objectDeletedInParent.ID), Is.True);
      Assert.That (subTransaction.IsInvalid (objectInvalidInSub.ID), Is.True);

      subTransaction.Discard ();

      var newSubTransaction = subTransaction.CreateEmptyTransactionOfSameType (true);

      Assert.That (newSubTransaction.IsEnlisted (objectInvalidInParent), Is.True);
      Assert.That (newSubTransaction.IsInvalid (objectInvalidInParent.ID), Is.True);

      Assert.That (newSubTransaction.IsEnlisted (objectDeletedInParent), Is.True);
      Assert.That (newSubTransaction.IsInvalid (objectDeletedInParent.ID), Is.True);

      Assert.That (newSubTransaction.IsEnlisted (objectInvalidInSub), Is.True);
      Assert.That (newSubTransaction.IsInvalid (objectInvalidInSub.ID), Is.True);
    }

    [Test]
    public void EnterDiscardingScopeEnablesDiscardBehavior ()
    {
      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (ClientTransactionScope.ActiveScope.AutoRollbackBehavior, Is.EqualTo (AutoRollbackBehavior.Discard));
      }
    }

    [Test]
    public void SubTransactionHasDifferentExtensions ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction();
      Assert.That (subTransaction.Extensions, Is.Not.SameAs (TestableClientTransaction.Extensions));

      var extensionStub1 = MockRepository.GenerateStub<IClientTransactionExtension>();
      extensionStub1.Stub (stub => stub.Key).Return ("E1");
      var extensionStub2 = MockRepository.GenerateStub<IClientTransactionExtension>();
      extensionStub2.Stub (stub => stub.Key).Return ("E2");

      TestableClientTransaction.Extensions.Add (extensionStub1);
      Assert.That (TestableClientTransaction.Extensions, Has.Member (extensionStub1));
      Assert.That (subTransaction.Extensions, Has.No.Member (extensionStub1));

      subTransaction.Extensions.Add (extensionStub2);
      Assert.That (subTransaction.Extensions, Has.Member (extensionStub2));
      Assert.That (TestableClientTransaction.Extensions, Has.No.Member (extensionStub2));
    }

    [Test]
    public void SubTransactionHasSameApplicationData ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction();
      Assert.That (subTransaction.ApplicationData, Is.SameAs (TestableClientTransaction.ApplicationData));
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = 
      "The operation cannot be executed because the ClientTransaction is read-only. Offending transaction modification: SubTransactionCreating."
        )]
    public void NoTwoSubTransactionsAtSameTime ()
    {
      TestableClientTransaction.CreateSubTransaction();
      TestableClientTransaction.CreateSubTransaction();
    }

   [Test]
    public void EnlistedObjects_SharedWithParentTransaction ()
    {
      var subTx = TestableClientTransaction.CreateSubTransaction ();

      var order = DomainObjectMother.CreateObjectInOtherTransaction<Order> ();
      Assert.That (subTx.IsEnlisted (order), Is.False);
      Assert.That (TestableClientTransaction.IsEnlisted (order), Is.False);

      subTx.EnlistDomainObject (order);
      Assert.That (subTx.IsEnlisted (order), Is.True);
      Assert.That (TestableClientTransaction.IsEnlisted (order), Is.True);
    }



    [Test]
    public void SubTransactionCreatedEvent ()
    {
      ClientTransaction subTransactionFromEvent = null;

      TestableClientTransaction.SubTransactionCreated += delegate (object sender, SubTransactionCreatedEventArgs args)
      {
        Assert.That (sender, Is.SameAs (TestableClientTransaction));
        Assert.That (args.SubTransaction, Is.Not.Null);
        subTransactionFromEvent = args.SubTransaction;
      };

      Assert.That (subTransactionFromEvent, Is.Null);
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction();
      Assert.That (subTransactionFromEvent, Is.Not.Null);
      Assert.That (subTransactionFromEvent, Is.SameAs (subTransaction));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "The TestableClientTransaction cannot be made writeable twice. A common reason for this error is that a subtransaction is accessed while its "
        + "parent transaction is engaged in a load operation. During such an operation, the subtransaction cannot be used.")]
    public void Throws_WhenUsedWhileParentIsWriteable ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        Type unlockerType = typeof (ClientTransaction).Assembly.GetType ("Remotion.Data.DomainObjects.Infrastructure.TransactionUnlocker");
        object unlocker =
            Activator.CreateInstance (
                unlockerType, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { TestableClientTransaction }, null);
        using ((IDisposable) unlocker)
        {
          Order.GetObject (DomainObjectIDs.Order1);
        }
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "The TestableClientTransaction cannot be made writeable twice. A common reason for this error is that a subtransaction is accessed while its " 
        + "parent transaction is engaged in a load operation. During such an operation, the subtransaction cannot be used.")]
    public void Throws_WhenUsedWhileParentIsWriteable_IntegrationTest ()
    {
      ClientTransaction subTransaction = TestableClientTransaction.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        TestableClientTransaction.Loaded += delegate { subTransaction.GetObjects<Order> (DomainObjectIDs.Order1); };
        Order.GetObject (DomainObjectIDs.Order2);
      }
    }
  }
}
