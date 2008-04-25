using System;
using NUnit.Framework;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class ClientTransactionMixinTest
  {
    [Test]
    public void ClientTransactionCanBeMixed ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClientTransaction)).Clear().AddMixins (typeof (InvertingClientTransactionMixin)).EnterScope())
      {
        ClientTransaction mixedTransaction = ClientTransaction.NewRootTransaction ();
        Assert.IsNotNull (mixedTransaction);
        Assert.IsNotNull (Mixin.Get<InvertingClientTransactionMixin> (mixedTransaction));
      }
    }

    [Test]
    public void SubTransactionsAlsoMixed ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClientTransaction)).Clear().AddMixins (typeof (InvertingClientTransactionMixin)).EnterScope())
      {
        ClientTransaction mixedTransaction = ClientTransaction.NewRootTransaction ();
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
        ClientTransaction invertedTransaction = ClientTransaction.NewRootTransaction();

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
        IClientTransactionWithID transactionWithID = (IClientTransactionWithID) ClientTransaction.NewRootTransaction ();
        Assert.AreEqual (transactionWithID.ID.ToString (), transactionWithID.ToString ());
        IClientTransactionWithID subTransactionWithID = (IClientTransactionWithID) transactionWithID.AsClientTransaction.CreateSubTransaction ();
        Assert.AreNotEqual (transactionWithID.ID, subTransactionWithID.ID);
        Assert.AreEqual (transactionWithID.ID, ((IClientTransactionWithID) subTransactionWithID.AsClientTransaction.ParentTransaction).ID);
      }
    }
  }
}