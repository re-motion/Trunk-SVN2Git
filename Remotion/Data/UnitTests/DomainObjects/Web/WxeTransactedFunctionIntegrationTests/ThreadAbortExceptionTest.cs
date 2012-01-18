using System.Threading;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.Web.WxeTransactedFunctionIntegrationTests.WxeFunctions;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Web.WxeTransactedFunctionIntegrationTests
{
  [TestFixture]
  public class ThreadAbortExceptionTest : WxeTransactedFunctionIntegrationTestBase
  {
    [Test]
    public void ThreadAbortException ()
    {
      var function = new ThreadAbortTestTransactedFunction ();
      try
      {
        function.Execute (Context);
        Assert.Fail ("Expected ThreadAbortException");
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort ();
      }

      Assert.That (function.FirstStepExecuted, Is.True);
      Assert.That (function.SecondStepExecuted, Is.False);
      Assert.That (function.ThreadAborted, Is.True);

      function.Execute (Context);

      Assert.That (function.FirstStepExecuted, Is.True);
      Assert.That (function.SecondStepExecuted, Is.True);
    }

    [Test]
    public void ThreadAbortExceptionInNestedFunction ()
    {
      var nestedFunction = new ThreadAbortTestTransactedFunction ();
      ClientTransactionScope originalScope = ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ();
      var parentFunction =
          new CreateRootWithChildTestTransactedFunction (ClientTransactionScope.CurrentTransaction, nestedFunction);

      try
      {
        parentFunction.Execute (Context);
        Assert.Fail ("Expected ThreadAbortException");
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort ();
      }

      Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (originalScope));

      Assert.That (nestedFunction.FirstStepExecuted, Is.True);
      Assert.That (nestedFunction.SecondStepExecuted, Is.False);
      Assert.That (nestedFunction.ThreadAborted, Is.True);

      parentFunction.Execute (Context);

      Assert.That (nestedFunction.FirstStepExecuted, Is.True);
      Assert.That (nestedFunction.SecondStepExecuted, Is.True);

      Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (originalScope));
      originalScope.Leave ();
    }

    [Test]
    public void ThreadAbortExceptionInNestedFunctionWithThreadMigration ()
    {
      var nestedFunction = new ThreadAbortTestTransactedFunction ();
      var originalScope = ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ();
      var parentFunction =
          new CreateRootWithChildTestTransactedFunction (ClientTransactionScope.CurrentTransaction, nestedFunction);

      try
      {
        parentFunction.Execute (Context);
        Assert.Fail ("Expected ThreadAbortException");
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort ();
      }

      Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (originalScope));

      ThreadRunner.Run (
          delegate
          {
            Assert.That (ClientTransactionScope.ActiveScope, Is.Null, "ActiveScope is not null before execute.");
            Assert.That (nestedFunction.FirstStepExecuted, Is.True);
            Assert.That (nestedFunction.SecondStepExecuted, Is.False);
            Assert.That (nestedFunction.ThreadAborted, Is.True);

            parentFunction.Execute (Context);

            Assert.That (nestedFunction.FirstStepExecuted, Is.True);
            Assert.That (nestedFunction.SecondStepExecuted, Is.True);
            Assert.That (ClientTransactionScope.ActiveScope, Is.Null, "ActiveScope is not null after execute.");
            //TODO: Before there was a transaction, now there isn't                           
            //Assert.That ( ClientTransactionScope.CurrentTransaction, Is.SameAs (originalScope.ScopedTransaction)); // but same transaction as on old thread
          });

      originalScope.Leave ();
    }
  }
}