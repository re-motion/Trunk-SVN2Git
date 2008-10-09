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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class CreateChildIfParentTransactionModeTest
  {
    [Test]
    public void CreateTransactionStrategy_WithoutParentFunction_And_WithoutParentTransaction ()
    {
      WxeContextFactory wxeContextFactory = new WxeContextFactory ();
      WxeContext context = wxeContextFactory.CreateContext (new TestFunction ());
      
      ITransactionMode transactionMode = new CreateChildIfParentTransactionMode<TestTransactionFactory> (true);
      TransactionStrategyBase strategy = transactionMode.CreateTransactionStrategy (new TestFunction2 (transactionMode), context);

      Assert.That (strategy, Is.InstanceOfType (typeof (RootTransactionStrategy)));
      Assert.That (strategy.GetNativeTransaction<TestTransaction>(), Is.InstanceOfType (typeof (TestTransaction)));
      Assert.That (((RootTransactionStrategy) strategy).AutoCommit, Is.True);
      Assert.That (((RootTransactionStrategy) strategy).Transaction, Is.InstanceOfType (typeof (TestTransaction)));
      Assert.That (strategy.OuterTransactionStrategy, Is.InstanceOfType (typeof (NullTransactionStrategy)));
    }

    [Test]
    public void CreateTransactionStrategy_WithParentFunction_And_WithoutParentTransaction ()
    {
      ITransactionMode transactionMode = new CreateRootTransactionMode<TestTransactionFactory> (true);

      WxeFunction2 parentFunction = new TestFunction2 (new NoneTransactionMode ());
      WxeFunction2 childFunction = new TestFunction2 (transactionMode);
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
            Assert.That (strategy.OuterTransactionStrategy, Is.SameAs (parentFunction.TransactionStrategy));
          });

      parentFunction.Execute (context);
    }

    [Test]
    public void CreateTransactionStrategy_WithParentTransaction ()
    {
      ITransactionMode transactionMode = new CreateChildIfParentTransactionMode<TestTransactionFactory> (true);

      WxeFunction2 parentFunction = new TestFunction2 (new CreateRootTransactionMode<TestTransactionFactory> (true));
      WxeFunction2 childFunction = new TestFunction2 (transactionMode);
      parentFunction.Add (childFunction);

      WxeStep stepMock = MockRepository.GenerateMock<WxeStep>();
      childFunction.Add (stepMock);

      WxeContextFactory wxeContextFactory = new WxeContextFactory();
      WxeContext context = wxeContextFactory.CreateContext (new TestFunction());

      stepMock.Expect (mock => mock.Execute (context)).Do (
          invocation =>
          {
            TransactionStrategyBase strategy = childFunction.TransactionStrategy;
            Assert.That (strategy, Is.InstanceOfType (typeof (ChildTransactionStrategy)));
            Assert.That (((ChildTransactionStrategy) strategy).AutoCommit, Is.True);
            Assert.That (strategy.OuterTransactionStrategy, Is.SameAs (parentFunction.TransactionStrategy));
          });

      parentFunction.Execute (context);
    }

    [Test]
    public void CreateTransactionStrategy_WithParentTransactionInGrandParentFunction ()
    {
      ITransactionMode transactionMode = new CreateChildIfParentTransactionMode<TestTransactionFactory> (true);

      WxeFunction2 grandParentFunction = new TestFunction2 (new CreateRootTransactionMode<TestTransactionFactory> (true));

      WxeFunction2 parentFunction = new TestFunction2 (new NoneTransactionMode ());
      grandParentFunction.Add (parentFunction);

      WxeFunction2 childFunction = new TestFunction2 (transactionMode);
      parentFunction.Add (childFunction);

      WxeStep stepMock = MockRepository.GenerateMock<WxeStep> ();
      childFunction.Add (stepMock);

      WxeContextFactory wxeContextFactory = new WxeContextFactory ();
      WxeContext context = wxeContextFactory.CreateContext (new TestFunction ());

      stepMock.Expect (mock => mock.Execute (context)).Do (
          invocation =>
          {
            TransactionStrategyBase strategy = childFunction.TransactionStrategy;
            Assert.That (strategy, Is.InstanceOfType (typeof (ChildTransactionStrategy)));
            Assert.That (((ChildTransactionStrategy) strategy).AutoCommit, Is.True);
            Assert.That (strategy.OuterTransactionStrategy, Is.SameAs (grandParentFunction.TransactionStrategy));
          });

      grandParentFunction.Execute (context);
    }

    [Test]
    public void IsSerializeable ()
    {
      var deserialized = Serializer.SerializeAndDeserialize (new CreateChildIfParentTransactionMode<TestTransactionFactory> (true));

      Assert.That (deserialized.AutoCommit, Is.True);
    }
  }
}