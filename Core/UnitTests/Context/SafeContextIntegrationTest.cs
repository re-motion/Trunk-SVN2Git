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
using System.Runtime.Remoting.Messaging;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Context;

namespace Remotion.UnitTests.Context
{
  [TestFixture]
  public class SafeContextIntegrationTest
  {
    [Test]
    public void SetGetFreeData ()
    {
      Assert.That (SafeContext.Instance.GetData ("Integration"), Is.Null);

      SafeContext.Instance.SetData ("Integration", "value");
      Assert.That (SafeContext.Instance.GetData ("Integration"), Is.EqualTo ("value"));
      Assert.That (CallContext.GetData ("Integration"), Is.EqualTo ("value"));

      SafeContext.Instance.SetData ("Integration", "other value");
      Assert.That (SafeContext.Instance.GetData ("Integration"), Is.EqualTo ("other value"));

      SafeContext.Instance.FreeData("Integration");
      Assert.That (SafeContext.Instance.GetData ("Integration"), Is.Null);
      Assert.That (CallContext.GetData ("Integration"), Is.Null);
    }
  }
}