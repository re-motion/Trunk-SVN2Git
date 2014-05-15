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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Collections;

namespace Remotion.Utilities
{
  public static class ReflectionUtility
  {
    public delegate bool CompareValues (object propertyOrFieldValue, object compareToValue);

    private static readonly LockingCacheDecorator<Tuple<Type, Type>, bool> s_canAscribeCache = CacheFactory.CreateWithLocking<Tuple<Type, Type>, bool>();
    private static readonly LockingCacheDecorator<Tuple<Type, Type>, bool> s_canAscribeInternalCache =
        CacheFactory.CreateWithLocking<Tuple<Type, Type>, bool>();
    private static readonly LockingCacheDecorator<Tuple<Type, Type>, Type[]> s_ascribedGenericArgumentsCache =
        CacheFactory.CreateWithLocking<Tuple<Type, Type>, Type[]>();
    private static readonly LockingCacheDecorator<Tuple<Type, Type, Type>, bool> s_canDirectlyAscribeToGenericTypeInternalCache =
        CacheFactory.CreateWithLocking<Tuple<Type, Type, Type>, bool>();

    [Obsolete ("Replace with AttributeUtilities.GetCustomAttriubte")]
    public static object GetSingleAttribute (MemberInfo member, Type attributeType, bool inherit, bool throwExceptionIfNotPresent)
    {
      object[] attributes = member.GetCustomAttributes (attributeType, inherit);
      if (attributes.Length > 1)
        throw new InvalidOperationException (String.Format ("More that one attribute of type {0} found for {1} {2}. Only single attributes are supported by this method.", attributeType.FullName, member.MemberType, member));
      if (attributes.Length == 0)
      {
        if (throwExceptionIfNotPresent)
          throw new ApplicationException (String.Format ("{0} {1} does not have attribute {2}.", member.MemberType, member, attributeType.FullName));
        else
          return null;
      }
      return attributes[0];
    }

    public static object GetAttributeArrayMemberValue (
        MemberInfo reflectionObject,
        Type attributeType,
        bool inherit,
        MemberInfo fieldOrProperty,
        MemberInfo comparePropertyOrField,
        object compareToValue,
        CompareValues comparer)
    {
      object[] attributes = reflectionObject.GetCustomAttributes (attributeType, inherit);
      if (attributes == null || attributes.Length == 0)
        return null;
      foreach (Attribute attribute in attributes)
      {
        if (comparer (GetFieldOrPropertyValue (attribute, comparePropertyOrField), compareToValue))
          return GetFieldOrPropertyValue (attribute, fieldOrProperty);
      }
      return null;
    }


    public static object GetAttributeMemberValue (MemberInfo reflectionObject, Type attributeType, bool inherit, MemberInfo fieldOrProperty)
    {
      object[] attributes = reflectionObject.GetCustomAttributes (attributeType, inherit);
      if (attributes == null || attributes.Length == 0)
        return null;
      if (attributes.Length > 1)
        throw new NotSupportedException (String.Format ("Cannot get member value for multiple attributes. Reflection object {0} has {1} instances of attribute {2}", reflectionObject.Name, attributes.Length, attributeType.FullName));
      return GetFieldOrPropertyValue (attributes[0], fieldOrProperty);
    }

    public static MemberInfo GetFieldOrProperty (Type type, string fieldOrPropertyName, BindingFlags bindingFlags, bool throwExceptionIfNotFound)
    {
      MemberInfo member = type.GetField (fieldOrPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      if (member != null)
        return member;

      member = type.GetProperty (fieldOrPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      if (member != null)
        return member;

      if (throwExceptionIfNotFound)
        throw new ArgumentException (String.Format ("{0} is not an instance field or property of type {1}.", fieldOrPropertyName, type.FullName), "fieldOrPropertyName");
      return null;
    }


    public static object GetFieldOrPropertyValue (object obj, string fieldOrPropertyName)
    {
      return GetFieldOrPropertyValue (obj, fieldOrPropertyName, BindingFlags.Public);
    }

    public static object GetFieldOrPropertyValue (object obj, string fieldOrPropertyName, BindingFlags bindingFlags)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);
      MemberInfo fieldOrProperty = GetFieldOrProperty (obj.GetType (), fieldOrPropertyName, bindingFlags, true);
      return GetFieldOrPropertyValue (obj, fieldOrProperty);
    }

    public static object GetFieldOrPropertyValue (object obj, MemberInfo fieldOrProperty)
    {
      if (obj == null)
        throw new ArgumentNullException ("obj");
      if (fieldOrProperty == null)
        throw new ArgumentNullException ("fieldOrProperty");

