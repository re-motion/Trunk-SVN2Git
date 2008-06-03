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
using Remotion.Web.Security.ExecutionEngine;
using Remotion.Web.UnitTests.Security.Domain;

namespace Remotion.Web.UnitTests.Security.ExecutionEngine
{
  [TestFixture]
  public class WxeDemandCreatePermissionAttributeTest
  {
    [Test]
    public void Initialize ()
    {
      WxeDemandCreatePermissionAttribute attribute = new WxeDemandCreatePermissionAttribute (typeof (SecurableObject));

      Assert.AreEqual (MethodType.Constructor, attribute.MethodType);
      Assert.AreSame ( typeof (SecurableObject), attribute.SecurableClass);
    }
  }
}
