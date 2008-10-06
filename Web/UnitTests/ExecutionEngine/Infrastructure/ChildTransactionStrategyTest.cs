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
using Remotion.Web.ExecutionEngine.Infrastructure;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class ChildTransactionStrategyTest
  {
    [Test]
    public void GetAutoCommit ()
    {
      var executionContextStub = MockRepository.GenerateStub<IWxeFunctionExecutionContext> ();
      TransactionStrategyBase strategy = new ChildTransactionStrategy (true, NullTransactionStrategy.Null, executionContextStub);

      Assert.That (strategy.AutoCommit, Is.True);
    }

    [Test]
    public void IsNull ()
    {
      var executionContextStub = MockRepository.GenerateStub<IWxeFunctionExecutionContext> ();
      INullObject strategy = new ChildTransactionStrategy (true, NullTransactionStrategy.Null, executionContextStub);

      Assert.That (strategy.IsNull, Is.False);
    }
  }
}