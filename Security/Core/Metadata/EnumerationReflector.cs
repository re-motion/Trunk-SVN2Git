using System;
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

      System.Collections.IList values = Enum.GetValues (type);

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