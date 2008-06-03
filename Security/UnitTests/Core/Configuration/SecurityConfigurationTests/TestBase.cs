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
using Remotion.Development.UnitTesting;
using Remotion.Security.Configuration;

namespace Remotion.Security.UnitTests.Core.Configuration.SecurityConfigurationTests
{
  public class TestBase
  {
    private SecurityConfiguration _configuration;

    [SetUp]
    public virtual void SetUp ()
    {
      _configuration = new SecurityConfiguration();
      SetCurrentSecurityConfiguration (null);
    }

    [TearDown]
    public virtual void TearDown ()
    {
      SetCurrentSecurityConfiguration (null);
    }

    protected SecurityConfiguration Configuration
    {
      get
      {
        return _configuration;
      }
    }

    private void SetCurrentSecurityConfiguration (SecurityConfiguration configuration)
    {
      PrivateInvoke.InvokeNonPublicStaticMethod (typeof (SecurityConfiguration), "SetCurrent", configuration);
    }
  }
}
