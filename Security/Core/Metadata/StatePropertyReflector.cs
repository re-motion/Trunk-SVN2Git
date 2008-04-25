using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{

  public class StatePropertyReflector : IStatePropertyReflector
  {
    // types

    // static members

    // member fields

    private IEnumerationReflector _enumerationReflector;

    // construction and disposing

    public StatePropertyReflector () : this (new EnumerationReflector())
    {
    }

    public StatePropertyReflector (IEnumerationReflector enumerationReflector)
    {
      ArgumentUtility.CheckNotNull ("enumerationReflector", enumerationReflector);
      _enumerationReflector = enumerationReflector;
    }

    // methods and properties

    public IEnumerationReflector EnumerationTypeReflector
    {
      get { return _enumerationReflector; }
    }

    public StatePropertyInfo GetMetadata (PropertyInfo property, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      if (!property.PropertyType.IsEnum)
      {
        throw new ArgumentException (
            string.Format ("The type of the property '{0}' in type '{1}' is not an enumerated type.", property.Name, property.DeclaringType.FullName),
            "property");
      }

      if (!Attribute.IsDefined (property.PropertyType, typeof (SecurityStateAttribute), false))
      {
        throw new ArgumentException (string.Format ("The type of the property '{0}' in type '{1}' does not have the {2} applied.", 
                property.Name, property.DeclaringType.FullName, typeof (SecurityStateAttribute).FullName),
            "property");
      }

      ArgumentUtility.CheckNotNull ("cache", cache);

      StatePropertyInfo info = cache.GetStatePropertyInfo (property);
      if (info == null)
      {
        info = new StatePropertyInfo ();
        info.Name = property.Name;
        PermanentGuidAttribute attribute = (PermanentGuidAttribute) Attribute.GetCustomAttribute (property, typeof (PermanentGuidAttribute), true);
        if (attribute != null)
          info.ID = attribute.Value.ToString ();
        info.Values = new List<EnumValueInfo> (_enumerationReflector.GetValues (property.PropertyType, cache).Values);

        cache.AddStatePropertyInfo (property, info);
      }
      return info;
    }
  }
}