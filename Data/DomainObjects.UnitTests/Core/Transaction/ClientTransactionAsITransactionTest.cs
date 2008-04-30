using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Transaction
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