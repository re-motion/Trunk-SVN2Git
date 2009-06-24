// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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

    public static ITypeDiscoveryService DefaultService
    {
      get
      {
        lock (_defaultServiceLock)
        {
          if (_defaultService == null)
          {
            _defaultService =
                VersionDependentImplementationBridge<IAssemblyFinderTypeDiscoveryServiceFactoryImplementation>.Implementation.CreateTypeDiscoveryService();
          }
          return _defaultService;
        }
      }
    }

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

    public static Type GetType (string typeName, bool throwOnError)
    {
      if (DesignerUtility.IsDesignMode)
      {
        Type type = DesignerUtility.GetDesignModeType (typeName);
        if (type == null && throwOnError)
        {
          string message = string.Format ("Type '{0}' could not be loaded by the designer host.", typeName);
          throw new TypeLoadException (message);
        }
        return type;
      }
      else
        return Type.GetType (typeName, throwOnError);
    }
  }
}
