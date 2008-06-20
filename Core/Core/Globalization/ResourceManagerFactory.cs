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
using System.Reflection;
using System.Resources;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Globalization
{
  /// <summary>
  /// Provides methods to create resource managers for given resource management information.
  /// </summary>
  public class ResourceManagerFactory
  {
    private readonly InterlockedCache<string, ResourceManager> _resourceManagersCache = new InterlockedCache<string, ResourceManager> ();

    /// <summary>
    ///   Returns an <b>ResourceManager</b> array for the resource containers specified through the 
    ///   <paramref name="resourceAttributes"/>.
    /// </summary>
    /// <include file='doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManagers/*' />
    public ResourceManager[] GetResourceManagers<TAttribute> (Assembly assembly, TAttribute[] resourceAttributes)
        where TAttribute : Attribute, IResourcesAttribute
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      ArgumentUtility.CheckNotNull ("resourceAttributes", resourceAttributes);

      ResourceManager[] resourceManagers = new ResourceManager[resourceAttributes.Length];

      //  Load the ResourceManagers for the type's resources

      for (int index = 0; index < resourceAttributes.Length; index++)
      {
        Assembly resourceAssembly = resourceAttributes[index].ResourceAssembly;
        if (resourceAssembly == null)
          resourceAssembly = assembly;
        string key = resourceAttributes[index].BaseName + " in " + resourceAssembly.FullName;

        //  Look in cache 
        resourceManagers[index] = _resourceManagersCache.GetOrCreateValue (
            key,
            delegate
            {
              string baseName = resourceAttributes[index].BaseName;
              return new ResourceManager (baseName, resourceAssembly);
            });
      }

      return resourceManagers;
    }
  }
}