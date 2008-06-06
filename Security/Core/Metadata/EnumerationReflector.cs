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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{

  public class EnumerationReflector : IEnumerationReflector
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public EnumerationReflector ()
    {
    }

    // methods and properties

    public Dictionary<Enum, EnumValueInfo> GetValues (Type type, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      if (!type.IsEnum)
        throw new ArgumentException (string.Format ("The type '{0}' is not an enumerated type.", type.FullName), "type");
      ArgumentUtility.CheckNotNull ("cache", cache);

      IList values = Enum.GetValues (type);

      Dictionary<Enum, EnumValueInfo> enumValueInfos = new Dictionary<Enum, EnumValueInfo> ();
      for (int i = 0; i < values.Count; i++)
      {
        Enum value = (Enum) values[i];
        enumValueInfos.Add (value, GetValue (value, cache));
      }

      return enumValueInfos;
    }

    public EnumValueInfo GetValue (Enum value, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNull ("cache", cache);

      EnumValueInfo info = cache.GetEnumValueInfo (value);
      if (info == null)
      {
        string name = value.ToString ();
        info = new EnumValueInfo (TypeUtility.GetPartialAssemblyQualifiedName (value.GetType ()), name, Convert.ToInt32 (value));
        FieldInfo fieldInfo = value.GetType ().GetField (name, BindingFlags.Static | BindingFlags.Public);
        PermanentGuidAttribute attribute = (PermanentGuidAttribute) Attribute.GetCustomAttribute (fieldInfo, typeof (PermanentGuidAttribute), false);
        if (attribute != null)
          info.ID = attribute.Value.ToString ();

        cache.AddEnumValueInfo (value, info);
      }

      return info;
    }
  }

}
