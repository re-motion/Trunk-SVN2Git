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
using System.Linq;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Provides a generalized implementation of the algorithms used to translate resource attributes into <see cref="IResourceManager"/> instances.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  public sealed class ResourceManagerResolver : IResourceManagerResolver
  {
    private readonly LockingCacheDecorator<Type, ResourceManagerCacheEntry> _resourceManagerWrappersCache =
        CacheFactory.CreateWithLocking<Type, ResourceManagerCacheEntry>();

    private readonly ResourceManagerFactory _resourceManagerFactory = new ResourceManagerFactory();

    /// <summary>
    /// Tries to get a <see cref="IResourceManager"/> for the given <paramref name="objectType"/>, returning <see langword="null" /> if the type has no resources defined. 
    /// The <see cref="IResourceManager"/> is retrieved from the cache if possible; if not, a new <see cref="IResourceManager"/> is created and added
    /// to the cache.
    /// </summary>
    /// <param name="objectType">The type to get an <see cref="IResourceManager"/> for.</param>
    /// <returns>
    /// A <see cref="ResourceManagerCacheEntry"/> object for the given <paramref name="objectType"/>. The object is 
    /// <see cref="ResourceManagerCacheEntry.Empty"/> if no resources could be found.
    /// </returns>
    public IResourceManager GetResourceManager (Type objectType)
    {
      ArgumentUtility.CheckNotNull ("objectType", objectType);

      // 1. Try to get resource manager from cache using objectType.
      // 2. If miss, get resource definition stream.
      // 3. Get first type from definition stream. Try to get resource manager from cache using that type.
      // 4. If miss, create resource manager from definition stream.
      // Steps 2-4 happen in CreateCacheEntry.

      var cacheEntry = _resourceManagerWrappersCache.GetOrCreateValue (objectType, CreateCacheEntry);
      return cacheEntry.IsEmpty ? NullResourceManager.Instance :  cacheEntry.ResourceManager;
    }

    private ResourceManagerCacheEntry CreateCacheEntry (Type objectType)
    {
      var resourceDefinitions = GetResourceDefinitionStream (objectType).ToArray();

      // Get the first resource definition in the stream (this is the type itself, or the first base class that has a resource definition).
      // If that first definition's defining type already has a resource manager, we'll use that resource manager. Otherwise, we'll create a new one
      // and store it for that first type.
      var firstResourceDefinition = resourceDefinitions.FirstOrDefault();
      if (firstResourceDefinition == null)
        return ResourceManagerCacheEntry.Empty;

      if (firstResourceDefinition.Type == objectType)
      {
        // Note: We cannot add the new entry to the cache here - the caller will do that.
        return ResourceManagerCacheEntry.Create (firstResourceDefinition.Type, CreateResourceManagerSet (resourceDefinitions));
      }
      else
      {
        // Create or get the entry for the base class from cache.
        // Then reuse that cache entry for the objectType.

        return _resourceManagerWrappersCache.GetOrCreateValue (
            firstResourceDefinition.Type,
            arg => ResourceManagerCacheEntry.Create (firstResourceDefinition.Type, CreateResourceManagerSet (resourceDefinitions)));
      }
    }

    private IEnumerable<ResourceDefinition> GetResourceDefinitionStream (Type type)
    {
      var currentType = type;
      while (currentType != null)
      {
        var definition = GetResourceDefinition (currentType);
        if (definition.HasResources)
          yield return definition;
        currentType = currentType.BaseType;
      }
    }

    private ResourceDefinition GetResourceDefinition (Type type)
    {
      IResourcesAttribute[] resourceAttributes = AttributeUtility.GetCustomAttributes<IResourcesAttribute> (type, false);
      return new ResourceDefinition (type, resourceAttributes);
    }

    private ResourceManagerSet CreateResourceManagerSet (IEnumerable<ResourceDefinition> resourceDefinitions)
    {
      return ResourceManagerWrapper.CreateWrapperSet (
          resourceDefinitions
              .SelectMany (definition => definition.GetAllAttributePairs())
              .SelectMany (ap => _resourceManagerFactory.GetResourceManagers (ap.Item1.Assembly, ap.Item2)));
    }
  }
}