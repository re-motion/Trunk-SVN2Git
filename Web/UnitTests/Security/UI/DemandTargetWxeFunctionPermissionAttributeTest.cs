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
using Remotion.Web.Security.UI;
using Remotion.Web.UnitTests.Security.ExecutionEngine;

namespace Remotion.Web.UnitTests.Security.UI
{
  [TestFixture]
  public class DemandTargetWxeFunctionPermissionAttributeTest
  {
    [Test]
    public void Initialize ()
    {
      DemandTargetWxeFunctionPermissionAttribute attribute = new DemandTargetWxeFunctionPermissionAttribute (typeof (TestFunctionWithThisObject));

      Assert.AreEqual (PermissionSource.WxeFunction, attribute.PermissionSource);
      Assert.AreEqual (typeof (TestFunctionWithThisObject), attribute.FunctionType);
    }
  }
}
