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
