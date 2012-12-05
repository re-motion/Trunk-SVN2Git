﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Scripting.Ast;
using Remotion.FunctionalProgramming;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit
{
  /// <summary>
  /// Implements <see cref="IProxySerializationEnabler"/>.
  /// </summary>
  public class ProxySerializationEnabler : IProxySerializationEnabler
  {
    private const string c_serializationKeyPrefix = "<tp>";

    private static readonly MethodInfo s_getValueMethod =
        MemberInfoFromExpressionUtility.GetMethod ((SerializationInfo obj) => obj.GetValue ("", null));
    private static readonly MethodInfo s_onDeserializationMethod =
      MemberInfoFromExpressionUtility.GetMethod ((IDeserializationCallback obj) => obj.OnDeserialization (null));

    public void MakeSerializable (MutableType mutableType, MethodInfo initializationMethod)
    {
      ArgumentUtility.CheckNotNull ("mutableType", mutableType);
      // initializationMethod may be null

      // TODO Review: Use IsAssignableTo
      var implementsSerializable = mutableType.GetInterfaces().Contains (typeof (ISerializable));
      var serializedFields = mutableType
          .AddedFields
          .Where (f => !f.IsStatic && !f.GetCustomAttributes (typeof (NonSerializedAttribute), false).Any())
          .ToArray();
      // TODO Review: Use IsAssignableTo
      var implementsDeserializationCallback = mutableType.GetInterfaces ().Contains (typeof (IDeserializationCallback));
      var hasInstanceInitializations = initializationMethod != null;

      if (implementsSerializable && serializedFields.Length != 0)
      {
        OverrideGetObjectData (mutableType, serializedFields);
        AdaptDeserializationConstructor (mutableType, serializedFields);
      }

      if (hasInstanceInitializations)
      {
        if (implementsDeserializationCallback)
          OverrideOnDeserialization (mutableType, initializationMethod);
        else if (mutableType.IsSerializable)
          ExplicitlyImplementOnDeserialization (mutableType, initializationMethod);
      }
    }

    private static void ExplicitlyImplementOnDeserialization (MutableType mutableType, MethodInfo initializationMethod)
    {
      mutableType.AddInterface (typeof (IDeserializationCallback));
      mutableType.AddExplicitOverride (s_onDeserializationMethod, ctx => Expression.Call (ctx.This, initializationMethod));
    }

    private static void OverrideOnDeserialization (MutableType mutableType, MethodInfo initializationMethod)
    {
      var onDeserializationOverride = mutableType.GetOrAddMutableMethod (s_onDeserializationMethod);
      onDeserializationOverride.SetBody (ctx => Expression.Block (typeof (void), ctx.PreviousBody, Expression.Call (ctx.This, initializationMethod)));
    }

    private void OverrideGetObjectData (MutableType mutableType, MutableFieldInfo[] serializedFields)
    {
      var interfaceMethod = MemberInfoFromExpressionUtility.GetMethod ((ISerializable obj) => obj.GetObjectData (null, new StreamingContext()));
      var getObjectDataOverride = mutableType.GetOrAddMutableMethod (interfaceMethod);

      if (!getObjectDataOverride.CanSetBody)
        throw new NotSupportedException (
          "The underlying type implements ISerializable but GetObjectData cannot be overridden. "
          + "Make sure that GetObjectData is implemented implicitly (not explicitly) and virtual.");

      getObjectDataOverride.SetBody (ctx => BuildSerializationBody (ctx, serializedFields, ctx.PreviousBody, SerializeField));
    }

    private void AdaptDeserializationConstructor (MutableType mutableType, IEnumerable<FieldInfo> serializedFields)
    {
      var deserializationConstructor = mutableType.GetConstructor (
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
          null,
          new[] { typeof (SerializationInfo), typeof (StreamingContext) },
          null);
      if (deserializationConstructor == null)
        throw new InvalidOperationException ("The underlying type implements 'ISerializable' but does not define a deserialization constructor.");

      var mutableConstructor = mutableType.GetMutableConstructor (deserializationConstructor);
      mutableConstructor.SetBody (ctx => BuildSerializationBody (ctx, serializedFields, ctx.PreviousBody, DeserializeField));
    }

    // TODO Review: Refactor this method to return an IEnumerable<FieldInfo, string>, then input this to a BuildSerializationBody/BuildDeserializationBody method.
    private Expression BuildSerializationBody (
        MethodBaseBodyContextBase ctx,
        IEnumerable<FieldInfo> serializedFields,
        Expression previousBody,
        Func<Expression, Expression, string, FieldInfo, Expression> expressionProvider)
    {
      // TODO Review: Move check for serializable fields out to caller and do not create an override if there are no serializable fields.
      var fieldSerializations = serializedFields
          .ToLookup (f => f.Name)
          .SelectMany (
              fieldsByName =>
              {
                var fields = fieldsByName.ToList();

                var serializationKeyProvider =
                    fields.Count == 1
                        ? (Func<FieldInfo, string>) (f => c_serializationKeyPrefix + f.Name)
                        : (f => string.Format ("{0}{1}@{2}", c_serializationKeyPrefix, f.Name, f.FieldType.FullName));

                return fields.Select (f => expressionProvider (ctx.This, ctx.Parameters[0], serializationKeyProvider (f), f));
              });

      var expressions = EnumerableUtility.Singleton (previousBody).Concat (fieldSerializations);

      return Expression.Block (typeof (void), expressions);
    }

    private Expression SerializeField (Expression @this, Expression serializationInfo, string serializationKey, FieldInfo field)
    {
      return Expression.Call (serializationInfo, "AddValue", Type.EmptyTypes, Expression.Constant (serializationKey), Expression.Field (@this, field));
    }

    private Expression DeserializeField (Expression @this, Expression serializationInfo, string serializationKey, FieldInfo field)
    {
      var type = field.FieldType;
      return Expression.Assign (
          Expression.Field (@this, field),
          Expression.Convert (
              Expression.Call (serializationInfo, s_getValueMethod, Expression.Constant (serializationKey), Expression.Constant (type)), type));
    }
  }
}