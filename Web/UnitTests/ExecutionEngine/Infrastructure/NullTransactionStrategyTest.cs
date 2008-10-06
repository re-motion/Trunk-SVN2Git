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
using Remotion.Data;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class NullTransactionStrategyTest
  {
    private ITransactionStrategy _strategy;

    [SetUp]
    public void SetUp ()
    {
      _strategy = NullTransactionStrategy.Null;
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (_strategy.IsNull, Is.True);
    }

    [Test]
    public void GetNativeTransaction ()
    {
      Assert.That (_strategy.GetNativeTransaction<ITransaction>(), Is.Null);
    }

    [Test]
    public void RegisterObjects ()
    {
      _strategy.RegisterObjects (null);
    }

    [Test]
    public void Commit()
    {
      _strategy.Commit();
    }

    [Test]
    public void Rollback ()
    {
      _strategy.Rollback ();
    }

    [Test]
    public void Reset ()
    {
      _strategy.Reset ();
    }
  }
}