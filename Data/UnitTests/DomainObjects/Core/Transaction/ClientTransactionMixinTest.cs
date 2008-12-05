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
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Transaction
{
  [TestFixture]
  public class ClientTransactionMixinTest
  {
    [Test]
    public void ClientTransactionCanBeMixed ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClientTransaction)).Clear().AddMixins (typeof (InvertingClientTransactionMixin)).EnterScope())
      {
        ClientTransaction mixedTransaction = ClientTransaction.CreateRootTransaction ();
        Assert.IsNotNull (mixedTransaction);
        Assert.IsNotNull (Mixin.Get<InvertingClientTransactionMixin> (mixedTransaction));
      }
    }

    [Test]
    public void SubTransactionsAlsoMixed ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClientTransaction)).Clear().AddMixins (typeof (InvertingClientTransactionMixin)).EnterScope())
      {
        ClientTransaction mixedTransaction = ClientTransaction.CreateRootTransaction ();
        ClientTransaction mixedSubTransaction = mixedTransaction.CreateSubTransaction ();
        Assert.IsNotNull (mixedSubTransaction);
        Assert.IsNotNull (Mixin.Get<InvertingClientTransactionMixin> (mixedSubTransaction));
      }
    }

    [Test]
    public void TransactionMethodsCanBeOverridden ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClientTransaction)).Clear().AddMixins (typeof (InvertingClientTransactionMixin)).EnterScope())
      {
        ClientTransaction invertedTransaction = ClientTransaction.CreateRootTransaction();

        bool committed = false;
        bool rolledBack = false;
        invertedTransaction.Committed += delegate { committed = true; };
        invertedTransaction.RolledBack += delegate { rolledBack = true; };

        Assert.IsFalse (rolledBack);
        Assert.IsFalse (committed);

        invertedTransaction.Commit();

        Assert.IsTrue (rolledBack);
        Assert.IsFalse (committed);

        rolledBack = false;
        invertedTransaction.Rollback();

        Assert.IsFalse (rolledBack);
        Assert.IsTrue (committed);
      }
    }

    [Test]
    public void MixinCanAddInterface ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClientTransaction)).Clear().AddMixins (typeof (ClientTransactionWithIDMixin)).EnterScope())
      {
        IClientTransactionWithID transactionWithID = (IClientTransactionWithID) ClientTransaction.CreateRootTransaction ();
        Assert.AreEqual (transactionWithID.ID.ToString (), transactionWithID.ToString ());
        IClientTransactionWithID subTransactionWithID = (IClientTransactionWithID) transactionWithID.AsClientTransaction.CreateSubTransaction ();
        Assert.AreNotEqual (transactionWithID.ID, subTransactionWithID.ID);
        Assert.AreEqual (transactionWithID.ID, ((IClientTransactionWithID) subTransactionWithID.AsClientTransaction.ParentTransaction).ID);
      }
    }
  }
}
