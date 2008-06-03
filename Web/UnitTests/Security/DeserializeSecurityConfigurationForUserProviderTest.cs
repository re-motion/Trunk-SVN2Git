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
using Remotion.Development.UnitTesting.Configuration;
using Remotion.Security.Configuration;
using Remotion.Web.Security;

namespace Remotion.Web.UnitTests.Security
{
  [TestFixture]
  public class DeserializeSecurityConfigurationForUserProviderTest
  {
    [Test]
    public void Test_WithHttpContextUserProvider ()
    {
      SecurityConfiguration configuration = new SecurityConfiguration ();
      string xmlFragment = @"<remotion.security defaultUserProvider=""HttpContext"" />";
      ConfigurationHelper.DeserializeSection (configuration, xmlFragment);
      Assert.IsInstanceOfType (typeof (HttpContextUserProvider), configuration.UserProvider);
    }

  }
}
