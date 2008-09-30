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

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure.RootTransactionStrategyTests
{
  [TestFixture]
  public class Common : RootTransactionStrategyTestBase
  {
    private RootTransactionStrategy _strategy;

    public override void SetUp ()
    {
      base.SetUp();
      _strategy = CreateRootTransactionStrategy (true);
    }

    [Test]
    public void GetInnerListener ()
    {
      Assert.That (_strategy.InnerListener, Is.SameAs (ExecutionListenerMock));
    }

    [Test]
    public void GetAutoCommit ()
    {
      Assert.That (_strategy.AutoCommit, Is.True);
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_strategy.IsNull, Is.False);
    }

    [Test]
    public void GetScopeManager ()
    {
      Assert.That (_strategy.ScopeManager, Is.SameAs (ScopeManagerMock));
    }

    [Test]
    public void GetTransaction ()
    {
      Assert.That (_strategy.Transaction, Is.SameAs (TransactionMock));
    }
  }
}