/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Web.ExecutionEngine;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure.RootTransactionStrategyTests
{
  [TestFixture]
  public class OnExecutionStop:RootTransactionStrategyTestBase
  {
    [Test]
    public void Test_WithoutAutoCommit ()
    {
      var strategy = CreateRootTransactionStrategy (false);

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        ExecutionContextMock.Expect (mock => mock.ParentTransactionStrategy).Return (null);
        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ());
      }

      MockRepository.ReplayAll ();

      strategy.OnExecutionStop (Context);

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithAutoCommit ()
    {
      var strategy = CreateRootTransactionStrategy (true);

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        TransactionMock.Expect (mock => mock.Commit ());
        ExecutionContextMock.Expect (mock => mock.ParentTransactionStrategy).Return (null);
        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ());
      }

      MockRepository.ReplayAll ();

      strategy.OnExecutionStop (Context);

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithParentTransactionStrategy ()
    {
      var strategy = CreateRootTransactionStrategy (true);
      var expectedObjects = new[] { new object () };

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        TransactionMock.Expect (mock => mock.Commit ());

        ExecutionContextMock.Expect (mock => mock.ParentTransactionStrategy).Return (ParentTransactionStrategyMock);
        ExecutionContextMock.Expect (mock => mock.GetOutParameters ()).Return (expectedObjects);
        ParentTransactionStrategyMock.Expect (mock => mock.RegisterObjects (expectedObjects));

        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ());
      }

      MockRepository.ReplayAll ();

      strategy.OnExecutionStop (Context);

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "OnExecutionStop may not be invoked unless OnExecutionPlay was called first.")]
    public void Test_WithNullScope ()
    {
      var strategy = CreateRootTransactionStrategy (true);
      
      Assert.That (strategy.Scope, Is.Null);
      
      strategy.OnExecutionStop (Context);
    }

    [Test]
    public void Test_InnerListenerThrows ()
    {
      var strategy = CreateRootTransactionStrategy (true);
      var innerException = new ApplicationException ("InnerListener Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context)).Throw (innerException);
        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ());
      }

      MockRepository.ReplayAll ();

      try
      {
        strategy.OnExecutionStop (Context);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_CommitThrows ()
    {
      var strategy = CreateRootTransactionStrategy (true);
      var innerException = new ApplicationException ("Commit Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        TransactionMock.Expect (mock => mock.Commit ()).Throw (innerException);
        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ());
      }

      MockRepository.ReplayAll ();

      try
      {
        strategy.OnExecutionStop (Context);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_GetParentTransactionThrows ()
    {
      var strategy = CreateRootTransactionStrategy (false);
      var innerException = new ApplicationException ("ParentTransactionStrategy Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        ExecutionContextMock.Expect (mock => mock.ParentTransactionStrategy).Throw (innerException);
        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ());
      }

      MockRepository.ReplayAll ();

      try
      {
        strategy.OnExecutionStop (Context);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_GetOutParameterThrows ()
    {
      var strategy = CreateRootTransactionStrategy (false);
      var innerException = new ApplicationException ("GetOutParameters Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));

        ExecutionContextMock.Expect (mock => mock.ParentTransactionStrategy).Return (ParentTransactionStrategyMock);
        ExecutionContextMock.Expect (mock => mock.GetOutParameters()).Throw (innerException);

        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ());
      }

      MockRepository.ReplayAll ();

      try
      {
        strategy.OnExecutionStop (Context);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_RegisterObjectsThrows ()
    {
      var strategy = CreateRootTransactionStrategy (false);
      var innerException = new ApplicationException ("GetOutParameters Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));

        ExecutionContextMock.Expect (mock => mock.ParentTransactionStrategy).Return (ParentTransactionStrategyMock);
        ExecutionContextMock.Expect (mock => mock.GetOutParameters ()).Return (new object[0]);
        ParentTransactionStrategyMock.Expect (mock => mock.RegisterObjects (Arg<IEnumerable>.Is.Anything)).Throw (innerException);

        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ());
      }

      MockRepository.ReplayAll ();

      try
      {
        strategy.OnExecutionStop (Context);
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_LeaveThrows ()
    {
      var strategy = CreateRootTransactionStrategy (false);
      var innerException = new Exception ("Leave Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        ExecutionContextMock.Expect (mock => mock.ParentTransactionStrategy).Return (null);
        ScopeMock.Expect (mock => mock.Leave ()).Throw (innerException);
      }

      MockRepository.ReplayAll ();

      try
      {
        strategy.OnExecutionStop (Context);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Not.Null);
    }

    [Test]
    public void Test_ReleaseThrows ()
    {
      var strategy = CreateRootTransactionStrategy (false);
      var innerException = new Exception ("Release Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        ExecutionContextMock.Expect (mock => mock.ParentTransactionStrategy).Return (null);
        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ()).Throw (innerException);
      }

      MockRepository.ReplayAll ();

      try
      {
        strategy.OnExecutionStop (Context);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_InnerListenerThrows_And_LeaveThrows ()
    {
      var strategy = CreateRootTransactionStrategy (true);
      var innerException = new Exception ("InnerListener Exception");
      var outerException = new Exception ("Leave Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context)).Throw (innerException);
        ScopeMock.Expect (mock => mock.Leave ()).Throw (outerException);
      }

      MockRepository.ReplayAll ();

      try
      {
        strategy.OnExecutionStop (Context);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
        Assert.That (actualException.OuterException, Is.SameAs (outerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Not.Null);
    }

    [Test]
    public void Test_CommitThrows_And_LeaveThrows ()
    {
      var strategy = CreateRootTransactionStrategy (true);
      var innerException = new Exception ("Commit Exception");
      var outerException = new Exception ("Leave Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        TransactionMock.Expect (mock => mock.Commit ()).Throw (innerException);
        ScopeMock.Expect (mock => mock.Leave ()).Throw (outerException);
      }

      MockRepository.ReplayAll ();

      try
      {
        strategy.OnExecutionStop (Context);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
        Assert.That (actualException.OuterException, Is.SameAs (outerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Not.Null);
    }

    [Test]
    public void Test_InnerListenerThrows_And_ReleaseThrows ()
    {
      var strategy = CreateRootTransactionStrategy (true);
      var innerException = new Exception ("InnerListener Exception");
      var outerException = new Exception ("Release Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context)).Throw (innerException);
        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ()).Throw (outerException);
      }

      MockRepository.ReplayAll ();

      try
      {
        strategy.OnExecutionStop (Context);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
        Assert.That (actualException.OuterException, Is.SameAs (outerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_CommitThrows_And_ReleaseThrows ()
    {
      var strategy = CreateRootTransactionStrategy (true);
      var innerException = new Exception ("Commit Exception");
      var outerException = new Exception ("Release Exception");

      InvokeOnExecutionPlay (strategy);
      using (MockRepository.Ordered ())
      {
        ExecutionListenerMock.Expect (mock => mock.OnExecutionStop (Context));
        TransactionMock.Expect (mock => mock.Commit ()).Throw (innerException);
        ScopeMock.Expect (mock => mock.Leave ());
        TransactionMock.Expect (mock => mock.Release ()).Throw (outerException);
      }

      MockRepository.ReplayAll ();

      try
      {
        strategy.OnExecutionStop (Context);
        Assert.Fail ("Expected Exception");
      }
      catch (WxeFatalExecutionException actualException)
      {
        Assert.That (actualException.InnerException, Is.SameAs (innerException));
        Assert.That (actualException.OuterException, Is.SameAs (outerException));
      }

      MockRepository.VerifyAll ();
      Assert.That (strategy.Scope, Is.Null);
    }
  }
}