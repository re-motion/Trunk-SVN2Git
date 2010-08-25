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
using System.Reflection;
using Remotion.Collections;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{
  public class ReflectionBasedMemberResolver : IMemberResolver
  {
    private const string c_memberNotFoundMessage = "The member '{0}' could not be found.";
    private const string c_memberPermissionsOnlyInBaseClassMessage =
        "The {2} must not be defined on members overriden or redefined in derived classes. A member '{0}' exists in class '{1}' and its base class.";

    private class CacheKey : IEquatable<CacheKey>
    {
      private readonly Type _type;
      private readonly string _memberName;
      private readonly BindingFlags _bindingFlags;

      public CacheKey (Type type, string memberName, BindingFlags bindingFlags)
      {
        Assertion.DebugAssert (type != null, "Parameter 'type' is null.");
        Assertion.DebugAssert (!string.IsNullOrEmpty (memberName), "Parameter 'memberName' is null or empty.");

        _type = type;
        _memberName = memberName;
        _bindingFlags = bindingFlags;
      }

      public Type Type
      {
        get { return _type; }
      }

      public string MemberName
      {
        get { return _memberName; }
      }

      public BindingFlags BindingFlags
      {
        get { return _bindingFlags; }
      }

      public override int GetHashCode ()
      {
        return _type.GetHashCode () ^ _memberName[0];
      }

      public bool Equals (CacheKey other)
      {
        return EqualityUtility.NotNullAndSameType (this, other)
               && _type.Equals (other._type)
               && string.Equals (_memberName, other._memberName)
               && _bindingFlags == other._bindingFlags;
      }
    }

    private static readonly ICache<CacheKey, IMemberInformation> s_cache = new InterlockedCache<CacheKey, IMemberInformation> ();

    public IMethodInformation GetMethodInformation (Type type, string methodName, MemberAffiliation memberAffiliation)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);
      
      switch (memberAffiliation)
      {
        case MemberAffiliation.Instance:
          return (IMethodInformation) GetMemberFromCache (type, methodName, BindingFlags.Public | BindingFlags.Instance, MemberTypes.Method);
        case MemberAffiliation.Static:
          return (IMethodInformation) GetMemberFromCache (type, methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, MemberTypes.Method);
        default:
          throw new ArgumentException ("Wrong parameter 'memberAffiliation' passed.");
      }
    }

    public IMethodInformation GetMethodInformation (Type type, MethodInfo methodInfo, MemberAffiliation memberAffiliation)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);
      
      return GetMethodInformation (type, methodInfo.Name, memberAffiliation);
    }

    public IPropertyInformation GetPropertyInformation (Type type, string propertyName)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      return (IPropertyInformation) GetMemberFromCache (
          type, propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, MemberTypes.Property);
    }

    public IPropertyInformation GetPropertyInformation (Type type, PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      return (IPropertyInformation) GetMemberFromCache (
          type, propertyInfo.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, MemberTypes.Property);
    }

    private IMemberInformation GetMemberFromCache (Type type, string memberName, BindingFlags bindingFlags, MemberTypes memberType)
    {
      var cacheKey = new CacheKey (type, memberName, bindingFlags);
      return s_cache.GetOrCreateValue (cacheKey, key => GetMember (key.Type, key.MemberName, key.BindingFlags, memberType));
    }

    private IMemberInformation GetMember (Type type, string memberName, BindingFlags bindingFlags, MemberTypes memberType)
    {
      string attributeName = typeof (DemandPermissionAttribute).Name;

      if (!TypeHasMember (type, memberType, memberName, bindingFlags))
        throw new ArgumentException (string.Format (c_memberNotFoundMessage, memberName), "memberName");

      var foundMembers = new List<MemberInfo> ();
      for (Type currentType = type; currentType != null; currentType = currentType.BaseType)
        foundMembers.AddRange (currentType.FindMembers (memberType, bindingFlags | BindingFlags.DeclaredOnly, IsSecuredMember, memberName));

      if (foundMembers.Count == 0)
      {
        if (memberType == MemberTypes.Method)
          return new NullMethodInformation();
        return new NullPropertyInformation();
      }

      MemberInfo foundMember = foundMembers[0];
      if (type.BaseType != null && foundMember.DeclaringType == type && TypeHasMember (type.BaseType, memberType, memberName, bindingFlags))
      {
        throw new ArgumentException (
            string.Format (c_memberPermissionsOnlyInBaseClassMessage, memberName, type.FullName, attributeName), "memberName");
      }

      if (memberType == MemberTypes.Method)
        return new MethodInfoAdapter ((MethodInfo) foundMember);

      return new PropertyInfoAdapter ((PropertyInfo) foundMember);
    }

    private bool TypeHasMember (Type type, MemberTypes memberType, string methodName, BindingFlags bindingFlags)
    {
      MemberInfo[] existingMembers = type.GetMember (methodName, memberType, bindingFlags);
      return existingMembers.Length > 0;
    }

    private bool IsSecuredMember (MemberInfo member, object filterCriteria)
    {
      string memberName = (string) filterCriteria;
      return member.Name == memberName && member.IsDefined (typeof (DemandPermissionAttribute), false);
    }

    public bool IsNull
    {
      get { return false; }
    }
  }
}