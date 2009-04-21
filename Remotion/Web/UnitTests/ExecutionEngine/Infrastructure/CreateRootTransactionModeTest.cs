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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class CreateRootTransactionModeTest
  {
    [Test]
    public void CreateTransactionStrategy_WithoutParentFunction ()
    {
      WxeContextFactory wxeContextFactory = new WxeContextFactory();
      WxeContext context = wxeContextFactory.CreateContext (new TestFunction());

      ITransactionMode transactionMode = new CreateRootTransactionMode (true, new TestTransactionFactory ());
      TransactionStrategyBase strategy = transactionMode.CreateTransactionStrategy (new TestFunction2 (transactionMode), context);

      Assert.That (strategy, Is.InstanceOfType (typeof (RootTransactionStrategy)));
      Assert.That (strategy.GetNativeTransaction<TestTransaction> (), Is.InstanceOfType (typeof (TestTransaction)));
      Assert.That (strategy.OuterTransactionStrategy, Is.InstanceOfType (typeof (NullTransactionStrategy)));
      Assert.That (((RootTransactionStrategy) strategy).AutoCommit, Is.True);
      Assert.That (((RootTransactionStrategy) strategy).Transaction, Is.InstanceOfType (typeof (TestTransaction)));
    }

    [Test]
    public void CreateTransactionStrategy_WithParentFunction ()
    {
      ITransactionMode transactionMode = new CreateRootTransactionMode (true, new TestTransactionFactory ());

      WxeFunction parentFunction = new TestFunction2 (new NoneTransactionMode ());
      WxeFunction childFunction = new TestFunction2 (transactionMode);
      parentFunction.Add (childFunction);

      WxeStep stepMock = MockRepository.GenerateMock<WxeStep> ();
      childFunction.Add (stepMock);

      WxeContextFactory wxeContextFactory = new WxeContextFactory ();
      WxeContext context = wxeContextFactory.CreateContext (new TestFunction ());

      stepMock.Expect (mock => mock.Execute (context)).Do (
          invocation =>
          {
            TransactionStrategyBase strategy = transactionMode.CreateTransactionStrategy (childFunction, context);
            Assert.That (strategy, Is.InstanceOfType (typeof (RootTransactionStrategy)));
            Assert.That (strategy.OuterTransactionStrategy, Is.SameAs (((TestFunction2) parentFunction).TransactionStrategy));
          });

      parentFunction.Execute (context);
    }
  }
}
