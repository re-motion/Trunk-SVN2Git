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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using JetBrains.Annotations;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Globalization
{
  /// <summary>
  /// Use this class to get the clear text representations of enumeration values.
  /// </summary>
  /// <remarks>
  /// Use the <see cref="EnumDescriptionResourceAttribute"/> to provide globalization support for the enum type
  /// or the <see cref="EnumDescriptionAttribute"/> to only provide friendly names for the individual enum values.
  /// </remarks>
  public static class EnumDescription
  {
    /// <summary>This is for enums with the EnumDescriptionAttribute on values. </summary>
    private static readonly ICache<Type, IDictionary<Enum, EnumValue>> s_staticEnumValues =
        CacheFactory.CreateWithLocking<Type, IDictionary<Enum, EnumValue>>();

    /// <summary> This is for enums with the EnumDescriptionResourceAttribute.  </summary>
    private static readonly ICache<Type, ResourceManager> s_enumResourceManagers = CacheFactory.CreateWithLocking<Type, ResourceManager>();

    [NotNull]
    public static EnumValue[] GetAllValues ([NotNull] Type enumType)
    {
      ArgumentUtility.CheckNotNull ("enumType", enumType);

      var resourceManager = GetResourceManagerFromCache (enumType);
      if (resourceManager != null)
      {
        var data = GetEnumData (enumType);
        var values = new EnumValue[data.Length];

        for (int i = 0; i < data.Length; ++i)
        {
          var value = data[i].Item2;
          values[i] = new EnumValue (value, GetDescription (value, resourceManager));
        }

        return values;
      }
      else
      {
        var enumValues = GetStaticEnumValuesFromCache (enumType);
        return enumValues.Values.ToArray();
      }
    }

    [NotNull]
    public static EnumValue[] GetAllValues ([NotNull] Type enumType, [CanBeNull] CultureInfo culture)
    {
      ArgumentUtility.CheckNotNull ("enumType", enumType);

      using (new CultureScope (CultureInfo.CurrentCulture, culture ?? CultureInfo.CurrentUICulture))
      {
        return GetAllValues (enumType);
      }
    }

    [NotNull]
    public static string GetDescription ([NotNull] Enum value)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      Type enumType = value.GetType();
      var resourceManager = GetResourceManagerFromCache (enumType);
      if (resourceManager != null)
        return GetDescription (value, resourceManager);
      else
      {
        var enumValues = GetStaticEnumValuesFromCache (enumType);
        EnumValue enumValue;
        if (enumValues.TryGetValue (value, out enumValue))
          return enumValue.Description;

        return value.ToString();
      }
    }

    [NotNull]
    public static string GetDescription ([NotNull] Enum value, [CanBeNull] CultureInfo culture)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      using (new CultureScope (CultureInfo.CurrentCulture, culture ?? CultureInfo.CurrentUICulture))
      {
        return GetDescription (value);
      }
    }

    private static string GetDescription (Enum value, ResourceManager resourceManager)
    {
      return resourceManager.GetString (value.GetType().FullName + "." + value.ToString()) ?? value.ToString();
    }

    private static ResourceManager GetResourceManagerFromCache (Type enumType)
    {
      return s_enumResourceManagers.GetOrCreateValue (enumType, GetResourceManager);
    }

    private static ResourceManager GetResourceManager (Type enumType)
    {
      var resourceAttribute = AttributeUtility.GetCustomAttribute<EnumDescriptionResourceAttribute> (enumType, false);
      if (resourceAttribute == null)
        return null;

      return new ResourceManager (resourceAttribute.BaseName, enumType.Assembly, null);
    }

    private static IDictionary<Enum, EnumValue> GetStaticEnumValuesFromCache (Type enumType)
    {
      return s_staticEnumValues.GetOrCreateValue (enumType, GetStaticEnumValues);
    }

    private static IDictionary<Enum, EnumValue> GetStaticEnumValues (Type enumType)
    {
      var dictionary = new Dictionary<Enum, EnumValue>();
      foreach (var enumData in GetEnumData (enumType))
      {
        var field = enumData.Item1;
        var value = enumData.Item2;

        var descriptionAttribute = AttributeUtility.GetCustomAttribute<EnumDescriptionAttribute> (field, false);
        if (descriptionAttribute != null)
          dictionary.Add (value, new EnumValue (value, descriptionAttribute.Description));
        else
          dictionary.Add (value, new EnumValue (value, value.ToString()));
      }
      return dictionary;
    }

    private static Tuple<FieldInfo, Enum>[] GetEnumData (Type enumType)
    {
      return enumType.GetFields (BindingFlags.Static | BindingFlags.Public).Select (f => Tuple.Create (f, (Enum) f.GetValue (null))).ToArray();
    }
  }
}