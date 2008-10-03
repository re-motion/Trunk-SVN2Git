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
      Assert.That (WxeTransactionMode<TestTransactionFactory>.None, Is.InstanceOfType (typeof (NoneTransactionMode)));
    }

    [Test]
    public void GetCreateRoot ()
    {
      ITransactionMode transactionMode = WxeTransactionMode<TestTransactionFactory>.CreateRoot;
      Assert.That (transactionMode, Is.InstanceOfType (typeof (CreateRootTransactionMode<TestTransactionFactory>)));
      Assert.That (transactionMode.AutoCommit, Is.False);
    }

    [Test]
    public void GetCreateRootWithAutoCommit ()
    {
      ITransactionMode transactionMode = WxeTransactionMode<TestTransactionFactory>.CreateRootWithAutoCommit;
      Assert.That (transactionMode, Is.InstanceOfType (typeof (CreateRootTransactionMode<TestTransactionFactory>)));
      Assert.That (transactionMode.AutoCommit, Is.True);
    }

    [Test]
    public void GetCreateChildIfParent ()
    {
      ITransactionMode transactionMode = WxeTransactionMode<TestTransactionFactory>.CreateChildIfParent;
      Assert.That (transactionMode, Is.InstanceOfType (typeof (CreateChildIfParentTransactionMode<TestTransactionFactory>)));
      Assert.That (transactionMode.AutoCommit, Is.False);
    }

    [Test]
    public void GetCreateChildIfParentWithAutoCommit ()
    {
      ITransactionMode transactionMode = WxeTransactionMode<TestTransactionFactory>.CreateChildIfParentWithAutoCommit;
      Assert.That (transactionMode, Is.InstanceOfType (typeof (CreateChildIfParentTransactionMode<TestTransactionFactory>)));
      Assert.That (transactionMode.AutoCommit, Is.True);
    }
  }
}