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
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class CreateChildIfParentTransactionModeTest
  {
    [Test]
    public void GetStrategy_WithoutParentTransaction ()
    {
      ITransactionMode transactionMode = new CreateChildIfParentTransactionMode<TestTransactionScopeManager2> (true);
      var executionListenerStub = MockRepository.GenerateStub<IWxeFunctionExecutionListener> ();
      ITransactionStrategy strategy = transactionMode.GetStrategy (new TestFunction2 (), executionListenerStub);

      Assert.That (strategy, Is.InstanceOfType (typeof (RootTransactionStrategy<TestTransactionScopeManager2>)));
      Assert.That (((TransactionStrategyBase) strategy).InnerListener, Is.SameAs (executionListenerStub));
      Assert.That (strategy.AutoCommit, Is.True);
    }

    [Test]
    public void IsSerializeable ()
    {
      var deserialized = Serializer.SerializeAndDeserialize (new CreateChildIfParentTransactionMode<TestTransactionScopeManager2> (true));

      Assert.That (deserialized.AutoCommit, Is.True);
    }
  }
}