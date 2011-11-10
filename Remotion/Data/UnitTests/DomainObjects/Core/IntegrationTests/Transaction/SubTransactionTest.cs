// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Assert.That (subTransaction, Is.Not.Null);
      Assert.That (ClientTransactionTestHelper.GetPersistenceStrategy (subTransaction), Is.TypeOf (typeof (SubPersistenceStrategy)));
    }

    [Test]
    public void CreateSubTransaction_OfSubTransaction ()
    {
      ClientTransaction subTransaction1 = ClientTransactionMock.CreateSubTransaction();
      ClientTransaction subTransaction2 = subTransaction1.CreateSubTransaction();
      Assert.That (ClientTransactionTestHelper.GetPersistenceStrategy (subTransaction2), Is.TypeOf (typeof (SubPersistenceStrategy)));
    }

    [Test]
    public void CreateSubTransaction_SetsParentReadonly ()
    {
      Assert.That (ClientTransactionMock.IsReadOnly, Is.False);
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Assert.That (ClientTransactionMock.IsReadOnly, Is.True);
      Assert.That (subTransaction.IsReadOnly, Is.False);

      ClientTransaction subTransaction2 = subTransaction.CreateSubTransaction();
      Assert.That (subTransaction.IsReadOnly, Is.True);
      Assert.That (subTransaction2.IsReadOnly, Is.False);
    }

    [Test]
    public void ParentTransaction ()
    {
      ClientTransaction subTransaction1 = ClientTransactionMock.CreateSubTransaction ();
      Assert.That (subTransaction1.ParentTransaction, Is.SameAs (ClientTransactionMock));

      ClientTransaction subTransaction2 = subTransaction1.CreateSubTransaction ();
      Assert.That (subTransaction2.ParentTransaction, Is.SameAs (subTransaction1));
    }

    [Test]
    public void ActiveSubTansaction ()
    {
      ClientTransaction subTransaction1 = ClientTransactionMock.CreateSubTransaction ();
      Assert.That (ClientTransactionMock.SubTransaction, Is.SameAs (subTransaction1));

      ClientTransaction subTransaction2 = subTransaction1.CreateSubTransaction ();
      Assert.That (subTransaction1.SubTransaction, Is.SameAs (subTransaction2));
      Assert.That (subTransaction2.SubTransaction, Is.Null);

      subTransaction2.Discard();

      Assert.That (subTransaction1.SubTransaction, Is.Null);
      Assert.That (ClientTransactionMock.SubTransaction, Is.SameAs (subTransaction1));

      subTransaction1.Discard();
      Assert.That (ClientTransactionMock.SubTransaction, Is.Null);
    }

    [Test]
    public void RootTransaction ()
    {
      ClientTransaction subTransaction1 = ClientTransactionMock.CreateSubTransaction ();
      ClientTransaction subTransaction2 = subTransaction1.CreateSubTransaction ();

      Assert.That (ClientTransactionMock.RootTransaction, Is.SameAs (ClientTransactionMock));
      Assert.That (subTransaction1.RootTransaction, Is.SameAs (ClientTransactionMock));
      Assert.That (subTransaction2.RootTransaction, Is.SameAs (ClientTransactionMock));
    }

    [Test]
    public void LeafTransaction ()
    {
      ClientTransaction subTransaction1 = ClientTransactionMock.CreateSubTransaction ();
      ClientTransaction subTransaction2 = subTransaction1.CreateSubTransaction ();

      Assert.That (ClientTransactionMock.LeafTransaction, Is.SameAs (subTransaction2));
      Assert.That (subTransaction1.LeafTransaction, Is.SameAs (subTransaction2));
      Assert.That (subTransaction2.LeafTransaction, Is.SameAs (subTransaction2));

      subTransaction2.Discard();

      Assert.That (ClientTransactionMock.LeafTransaction, Is.SameAs (subTransaction1));
      Assert.That (subTransaction1.LeafTransaction, Is.SameAs (subTransaction1));

      subTransaction1.Discard ();

      Assert.That (ClientTransactionMock.LeafTransaction, Is.SameAs (ClientTransactionMock));
    }

    [Test]
    public void CreateEmptyTransactionOfSameType_ForSubTransaction ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      subTransaction.Discard();
      ClientTransaction newSubTransaction = subTransaction.CreateEmptyTransactionOfSameType();
      Assert.That (subTransaction.ParentTransaction, Is.SameAs (ClientTransactionMock));
      Assert.That (subTransaction.RootTransaction, Is.SameAs (ClientTransactionMock));
      Assert.That (newSubTransaction, Is.Not.SameAs (subTransaction));
      Assert.That (newSubTransaction.GetType(), Is.EqualTo (subTransaction.GetType()));
    }

    [Test]
    public void CreateEmptyTransactionOfSameType_ForRootTransaction ()
    {
      var rootTransaction = ClientTransaction.CreateRootTransaction ();
      ClientTransaction newRootTransaction = rootTransaction.CreateEmptyTransactionOfSameType ();
      ClientTransaction subTransaction = rootTransaction.CreateSubTransaction ();
      Assert.That (subTransaction.ParentTransaction, Is.SameAs (rootTransaction));
      Assert.That (subTransaction.RootTransaction, Is.SameAs (rootTransaction));
      Assert.That (newRootTransaction, Is.Not.SameAs (rootTransaction));
      Assert.That (newRootTransaction.GetType(), Is.EqualTo (rootTransaction.GetType()));
      Assert.That (
          ClientTransactionTestHelper.GetPersistenceStrategy (newRootTransaction).GetType(),
          Is.EqualTo (
              ClientTransactionTestHelper.GetPersistenceStrategy (rootTransaction).GetType()));
    }

    [Test]
    public void EnterDiscardingScopeEnablesDiscardBehavior ()
    {
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (ClientTransactionScope.ActiveScope.AutoRollbackBehavior, Is.EqualTo (AutoRollbackBehavior.Discard));
      }
    }

    [Test]
    public void SubTransactionHasDifferentExtensions ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Assert.That (subTransaction.Extensions, Is.Not.SameAs (ClientTransactionMock.Extensions));

      var extensionStub1 = MockRepository.GenerateStub<IClientTransactionExtension>();
      extensionStub1.Stub (stub => stub.Key).Return ("E1");
      var extensionStub2 = MockRepository.GenerateStub<IClientTransactionExtension>();
      extensionStub2.Stub (stub => stub.Key).Return ("E2");

      ClientTransactionMock.Extensions.Add (extensionStub1);
      Assert.That (ClientTransactionMock.Extensions, Has.Member (extensionStub1));
      Assert.That (subTransaction.Extensions, Has.No.Member (extensionStub1));

      subTransaction.Extensions.Add (extensionStub2);
      Assert.That (subTransaction.Extensions, Has.Member (extensionStub2));
      Assert.That (ClientTransactionMock.Extensions, Has.No.Member (extensionStub2));
    }

    [Test]
    public void SubTransactionHasSameApplicationData ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Assert.That (subTransaction.ApplicationData, Is.SameAs (ClientTransactionMock.ApplicationData));
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionReadOnlyException), ExpectedMessage = 
      "The operation cannot be executed because the ClientTransaction is read-only. Offending transaction modification: SubTransactionCreating."
        )]
    public void NoTwoSubTransactionsAtSameTime ()
    {
      ClientTransactionMock.CreateSubTransaction();
      ClientTransactionMock.CreateSubTransaction();
    }

   [Test]
    public void EnlistedObjects_SharedWithParentTransaction ()
    {
      var subTx = ClientTransactionMock.CreateSubTransaction ();

      var order = DomainObjectMother.CreateObjectInOtherTransaction<Order> ();
      Assert.That (subTx.IsEnlisted (order), Is.False);
      Assert.That (ClientTransactionMock.IsEnlisted (order), Is.False);

      subTx.EnlistDomainObject (order);
      Assert.That (subTx.IsEnlisted (order), Is.True);
      Assert.That (ClientTransactionMock.IsEnlisted (order), Is.True);
    }



    [Test]
    public void SubTransactionCreatedEvent ()
    {
      ClientTransaction subTransactionFromEvent = null;

      ClientTransactionMock.SubTransactionCreated += delegate (object sender, SubTransactionCreatedEventArgs args)
      {
        Assert.That (sender, Is.SameAs (ClientTransactionMock));
        Assert.That (args.SubTransaction, Is.Not.Null);
        subTransactionFromEvent = args.SubTransaction;
      };

      Assert.That (subTransactionFromEvent, Is.Null);
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      Assert.That (subTransactionFromEvent, Is.Not.Null);
      Assert.That (subTransactionFromEvent, Is.SameAs (subTransaction));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "The ClientTransactionMock cannot be made writeable twice. A common reason for this error is that a subtransaction is accessed while its "
        + "parent transaction is engaged in a load operation. During such an operation, the subtransaction cannot be used.")]
    public void Throws_WhenUsedWhileParentIsWriteable ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        Type unlockerType = typeof (ClientTransaction).Assembly.GetType ("Remotion.Data.DomainObjects.Infrastructure.TransactionUnlocker");
        object unlocker =
            Activator.CreateInstance (
                unlockerType, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { ClientTransactionMock }, null);
        using ((IDisposable) unlocker)
        {
          Order.GetObject (DomainObjectIDs.Order1);
        }
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = 
        "The ClientTransactionMock cannot be made writeable twice. A common reason for this error is that a subtransaction is accessed while its " 
        + "parent transaction is engaged in a load operation. During such an operation, the subtransaction cannot be used.")]
    public void Throws_WhenUsedWhileParentIsWriteable_IntegrationTest ()
    {
      ClientTransaction subTransaction = ClientTransactionMock.CreateSubTransaction();
      using (subTransaction.EnterDiscardingScope())
      {
        ClientTransactionMock.Loaded += delegate { subTransaction.GetObjects<Order> (DomainObjectIDs.Order1); };
        Order.GetObject (DomainObjectIDs.Order2);
      }
    }
  }
}