      if (fieldOrProperty is FieldInfo)
        return ((FieldInfo) fieldOrProperty).GetValue (obj);
      else if (fieldOrProperty is PropertyInfo)
        return ((PropertyInfo) fieldOrProperty).GetValue (obj, new object[0]);
      else
        throw new ArgumentException (String.Format ("Argument must be either FieldInfo or PropertyInfo but is {0}.", fieldOrProperty.GetType ().FullName), "fieldOrProperty");
    }


    public static void SetFieldOrPropertyValue (object obj, string fieldOrPropertyName, object value)
    {
      SetFieldOrPropertyValue (obj, fieldOrPropertyName, BindingFlags.Public, value);
    }

    public static void SetFieldOrPropertyValue (object obj, string fieldOrPropertyName, BindingFlags bindingFlags, object value)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);
      MemberInfo fieldOrProperty = GetFieldOrProperty (obj.GetType (), fieldOrPropertyName, bindingFlags, true);
      SetFieldOrPropertyValue (obj, fieldOrProperty, value);
    }

    public static void SetFieldOrPropertyValue (object obj, MemberInfo fieldOrProperty, object value)
    {
      if (obj == null)
        throw new ArgumentNullException ("obj");
      if (fieldOrProperty == null)
        throw new ArgumentNullException ("fieldOrProperty");

      if (fieldOrProperty is FieldInfo)
        ((FieldInfo) fieldOrProperty).SetValue (obj, value);
      else if (fieldOrProperty is PropertyInfo)
        ((PropertyInfo) fieldOrProperty).SetValue (obj, value, new object[0]);
      else
        throw new ArgumentException (String.Format ("Argument must be either FieldInfo or PropertyInfo but is {0}.", fieldOrProperty.GetType ().FullName), "fieldOrProperty");
    }

    public static Type GetFieldOrPropertyType (MemberInfo fieldOrProperty)
    {
      if (fieldOrProperty is FieldInfo)
        return ((FieldInfo) fieldOrProperty).FieldType;
      else if (fieldOrProperty is PropertyInfo)
        return ((PropertyInfo) fieldOrProperty).PropertyType;
      else
        throw new ArgumentException ("Argument must be FieldInfo or PropertyInfo.", "fieldOrProperty");
    }

    /// <summary>
    /// <see cref="PropertyInfo"/> object for the property on the direct or indirect base class in which the property represented by this instance was first declared.
    /// </summary>
    /// <returns>A <see cref="PropertyInfo"/> object for the first implementation of this method.</returns>
    public static PropertyInfo GetBaseDefinition (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      MethodInfo[] accessors = propertyInfo.GetAccessors (true);
      if (accessors.Length == 0)
      {
        throw new ArgumentException (
            String.Format ("The property does not define any accessors.\r\n  Type: {0}, property: {1}", propertyInfo.DeclaringType, propertyInfo.Name),
            "propertyInfo");
      }

      var originalDeclaringType = GetOriginalDeclaringType (propertyInfo);
      if (propertyInfo.DeclaringType == originalDeclaringType)
        return propertyInfo;

      var baseDefinition = originalDeclaringType.GetProperty (
          propertyInfo.Name,
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly,
          null,
          propertyInfo.PropertyType,
          propertyInfo.GetIndexParameters().Select (pi => pi.ParameterType).ToArray(), null);

      if (baseDefinition == null)
      {
        throw new MissingMemberException (
            string.Format (
                "The property '{0}' declared on derived type '{1}' could not be resolved for base type '{2}'.",
                propertyInfo.Name,
                propertyInfo.DeclaringType.FullName,
                originalDeclaringType.FullName));
      }

      return baseDefinition;
    }

    /// <summary>
    /// Returns the <see cref="Type"/> where the property was initially decelared.
    /// </summary>
    /// <param name="propertyInfo">The property whose identifier should be returned. Must not be <see langword="null" />.</param>
    /// <returns>The <see cref="Type"/> where the property was declared for the first time.</returns>
    public static Type GetOriginalDeclaringType (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      MethodInfo[] accessors = propertyInfo.GetAccessors (true);
      if (accessors.Length == 0)
      {
        throw new ArgumentException (
            String.Format ("The property does not define any accessors.\r\n  Type: {0}, property: {1}", propertyInfo.DeclaringType, propertyInfo.Name),
            "propertyInfo");
      }

      return GetOriginalDeclaringType (accessors[0]);
    }

    /// <summary>
    /// Returns the <see cref="Type"/> where the method was initially declared.
    /// </summary>
    /// <param name="methodInfo">The method whose type should be returned. Must not be <see langword="null" />.</param>
    /// <returns>The <see cref="Type"/> where the method was declared for the first time.</returns>
    public static Type GetOriginalDeclaringType (MethodInfo methodInfo)
    {
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);
      return methodInfo.GetBaseDefinition ().DeclaringType;
    }

    
    /// <summary>
    /// Determines whether the given <see cref="PropertyInfo"/> is the original base declaration.
    /// </summary>
    /// <param name="propertyInfo">The property info to check.</param>
    /// <returns>
    /// 	<see langword="true"/> if the <paramref name="propertyInfo"/> is the first declaration of the property; <see langword="false"/> if it is an 
    /// 	overrride.
    /// </returns>
    public static bool IsOriginalDeclaration (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

      Type originalDeclaringType = GetOriginalDeclaringType (propertyInfo);
      return propertyInfo.DeclaringType == originalDeclaringType;
    }


    /// <summary>
    /// Guesses whether the given property is an explicit interface implementation by checking whether it has got private virtual final accessors.
    /// This can be used as a heuristic to find explicit interface properties without having to check InterfaceMaps for every interface on
    /// info.DeclaringType. With C# and VB.NET, the heuristic should always be right.
    /// </summary>
    /// <param name="info">The property to check.</param>
    /// <returns>True, if the property is very likely an explicit interface implementation (at least in C# and VB.NET code); otherwise, false.</returns>
    public static bool GuessIsExplicitInterfaceProperty (PropertyInfo info)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      return info.GetAccessors (true).Any (accessor => accessor.IsPrivate && accessor.IsVirtual && accessor.IsFinal);
    }


    /// <summary>
    /// Evaluates whether the <paramref name="type"/> can be ascribed to the <paramref name="ascribeeType"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to check. Must not be <see langword="null" />.</param>
    /// <param name="ascribeeType">The <see cref="Type"/> to check the <paramref name="type"/> against. Must not be <see langword="null" />.</param>
    /// <returns>
    /// <see langword="true"/> if the <paramref name="type"/> is the <paramref name="ascribeeType"/> or its instantiation, 
    /// its subclass or the implementation of an interface in case the <paramref name="ascribeeType"/> is an interface.
    /// </returns>
    public static bool CanAscribe (Type type, Type ascribeeType)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("ascribeeType", ascribeeType);

      return s_canAscribeCache.GetOrCreateValue (
          Tuple.Create (type, ascribeeType),
          key =>
          {
            if (key.Item2.IsInterface)
            {
              if (key.Item1.IsInterface && CanAscribeInternalFromCache (key.Item1, key.Item2))
                return true;

              return Array.Exists (key.Item1.GetInterfaces(), current => CanAscribeInternalFromCache (current, key.Item2));
            }
            else
              return CanAscribeInternalFromCache(type, ascribeeType);
          });
    }

    /// <summary>
    /// Returns the type arguments for the ascribed <paramref name="ascribeeType"/> as inherited or implemented by a given <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> for which to return the type parameter. Must not be <see langword="null" />.</param>
    /// <param name="ascribeeType">The <see cref="Type"/> to check the <paramref name="type"/> against. Must not be <see langword="null" />.</param>
    /// <returns>A <see cref="Type"/> array containing the generic arguments of the <paramref name="ascribeeType"/> as it is inherited or implemented
    /// by <paramref name="type"/>.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the <paramref name="type"/> is not the <paramref name="ascribeeType"/> or its instantiation, its subclass or the implementation
    /// of an interface in case the <paramref name="ascribeeType"/> is an interface.
    /// </exception>
    /// <exception cref="AmbiguousMatchException">
    /// Thrown if the <paramref name="type"/> is an interface and implements the interface <paramref name="ascribeeType"/> or its instantiations
    /// more than once.
    /// </exception>
    public static Type[] GetAscribedGenericArguments (Type type, Type ascribeeType)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("ascribeeType", ascribeeType);

      return s_ascribedGenericArgumentsCache.GetOrCreateValue (
          Tuple.Create (type, ascribeeType), key => GetAscribedGenericArgumentsWithoutCache (key.Item1, key.Item2));
    }

    private static bool CanAscribeInternalFromCache (Type type, Type ascribeeType)
    {
      return s_canAscribeInternalCache.GetOrCreateValue (Tuple.Create (type, ascribeeType), key => CanAscribeInternal (key.Item1, key.Item2));
    }

    private static bool CanAscribeInternal (Type type, Type ascribeeType)
    {
      if (!ascribeeType.IsGenericType)
        return ascribeeType.IsAssignableFrom (type);

      Type ascribeeGenericTypeDefinition = ascribeeType.GetGenericTypeDefinition();
      for (Type currentType = type; currentType != null; currentType = currentType.BaseType)
      {
        if (CanDirectlyAscribeToGenericTypeInternalFromCache (currentType, ascribeeType, ascribeeGenericTypeDefinition))
          return true;
      }
      return false;
    }

    private static Type[] GetAscribedGenericArgumentsWithoutCache (Type type, Type ascribeeType)
    {
      if (!ascribeeType.IsGenericType)
      {
        if (ascribeeType.IsAssignableFrom (type))
          return Type.EmptyTypes;
        else
          throw ArgumentUtility.CreateArgumentTypeException ("type", type, ascribeeType);
      }
      else if (ascribeeType.IsInterface)
        return GetAscribedGenericInterfaceArgumentsInternal (type, ascribeeType);
      else
        return GetAscribedGenericClassArgumentsInternal (type, ascribeeType);
    }

    private static Type[] GetAscribedGenericInterfaceArgumentsInternal (Type type, Type ascribeeType)
    {
      Assertion.IsTrue (ascribeeType.IsGenericType);
      Assertion.IsTrue (ascribeeType.IsInterface);

      Type ascribeeGenericTypeDefinition = ascribeeType.GetGenericTypeDefinition ();

      Type conreteSpecialization; // concrete specialization of ascribeeType implemented by type
      // is type itself a specialization of ascribeeType?
      if (type.IsInterface && CanDirectlyAscribeToGenericTypeInternalFromCache (type, ascribeeType, ascribeeGenericTypeDefinition))
        conreteSpecialization = type;
      else
      {
        // Type.GetInterfaces will return all interfaces inherited by type. We will filter it to those that are directly ascribable
        // to ascribeeType. Since interfaces have no base types, these can only be closed or constructed specializations of ascribeeType.
        Type[] ascribableInterfaceTypes = Array.FindAll (
            type.GetInterfaces (),
            current => CanDirectlyAscribeToGenericTypeInternalFromCache (current, ascribeeType, ascribeeGenericTypeDefinition));

        if (ascribableInterfaceTypes.Length == 0)
          conreteSpecialization = null;
        else if (ascribableInterfaceTypes.Length == 1)
          conreteSpecialization = ascribableInterfaceTypes[0];
        else
        {
          string message = 
              String.Format ("The type {0} implements the given interface type {1} more than once.", type.FullName, ascribeeType.FullName);
          throw new AmbiguousMatchException (message);
        }
      }

      if (conreteSpecialization == null)
        throw ArgumentUtility.CreateArgumentTypeException ("type", type, ascribeeType);

      Assertion.IsTrue (conreteSpecialization.GetGenericTypeDefinition () == ascribeeType.GetGenericTypeDefinition ());
      return conreteSpecialization.GetGenericArguments ();
    }

    private static Type[] GetAscribedGenericClassArgumentsInternal (Type type, Type ascribeeType)
    {
      Assertion.IsTrue (ascribeeType.IsGenericType);
      Assertion.IsTrue (!ascribeeType.IsInterface);

      Type ascribeeGenericTypeDefinition = ascribeeType.GetGenericTypeDefinition ();

      // Search via base type until we find a type that is directly ascribable to the base type. That's the type whose generic arguments we want
      Type currentType = type;
      while (currentType != null && !CanDirectlyAscribeToGenericTypeInternalFromCache (currentType, ascribeeType, ascribeeGenericTypeDefinition))
        currentType = currentType.BaseType;

      if (currentType != null)
        return currentType.GetGenericArguments ();
      else
        throw ArgumentUtility.CreateArgumentTypeException ("type", type, ascribeeType);
    }

    private static bool CanDirectlyAscribeToGenericTypeInternalFromCache (Type type, Type ascribeeType, Type ascribeeGenericTypeDefinition)
    {
      return s_canDirectlyAscribeToGenericTypeInternalCache.GetOrCreateValue (
        Tuple.Create (type, ascribeeType, ascribeeGenericTypeDefinition),
        key => CanDirectlyAscribeToGenericTypeInternal (key.Item1, key.Item2, key.Item3));
    }

    private static bool CanDirectlyAscribeToGenericTypeInternal (Type type, Type ascribeeType, Type ascribeeGenericTypeDefinition)
    {
      Assertion.IsNotNull (ascribeeType);

      if (!type.IsGenericType || type.GetGenericTypeDefinition () != ascribeeGenericTypeDefinition)
        return false;

      if (ascribeeType != ascribeeGenericTypeDefinition)
        return ascribeeType.IsAssignableFrom (type);
      else
        return ascribeeType.IsAssignableFrom (type.GetGenericTypeDefinition ());
    }

    [Obsolete ("This method has been replaced by MemberInfoFromExpressionUtility.GetMember. (1.13.148)", true)]
    public static MemberInfo GetMemberFromExpression<TSourceObject, TResult> (Expression<Func<TSourceObject, TResult>> memberAccessExpression)
    {
      throw new NotImplementedException ("Obsolete. Use MemberInfoFromExpressionUtility.GetMember instead.");
    }
  }
}
