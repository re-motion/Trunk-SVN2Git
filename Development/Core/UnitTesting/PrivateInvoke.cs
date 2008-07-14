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
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting
{
  /// <summary>
  /// Provides utility functions for accessing non-public types and members.
  /// </summary>
  public sealed class PrivateInvoke
  {
    // static members

    private static MethodInfo GetMethod (Type type, string methodName, BindingFlags bindingFlags, object[] arguments)
    {
      Debug.Assert (methodName != null);
      return (MethodInfo) GetMethodBaseInternal (type, methodName, type.GetMethods (bindingFlags), arguments);
    }

    private static ConstructorInfo GetConstructor (Type type, BindingFlags bindingFlags, object[] arguments)
    {
      return (ConstructorInfo) GetMethodBaseInternal (type, null, type.GetConstructors (bindingFlags), arguments);
    }

    private static MethodBase GetMethodBaseInternal (Type type, string methodName, MethodBase[] methods, object[] arguments)
    {
      MethodBase callMethod = null;

      foreach (MethodBase method in methods)
      {
        if (methodName == null || methodName == method.Name)
        {
          ParameterInfo[] parameters = method.GetParameters ();
          if (parameters.Length == arguments.Length)
          {
            bool isMatch = true;
            for (int i = 0; i < parameters.Length; ++i)
            {
              object argument = arguments[i];
              Type parameterType = parameters[i].ParameterType;

              if (! (    (argument == null && ! parameterType.IsValueType)        // null is a valid argument for any reference type
                      || (argument != null && parameterType.IsAssignableFrom (argument.GetType()))))  
              {
                isMatch = false;
                break;
              }
            }
            if (isMatch)
            {
              if (callMethod != null)
                throw new AmbiguousMethodNameException (methodName, type);
              callMethod = method;
            }
          }
        }
      }
      if (callMethod == null)
        throw new MethodNotFoundException (methodName, type);

      return callMethod;
    }

    private static PropertyInfo GetPropertyRecursive (Type type, BindingFlags bindingFlags, string propertyName)
    {
      for (PropertyInfo property = null; type != null; type = type.BaseType)
      {
        property = type.GetProperty (propertyName, bindingFlags);
        if (property != null)
          return property;
      }
      return null;
    }

    private static FieldInfo GetFieldRecursive (Type type, BindingFlags bindingFlags, string fieldName)
    {
      for (FieldInfo field = null; type != null; type = type.BaseType)
      {
        field = type.GetField (fieldName, bindingFlags);
        if (field != null)
          return field;
      }
      return null;
    }

    #region InvokeMethod methods

    public static object InvokeNonPublicStaticMethod (Type type, string methodName, params object[] arguments)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      return InvokeMethodInternal (null, type, BindingFlags.Static | BindingFlags.NonPublic, methodName, arguments);
    }

    public static object InvokePublicStaticMethod (Type type, string methodName, params object[] arguments)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      return InvokeMethodInternal (null, type, BindingFlags.Static | BindingFlags.Public, methodName, arguments);
    }

    public static object InvokeNonPublicMethod (object target, string methodName, params object[] arguments)
    {
      ArgumentUtility.CheckNotNull ("target", target);

      return InvokeNonPublicMethod (target, target.GetType(), methodName, arguments);
    }

    public static object InvokeNonPublicMethod (object target, Type definingType, string methodName, params object[] arguments)
    {
      ArgumentUtility.CheckNotNull ("target", target);
      ArgumentUtility.CheckNotNull ("definingType", definingType);
      ArgumentUtility.CheckType ("target", target, definingType);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      return InvokeMethodInternal (target, definingType, BindingFlags.Instance | BindingFlags.NonPublic, methodName, arguments);
    }

    public static object InvokePublicMethod (object target, string methodName, params object[] arguments)
    {
      ArgumentUtility.CheckNotNull ("target", target);
      ArgumentUtility.CheckNotNullOrEmpty ("methodName", methodName);

      return InvokeMethodInternal (target, target.GetType(), BindingFlags.Instance | BindingFlags.Public, methodName, arguments);
    }

    private static object InvokeMethodInternal (object instance, Type type, BindingFlags bindingFlags, string methodName, object[] arguments)
    {
      if (arguments == null)
        arguments = new object[] { null };

      MethodInfo callMethod = GetMethod (type, methodName, bindingFlags, arguments);

      try
      {
        return callMethod.Invoke (instance, bindingFlags, null, arguments, CultureInfo.InvariantCulture);
      }
      catch (TargetInvocationException e)
      {
        throw e.InnerException;
      }
    }

    #endregion

    #region CreateInstance methods

    public static object CreateInstancePublicCtor (string assemblyString, string typeName, params object[] arguments)
    {
      return CreateInstancePublicCtor (Assembly.Load (assemblyString), typeName, arguments);
    }

    public static object CreateInstancePublicCtor (Assembly assembly, string typeName, params object[] arguments)
    {
      return CreateInstancePublicCtor (assembly.GetType (typeName, true, false), arguments);
    }

    public static object CreateInstancePublicCtor (string typeName, params object[] arguments)
    {
      return CreateInstancePublicCtor (ContextAwareTypeDiscoveryUtility.GetType (typeName, true), arguments);
    }

    public static object CreateInstancePublicCtor (Type type, params object[] arguments)
    {
      return CreateInstanceInternal (type, true, arguments);
    }

    public static object CreateInstanceNonPublicCtor (string assemblyString, string typeName, params object[] arguments)
    {
      return CreateInstanceNonPublicCtor (Assembly.Load (assemblyString), typeName, arguments);
    }

    public static object CreateInstanceNonPublicCtor (Assembly assembly, string typeName, params object[] arguments)
    {
      return CreateInstanceNonPublicCtor (assembly.GetType (typeName, true, false), arguments);
    }

    public static object CreateInstanceNonPublicCtor (string typeName, params object[] arguments)
    {
      return CreateInstanceNonPublicCtor (ContextAwareTypeDiscoveryUtility.GetType (typeName, true), arguments);
    }

    public static object CreateInstanceNonPublicCtor (Type type, params object[] arguments)
    {
      return CreateInstanceInternal (type, false, arguments);
    }

    private static object CreateInstanceInternal (Type type, bool isPublic, object[] arguments)
    {
      if (arguments == null)
        arguments = new object[] { null };

      BindingFlags bindingFlags = BindingFlags.Instance;
      bindingFlags |= isPublic ? BindingFlags.Public : BindingFlags.NonPublic;

      ConstructorInfo ctor = GetConstructor (type, bindingFlags, arguments);

      try
      {
        return ctor.Invoke (BindingFlags.CreateInstance, null, arguments, CultureInfo.InvariantCulture);
      }
      catch (TargetInvocationException e)
      {
        throw e.InnerException;
      }
    }

    #endregion

    #region GetProperty methods

    public static object GetPublicProperty (object target, string propertyName)
    {
      if (target == null) throw new ArgumentNullException ("target");
      return GetPropertyInternal (target, target.GetType(), BindingFlags.Instance | BindingFlags.Public, propertyName);
    }

    public static object GetNonPublicProperty (object target, string propertyName)
    {
      if (target == null) throw new ArgumentNullException ("target");
      return GetPropertyInternal (target, target.GetType(), BindingFlags.Instance | BindingFlags.NonPublic, propertyName);
    }

    public static object GetPublicStaticProperty (Type type, string propertyName)
    {
      if (type == null) throw new ArgumentNullException ("type");
      return GetPropertyInternal (null, type, BindingFlags.Static | BindingFlags.Public, propertyName);
    }

    public static object GetNonPublicStaticProperty (Type type, string propertyName)
    {
      if (type == null) throw new ArgumentNullException ("type");
      return GetPropertyInternal (null, type, BindingFlags.Static | BindingFlags.NonPublic, propertyName);
    }

    private static object GetPropertyInternal (object instance, Type type, BindingFlags bindingFlags, string propertyName)
    {
      PropertyInfo property = GetPropertyRecursive (type, bindingFlags, propertyName);
      try
      {
        return property.GetValue (instance, new object[] {});
      }
      catch (TargetInvocationException e)
      {
        throw e.InnerException;
      }
    }

    #endregion

    #region SetProperty methods

    public static void SetPublicProperty (object target, string propertyName, object value)
    {
      if (target == null) throw new ArgumentNullException ("target");
      SetPropertyInternal (target, target.GetType(), BindingFlags.Instance | BindingFlags.Public, propertyName, value);
    }

    public static void SetNonPublicProperty (object target, string propertyName, object value)
    {
      if (target == null) throw new ArgumentNullException ("target");
      SetPropertyInternal (target, target.GetType(), BindingFlags.Instance | BindingFlags.NonPublic, propertyName, value);
    }

    public static void SetPublicStaticProperty (Type type, string propertyName, object value)
    {
      if (type == null) throw new ArgumentNullException ("type");
      PropertyInfo property = type.GetProperty (propertyName, BindingFlags.Static | BindingFlags.Public);
      SetPropertyInternal (null, type, BindingFlags.Static | BindingFlags.Public, propertyName, value);
    }

    public static void SetNonPublicStaticProperty (Type type, string propertyName, object value)
    {
      if (type == null) throw new ArgumentNullException ("type");
      SetPropertyInternal (null, type, BindingFlags.Static | BindingFlags.NonPublic, propertyName, value);
    }

    private static void SetPropertyInternal (object instance, Type type, BindingFlags bindingFlags, string propertyName, object value)
    {
      PropertyInfo property = GetPropertyRecursive (type, bindingFlags, propertyName);
      try
      {
        property.SetValue (instance, value, new object[] {});
      }
      catch (TargetInvocationException e)
      {
        throw e.InnerException;
      }
    }

    #endregion

    #region GetField methods

    public static object GetPublicField (object target, string fieldName)
    {
      if (target == null) throw new ArgumentNullException ("target");
      return GetFieldInternal (target, target.GetType(), BindingFlags.Instance | BindingFlags.Public, fieldName);
    }

    public static object GetNonPublicField (object target, string fieldName)
    {
      if (target == null) throw new ArgumentNullException ("target");
      return GetFieldInternal (target, target.GetType(), BindingFlags.Instance | BindingFlags.NonPublic, fieldName);
    }

    public static object GetPublicStaticField (Type type, string fieldName)
    {
      if (type == null) throw new ArgumentNullException ("type");
      return GetFieldInternal (null, type, BindingFlags.Static | BindingFlags.Public, fieldName);
    }

    public static object GetNonPublicStaticField (Type type, string fieldName)
    {
      if (type == null) throw new ArgumentNullException ("type");
      return GetFieldInternal (null, type, BindingFlags.Static | BindingFlags.NonPublic, fieldName);
    }

    private static object GetFieldInternal (object instance, Type type, BindingFlags bindingFlags, string fieldName)
    {
      FieldInfo field = GetFieldRecursive (type, bindingFlags, fieldName);
      try
      {
        return field.GetValue (instance);
      }
      catch (TargetInvocationException e)
      {
        throw e.InnerException;
      }
    }

    #endregion

    #region SetField methods

    public static void SetPublicField (object target, string fieldName, object value)
    {
      if (target == null) throw new ArgumentNullException ("target");
      SetFieldInternal (target, target.GetType(), BindingFlags.Instance | BindingFlags.Public, fieldName, value);
    }

    public static void SetNonPublicField (object target, string fieldName, object value)
    {
      if (target == null) throw new ArgumentNullException ("target");
      SetFieldInternal (target, target.GetType(), BindingFlags.Instance | BindingFlags.NonPublic, fieldName, value);
    }

    public static void SetPublicStaticField (Type type, string fieldName, object value)
    {
      if (type == null) throw new ArgumentNullException ("type");
      SetFieldInternal (null, type, BindingFlags.Static | BindingFlags.Public, fieldName, value);
    }

    public static void SetNonPublicStaticField (Type type, string fieldName, object value)
    {
      if (type == null) throw new ArgumentNullException ("type");
      SetFieldInternal (null, type, BindingFlags.Static | BindingFlags.NonPublic, fieldName, value);
    }

    private static void SetFieldInternal (object instance, Type type, BindingFlags bindingFlags, string fieldName, object value)
    {
      FieldInfo field = GetFieldRecursive (type, bindingFlags, fieldName);
      try
      {
        field.SetValue (instance, value);
      }
      catch (TargetInvocationException e)
      {
        throw e.InnerException;
      }
    }

    #endregion

    // construction and disposal

    private PrivateInvoke()
    {
    }
  }



  [Serializable]
  public class AmbiguousMethodNameException: Exception
  {
    private const string c_errorMessage = "Method name \"{0}\" is ambiguous in type {1}.";

    public AmbiguousMethodNameException (string methodName, Type type)
      : this (string.Format (c_errorMessage, methodName, type.FullName))
    {
    }

    public AmbiguousMethodNameException (string message)
      : base (message)
    {
    }

    protected AmbiguousMethodNameException (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
    }
  }

  [Serializable]
  public class MethodNotFoundException: Exception
  {
    private const string c_errorMessage = "There is no method \"{0}\" in type {1} that accepts the specified argument types.";

    public MethodNotFoundException (string methodName, Type type)
      : this (string.Format (c_errorMessage, methodName, type.FullName))
    {
    }

    public MethodNotFoundException (string message)
      : base (message)
    {
    }

    protected MethodNotFoundException (SerializationInfo info, StreamingContext context)
      : base (info, context)
    {
    }
  }
}
