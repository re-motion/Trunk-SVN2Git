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
using JetBrains.Annotations;
using Remotion.Collections;
using Remotion.Globalization;
using Remotion.Globalization.Implementation;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Globalization
{
  /// <summary>
  /// Retrieves and caches <see cref="IResourceManager"/>'s for mixin-types.
  /// </summary>
  public class MixinGlobalizationService : IGlobalizationService
  {
    private volatile MixinConfiguration _mixinConfiguration;
    private readonly IResourceManagerResolver _resourceManagerResolver;
    private readonly ICache<ITypeInformation, IResourceManager> _resourceManagerCache =
        CacheFactory.CreateWithLocking<ITypeInformation, IResourceManager>();

    public MixinGlobalizationService (IResourceManagerResolver resourceManagerResolver)
    {
      ArgumentUtility.CheckNotNull ("resourceManagerResolver", resourceManagerResolver);

      _resourceManagerResolver = resourceManagerResolver;
    }

    public IResourceManager GetResourceManager (ITypeInformation typeInformation)
    {
      ArgumentUtility.CheckNotNull ("typeInformation", typeInformation);

      var masterConfiguration = MixinConfiguration.GetMasterConfiguration();

      if (masterConfiguration != MixinConfiguration.ActiveConfiguration)
        return GetResourceManagerFromType (typeInformation);

      if (masterConfiguration != _mixinConfiguration)
      {
        _resourceManagerCache.Clear();
        _mixinConfiguration = masterConfiguration;
      }

      return _resourceManagerCache.GetOrCreateValue (typeInformation, GetResourceManagerFromType);
    }

    [NotNull]
    private IResourceManager GetResourceManagerFromType (ITypeInformation typeInformation)
    {
      var type = typeInformation.AsRuntimeType();
      if (type == null)
        return NullResourceManager.Instance;

      var classContext = MixinConfiguration.ActiveConfiguration.GetContext (type);
      if (classContext == null)
        return NullResourceManager.Instance;

      var resourceMangers = new List<IResourceManager>();
      CollectResourceManagersRecursively (classContext.Type, resourceMangers);

      if (resourceMangers.Any())
        return new ResourceManagerSet (resourceMangers);

      return NullResourceManager.Instance;
    }

    private void CollectResourceManagersRecursively (Type type, List<IResourceManager> collectedResourceMangers)
    {
      var mixinTypes = MixinTypeUtility.GetMixinTypesExact (type);

      foreach (var mixinType in mixinTypes)
        CollectResourceManagersRecursively (mixinType, collectedResourceMangers);

      collectedResourceMangers.AddRange (mixinTypes.Select (mixinType => _resourceManagerResolver.Resolve (mixinType).ResourceManager));
    }
  }
}