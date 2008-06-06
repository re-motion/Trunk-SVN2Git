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
using System.ComponentModel.Design;
using System.Runtime.Remoting.Messaging;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  public static class ContextAwareTypeDiscoveryService
  {
    private static readonly string s_defaultServiceKey = typeof (ContextAwareTypeDiscoveryService).FullName + ".DefaultService";

    public static readonly CallContextSingleton<ITypeDiscoveryService> DefaultService =
        new CallContextSingleton<ITypeDiscoveryService> (s_defaultServiceKey, CreateDefaultService);

    private static ITypeDiscoveryService CreateDefaultService ()
    {
      return new AssemblyFinderTypeDiscoveryService (new AssemblyFinder (ApplicationAssemblyFinderFilter.Instance, false));
    }

    public static ITypeDiscoveryService GetInstance ()
    {
      if (DesignerUtility.IsDesignMode)
        return (ITypeDiscoveryService) DesignerUtility.DesignerHost.GetService (typeof (ITypeDiscoveryService));
      else
        return DefaultService.Current;
    }
  }
}
