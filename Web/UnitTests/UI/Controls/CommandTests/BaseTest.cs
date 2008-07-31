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
using Remotion.Development.Web.UnitTesting.Configuration;
using Remotion.Security;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Development.Web.UnitTesting.AspNetFramework;

namespace Remotion.Web.UnitTests.UI.Controls.CommandTests
{
  public class BaseTest
  {
    [TearDown]
    public virtual void TearDown ()
    {
      HttpContextHelper.SetCurrent (null);
      WebConfigurationMock.Current = null;
      Remotion.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);
      AdapterRegistry.Instance.SetAdapter (typeof (IWebSecurityAdapter), null);
      AdapterRegistry.Instance.SetAdapter (typeof (IWxeSecurityAdapter), null);
    }
  }
}
