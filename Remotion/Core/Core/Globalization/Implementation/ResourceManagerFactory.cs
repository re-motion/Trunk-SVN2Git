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
    private readonly LockingCacheDecorator<Tuple<Assembly, string>, ResourceManager> _resourceManagersCache =
        CacheFactory.CreateWithLocking<Tuple<Assembly, string>, ResourceManager>();

    /// <summary>
    ///   Returns an <b>ResourceManager</b> array for the resource containers specified through the 
    ///   <paramref name="resourceAttributes"/>.
    /// </summary>
    /// <include file='..\..\doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManagers/*' />
    public IEnumerable<ResourceManager> GetResourceManagers (Assembly assembly, IEnumerable<IResourcesAttribute> resourceAttributes)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      ArgumentUtility.CheckNotNull ("resourceAttributes", resourceAttributes);

      return resourceAttributes.Select (resourcesAttribute => GetResourceManagerFromCache (assembly, resourcesAttribute));
    }

    private ResourceManager GetResourceManagerFromCache (Assembly assembly, IResourcesAttribute resourcesAttribute)
    {
      return _resourceManagersCache.GetOrCreateValue (
          Tuple.Create (resourcesAttribute.ResourceAssembly ?? assembly, resourcesAttribute.BaseName),
          key => new ResourceManager (key.Item2, key.Item1));
    }
  }
}