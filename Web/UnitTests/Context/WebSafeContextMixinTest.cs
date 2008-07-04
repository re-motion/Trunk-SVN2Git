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
using Remotion.Context;
using Remotion.Web.Context;

namespace Remotion.Web.UnitTests.Context
{
  [TestFixture]
  public class WebSafeContextMixinTest
  {
    [Test]
    public void StorageProvider_IsHttpProvider_InWebProject ()
    {
      Assert.That (SafeContext.Instance, Is.InstanceOfType (typeof (HttpContextStorageProvider)));
    }
  }
}