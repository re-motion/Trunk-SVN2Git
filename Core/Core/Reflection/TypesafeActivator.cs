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
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Create an instance of a known type using fixed parameter types for the constructor.
  /// </summary>
  /// <remarks>
  /// While <see cref="Activator.CreateInstance(Type,object[])"/> uses the types of passed arguments to select the correct constructor, this class
  /// uses the types of the expressions you use at compile time. Use the following code to create an instance of a class called MyClass using a 
  /// constructor that has an argument of type string:
  /// <code>
  ///   ParameterType p;
  ///   YourClass obj = TypesafeActivator.CreateInstance&lt;YourClass&gt;().With (p);
  /// </code>
  /// This code always selects the constructor that accepts an argument of type ParameterType, even if the value passed is null or an instance
  /// of a subclass of ParameterType.
  /// </remarks>
  public static class TypesafeActivator
  {
    public static FuncInvoker<T> CreateInstance<T> ()
    {
      return new FuncInvoker<T> (new ConstructorLookupInfo (typeof (T)).GetDelegate);
    }

    public static FuncInvoker<T> CreateInstance<T> (BindingFlags bindingFlags)
    {
      return new FuncInvoker<T> (new ConstructorLookupInfo (typeof (T), bindingFlags).GetDelegate);
    }

    public static FuncInvoker<T> CreateInstance<T> (
        BindingFlags bindingFlags, Binder binder, CallingConventions callingConvention, ParameterModifier[] parameterModifiers)
    {
      return new FuncInvoker<T> (new ConstructorLookupInfo (typeof (T), bindingFlags, binder, callingConvention, parameterModifiers).GetDelegate);
    }

    public static FuncInvoker<object> CreateInstance (Type type)
    {
      return CreateInstance<object> (type);
    }

    public static FuncInvoker<TMinimal> CreateInstance<TMinimal> (Type type)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (TMinimal));
      return new FuncInvoker<TMinimal> (new ConstructorLookupInfo (type).GetDelegate);
    }

    public static FuncInvoker<object> CreateInstance (Type type, BindingFlags bindingFlags)
    {
      return CreateInstance<object> (type, bindingFlags);
    }

    public static FuncInvoker<TMinimal> CreateInstance<TMinimal> (Type type, BindingFlags bindingFlags)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (TMinimal));
      return new FuncInvoker<TMinimal> (new ConstructorLookupInfo (type, bindingFlags).GetDelegate);
    }

    public static FuncInvoker<object> CreateInstance (
        Type type, BindingFlags bindingFlags, Binder binder, CallingConventions callingConvention, ParameterModifier[] parameterModifiers)
    {
      return CreateInstance<object> (type, bindingFlags, binder, callingConvention, parameterModifiers);
    }

    public static FuncInvoker<TMinimal> CreateInstance<TMinimal> (
        Type type, BindingFlags bindingFlags, Binder binder, CallingConventions callingConvention, ParameterModifier[] parameterModifiers)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (TMinimal));
      return new FuncInvoker<TMinimal> (new ConstructorLookupInfo (type, bindingFlags, binder, callingConvention, parameterModifiers).GetDelegate);
    }
  }
}
