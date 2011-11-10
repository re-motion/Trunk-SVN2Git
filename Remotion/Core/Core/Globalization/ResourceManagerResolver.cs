// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using System.Resources;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Globalization
{
  /// <summary>
  /// Provides a generalized implementation of the algorithms used to translate resource attributes into <see cref="IResourceManager"/> instances.
  /// </summary>
  /// <typeparam name="TAttribute">The type of the resource attribute to be resolved by this class.</typeparam>
  public class ResourceManagerResolver<TAttribute>
      where TAttribute: Attribute, IResourcesAttribute
  {
    private readonly LockingCacheDecorator<object, ResourceManagerCacheEntry> _resourceManagerWrappersCache =
        CacheFactory.CreateWithLocking<object, ResourceManagerCacheEntry>();

    private readonly ResourceManagerFactory _resourceManagerFactory = new ResourceManagerFactory();

    protected ResourceManagerFactory ResourceManagerFactory
    {
      get { return _resourceManagerFactory; }
    }

    /// <summary>
    ///   Returns an instance of <c>IResourceManager</c> for the resource container specified
    ///   in the class declaration of the type.
    /// </summary>
    /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManager/Common/*' />
    /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManager/param[@name="objectType" or @name="includeHierarchy"]' />
    public IResourceManager GetResourceManager (Type objectType, bool includeHierarchy)
    {
      ArgumentUtility.CheckNotNull ("objectType", objectType);
      ArgumentUtility.CheckNotNull ("includeHierarchy", includeHierarchy);

      Type definingType;
      return GetResourceManager (objectType, includeHierarchy, out definingType);
    }

    /// <summary>
    ///   Returns a <c>IResourceManager</c> set for the resource containers specified
    ///   in the class declaration of the type, throwing an exception if no resources can be found.
    /// </summary>
    /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManager/Common/*' />
    /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManager/param[@name="objectType" or @name="includeHierarchy" or @name="definingType"]' />
    public IResourceManager GetResourceManager (Type objectType, bool includeHierarchy, out Type definingType)
    {
      ArgumentUtility.CheckNotNull ("objectType", objectType);

      var cacheEntry = GetResourceManagerCacheEntry (objectType, includeHierarchy);
      if (cacheEntry.IsEmpty)
      {
        string message = string.Format (
            "Type {0} and its base classes do not define the attribute {1}.", objectType.FullName, typeof (TAttribute).Name);
        throw new ResourceException (message);
      }

      definingType = cacheEntry.DefiningType;
      return cacheEntry.ResourceManager;
    }

    /// <summary>
    /// Tries to get a <see cref="IResourceManager"/> for the given <paramref name="objectType"/> (see 
    /// <see cref="GetResourceManager(System.Type,bool,out System.Type)"/>), returning <see langword="null" /> if the type has no resources defined. 
    /// The <see cref="IResourceManager"/> is retrieved from the cache if possible; if not, a new <see cref="IResourceManager"/> is created and added
    /// to the cache.
    /// </summary>
    /// <param name="objectType">The type to get an <see cref="IResourceManager"/> for.</param>
    /// <param name="includeHierarchy">Determines whether to include all resources defined in the <paramref name="objectType"/>'s hierarchy. If
    /// set to <see langword="false" />, only those of the first type with resources in the hierarchy are included.</param>
    /// <returns>
    /// A <see cref="ResourceManagerCacheEntry"/> object for the given <paramref name="objectType"/>. The object is 
    /// <see cref="ResourceManagerCacheEntry.Empty"/> if no resources could be found.
    /// </returns>
    public virtual ResourceManagerCacheEntry GetResourceManagerCacheEntry (Type objectType, bool includeHierarchy)
    {
      ArgumentUtility.CheckNotNull ("objectType", objectType);

      // 1. Try to get resource manager from cache using objectType.
      // 2. If miss, get resource definition stream.
      // 3. Get first type from definition stream. Try to get resource manager from cache using that type.
      // 4. If miss, create resource manager from definition stream.
      // Steps 2-4 happen in CreateCacheEntry.

      var key = GetResourceManagerSetCacheKey (objectType, includeHierarchy);
      var cacheEntry = _resourceManagerWrappersCache.GetOrCreateValue (
          key,
          arg => CreateCacheEntry (objectType, includeHierarchy));

      return cacheEntry;
    }

    public virtual IEnumerable<ResourceDefinition<TAttribute>> GetResourceDefinitionStream (Type type, bool includeHierarchy)
    {
      Type currentType = type;
      while (currentType != null)
      {
        ResourceDefinition<TAttribute> definition = GetResourceDefinition (type, currentType);
        if (definition.HasResources)
        {
          yield return definition;
          if (!includeHierarchy)
            yield break;
        }
        currentType = currentType.BaseType;
      }
    }

    protected virtual ResourceDefinition<TAttribute> GetResourceDefinition (Type type, Type currentType)
    {
      TAttribute[] resourceAttributes = AttributeUtility.GetCustomAttributes<TAttribute> (currentType, false);
      return new ResourceDefinition<TAttribute> (currentType, resourceAttributes);
    }

    protected virtual object GetResourceManagerSetCacheKey (Type definingType, bool includeHierarchy)
    {
      return definingType.AssemblyQualifiedName + "/" + includeHierarchy;
    }

    private ResourceManagerCacheEntry CreateCacheEntry (Type objectType, bool includeHierarchy)
    {
      var resourceDefinitions = GetResourceDefinitionStream (objectType, includeHierarchy);

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

        object firstDefinitionKey = GetResourceManagerSetCacheKey (firstResourceDefinition.Type, includeHierarchy);
        return _resourceManagerWrappersCache.GetOrCreateValue (
            firstDefinitionKey,
            arg => ResourceManagerCacheEntry.Create (firstResourceDefinition.Type, CreateResourceManagerSet (resourceDefinitions)));
      }
    }

    private ResourceManagerSet CreateResourceManagerSet (IEnumerable<ResourceDefinition<TAttribute>> resourceDefinitions)
    {
      var resourceManagers = new List<ResourceManager>();
      foreach (ResourceDefinition<TAttribute> definition in resourceDefinitions)
      {
        foreach (Tuple<Type, TAttribute[]> attributePair in definition.GetAllAttributePairs())
        {
          ResourceManager[] resourceManagersForAttributePair = _resourceManagerFactory.GetResourceManagers (
              attributePair.Item1.Assembly, attributePair.Item2);
          resourceManagers.InsertRange (0, resourceManagersForAttributePair);
        }
      }

      //  Create a new resource mananger wrapper set and return it.
      var resourceManagerArray = resourceManagers.ToArray();
      return ResourceManagerWrapper.CreateWrapperSet (resourceManagerArray);
    }
  }
}