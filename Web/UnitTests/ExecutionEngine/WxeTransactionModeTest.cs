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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class WxeTransactionModeTest
  {
    [Test]
    public void GetNone ()
    {
      Assert.That (WxeTransactionMode<TestTransactionScopeManager2>.None, Is.InstanceOfType (typeof (NoneTransactionMode)));
    }

    [Test]
    public void GetCreateRoot ()
    {
      ITransactionMode transactionMode = WxeTransactionMode<TestTransactionScopeManager2>.CreateRoot;
      Assert.That (transactionMode, Is.InstanceOfType (typeof (CreateRootTransactionMode<TestTransactionScopeManager2>)));
      Assert.That (transactionMode.AutoCommit, Is.False);
    }

    [Test]
    public void GetCreateRootWithAutoCommit ()
    {
      ITransactionMode transactionMode = WxeTransactionMode<TestTransactionScopeManager2>.CreateRootWithAutoCommit;
      Assert.That (transactionMode, Is.InstanceOfType (typeof (CreateRootTransactionMode<TestTransactionScopeManager2>)));
      Assert.That (transactionMode.AutoCommit, Is.True);
    }

    [Test]
    public void GetCreateChildIfParent ()
    {
      ITransactionMode transactionMode = WxeTransactionMode<TestTransactionScopeManager2>.CreateChildIfParent;
      Assert.That (transactionMode, Is.InstanceOfType (typeof (CreateChildIfParentTransactionMode<TestTransactionScopeManager2>)));
      Assert.That (transactionMode.AutoCommit, Is.False);
    }

    [Test]
    public void GetCreateChildIfParentWithAutoCommit ()
    {
      ITransactionMode transactionMode = WxeTransactionMode<TestTransactionScopeManager2>.CreateChildIfParentWithAutoCommit;
      Assert.That (transactionMode, Is.InstanceOfType (typeof (CreateChildIfParentTransactionMode<TestTransactionScopeManager2>)));
      Assert.That (transactionMode.AutoCommit, Is.True);
    }
  }
}