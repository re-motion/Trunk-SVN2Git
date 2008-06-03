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
using System.Reflection;
using Remotion.Utilities;
using Remotion.Collections;

namespace Remotion.Mixins.Utilities
{
  public static class ReflectionUtility
  {
    public static bool IsEqualOrInstantiationOf (Type typeToCheck, Type expectedType)
    {
      ArgumentUtility.CheckNotNull ("typeToCheck", typeToCheck);
      ArgumentUtility.CheckNotNull ("expectedType", expectedType);

      return typeToCheck.Equals (expectedType) || (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition().Equals (expectedType));
    }

    public static bool IsPublicOrProtected (MethodBase methodToCheck)
    {
      ArgumentUtility.CheckNotNull ("methodToCheck", methodToCheck);
      return methodToCheck.IsPublic || methodToCheck.IsFamily || methodToCheck.IsFamilyOrAssembly;
    }

    public static bool IsPublicOrProtectedOrExplicit (MethodBase methodToCheck)
    {
      ArgumentUtility.CheckNotNull ("methodToCheck", methodToCheck);
      return IsPublicOrProtected (methodToCheck) || (methodToCheck.IsPrivate && methodToCheck.IsVirtual);
    }

    public static bool IsNewSlotMember (MemberInfo member)
    {
      ArgumentUtility.CheckNotNull ("member", member);
      return CheckMethodAttributeOnMember (member, MethodAttributes.NewSlot);
    }

    public static bool IsVirtualMember (MemberInfo member)
    {
      ArgumentUtility.CheckNotNull ("member", member);
      return CheckMethodAttributeOnMember (member, MethodAttributes.Virtual);
    }

    private static bool CheckMethodAttributeOnMember (MemberInfo member, MethodAttributes attribute)
    {
      MethodInfo method = member as MethodInfo;
      if (method != null)
        return (method.Attributes & attribute) == attribute;

      PropertyInfo property = member as PropertyInfo;
      if (property != null)
      {
        MethodInfo getMethod = property.GetGetMethod (true);
        MethodInfo setMethod = property.GetSetMethod (true);
        return (getMethod != null && CheckMethodAttributeOnMember (getMethod, attribute))
            || (setMethod != null && CheckMethodAttributeOnMember (setMethod, attribute));
      }

      EventInfo eventInfo = member as EventInfo;
      if (eventInfo != null)
        return CheckMethodAttributeOnMember(eventInfo.GetAddMethod (), attribute)
            || CheckMethodAttributeOnMember(eventInfo.GetRemoveMethod (), attribute);

      string message = string.Format (
          "The given member {0}.{1} is neither property, method, nor event.",
          member.DeclaringType.FullName,
          member.Name);
      throw new ArgumentException (message, "member");
    }

    public static IEnumerable<MethodInfo> RecursiveGetAllMethods (Type type, BindingFlags bindingFlags)
    {
      foreach (MethodInfo method in type.GetMethods(bindingFlags | BindingFlags.DeclaredOnly))
        yield return method;

      if (type.BaseType != null)
      {
        foreach (MethodInfo method in RecursiveGetAllMethods (type.BaseType, bindingFlags))
          yield return method;
      }
    }

    public static IEnumerable<PropertyInfo> RecursiveGetAllProperties (Type type, BindingFlags bindingFlags)
    {
      foreach (PropertyInfo property in type.GetProperties (bindingFlags | BindingFlags.DeclaredOnly))
        yield return property;

      if (type.BaseType != null)
      {
        foreach (PropertyInfo property in RecursiveGetAllProperties (type.BaseType, bindingFlags))
          yield return property;
      }
    }

    public static IEnumerable<EventInfo> RecursiveGetAllEvents (Type type, BindingFlags bindingFlags)
    {
      foreach (EventInfo eventInfo in type.GetEvents (bindingFlags | BindingFlags.DeclaredOnly))
        yield return eventInfo;

      if (type.BaseType != null)
      {
        foreach (EventInfo eventInfo in RecursiveGetAllEvents (type.BaseType, bindingFlags))
          yield return eventInfo;
      }
    }

    public static Type[] GetMethodParameterTypes(MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);

      ParameterInfo[] parameters = method.GetParameters();
      Type[] parameterTypes = new Type[parameters.Length];
      for (int i = 0; i < parameterTypes.Length; ++i)
        parameterTypes[i] = parameters[i].ParameterType;
      
      return parameterTypes;
    }

    public static Tuple<Type, Type[]> GetMethodSignature (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);
      
      Type[] parameterTypes = GetMethodParameterTypes (method);
      return new Tuple<Type, Type[]> (method.ReturnType, parameterTypes);
    }

    public static bool IsGenericParameterAssociatedWithAttribute (Type genericParameter, Type attributeType)
    {
      ArgumentUtility.CheckNotNull ("genericParameter", genericParameter);
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      if (genericParameter.IsDefined(attributeType, false))
        return true;
      
      Type declaringType = genericParameter.DeclaringType;
      Type baseType = declaringType.BaseType;

      if (!baseType.IsGenericType)
        return false;

      Type baseTypeDefinition = baseType.GetGenericTypeDefinition();
      Type[] baseTypeGenericParameters = baseTypeDefinition.GetGenericArguments();

      Type[] genericArguments = baseType.GetGenericArguments();
      for (int i = 0; i < genericArguments.Length; i++)
      {
        Type baseGenericArgument = genericArguments[i];
        if (baseGenericArgument.Equals (genericParameter))
          return baseTypeGenericParameters[i].IsDefined (attributeType, false);
      }
      return false;
    }

    public static IEnumerable<Type> GetGenericParametersAssociatedWithAttribute (Type genericTypeDefinition, Type attributeType)
    {
      ArgumentUtility.CheckNotNull ("genericTypeDefinition", genericTypeDefinition);
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      if (!genericTypeDefinition.ContainsGenericParameters)
        throw new ArgumentException ("Argument must contain generic parameters.", "genericTypeDefinition");

      foreach (Type genericArgument in genericTypeDefinition.GetGenericArguments ())
      {
        if (genericArgument.IsGenericParameter && ReflectionUtility.IsGenericParameterAssociatedWithAttribute (genericArgument, attributeType))
          yield return genericArgument;
      }
    }

    private static Type GetUnspecializedType (Type type)
    {
      if (type.IsGenericType)
        return type.GetGenericTypeDefinition ();
      else
        return type;
    }

    // Checks whether two types are equal, ignoring any generic instantiations of either parameter
    public static bool IsSameTypeIgnoreGenerics (Type potentialDerivedType, Type potentialBaseType)
    {
      ArgumentUtility.CheckNotNull ("potentialDerivedType", potentialDerivedType);
      ArgumentUtility.CheckNotNull ("potentialBaseType", potentialBaseType);
      return GetUnspecializedType (potentialDerivedType).Equals (GetUnspecializedType (potentialBaseType));
    }

    public static bool IsAssemblySigned (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      return IsAssemblySigned (assembly.GetName ());
    }

    public static bool IsAssemblySigned (AssemblyName assemblyName)
    {
      ArgumentUtility.CheckNotNull ("assemblyName", assemblyName);
      byte[] publicKeyOrToken = assemblyName.GetPublicKey () ?? assemblyName.GetPublicKeyToken ();
      return publicKeyOrToken != null && publicKeyOrToken.Length > 0;
    }
  }
}
