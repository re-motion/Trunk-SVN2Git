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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using Remotion.Collections;
using Remotion.Configuration;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{
  public class PermissionReflector : ExtendedProviderBase, IPermissionProvider
  {
    // constants

    private const string c_memberNotFoundMessage = "The member '{0}' could not be found.";
    private const string c_memberHasMultipleAttributesMessage = "The member '{0}' has multiple {1} defined.";

    private const string c_memberPermissionsOnlyInBaseClassMessage =
        "The {2} must not be defined on members overriden or redefined in derived classes. A member '{0}' exists in class '{1}' and its base class.";

    // types

    // static members

    private static Cache<Tuple<Type, Type, string, BindingFlags>, Enum[]> s_cache = new Cache<Tuple<Type, Type, string, BindingFlags>, Enum[]>();

    protected static Cache<Tuple<Type, Type, string, BindingFlags>, Enum[]> Cache
    {
      get { return s_cache; }
    }

    // member fields

    // construction and disposing

    public PermissionReflector ()
        : this ("Reflection", new NameValueCollection())
    {
    }

    public PermissionReflector (string name, NameValueCollection config)
        : base (name, config)
    {
    }

    // methods and properties

    public Enum[] GetRequiredMethodPermissions (Type type, string methodName)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      return GetPermissionsFromCache<DemandMethodPermissionAttribute> (type, methodName, BindingFlags.Public | BindingFlags.Instance);
    }

    public Enum[] GetRequiredStaticMethodPermissions (Type type, string methodName)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      return GetPermissionsFromCache<DemandMethodPermissionAttribute> (
          type, methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
    }

    public Enum[] GetRequiredPropertyReadPermissions (Type type, string propertyName)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      return GetPermissionsFromCache<DemandPropertyReadPermissionAttribute> (
          type, propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public Enum[] GetRequiredPropertyWritePermissions (Type type, string propertyName)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      return GetPermissionsFromCache<DemandPropertyWritePermissionAttribute> (
          type, propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }

    public Enum[] GetPermissions<TAttribute> (MemberInfo methodInfo) where TAttribute: BaseDemandPermissionAttribute
    {
      var permissionAttribute = AttributeUtility.GetCustomAttribute<TAttribute>(methodInfo, true);
      if (permissionAttribute == null)
        return new Enum[0];

      var permissions = new List<Enum> ();
      foreach (Enum accessTypeEnum in permissionAttribute.GetAccessTypes())
      {
        if (!permissions.Contains (accessTypeEnum))
          permissions.Add (accessTypeEnum);
      }

      return permissions.ToArray();
    }

    private Enum[] GetPermissionsFromCache<TAttribute> (Type type, string memberName, BindingFlags bindingFlags)
        where TAttribute: BaseDemandPermissionAttribute
    {
      var cacheKey = new Tuple<Type, Type, string, BindingFlags> (typeof (TAttribute), type, memberName, bindingFlags);
      Enum[] cachedPermissions;
      if (!s_cache.TryGetValue (cacheKey, out cachedPermissions))
      {
        Enum[] permissions = GetPermissions<TAttribute> (type, memberName, bindingFlags);
        lock (s_cache)
        {
          if (!s_cache.TryGetValue (cacheKey, out cachedPermissions))
          {
            s_cache.Add (cacheKey, permissions);
            cachedPermissions = permissions;
          }
        }
      }
      return cachedPermissions;
    }

    private Enum[] GetPermissions<TAttribute> (Type type, string memberName, BindingFlags bindingFlags)
        where TAttribute: BaseDemandPermissionAttribute
    {
      MemberTypes memberType = GetApplicableMemberTypesFromAttributeType (typeof (TAttribute));
      string attributeName = typeof (TAttribute).Name;

      if (!TypeHasMember (type, memberType, memberName, bindingFlags))
        throw new ArgumentException (string.Format (c_memberNotFoundMessage, memberName), "memberName");

      var foundMembers = new List<MemberInfo>();
      for (Type currentType = type; currentType != null; currentType = currentType.BaseType)
      {
        foundMembers.AddRange (
            currentType.FindMembers (memberType, bindingFlags | BindingFlags.DeclaredOnly, IsSecuredMember<TAttribute>, memberName));
      }
      if (foundMembers.Count == 0)
        return new Enum[0];

      MemberInfo foundMember = foundMembers[0];
      if (type.BaseType != null && foundMember.DeclaringType == type && TypeHasMember (type.BaseType, memberType, memberName, bindingFlags))
      {
        throw new ArgumentException (
            string.Format (c_memberPermissionsOnlyInBaseClassMessage, memberName, type.FullName, attributeName), "memberName");
      }

      if (foundMembers.Count > 1)
        throw new ArgumentException (string.Format (c_memberHasMultipleAttributesMessage, memberName, attributeName), "memberName");

      return GetPermissions<TAttribute> (foundMember);
    }

    private MemberTypes GetApplicableMemberTypesFromAttributeType (Type attributeType)
    {
      var attributeUsageAttributes = (AttributeUsageAttribute[]) attributeType.GetCustomAttributes (typeof (AttributeUsageAttribute), false);
      AttributeTargets targets = attributeUsageAttributes[0].ValidOn;

      MemberTypes memberTypes = 0;

      if ((targets & AttributeTargets.Method) != 0)
        memberTypes |= MemberTypes.Method;

      if ((targets & AttributeTargets.Property) != 0)
        memberTypes |= MemberTypes.Property;

      return memberTypes;
    }

    private bool TypeHasMember (Type type, MemberTypes memberType, string methodName, BindingFlags bindingFlags)
    {
      MemberInfo[] existingMembers = type.GetMember (methodName, memberType, bindingFlags);
      return existingMembers.Length > 0;
    }

    private bool IsSecuredMember<TAttribute> (MemberInfo member, object filterCriteria) where TAttribute: BaseDemandPermissionAttribute
    {
      string memberName = (string) filterCriteria;
      return member.Name == memberName && member.IsDefined (typeof (TAttribute), false);
    }
  }
}