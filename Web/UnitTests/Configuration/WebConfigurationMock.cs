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
using Remotion.Development.UnitTesting;
using Remotion.Web.Configuration;

namespace Remotion.Web.UnitTests.Configuration
{

/// <summary> 
///   Provides helper methods for initalizing a <see cref="WebConfiguration"/> object when simulating ASP.NET 
///   request cycles. 
/// </summary>
public class WebConfigurationMock: WebConfiguration
{
  public WebConfigurationMock()
  {
  }
  
  public static new WebConfiguration Current
  {
    get { return WebConfiguration.Current; }
    set {PrivateInvoke.SetNonPublicStaticField (typeof (WebConfiguration), "s_current", value); }
  }
}

}
