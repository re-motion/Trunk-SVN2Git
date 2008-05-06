using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Authentication;
using NUnit.Framework;
using Remotion.Data;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{

  [TestFixture]
  public class WxeTransactionBaseTest : WxeTest
  {
    private WxeTransactionMock _wxeTransaction;

    private WxeTransactionMock _rootWxeTransaction;

    private WxeTransactionMock _parentWxeTransaction;
    private TestTransaction _parentTransaction;

    private WxeTransactionMock _childWxeTransaction;

    private string _events;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();

      _wxeTransaction = new WxeTransactionMock (null, false, false);

      _rootWxeTransaction = new WxeTransactionMock (null, false, true);
      _rootWxeTransaction.Transaction = new TestTransaction ();
      _rootWxeTransaction.PublicCheckAndSetCurrentTransaction (_rootWxeTransaction.Transaction);
      ((TestTransaction) _rootWxeTransaction.Transaction).CanCreateChild = true;
      _rootWxeTransaction.TransactionCommitting += delegate { _events += " committing"; };
      _rootWxeTransaction.TransactionCommitted += delegate { _events += " committed"; };
      _rootWxeTransaction.TransactionRollingBack += delegate { _events += " rollingBack"; };
      _rootWxeTransaction.TransactionRolledBack += delegate { _events += " rolledBack"; };

      _parentWxeTransaction = new WxeTransactionMock (null, true, false); // this simulates Execute
      _parentTransaction = new TestTransaction ();
      _parentTransaction.CanCreateChild = true;
      _parentWxeTransaction.Transaction = _parentTransaction;
      _parentWxeTransaction.PublicCheckAndSetCurrentTransaction (_parentTransaction);

      _childWxeTransaction = new WxeTransactionMock (null, true, false);
      _parentWxeTransaction.Add (_childWxeTransaction);

      _events = string.Empty;
    }

    [Test]
    public void TestCtor1 ()
    {
      WxeStepList steps = new WxeStepList ();
      WxeStep step = new TestStep ();
      steps.Add (step);
      bool autoCommit = true;
      bool forceRoot = false;
      WxeTransactionMock wxeTransaction = new WxeTransactionMock (steps, autoCommit, forceRoot);

      Assert.AreEqual (1, wxeTransaction.Steps.Count);
      Assert.AreSame (step, wxeTransaction.Steps[0]);
      Assert.AreEqual (autoCommit, wxeTransaction.AutoCommit);
      Assert.AreEqual (forceRoot, wxeTransaction.ForceRoot);
    }

    [Test]
    public void TestCtor2 ()
    {
      WxeStepList steps = null;
      bool autoCommit = false;
      bool forceRoot = true;
      WxeTransactionMock wxeTransaction = new WxeTransactionMock (steps, autoCommit, forceRoot);

      Assert.AreEqual (0, wxeTransaction.Steps.Count);
      Assert.AreEqual (autoCommit, wxeTransaction.AutoCommit);
      Assert.AreEqual (forceRoot, wxeTransaction.ForceRoot);
    }

    [Test]
    public void GetParentWxeTransaction ()
    {
      Assert.AreSame (_parentWxeTransaction, _childWxeTransaction.GetParentTransaction ());
    }

    [Test]
    public void GetParentTransactedFunction ()
    {
      TestFunctionWithSpecificTransaction function = new TestFunctionWithSpecificTransaction (new TestTransaction ());
      function.Execute (CurrentWxeContext);
      TestWxeTransaction transaction = function.WxeTransaction;
      Assert.IsNotNull (transaction);
      Assert.AreSame (function, transaction.GetParentTransactedFunction());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This object has not yet been encapsulated by a transacted function.")]
    public void GetParentTransactedFunctionThrowsIfNoFunction ()
    {
      TestWxeTransaction transaction = new TestWxeTransaction ();
      transaction.GetParentTransactedFunction ();
    }

    [Test]
    public void GetTransaction ()
    {
      Assert.AreSame (_parentTransaction, _parentWxeTransaction.Transaction);
    }

    [Test]
    public void CreateChildTransaction ()
    {
      TestTransaction parentTransaction = new TestTransaction ();
      parentTransaction.CanCreateChild = true;
      ITransaction childTransaction = _wxeTransaction.CreateChildTransaction (parentTransaction);
      Assert.IsNotNull (childTransaction);
      Assert.IsTrue (childTransaction.IsChild);
      Assert.AreSame (parentTransaction, childTransaction.Parent);
    }

    [Test]
    public void CreateChildTransactionAsRootTransaction ()
    {
      TestTransaction parentTransaction = new TestTransaction ();
      parentTransaction.CanCreateChild = false;
      ITransaction childTransaction = _wxeTransaction.CreateChildTransaction (parentTransaction);
      Assert.IsNotNull (childTransaction);
      Assert.IsFalse (childTransaction.IsChild);
    }

    [Test]
    public void CreateTransactionAsRootTransactionBecauseOfNoParentWxeTransaction ()
    {
      ITransaction transaction = _wxeTransaction.CreateTransaction ();
      Assert.IsTrue (_wxeTransaction.HasCreatedRootTransaction);
      Assert.IsNotNull (transaction);
    }

    [Test]
    public void CreateTransactionAsRootTransactionBecauseOfNoParentTransaction ()
    {
      _parentWxeTransaction.Transaction = null;
      ITransaction transaction = _childWxeTransaction.CreateTransaction ();
      Assert.IsTrue (_childWxeTransaction.HasCreatedRootTransaction);
      Assert.IsNotNull (transaction);
    }

    [Test]
    public void CreateTransactionAsRootTransactionBecauseOfForceRoot ()
    {
      _childWxeTransaction.ForceRoot = true;
      ITransaction transaction = _childWxeTransaction.CreateTransaction ();
      Assert.IsTrue (_childWxeTransaction.HasCreatedRootTransaction);
      Assert.IsNotNull (transaction);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void CreateTransactionAsChildTransactionWithInvalidCurrentTransaction ()
    {
      TestTransaction.Current = new TestTransaction ();
      ITransaction transaction = _childWxeTransaction.CreateTransaction ();
    }

    [Test]
    public void CreateTransactionAsChildTransactionWithNoCurrentTransaction ()
    {
      TestTransaction.Current = null;
      ITransaction transaction = _childWxeTransaction.CreateTransaction ();
      Assert.IsFalse (_childWxeTransaction.HasCreatedRootTransaction);
      Assert.IsNotNull (transaction);
    }

    [Test]
    public void CreateTransactionAsChildTransactionWithCurrentTransaction ()
    {
      TestTransaction.Current = (TestTransaction) _parentWxeTransaction.Transaction;
      ITransaction transaction = _childWxeTransaction.CreateTransaction ();
      Assert.IsFalse (_childWxeTransaction.HasCreatedRootTransaction);
      Assert.IsNotNull (transaction);
    }

    [Test]
    public void TestOnTransactionCreated ()
    {
      _wxeTransaction.TransactionCreating += delegate { _events += " creating"; };
      _wxeTransaction.TransactionCreated += delegate { _events += " created"; };
      TestTransaction transaction = _wxeTransaction.CreateTransaction ();

      Assert.AreEqual (" creating created", _events);
    }

    [Test]
    public void TestOnTransactionCommitting ()
    {
      _rootWxeTransaction.OnTransactionCommitting ();
      Assert.AreEqual (" committing", _events);
    }

    [Test]
    public void TestOnTransactionCommitted ()
    {
      _rootWxeTransaction.OnTransactionCommitted ();
      Assert.AreEqual (" committed", _events);
    }

    [Test]
    public void TestOnTransactionRollingBack ()
    {
      _rootWxeTransaction.OnTransactionRollingBack ();
      Assert.AreEqual (" rollingBack", _events);
    }

    [Test]
    public void TestOnTransactionRolledBack ()
    {
      _rootWxeTransaction.OnTransactionRolledBack ();
      Assert.AreEqual (" rolledBack", _events);
    }

    [Test]
    public void RollBackTransaction ()
    {
      TestTransaction rootTransaction = (TestTransaction) _rootWxeTransaction.Transaction;
      TestTransaction.Current = rootTransaction;
      rootTransaction.RolledBack += delegate { _events += " txRolledBack"; };

      _rootWxeTransaction.RollbackTransaction ();
      Assert.AreEqual (" rollingBack txRolledBack rolledBack", _events);
    }

    [Test]
    public void CommitTransaction ()
    {
      TestTransaction rootTransaction = (TestTransaction) _rootWxeTransaction.Transaction;
      TestTransaction.Current = rootTransaction;
      rootTransaction.Committed += delegate { _events += " txCommitted"; };

      _rootWxeTransaction.CommitTransaction ();
      Assert.AreEqual (" committing txCommitted committed", _events);
    }

    [Test]
    public void CommitAndReleaseTransaction ()
    {
      TestTransaction currentTransaction = new TestTransaction ();
      TestTransaction.Current = currentTransaction;

      TestTransaction transaction = (TestTransaction) _rootWxeTransaction.Transaction;
      _rootWxeTransaction.CommitAndReleaseTransaction ();

      Assert.IsTrue (transaction.IsCommitted);
      Assert.IsTrue (transaction.IsReleased);
      Assert.IsNull (_rootWxeTransaction.Transaction);
      Assert.AreSame (currentTransaction, TestTransaction.Current);
    }

    [Test]
    public void CommitAndReleaseTransactionCallsReleaseEvenWhenCommitThrows ()
    {
      TestTransaction currentTransaction = new TestTransaction ();
      TestTransaction.Current = currentTransaction;

      TestTransaction transaction = (TestTransaction) _rootWxeTransaction.Transaction;
      transaction.ThrowOnCommit = true;
      try
      {
        _rootWxeTransaction.CommitAndReleaseTransaction ();
        Assert.Fail ("Expected exception");
      }
      catch (CommitException)
      {
        // ok
      }

      Assert.IsFalse (transaction.IsCommitted);
      Assert.IsTrue (transaction.IsReleased);
      Assert.IsNull (_rootWxeTransaction.Transaction);
      Assert.AreSame (currentTransaction, TestTransaction.Current);
    }

    [Test]
    public void RollbackAndReleaseTransaction ()
    {
      TestTransaction currentTransaction = new TestTransaction ();
      TestTransaction.Current = currentTransaction;

      TestTransaction transaction = (TestTransaction) _rootWxeTransaction.Transaction;
      _rootWxeTransaction.RollbackAndReleaseTransaction ();

      Assert.IsTrue (transaction.IsRolledBack);
      Assert.IsTrue (transaction.IsReleased);
      Assert.IsNull (_rootWxeTransaction.Transaction);
      Assert.AreSame (currentTransaction, TestTransaction.Current);
    }

    [Test]
    public void RollbackAndReleaseTransactionCallsReleaseEvenWhenRollbackThrows ()
    {
      TestTransaction currentTransaction = new TestTransaction ();
      TestTransaction.Current = currentTransaction;

      TestTransaction transaction = (TestTransaction) _rootWxeTransaction.Transaction;
      transaction.ThrowOnRollback = true;
      try
      {
        _rootWxeTransaction.RollbackAndReleaseTransaction ();
        Assert.Fail ("Expected exception");
      }
      catch (RollbackException)
      {
        // ok
      }

      Assert.IsFalse (transaction.IsRolledBack);
      Assert.IsTrue (transaction.IsReleased);
      Assert.IsNull (_rootWxeTransaction.Transaction);
      Assert.AreSame (currentTransaction, TestTransaction.Current);
    }

    [Test]
    public void RestorePreviousCurrentTransaction ()
    {
      _wxeTransaction.StartExecution ();

      TestTransaction previousCurrentTransaction = new TestTransaction ();
      TestTransaction.Current = previousCurrentTransaction;
      _wxeTransaction.PublicSetPreviousTransaction (previousCurrentTransaction);
      _wxeTransaction.IsPreviousCurrentTransactionRestored = false;
      _wxeTransaction.PublicCheckAndSetCurrentTransaction (null);

      Assert.AreNotSame (previousCurrentTransaction, TestTransaction.Current);
      
      _wxeTransaction.PublicCheckAndRestorePreviousCurrentTransaction ();

      Assert.AreSame (previousCurrentTransaction, TestTransaction.Current);
      Assert.IsNull (_wxeTransaction.Transaction);
    }

    [Test]
    public void RestorePreviousCurrentTransactionOnlyOnce ()
    {
      _wxeTransaction.StartExecution ();

      TestTransaction.Current = new TestTransaction ();
      _wxeTransaction.PublicSetPreviousTransaction (TestTransaction.Current);
      _wxeTransaction.PublicCheckAndSetCurrentTransaction (null);
      _wxeTransaction.PublicCheckAndRestorePreviousCurrentTransaction ();

      TestTransaction currentTransaction = new TestTransaction ();
      TestTransaction.Current = currentTransaction;
      _wxeTransaction.PublicCheckAndRestorePreviousCurrentTransaction (); // should not do anything

      Assert.AreSame (currentTransaction, TestTransaction.Current);
    }

    [Test]
    public void TestSerialization ()
    {
      _parentWxeTransaction.AutoCommit = true;
      _parentWxeTransaction.ForceRoot = true;
      _parentWxeTransaction.IsPreviousCurrentTransactionRestored = true;
      _parentWxeTransaction.TransactionCreating += delegate { };
      _parentWxeTransaction.TransactionCreated += delegate { };
      _parentWxeTransaction.TransactionCommitting += delegate { };
      _parentWxeTransaction.TransactionCommitted += delegate { };
      _parentWxeTransaction.TransactionRollingBack += delegate { };
      _parentWxeTransaction.TransactionRolledBack += delegate { };

      _childWxeTransaction.Transaction = (TestTransaction) _parentWxeTransaction.Transaction.CreateChild ();
      _childWxeTransaction.PreviousTransactions.Push (_parentWxeTransaction.Transaction);

      WxeTransactionMock parentWxeTransaction;
      using (Stream stream = new MemoryStream ())
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        formatter.Serialize (stream, _parentWxeTransaction);
        stream.Position = 0;
        parentWxeTransaction = (WxeTransactionMock) formatter.Deserialize (stream);
      }
      WxeTransactionMock childWxeTransaction = (WxeTransactionMock) parentWxeTransaction.Steps[0];

      Assert.IsNotNull (parentWxeTransaction);
      Assert.IsNotNull (childWxeTransaction);
      Assert.IsNotNull (parentWxeTransaction.Transaction);
      Assert.IsNotNull (childWxeTransaction.Transaction);
      Assert.AreSame (parentWxeTransaction.Transaction, childWxeTransaction.PreviousTransactions.Peek());
      Assert.AreEqual (1, childWxeTransaction.PreviousTransactions.Count);
      Assert.IsTrue (parentWxeTransaction.AutoCommit);
      Assert.IsTrue (parentWxeTransaction.ForceRoot);
      Assert.IsTrue (parentWxeTransaction.IsPreviousCurrentTransactionRestored);
    }

    [Test]
    [ExpectedException(typeof (InvalidOperationException), ExpectedMessage = "Transaction cannot be reset before its execution has started.")]
    public void ResetThrowsWhenExecutionNotStarted ()
    {
      WxeTransactionMock wxeTransaction = new WxeTransactionMock (null, false, false);
      PrivateInvoke.InvokeNonPublicMethod (wxeTransaction, "Reset");
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Transaction cannot be reset after its execution has finished.")]
    public void ResetThrowsWhenExecutionHasFinished ()
    {
      WxeTransactionMock wxeTransaction = new WxeTransactionMock (null, false, false);
      wxeTransaction.Execute ();
      PrivateInvoke.InvokeNonPublicMethod (wxeTransaction, "Reset");
    }

    [Test]
    public void OriginalExceptionRetainedOnRollbackException ()
    {
      TestTransaction currentTransaction = new TestTransaction ();
      TestTransaction.Current = currentTransaction;

      TestTransaction transaction = (TestTransaction) _rootWxeTransaction.Transaction;
      transaction.ThrowOnRollback = true;

      _rootWxeTransaction.Add (new WxeDelegateStep (delegate { throw new AuthenticationException ("testo."); }));

      try
      {
        _rootWxeTransaction.Execute ();
        Assert.Fail ("Expected exception");
      }
      catch (WxeNonRecoverableTransactionException ex)
      {
        Assert.AreEqual ("An exception of type AuthenticationException caused a non-recoverable RollbackException.\r\nThe original exception message "
            + "was: 'testo.'\r\nThe transaction error message was: 'Exception of type 'Remotion.Web.UnitTests.ExecutionEngine.RollbackException' was "
            + "thrown.'", ex.Message);
        Assert.IsInstanceOfType (typeof (AuthenticationException), ex.InnerException);
        Assert.IsInstanceOfType (typeof (RollbackException), ex.TransactionException);
      }
    }

    [Test]
    public void OriginalExceptionRetainedOnReleaseException ()
    {
      TestTransaction currentTransaction = new TestTransaction ();
      TestTransaction.Current = currentTransaction;

      TestTransaction transaction = (TestTransaction) _rootWxeTransaction.Transaction;
      transaction.ThrowOnRelease = true;

      _rootWxeTransaction.Add (new WxeDelegateStep (delegate { throw new AuthenticationException ("testo."); }));

      try
      {
        _rootWxeTransaction.Execute ();
        Assert.Fail ("Expected exception");
      }
      catch (WxeNonRecoverableTransactionException ex)
      {
        Assert.AreEqual ("An exception of type AuthenticationException caused a non-recoverable ReleaseException.\r\nThe original exception message "
            + "was: 'testo.'\r\nThe transaction error message was: 'Exception of type 'Remotion.Web.UnitTests.ExecutionEngine.ReleaseException' was "
            + "thrown.'", ex.Message);
        Assert.IsInstanceOfType (typeof (AuthenticationException), ex.InnerException);
        Assert.IsInstanceOfType (typeof (ReleaseException), ex.TransactionException);
      }
    }

    [Test]
    public void AbortOnFunctionNotYetExecuted ()
    {
      TestTransaction currentTransaction = new TestTransaction ();
      TestTransaction.Current = currentTransaction;
      _rootWxeTransaction.Abort ();
      Assert.AreSame (currentTransaction, TestTransaction.Current);
    }

    [Test]
    public void ExceptionThrownBySetCurrentTransactionIsWrapped ()
    {
      _rootWxeTransaction.ThrowOnSetCurrent = true;
      try
      {
        _rootWxeTransaction.PublicCheckAndSetCurrentTransaction (new TestTransaction ());
        Assert.Fail ("Expected WxeNonRecoverableTransactionException.");
      }
      catch (WxeNonRecoverableTransactionException ex)
      {
        Assert.IsTrue (ex.InnerException is SetCurrentException);
        Assert.AreEqual ("WxeTransactionMock.SetCurrentTransaction threw an exception. See InnerException property.", ex.Message);
      }
    }

    [Test]
    public void ExceptionThrownByRestorePreviousCurrentTransactionIsWrapped ()
    {
      _rootWxeTransaction.ThrowOnSetPrevious = true;
      _rootWxeTransaction.IsPreviousCurrentTransactionRestored = false;
      try
      {
        _rootWxeTransaction.PublicCheckAndRestorePreviousCurrentTransaction ();
        Assert.Fail ("Expected WxeNonRecoverableTransactionException.");
      }
      catch (WxeNonRecoverableTransactionException ex)
      {
        Assert.IsTrue (ex.InnerException is SetPreviousCurrentException);
        Assert.AreEqual ("WxeTransactionMock.SetPreviousCurrentTransaction threw an exception. See InnerException property.", ex.Message);
      }
    }

    [Test]
    public void ExceptionThrownWithinResetCleansUpTransactionStack ()
    {
      TestTransaction before = new TestTransaction ();
      TestTransaction.Current = before;
      _rootWxeTransaction.ThrowOnGetRootTransaction = true;
      _rootWxeTransaction.Add (new WxeDelegateStep (delegate { _rootWxeTransaction.InternalReset(); }));
      try
      {
        _rootWxeTransaction.Execute (CurrentWxeContext);
        Assert.Fail ("Expected InvalidOperationException");
      }
      catch (InvalidOperationException)
      {
        // expected
      }
      Assert.AreSame (before, TestTransaction.Current);
    }

    [Test]
    public void ExceptionThrownInRollbackAfterExceptionCleansUpTransactionStack ()
    {
      TestTransaction before = new TestTransaction ();
      TestTransaction.Current = before;
      _rootWxeTransaction.ThrowOnGetRootTransaction = true;
      _rootWxeTransaction.Transaction.ThrowOnRollback = true;
      _rootWxeTransaction.Add (new WxeDelegateStep (delegate { throw new Exception (); }));
      try
      {
        _rootWxeTransaction.Execute (CurrentWxeContext);
        Assert.Fail ("Expected WxeNonRecoverableTransactionException");
      }
      catch (WxeNonRecoverableTransactionException)
      {
        // expected
      }
      Assert.AreSame (before, TestTransaction.Current);
    }
  }
}
