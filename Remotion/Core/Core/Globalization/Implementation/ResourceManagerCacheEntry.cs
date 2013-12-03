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
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Holds resource manager information cached by <see cref="ResourceManagerResolver"/>.
  /// </summary>
  public sealed class ResourceManagerCacheEntry
  {
    public static readonly ResourceManagerCacheEntry Empty = new ResourceManagerCacheEntry (null, null);

    public static ResourceManagerCacheEntry Create (Type definingType, IResourceManager resourceManager)
    {
      ArgumentUtility.CheckNotNull ("definingType", definingType);
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);

      return new ResourceManagerCacheEntry (definingType, resourceManager);
    }

    private readonly Type _definingType;
    private readonly IResourceManager _resourceManager;

    private ResourceManagerCacheEntry (Type definingType, IResourceManager resourceManager)
    {
      // Both arguments can be null if no resource manager exists for a given type.

      _definingType = definingType;
      _resourceManager = resourceManager;
    }

    public Type DefiningType
    {
      get { return _definingType; }
    }

    public IResourceManager ResourceManager
    {
      get { return _resourceManager; }
    }

    public bool IsEmpty
    {
      get { return _resourceManager == null; }
    }
  }
}