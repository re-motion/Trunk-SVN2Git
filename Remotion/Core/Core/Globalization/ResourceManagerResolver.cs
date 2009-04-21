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
using System.Collections.Generic;
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
      where TAttribute : Attribute, IResourcesAttribute
  {
    private readonly InterlockedCache<object, ResourceManagerSet> _resourceManagerWrappersCache = new InterlockedCache<object, ResourceManagerSet>();
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
    ///   in the class declaration of the type.
    /// </summary>
    /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManager/Common/*' />
    /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManager/param[@name="objectType" or @name="includeHierarchy" or @name="definingType"]' />
    public virtual IResourceManager GetResourceManager (Type objectType, bool includeHierarchy, out Type definingType)
    {

      IEnumerable<ResourceDefinition<TAttribute>> resourceDefinitions = GetResourceDefinitionStream (objectType, includeHierarchy);
      ResourceDefinition<TAttribute> firstResourceDefinition = EnumerableUtility.FirstOrDefault (resourceDefinitions);
      if (firstResourceDefinition == null)
      {
        string message = string.Format ("Type {0} and its base classes do not define the attribute {1}.", objectType.FullName, typeof (TAttribute).Name);
        throw new ResourceException (message);
      }

      definingType = firstResourceDefinition.Type;

      //  Look in cache and continue with the cached resource manager wrapper, if one is found
      object key = GetResourceManagerSetCacheKey (firstResourceDefinition.Type, includeHierarchy);
      return _resourceManagerWrappersCache.GetOrCreateValue (key, delegate { return CreateResourceManagerSet (resourceDefinitions); });
    }

    public IEnumerable<ResourceDefinition<TAttribute>> GetResourceDefinitionStream (Type type, bool includeHierarchy)
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

    private ResourceManagerSet CreateResourceManagerSet (IEnumerable<ResourceDefinition<TAttribute>> resourceDefinitions)
    {
      List<ResourceManager> resourceManagers = new List<ResourceManager> ();
      foreach (ResourceDefinition<TAttribute> definition in resourceDefinitions)
      {
        foreach (Tuple<Type, TAttribute[]> attributePair in definition.GetAllAttributePairs ())
        {
          ResourceManager[] resourceManagersForAttributePair = _resourceManagerFactory.GetResourceManagers (attributePair.A.Assembly, attributePair.B);
          resourceManagers.InsertRange (0, resourceManagersForAttributePair);
        }
      }

      //  Create a new resource mananger wrapper set and return it.
      ResourceManager[] resourceManagerArray = resourceManagers.ToArray ();
      return ResourceManagerWrapper.CreateWrapperSet (resourceManagerArray);
    }
  }
}
