﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.ExtensibleEnums;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Globalization.ExtensibleEnums.Implementation
{
  /// <summary>
  /// Retrieves the human-readable localized representation of extensible-enumeration objects.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor (typeof (IExtensibleEnumGlobalizationService), Lifetime = LifetimeKind.Singleton)]
  public sealed class ExtensibleEnumGlobalizationService : IExtensibleEnumGlobalizationService
  {
    private readonly IGlobalizationService _globalizationService;

    public ExtensibleEnumGlobalizationService (IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull ("globalizationService", globalizationService);

      _globalizationService = globalizationService;
    }

    public bool TryGetExtensibleEnumValueDisplayName (IExtensibleEnum value, out string result)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      var resourceType = value.GetValueInfo().DefiningMethod.DeclaringType;
      var resourceManager = _globalizationService.GetResourceManager (TypeAdapter.Create (resourceType));

      return resourceManager.TryGetString(value.ID, out result);
    }
  }
}