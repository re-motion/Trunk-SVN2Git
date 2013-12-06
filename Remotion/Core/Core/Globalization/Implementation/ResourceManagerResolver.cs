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
using System.Linq;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Default implementation of the <see cref="IResourceManagerResolver"/>.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  public sealed class ResourceManagerResolver : IResourceManagerResolver
  {
    private readonly LockingCacheDecorator<Type, ResolvedResourceManagerResult> _resourceManagerWrappersCache =
        CacheFactory.CreateWithLocking<Type, ResolvedResourceManagerResult>();

    private readonly ResourceManagerFactory _resourceManagerFactory = new ResourceManagerFactory();

    public ResolvedResourceManagerResult Resolve (Type objectType)
    {
      ArgumentUtility.CheckNotNull ("objectType", objectType);

      return GetResolvedResourceManagerFromCache (objectType);
    }

    private ResolvedResourceManagerResult GetResolvedResourceManagerFromCache (Type objectType)
    {
      if (objectType == null)
        return ResolvedResourceManagerResult.Null;

      return _resourceManagerWrappersCache.GetOrCreateValue (objectType, CreateResolvedResourceManagerResult);
    }

    private ResolvedResourceManagerResult CreateResolvedResourceManagerResult (Type objectType)
    {
      var definedResourceManager = CreateResourceManager (objectType);
      var inheritedResourceManager = GetResolvedResourceManagerFromCache (objectType.BaseType).ResourceManager;
      return ResolvedResourceManagerResult.Create (definedResourceManager, inheritedResourceManager);
    }

    private IResourceManager CreateResourceManager (Type type)
    {
      var resourceAttributes = AttributeUtility.GetCustomAttributes<IResourcesAttribute> (type, false);
      return ResourceManagerWrapper.CreateWrapperSet (_resourceManagerFactory.GetResourceManagers (type.Assembly, resourceAttributes.ToArray()));
    }
  }
}