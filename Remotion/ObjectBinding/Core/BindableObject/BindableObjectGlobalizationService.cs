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
using Remotion.Collections;
using Remotion.ExtensibleEnums;
using Remotion.Globalization;
using Remotion.Mixins.Globalization;
using Remotion.Reflection;
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

    private readonly ICache<ITypeInformation, IResourceManager> _resourceManagerCache =
        CacheFactory.CreateWithLocking<ITypeInformation, IResourceManager>();

    private readonly TypeConversionProvider _typeConversionProvider;

    public BindableObjectGlobalizationService ()
    {
      _typeConversionProvider = TypeConversionProvider.Create();
    }

    public string GetEnumerationValueDisplayName (Enum value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      return EnumDescription.GetDescription (value);
    }

    public string GetExtensibleEnumerationValueDisplayName (IExtensibleEnum value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      return value.GetLocalizedName();
    }

    public string GetBooleanValueDisplayName (bool value)
    {
      IResourceManager resourceManager = GetResourceManagerFromCache (TypeAdapter.Create (typeof (ResourceIdentifier)));
      return resourceManager.GetString (value ? ResourceIdentifier.True : ResourceIdentifier.False);
    }

    public string GetPropertyDisplayName (IPropertyInformation propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      // Note: Currently, MixedMultilingualResources requires the concrete mixed type and the concrete implemented property for globalization 
      // attribute analysis. We need to extract that information from BindableObjectMixinIntroducedPropertyInformation. The goal is to redesign mixin-
      // based globalization some time, so that we can work with ordinary IPropertyInformation objects

      var mixinIntroducedPropertyInformation = propertyInfo as BindableObjectMixinIntroducedPropertyInformation;
      var globalizedType = mixinIntroducedPropertyInformation != null ? mixinIntroducedPropertyInformation.ConcreteType : propertyInfo.DeclaringType;
      var property = mixinIntroducedPropertyInformation != null ? mixinIntroducedPropertyInformation.ConcreteProperty : propertyInfo;

      var resourceManager = GetResourceManagerFromCache (globalizedType);

      string resourceID = "property:" + property.Name;
      if (!resourceManager.ContainsResource (resourceID))
        return propertyInfo.Name;
      return resourceManager.GetString (resourceID);
    }

    private IResourceManager GetResourceManagerFromCache (ITypeInformation typeInformation)
    {
      IResourceManager resourceManager;
      if (_resourceManagerCache.TryGetValue (typeInformation, out resourceManager))
        return resourceManager;
      return _resourceManagerCache.GetOrCreateValue (typeInformation, GetResourceManager);
    }

    private IResourceManager GetResourceManager (ITypeInformation typeInformation)
    {
      if (!_typeConversionProvider.CanConvert (typeInformation.GetType(), typeof (Type)))
        return NullResourceManager.Instance;

      var type = (Type) _typeConversionProvider.Convert (typeInformation.GetType(), typeof (Type), typeInformation);

      if (MixedMultiLingualResources.ExistsResource (type))
        return MixedMultiLingualResources.GetResourceManager (type, true);

      return NullResourceManager.Instance;
    }
  }
}