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
using System.Reflection.Emit;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Creates wrapper methods for constructors so they can be called via delegates instead of reflection.
  /// </summary>
  public class ConstructorWrapper
  {
    //public static GetDelegateWith<T> GetConstructor<T> ()
    //{
    //  return GetConstructor<T> (BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, null);
    //}

    //public static GetDelegateWith<T> GetConstructor<T> (BindingFlags bindingFlags)
    //{
    //  return GetConstructor<T> (bindingFlags, null, CallingConventions.Any, null);
    //}

    //public static GetDelegateWith<T> GetConstructor<T> (
    //    BindingFlags bindingFlags, Binder binder, CallingConventions callingConvention, ParameterModifier[] parameterModifiers)
    //{
    //  return new CachedGetDelegateWith<T> (
    //      delegate (Type[] types, Type delegateType)
    //      {
    //        return CreateConstructorDelegate (typeof (T), bindingFlags, binder, callingConvention, types, parameterModifiers, delegateType);
    //      });
    //}

    //public static GetDelegateWith<object> GetConstructor (Type type)
    //{
    //  return GetConstructor (type, BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, null);
    //}

    //public static GetDelegateWith<object> GetConstructor (Type type, BindingFlags bindingFlags)
    //{
    //  return GetConstructor (type, bindingFlags, null, CallingConventions.Any, null);
    //}

    //public static GetDelegateWith<object> GetConstructor (
    //    Type type, BindingFlags bindingFlags, Binder binder, CallingConventions callingConvention, ParameterModifier[] parameterModifiers)
    //{
    //  return new CachedGetDelegateWith<object,Type> (
    //      type,
    //      delegate (Type[] types, Type delegateType)
    //      {
    //        return CreateConstructorDelegate (type, bindingFlags, binder, callingConvention, types, parameterModifiers, delegateType);
    //      });
    //}

    public static Type[] GetParameterTypes (Type delegateType)
    {
      if (!typeof (Delegate).IsAssignableFrom (delegateType))
        throw new ArgumentException ("Type must be a delegate type.", "delegateType");

      MethodInfo invokeMethod = delegateType.GetMethod ("Invoke");
      Assertion.IsNotNull (invokeMethod, "Delegate has no Invoke() method."); // according to the CLI specs, each delegate type must define this method

      return EnumerableUtility.SelectToArray<ParameterInfo, Type> (
          invokeMethod.GetParameters (),
          delegate (ParameterInfo par) { return par.ParameterType; });
    }

    public static Delegate CreateDelegate (
        Type type, Type delegateType,
        BindingFlags bindingFlags, Binder binder, CallingConventions callingConvention, ParameterModifier[] parameterModifiers)
    {
      return CreateDelegate (type, delegateType, bindingFlags, binder, callingConvention, GetParameterTypes (delegateType), parameterModifiers);
    }

    public static Delegate CreateDelegate (
        Type type, Type delegateType, 
        BindingFlags bindingFlags, Binder binder, CallingConventions callingConvention, Type[] parameterTypes, ParameterModifier[] parameterModifiers)
    {
      if (type.IsValueType && parameterTypes.Length == 0)
        return CreateValueTypeDefaultDelegate (type, delegateType);

      ConstructorInfo ctor = type.GetConstructor (bindingFlags, binder, callingConvention, parameterTypes, parameterModifiers);
      if (ctor == null)
      {
        string message = string.Format ("Type {0} does not contain a constructor with the following arguments types: {1}",
            type.FullName, SeparatedStringBuilder.Build (", ", parameterTypes, delegate (Type t) { return t.FullName; }));
        throw new MissingMethodException (message);
      }
      return CreateDelegate (ctor, delegateType);
    }

    /// <summary>
    /// Since value types do not have default constructors, an activation with zero parameters must create the object with the initobj IL opcode.
    /// </summary>
    private static Delegate CreateValueTypeDefaultDelegate (Type type, Type delegateType)
    {
      DynamicMethod method = new DynamicMethod ("ConstructorWrapper", type, Type.EmptyTypes, type);
      ILGenerator ilgen = method.GetILGenerator ();

      ilgen.DeclareLocal (type);
      ilgen.Emit (OpCodes.Ldloca_S, 0);     // load address of local variable
      ilgen.Emit (OpCodes.Initobj, type);   // initialize that object with default value
      ilgen.Emit (OpCodes.Ldloc_0);         // load local variable value
      ilgen.Emit (OpCodes.Ret);             // and return it

      return method.CreateDelegate (delegateType);
    }

    public static Delegate CreateDelegate (ConstructorInfo ctor, Type delegateType)
    {
      ParameterInfo[] parameters = ctor.GetParameters ();
      Type type = ctor.DeclaringType;
      DynamicMethod method = new DynamicMethod ("ConstructorWrapper", type, EmitUtility.GetParameterTypes (parameters), type);
      ILGenerator ilgen = method.GetILGenerator ();

      EmitUtility.PushParameters (ilgen, parameters.Length);
      ilgen.Emit (OpCodes.Newobj, ctor);
      ilgen.Emit (OpCodes.Ret);

      try
      {
        return method.CreateDelegate (delegateType);
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException ("Parameters of constructor and delegate type do not match.", ex);
      }
    }
  }
}
