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
using Remotion.Collections;
using Remotion.Globalization;
using Remotion.Mixins.Globalization;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  //TODO: doc
  public class BindableObjectGlobalizationService : IBindableObjectGlobalizationService
  {
    [ResourceIdentifiers]
    [MultiLingualResources ("Remotion.ObjectBinding.Globalization.BindableObjectGlobalizationService")]
    private enum ResourceIdentifier
    {
      True,
      False
    }

    private readonly InterlockedCache<Type, IResourceManager> _resourceManagerCache = new InterlockedCache<Type, IResourceManager>();

    public BindableObjectGlobalizationService ()
    {
    }

    public string GetEnumerationValueDisplayName (Enum value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      return EnumDescription.GetDescription (value) ?? value.ToString();
    }

    public string GetBooleanValueDisplayName (bool value)
    {
      IResourceManager resourceManager = GetResourceManagerFromCache (typeof (ResourceIdentifier));
      return resourceManager.GetString (value ? ResourceIdentifier.True : ResourceIdentifier.False);
    }

    public string GetPropertyDisplayName (IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      IResourceManager resourceManager = GetResourceManagerFromCache (propertyInfo.DeclaringType);
      string resourceID = "property:" + propertyInfo.Name;
      if (!resourceManager.ContainsResource (resourceID))
        return propertyInfo.Name;
      return resourceManager.GetString (resourceID);
    }

    private IResourceManager GetResourceManagerFromCache (Type type)
    {
      IResourceManager resourceManager;
      if (_resourceManagerCache.TryGetValue (type, out resourceManager))
        return resourceManager;
      return _resourceManagerCache.GetOrCreateValue (type, GetResourceManager);
    }

    private IResourceManager GetResourceManager (Type type)
    {
      if (!MixedMultiLingualResources.ExistsResource (type))
        return NullResourceManager.Instance;
      return MixedMultiLingualResources.GetResourceManager (type, true);
    }
  }
} 
