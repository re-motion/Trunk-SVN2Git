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
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  /// <summary>
  /// Retrieving the human-readable localized representation of enumeration objects.
  /// </summary>
  public class EnumerationGlobalizationService : IEnumerationGlobalizationService
  {
    private readonly ICache<Type, IResourceManager> _enumResourceManagers = CacheFactory.CreateWithLocking<Type, IResourceManager>();
    private readonly ICache<Type, IDictionary<Enum, string>> _staticEnumValues = CacheFactory.CreateWithLocking<Type, IDictionary<Enum, string>>();
    private readonly IGlobalizationService _globalizationService;

    //TODO RM-5831: review
    public EnumerationGlobalizationService (ICompoundGlobalizationService globalizationService)  
    {
      ArgumentUtility.CheckNotNull ("globalizationService", globalizationService);

      _globalizationService = globalizationService;
    }

    public string GetEnumerationValueDisplayName (Enum value)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      // TODO RM-5831: change this to have the implementation in here. Get IGlobalizationService injected into ctor.
      // cache ResourceManager for EnumType
      // if has ResourceManager, get globalization from ResourceManager and fallback to value.ToString()
      // else use EnumDescriptionAttriute as used in EnumDescription-class, but only cache the Enum as key and string as value, 
      //also cache the fallback to value.ToString()
      // Dpublicate existing tests for EnumDescription for GetDescription-API
      // Inject IMemberInformationNameResolver and add GetEnumerationValueName (Enum value) to API for resolving the identifier.

      var enumType = value.GetType();
      var resourceManager = _enumResourceManagers.GetOrCreateValue (enumType, type => _globalizationService.GetResourceManager (type));
      if (!resourceManager.IsNull) 
      {
        string resourceValue;
        //TODO: use IMemberInformationNameResolver
        if (resourceManager.TryGetString (ResourceIdentifiersAttribute.GetResourceIdentifier (value), out resourceValue))
          return resourceValue;
        return value.ToString();
      }

      var staticEnumValues = _staticEnumValues.GetOrCreateValue (enumType, GetStaticEnumValues);
      if (staticEnumValues.ContainsKey (value))
        return staticEnumValues[value];

      return value.ToString();
    }

    //TODO RM-5831: remove dictionary, cache by Enum value
    private IDictionary<Enum, string> GetStaticEnumValues (Type enumType)
    {
      var dictionary = new Dictionary<Enum, string>();
      foreach (var enumData in GetEnumData (enumType))
      {
        var field = enumData.Item1;
        var value = enumData.Item2;

        var descriptionAttribute = AttributeUtility.GetCustomAttribute<EnumDescriptionAttribute> (field, false);
        if (descriptionAttribute != null)
          dictionary.Add (value, descriptionAttribute.Description);
        else
          dictionary.Add (value, value.ToString());
      }
      return dictionary;
    }

    private Tuple<FieldInfo, Enum>[] GetEnumData (Type enumType)
    {
      return enumType.GetFields (BindingFlags.Static | BindingFlags.Public).Select (f => Tuple.Create (f, (Enum) f.GetValue (null))).ToArray();
    }
  }
}