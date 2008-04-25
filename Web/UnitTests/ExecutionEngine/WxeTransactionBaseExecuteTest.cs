using System;
using System.Collections.Specialized;
using System.Web;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UnitTests.AspNetFramework;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class WxeTransactionBaseExecuteTest
  {
    private HttpContext _currentHttpContext;
    private WxeContext _context;

    [SetUp]
    public void SetUp ()
    {
      _currentHttpContext = HttpContextHelper.CreateHttpContext ("GET", "Other.wxe", null);
      _currentHttpContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
      NameValueCollection queryString = new NameValueCollection ();
      queryString.Add (WxeHandler.Parameters.ReturnUrl, "/Root.wxe");
      HttpContextHelper.SetQueryString (_currentHttpContext, queryString);
      HttpContextHelper.SetCurrent (_currentHttpContext);

      _context = new WxeContextMock (_currentHttpContext);
    }

    private void PerformExecute (bool autoCommit, params Proc<WxeContext>[] stepDelegates)
    {
      MockRepository mockRepository = new MockRepository ();

      WxeStepList steps = new WxeStepList ();
      for (int i = 0; i < stepDelegates.Length; i++)
      {
        WxeStep step = mockRepository.CreateMock<WxeStep>();
        steps.Add (step);
      }

      WxeTransactionMock transaction = new WxeTransactionMock (steps, autoCommit, false);

      TestTransaction originalTransaction = TestTransaction.Current;

      // expectations
      for (int i = 0; i < stepDelegates.Length; i++)
      {
        steps[i].Execute (_context);

        Proc<WxeContext> stepDelegate = stepDelegates[i];
        LastCall.Do ((Proc<WxeContext>) delegate {
          Assert.AreNotSame (originalTransaction, TestTransaction.Current, "WxeTransactionBase must set a new current transaction");
          Assert.AreEqual (1, transaction.PreviousTransactions.Count);
         
          stepDelegate (_context);
        });
      }

      mockRepository.ReplayAll ();

      try
      {
        transaction.Execute (_context);
      }
      finally
      {
        mockRepository.VerifyAll();

        Assert.AreEqual (0, transaction.PreviousTransactions.Count, "WxeTransactionBase must restore original transaction.");
        Assert.AreSame (originalTransaction, TestTransaction.Current);
      }
    }

    [Test]
    public void SimpleExecute ()
    {
      PerformExecute (false, delegate { }, delegate { }); // empty steps
    }

    [Test]
    public void SetAndResetTransaction ()
    {
      TestTransaction.Current = new TestTransaction ();

      TestTransaction executeTransaction = null;
      PerformExecute (false, delegate { executeTransaction = TestTransaction.Current; });
      Assert.IsFalse (executeTransaction.IsCommitted);
    }

    [Test]
    public void SetAndResetTransactionWithAutoCommit ()
    {
      TestTransaction.Current = new TestTransaction ();

      TestTransaction executeTransaction = null;
      PerformExecute (true, delegate { executeTransaction = TestTransaction.Current; });
      Assert.IsTrue (executeTransaction.IsCommitted);
    }

    [Test]
    public void SetAndResetTransactionWithException ()
    {
      TestTransaction.Current = new TestTransaction ();

      TestTransaction executeTransaction = null;
      try
      {
        PerformExecute (false, delegate
        {
          executeTransaction = TestTransaction.Current;
          throw new ArgumentException ("fifi");
        });
        Assert.Fail ("Expected exception");
      }
      catch (ArgumentException ex)
      {
        Assert.AreEqual ("fifi", ex.Message);
      }
      
      Assert.IsFalse (executeTransaction.IsCommitted);
    }

    [Test]
    public void SetAndResetTransactionAutoCommitWithException ()
    {
      TestTransaction.Current = new TestTransaction ();

      TestTransaction executeTransaction = null;
      try
      {
        PerformExecute (true, delegate
        {
          executeTransaction = TestTransaction.Current;
          throw new ArgumentException ("fifi");
        });
        Assert.Fail ("Expected exception");
      }
      catch (ArgumentException ex)
      {
        Assert.AreEqual ("fifi", ex.Message);
      }

      Assert.IsFalse (executeTransaction.IsCommitted);
    }
  }
}
