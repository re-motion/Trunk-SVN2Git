// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Remotion.Utilities
{
  /// <summary>
  /// Use this attribute to specify a clear text representation of a certain enumeration value.
  /// </summary>
  [AttributeUsage (AttributeTargets.Field, AllowMultiple = false)]
  public class EnumDescriptionAttribute: Attribute
  {
    private string _description;

    public EnumDescriptionAttribute (string description)
	  {
      _description = description;
	  }

    public string Description
    {
      get { return _description; }
    }
  }

  [AttributeUsage (AttributeTargets.Enum, AllowMultiple = false)]
  public class EnumDescriptionResourceAttribute: Attribute
  {
    private string _baseName;

    public EnumDescriptionResourceAttribute (string baseName)
    {
      _baseName = baseName;
    }

    public string BaseName
    {
      get { return _baseName; }
    }
  }

  public struct EnumValue
  {
    public readonly Enum Value;
    public readonly string Description;
    public long NumericValue
    {
      get { return Convert.ToInt64 (Value); }
    }

    public EnumValue (Enum value, string description)
    {
      Value = value;
      Description = description;
    }
  }

  /// <summary>
  /// Use this class to get the clear text representations of enumeration values.
  /// </summary>
  public static class EnumDescription
  {
    /// <summary> IDictionary&lt;Type, IDictionary&lt;System.Enum, string&gt;&gt; </summary>
    /// <remarks> This is for enums with the EnumDescriptionAttribute on values. </remarks>
    private static IDictionary s_typeDescriptions;

    /// <summary> IDictionary&lt;string resourceKey, ResourceManager&gt; </summary>
    /// <remarks> This is for enums with the EnumDescriptionResourceAttribute. </remarks>
    private static IDictionary s_enumResourceManagers;

    static EnumDescription ()
    {
      lock (typeof (EnumDescription))
      {
        s_typeDescriptions = new HybridDictionary ();
        s_enumResourceManagers = new HybridDictionary ();
      }
    }


    public static EnumValue[] GetAllValues (Type enumType)
    {
      return GetAllValues (enumType, null);
    }
    
    public static EnumValue[] GetAllValues (Type enumType, CultureInfo culture)
    {
      EnumDescriptionResourceAttribute[] resourceAttributes = (EnumDescriptionResourceAttribute[]) 
            enumType.GetCustomAttributes (typeof (EnumDescriptionResourceAttribute), false);
      if (resourceAttributes.Length > 0)
      {
        FieldInfo[] fields = enumType.GetFields (BindingFlags.Static | BindingFlags.Public);
        EnumValue[] values = new EnumValue[fields.Length];
        ResourceManager rm = GetResourceManager (resourceAttributes[0].BaseName, enumType.Assembly);
        
        for (int i = 0; i < fields.Length; ++i)
        {
          Enum value = (Enum) fields[i].GetValue(null);
          string description = rm.GetString (enumType.FullName + "." + value.ToString(), culture);
          values[i] = new EnumValue (value, description);
        }
        return values;
      }
      else
      {
        IDictionary descriptions = GetOrCreateDescriptions (enumType);
        EnumValue[] values = new EnumValue[descriptions.Count];
        int i = 0;
        foreach (Enum value in descriptions.Keys)
        {
          values[i] = new EnumValue (value, (string) descriptions[value]);
          ++i;
        }
        return values;
      }
    }

    private static ResourceManager GetResourceManager (string baseName, Assembly assembly)
    {
      string resourceKey = baseName + " in " + assembly.FullName;

      ResourceManager rm = (ResourceManager) s_enumResourceManagers[resourceKey];
      if (rm == null)
      {
        lock (typeof (EnumDescription))
        {
          rm = (ResourceManager) s_enumResourceManagers[resourceKey];
          if (rm == null)
          {
            rm = new ResourceManager (baseName, assembly, null);
            s_enumResourceManagers[resourceKey] = rm;
          }
        }
      }
      return rm;
    }

    public static string GetDescription (Enum value)
    {
      return GetDescription (value, null);
    }

    public static string GetDescription (Enum value, CultureInfo culture)
    {
      Type enumType = value.GetType();
      EnumDescriptionResourceAttribute resourceAttribute = AttributeUtility.GetCustomAttribute<EnumDescriptionResourceAttribute>(enumType, false);
      if (resourceAttribute != null)
      {
        ResourceManager rm = GetResourceManager (resourceAttribute.BaseName, enumType.Assembly);
        return rm.GetString (enumType.FullName + "." + value.ToString(), culture);
      }
      else
      {
        IDictionary descriptions = GetOrCreateDescriptions (enumType);
        string description = (string) descriptions[value];
        if (description != null)
          return description;

        return value.ToString();
      }
    }

    private static IDictionary GetOrCreateDescriptions (Type enumType)
    {
      IDictionary descriptions = (IDictionary) s_typeDescriptions[enumType];
      if (descriptions == null)
      {
        lock (s_typeDescriptions)
        {
          descriptions = (IDictionary) s_typeDescriptions[enumType];
          if (descriptions == null)
          {
            descriptions = CreateDesciptionsDictionary (enumType);
            s_typeDescriptions.Add (enumType, descriptions);
          }
        }
      }
      return descriptions;
    }

    /// <returns>IDictionary&lt;System.Enum, string&gt;</returns>
    private static IDictionary CreateDesciptionsDictionary (Type enumType)
    {
      FieldInfo[] fields = enumType.GetFields (BindingFlags.Static | BindingFlags.Public);
      IDictionary dictionary = new HybridDictionary (fields.Length);
      foreach (FieldInfo field in fields)
      {
        EnumDescriptionAttribute descriptionAttribute = AttributeUtility.GetCustomAttribute<EnumDescriptionAttribute> (field, false);
        if (descriptionAttribute != null)
        {
          Enum value = (Enum) field.GetValue (null);
          dictionary.Add (value, descriptionAttribute.Description);
        }
      }
      return dictionary;
    }
  }
}
