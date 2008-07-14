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
using Remotion.BridgeInterfaces;
using Remotion.Implementation;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  public static class ContextAwareTypeDiscoveryUtility
  {
    private static ITypeDiscoveryService _defaultService = null;
    private static readonly object _defaultServiceLock = new object ();

    public static ITypeDiscoveryService GetInstance ()
    {
      if (DesignerUtility.IsDesignMode)
        return (ITypeDiscoveryService) DesignerUtility.DesignerHost.GetService (typeof (ITypeDiscoveryService));
      else
        return DefaultService;
    }

    public static void SetDefaultService (ITypeDiscoveryService newDefaultService)
    {
      lock (_defaultServiceLock)
      {
        _defaultService = newDefaultService;
      }
    }

    public static ITypeDiscoveryService DefaultService
    {
      get
      {
        lock (_defaultServiceLock)
        {
          if (_defaultService == null)
          {
            _defaultService =
                VersionDependentImplementationBridge<IAssemblyFinderTypeDiscoveryServiceImplementation>.Implementation.CreateTypeDiscoveryService();
          }
          return _defaultService;
        }
      }
    }
  }
}
