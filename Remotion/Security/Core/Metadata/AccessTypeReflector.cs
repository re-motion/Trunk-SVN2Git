// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{
  public class AccessTypeReflector : IAccessTypeReflector
  {
    // types

    // static members

    // member fields
    private IEnumerationReflector _enumerationReflector;
    private PermissionReflector _permissionReflector = new PermissionReflector();

    // construction and disposing

    public AccessTypeReflector ()
      : this (new EnumerationReflector ())
    {
    }

    public AccessTypeReflector (IEnumerationReflector enumerationReflector)
    {
      ArgumentUtility.CheckNotNull ("enumerationReflector", enumerationReflector);
      _enumerationReflector = enumerationReflector;
    }

    // methods and properties

    public IEnumerationReflector EnumerationTypeReflector
    {
      get { return _enumerationReflector; }
    }

    public List<EnumValueInfo> GetAccessTypesFromAssembly (Assembly assembly, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      ArgumentUtility.CheckNotNull ("cache", cache);

      List<EnumValueInfo> accessTypes = new List<EnumValueInfo> ();
      foreach (Type type in assembly.GetTypes ())
      {
        if (type.IsEnum && Attribute.IsDefined (type, typeof (AccessTypeAttribute), false))
        {
          Dictionary<Enum, EnumValueInfo> values = _enumerationReflector.GetValues (type, cache);
          foreach (KeyValuePair<Enum, EnumValueInfo> entry in values)
          {
            if (!cache.ContainsAccessType (entry.Key))
              cache.AddAccessType (entry.Key, entry.Value);
            accessTypes.Add (entry.Value);
          }
        }
      }

      return accessTypes;
    }

    public List<EnumValueInfo> GetAccessTypesFromType (Type type, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("cache", cache);

      Dictionary<Enum, EnumValueInfo> accessTypes = _enumerationReflector.GetValues (typeof (GeneralAccessTypes), cache);
      foreach (KeyValuePair<Enum, EnumValueInfo> entry in accessTypes)
      {
        if (!cache.ContainsAccessType (entry.Key))
          cache.AddAccessType (entry.Key, entry.Value);
      }

      AddAccessTypes (type, accessTypes, cache);

      return new List<EnumValueInfo> (accessTypes.Values);
    }


    private void AddAccessTypes (Type type, Dictionary<Enum, EnumValueInfo> accessTypes, MetadataCache cache)
    {
      MemberInfo[] instanceMethods = GetInstanceMethods (type);
      MemberInfo[] staticMethods = GetStaticMethods (type);
      MemberInfo[] constructors = GetConstructors (type);

      var memberInfos2 = instanceMethods
                            .Concat (staticMethods)
                            .Concat (constructors)
                            .Select (pi => (IMemberInformation) new MethodInfoAdapter ((MethodInfo) pi));

      AddAccessTypesFromAttribute<DemandMethodPermissionAttribute> (memberInfos2, accessTypes, cache);
      
      var properties = GetProperties (type);
      AddAccessTypesFromAttribute<DemandPropertyReadPermissionAttribute> (properties, accessTypes, cache);
      AddAccessTypesFromAttribute<DemandPropertyWritePermissionAttribute> (properties, accessTypes, cache);
    }

    private MemberInfo[] GetConstructors (Type type)
    {
      MemberInfo[] constructors = type.FindMembers (
          MemberTypes.Constructor,
          BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public,
          FindSecuredMembersFilter,
          new Type[] { typeof (DemandMethodPermissionAttribute) });
      return constructors;
    }

    private MemberInfo[] GetStaticMethods (Type type)
    {
      MemberInfo[] staticMethods = type.FindMembers (
          MemberTypes.Method,
          BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy,
          FindSecuredMembersFilter,
          new Type[] { typeof (DemandMethodPermissionAttribute) });
      return staticMethods;
    }

    private MemberInfo[] GetInstanceMethods (Type type)
    {
      MemberInfo[] instanceMethods = type.FindMembers (
          MemberTypes.Method,
          BindingFlags.Instance | BindingFlags.Public,
          FindSecuredMembersFilter,
          new Type[] { typeof (DemandMethodPermissionAttribute) });
      return instanceMethods;
    }

    private IEnumerable<IMemberInformation> GetProperties (Type type)
    {
      MemberInfo[] instanceProperties = type.FindMembers (
          MemberTypes.Property,
          BindingFlags.Instance | BindingFlags.Public,
          FindSecuredMembersFilter,
          new Type[] { typeof (DemandPropertyReadPermissionAttribute), typeof (DemandPropertyWritePermissionAttribute) });
      return instanceProperties.Cast<PropertyInfo> ().Select (pi => (IMemberInformation) new PropertyInfoAdapter (pi));
    }

    private bool FindSecuredMembersFilter (MemberInfo member, object filterCriteria)
    {
      Type[] requiredPermissionAttributes = (Type[]) filterCriteria;

      foreach (Type requiredPermissionAttribute in requiredPermissionAttributes)
      {
        if (Attribute.IsDefined (member, requiredPermissionAttribute, true))
          return true;
      }

      return false;
    }  

    private void AddAccessTypesFromAttribute<T> (IEnumerable<IMemberInformation> memberInfos, Dictionary<Enum, EnumValueInfo> accessTypes, MetadataCache cache) where T : BaseDemandPermissionAttribute
    {
      foreach (var memberInfo in memberInfos)
      {
        Enum[] values = _permissionReflector.GetPermissions<T> (memberInfo);
        foreach (Enum value in values)
        {
          EnumValueInfo accessType = _enumerationReflector.GetValue (value, cache);

          if (!cache.ContainsAccessType (value))
            cache.AddAccessType (value, accessType);

          if (!accessTypes.ContainsKey (value))
            accessTypes.Add (value, accessType);
        }
      }
    }
  }
}
