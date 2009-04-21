// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure.ScopedTransactionStrategyTests
{
  [TestFixture]
  public class Reset : ScopedTransactionStrategyTestBase
  {
    private ScopedTransactionStrategyBase _strategy;

    public override void SetUp ()
    {
      base.SetUp();
      _strategy = CreateScopedTransactionStrategy (true, NullTransactionStrategy.Null);
    }

    [Test]
    public void Test_WithoutScope ()
    {
      TransactionMock.BackToRecord ();
      using (MockRepository.Ordered())
      {
        TransactionMock.Expect (mock => mock.Reset ());
      }
      Assert.That (_strategy.Scope, Is.Null);
      MockRepository.ReplayAll();

      _strategy.Reset();

      MockRepository.VerifyAll();
      Assert.That (_strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithoutScope_And_ResetThrows ()
    {
      var exception = new ApplicationException ("Reset Exception");
      TransactionMock.Expect (mock => mock.Reset ()).Throw (exception);
      Assert.That (_strategy.Scope, Is.Null);
      MockRepository.ReplayAll();

      try
      {
        _strategy.Reset();
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (exception));
      }
      MockRepository.VerifyAll();
      Assert.That (_strategy.Scope, Is.Null);
    }

    [Test]
    public void Test_WithScope ()
    {
      InvokeOnExecutionPlay (_strategy);
      TransactionMock.BackToRecord ();
      var newScopeMock = MockRepository.StrictMock<ITransactionScope>();

      using (MockRepository.Ordered ())
      {
        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Reset ());
        TransactionMock.Expect (mock => mock.EnterScope ()).Return (newScopeMock);
      }
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
      MockRepository.ReplayAll();

      _strategy.Reset();

      MockRepository.VerifyAll();
      Assert.That (_strategy.Scope, Is.SameAs (newScopeMock));
    }

    [Test]
    public void Test_WithScope_And_LeaveThrows ()
    {
      InvokeOnExecutionPlay (_strategy);
      var exception = new ApplicationException ("Leave Exception");
      using (MockRepository.Ordered ())
      {
        ScopeMock.Expect (mock => mock.Leave ()).Throw (exception);
      }
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
      MockRepository.ReplayAll ();

      try
      {
        _strategy.Reset ();
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (exception));
      }
      MockRepository.VerifyAll ();
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
    }

    [Test]
    public void Test_WithScope_And_ResetThrows ()
    {
      InvokeOnExecutionPlay (_strategy);
      var exception = new ApplicationException ("Reset Exception");
      using (MockRepository.Ordered())
      {
        ScopeMock.Expect (mock => mock.Leave());
        TransactionMock.Expect (mock => mock.Reset ()).Throw (exception);
      }
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
      MockRepository.ReplayAll();

      try
      {
        _strategy.Reset();
        Assert.Fail ("Expected Exception");
      }
      catch (ApplicationException actualException)
      {
        Assert.That (actualException, Is.SameAs (exception));
      }
      MockRepository.VerifyAll();
      Assert.That (_strategy.Scope, Is.SameAs (ScopeMock));
    }
  }
}
