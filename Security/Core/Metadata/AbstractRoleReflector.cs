using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{

  public class AbstractRoleReflector : Remotion.Security.Metadata.IAbstractRoleReflector
  {
    // types

    // static members

    // member fields
    private IEnumerationReflector _enumerationReflector;

    // construction and disposing

    public AbstractRoleReflector ()
      : this (new EnumerationReflector ())
    {
    }

    public AbstractRoleReflector (IEnumerationReflector enumerationReflector)
    {
      ArgumentUtility.CheckNotNull ("enumerationReflector", enumerationReflector);
      _enumerationReflector = enumerationReflector;
    }

    // methods and properties

    public IEnumerationReflector EnumerationTypeReflector
    {
      get { return _enumerationReflector; }
    }

    public List<EnumValueInfo> GetAbstractRoles (Assembly assembly, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      ArgumentUtility.CheckNotNull ("cache", cache);

      List<EnumValueInfo> abstractRoles = new List<EnumValueInfo> ();
      foreach (Type type in assembly.GetTypes ())
      {
        if (type.IsEnum && Attribute.IsDefined (type, typeof (AbstractRoleAttribute), false))
        {
          Dictionary<Enum, EnumValueInfo> values = _enumerationReflector.GetValues (type, cache);
          foreach (KeyValuePair<Enum, EnumValueInfo> entry in values)
          {
            if (!cache.ContainsAbstractRole (entry.Key))
              cache.AddAbstractRole (entry.Key, entry.Value);
            abstractRoles.Add (entry.Value);
          }
        }
      }

      return abstractRoles;
    }
  }
}