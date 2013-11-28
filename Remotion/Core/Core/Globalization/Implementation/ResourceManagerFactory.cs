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
using System.Reflection;
using System.Resources;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Provides methods to create resource managers for given resource management information.
  /// </summary>
  public class ResourceManagerFactory
  {
    private readonly LockingCacheDecorator<string, ResourceManager> _resourceManagersCache = CacheFactory.CreateWithLocking<string, ResourceManager>();

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
