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
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class WxeTransactionModeTest
  {
    [Test]
    public void GetNullTransactionMode ()
    {
      Assert.That (
          WxeTransactionMode<TestTransaction, TestTransactionScope, TestTransactionScopeManager>.Null,
          Is.InstanceOfType (typeof (NullTransactionMode)));
    }

    [Test]
    public void GetCreateRootTransactionMode ()
    {
      Assert.That (
          WxeTransactionMode<TestTransaction, TestTransactionScope, TestTransactionScopeManager>.CreateRoot,
          Is.InstanceOfType (typeof (CreateRootTransactionMode<TestTransaction, TestTransactionScope, TestTransactionScopeManager>)));
    }
  }
}