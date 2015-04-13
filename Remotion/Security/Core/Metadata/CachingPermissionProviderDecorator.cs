// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Collections.Generic;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{
  /// <summary>
  /// Implements a cache for <see cref="IPermissionProvider"/> implementations.
  /// </summary>
  [ImplementationFor (typeof (IPermissionProvider), RegistrationType = RegistrationType.Decorator)]
  public class CachingPermissionProviderDecorator : IPermissionProvider
  {
    private readonly IPermissionProvider _innerPermissionProvider;

    public CachingPermissionProviderDecorator (IPermissionProvider innerPermissionProvider)
    {
      ArgumentUtility.CheckNotNull ("innerPermissionProvider", innerPermissionProvider);

      _innerPermissionProvider = innerPermissionProvider;
    }

    public IPermissionProvider InnerPermissionProvider
    {
      get { return _innerPermissionProvider; }
    }

    public IReadOnlyList<Enum> GetRequiredMethodPermissions (Type type, IMethodInformation methodInformation)
    {
      ArgumentUtility.DebugCheckNotNull ("type", type);
      ArgumentUtility.DebugCheckNotNull("methodInformation", methodInformation);

      return _innerPermissionProvider.GetRequiredMethodPermissions (type, methodInformation);
    }
  }
}