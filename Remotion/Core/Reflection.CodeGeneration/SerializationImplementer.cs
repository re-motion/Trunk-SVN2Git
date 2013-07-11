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
using System.Reflection;
using System.Runtime.Serialization;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration
{
  public static class SerializationImplementer
  {
    private static readonly SerializationEventRaiser s_serializationEventRaiser = new SerializationEventRaiser();

    private static bool IsPublicOrProtected (MethodBase method)
    {
      return method.IsPublic || method.IsFamily || method.IsFamilyOrAssembly;
    }

    public static IMethodEmitter ImplementGetObjectDataByDelegation (
        IClassEmitter classEmitter, 
        Func<IMethodEmitter, bool, MethodInvocationExpression> delegatingMethodInvocationGetter)
    {
      ArgumentUtility.CheckNotNull ("classEmitter", classEmitter);
      ArgumentUtility.CheckNotNull ("delegatingMethodInvocationGetter", delegatingMethodInvocationGetter);

      bool baseIsISerializable = typeof (ISerializable).IsAssignableFrom (classEmitter.BaseType);

      MethodInfo getObjectDataMethod =
          typeof (ISerializable).GetMethod ("GetObjectData", new[] {typeof (SerializationInfo), typeof (StreamingContext)});
      var newMethod = classEmitter.CreatePublicInterfaceMethodImplementation (getObjectDataMethod);

      if (baseIsISerializable)
        ImplementBaseGetObjectDataCall (classEmitter, newMethod);

      MethodInvocationExpression delegatingMethodInvocation = delegatingMethodInvocationGetter (newMethod, baseIsISerializable);
      if (delegatingMethodInvocation != null)
        newMethod.AddStatement (new ExpressionStatement (delegatingMethodInvocation));

      newMethod.ImplementByReturningVoid();

      return newMethod;
    }

    private static void ImplementBaseGetObjectDataCall (IClassEmitter classEmitter, IMethodEmitter getObjectDataMethod)
    {
      ConstructorInfo baseConstructor = classEmitter.BaseType.GetConstructor (
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
          null,
          CallingConventions.Any,
          new[] {typeof (SerializationInfo), typeof (StreamingContext)},
          null);
      if (baseConstructor == null || !IsPublicOrProtected (baseConstructor))
      {
        string message = string.Format (
            "No public or protected deserialization constructor in type {0} - serialization is not supported.",
            classEmitter.BaseType.FullName);
        getObjectDataMethod.ImplementByThrowing (typeof (InvalidOperationException), message);
        return;
      }

      MethodInfo baseGetObjectDataMethod =
          classEmitter.BaseType.GetMethod ("GetObjectData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
      if (baseGetObjectDataMethod == null || !IsPublicOrProtected (baseGetObjectDataMethod))
      {
        string message = string.Format ("No public or protected GetObjectData in type {0} - serialization is not supported.",
            classEmitter.BaseType.FullName);
        getObjectDataMethod.ImplementByThrowing (typeof (InvalidOperationException), message);
        return;
      }

      getObjectDataMethod.AddStatement (
          new ExpressionStatement (
              new MethodInvocationExpression (
                  SelfReference.Self,
                  baseGetObjectDataMethod,
                  new ReferenceExpression (getObjectDataMethod.ArgumentReferences[0]),
                  new ReferenceExpression (getObjectDataMethod.ArgumentReferences[1]))));
    }

    public static ConstructorEmitter ImplementDeserializationConstructorByThrowing (IClassEmitter classEmitter)
    {
      ArgumentUtility.CheckNotNull ("classEmitter", classEmitter);

      ConstructorEmitter emitter = classEmitter.CreateConstructor (new[] {typeof (SerializationInfo), typeof (StreamingContext)});
      emitter.CodeBuilder.AddStatement (
          new ThrowStatement (
              typeof (NotImplementedException),
              "The deserialization constructor should never be called; generated types are deserialized via IObjectReference helpers."));
      return emitter;
    }

    public static ConstructorEmitter ImplementDeserializationConstructorByThrowingIfNotExistsOnBase (IClassEmitter classEmitter)
    {
      ArgumentUtility.CheckNotNull ("classEmitter", classEmitter);

      var serializationConstructorSignature = new[] {typeof (SerializationInfo), typeof (StreamingContext)};
      ConstructorInfo baseConstructor = classEmitter.BaseType.GetConstructor (
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
          null,
          serializationConstructorSignature,
          null);
      if (baseConstructor == null)
        return ImplementDeserializationConstructorByThrowing (classEmitter);
      else
        return null;
    }

    public static void RaiseOnDeserialization (object deserializedObject, object sender)
    {
      ArgumentUtility.CheckNotNull ("deserializedObject", deserializedObject);
      s_serializationEventRaiser.RaiseDeserializationEvent (deserializedObject, sender);
    }

    public static void RaiseOnDeserializing (object deserializedObject, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("deserializedObject", deserializedObject);
      s_serializationEventRaiser.InvokeAttributedMethod (deserializedObject, typeof (OnDeserializingAttribute), context);
    }

    public static void RaiseOnDeserialized (object deserializedObject, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("deserializedObject", deserializedObject);
      s_serializationEventRaiser.InvokeAttributedMethod (deserializedObject, typeof (OnDeserializedAttribute), context);
    }
  }
}
