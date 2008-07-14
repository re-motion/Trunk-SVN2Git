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
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Utilities.Serialization
{
  public static class ReflectionObjectSerializer
  {
    private const BindingFlags c_bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    public static void SerializeType (Type type, string key, SerializationInfo info)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("info", info);

      if (type.IsGenericParameter)
      {
        string message = string.Format ("Type ({0}) must not be a generic parameter.", type.Name);
        throw new ArgumentException (message, "type");
      }

      info.AddValue(key + ".AssemblyQualifiedName", type.AssemblyQualifiedName);
    }

    public static Type DeserializeType (string key, SerializationInfo info)
    {
      string typeName = info.GetString (key + ".AssemblyQualifiedName");
      return ContextAwareTypeDiscoveryUtility.GetType (typeName, true);
    }

    public static void SerializeMethodBase (MethodBase methodOrConstructor, string key, SerializationInfo info)
    {
      SerializeType (methodOrConstructor.DeclaringType, key + ".DeclaringType", info);
      info.AddValue (key + ".MetadataToken", methodOrConstructor.MetadataToken);
      info.AddValue (key + ".IsGenericMethod", methodOrConstructor.IsGenericMethod);

      if (methodOrConstructor.IsGenericMethod)
        SerializeGenericArguments (methodOrConstructor.GetGenericArguments(), key + ".GetGenericArguments()", info);
    }

    public static MethodBase DeserializeMethodBase (string key, SerializationInfo info)
    {
      Type declaringType = DeserializeType (key + ".DeclaringType", info);
      int token = info.GetInt32 (key + ".MetadataToken");
      bool isGenericMethod = info.GetBoolean (key + ".IsGenericMethod");

      if (!isGenericMethod)
        return EnsureMethodIsOnCorrectType(declaringType.Module.ResolveMethod (token), declaringType);
      else
      {
        Type[] genericTypeArguments = null;
        if (declaringType.IsGenericType)
          genericTypeArguments = declaringType.GetGenericArguments();

        MethodBase genericMethodDefinition = declaringType.Module.ResolveMethod (token, genericTypeArguments, null);
        Type[] genericMethodArguments = DeserializeGenericArguments (key + ".GetGenericArguments()", genericMethodDefinition, info);

        MethodBase genericMethodDefinitionOnCorrectType = EnsureMethodIsOnCorrectType(declaringType.Module.ResolveMethod (token,
            genericTypeArguments, genericMethodArguments), declaringType);

        if (genericMethodDefinitionOnCorrectType.IsGenericMethodDefinition)
        {
          MethodInfo castMethod = (MethodInfo) genericMethodDefinitionOnCorrectType;
          return castMethod.MakeGenericMethod (genericMethodArguments);
        }
        else
          return genericMethodDefinitionOnCorrectType;
      }
    }

    // Due to a Reflection bug, Module.ResolveMethod often returns the wrong MethodBase objects when used in conjunction with constructed generic
    // types (see Microsoft Connect bug #278385 - https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=278385).
    // This method checks whether the given MethodBase object is associated with the given Type object and, if not, returns the correct MethodBase.
    // Due to a CLR bug, however, this method has its own caveats: it will sometimes return a generic method definition even if the given MethodBase
    // is a constructed type (see Bug #269853 - https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=269853).
    // Therefore, use as follows:
    // 1 - Use Module.ResolveMethod to resolve a method token to a MethodBase object.
    // 2 - Use EnsureMethodIsOnCorrectType to ensure the MethodBase is on the desired type.
    // 3 - Use MethodInfo.MakeGenericMethod if the method should be a closed constructed method.
    //
    // (Without the bugs, step 1 would be enough.)
    private static MethodBase EnsureMethodIsOnCorrectType (MethodBase methodOrConstructor, Type declaringType)
    {
      if (!declaringType.Equals (methodOrConstructor.DeclaringType))
      {
        RuntimeTypeHandle typeHandle = declaringType.TypeHandle;
        RuntimeMethodHandle methodHandle = methodOrConstructor.MethodHandle;
        return MethodBase.GetMethodFromHandle (methodHandle, typeHandle);
      }
      else
        return methodOrConstructor;
    }

    private static void SerializeGenericArguments (Type[] genericArguments, string key, SerializationInfo info)
    {
      info.AddValue (key + ".Length", genericArguments.Length);
      for (int i = 0; i < genericArguments.Length; ++i)
      {
        info.AddValue (key + "[" + i + "].IsGenericParameter", genericArguments[i].IsGenericParameter);
        if (!genericArguments[i].IsGenericParameter)
          SerializeType (genericArguments[i], key + "[" + i + "]", info);
      }
    }

    private static Type[] DeserializeGenericArguments (string key, MethodBase genericMethodDefinition, SerializationInfo info)
    {
      int length = info.GetInt32 (key + ".Length");
      Type[] genericParameters = genericMethodDefinition.GetGenericArguments ();
      if (length != genericParameters.Length)
      {
        string message = string.Format ("Invalid generic argument array for generic method {0}.{1}: expected length of {2}, but {3} given.",
          genericMethodDefinition.DeclaringType.FullName, genericMethodDefinition.Name, genericParameters.Length, length);
        throw new SerializationException (message);
      }

      Type[] genericArguments = new Type[length];
      for (int i = 0; i < genericArguments.Length; ++i)
      {
        bool isGenericParameter = info.GetBoolean (key + "[" + i + "].IsGenericParameter");
        if (isGenericParameter)
          genericArguments[i] = genericParameters[i];
        else
          genericArguments[i] = DeserializeType (key + "[" + i + "]", info);

      }
      return genericArguments;
    }

    /*private static void SerializeParameterTypes (ParameterInfo[] parameters, string key, SerializationInfo info)
    {
      info.AddValue (key + ".Length", parameters.Length);
      for (int i = 0; i < parameters.Length; ++i)
      {
        info.AddValue (key + "[" + i + "].IsGenericParameter", parameters[i].ParameterType.IsGenericParameter);
        if (parameters[i].ParameterType.IsGenericParameter)
        {
          if (parameters[i].ParameterType.DeclaringMethod == null)
            throw new NotSupportedException ("Generic type in method signature, this is not supported.");

          info.AddValue (key + ".GenericParameterPosition", parameters[i].ParameterType.GenericParameterPosition);
        }
        else
          SerializeType (parameters[i].ParameterType, key + "[" + i + "].ParameterType", info);
      }
    }

    private static Type[] DeserializeParameterTypes (string key, SerializationInfo info)
    {
      int length = info.GetInt32 (key + ".Length");
      Type[] parameterTypes = new Type[length];
      for (int i = 0; i < parameterTypes.Length; ++i)
      {
        bool isGenericParameter = info.GetBoolean (key + "[" + i + "].IsGenericParameter");
        if (isGenericParameter)
        {
          throw new NotImplementedException();
        }
        else
          parameterTypes[i] = DeserializeType (key + "[" + i + "].ParameterType", info);
      }

      return parameterTypes;
    }

    private static void SerializeParameterType (Type parameterType, string key, SerializationInfo info)
    {
      info.AddValue (key + ".IsGenericParameter", parameterType.IsGenericParameter);
      if (parameterType.IsGenericParameter)
      {
        if (parameterType.DeclaringMethod == null)
          throw new NotSupportedException ("Generic type in method signature, this is not supported.");

        info.AddValue (key + ".GenericParameterPosition", parameterType.GenericParameterPosition);
      }
      else
        SerializeType (parameterType, key, info);
    }*/
  }
}
