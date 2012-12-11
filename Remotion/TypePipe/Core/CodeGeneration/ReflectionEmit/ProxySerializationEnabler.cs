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
using Remotion.Collections;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.Serialization.Implementation;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.TypePipe.CodeGeneration.ReflectionEmit
{
  /// <summary>
  /// Implements <see cref="IProxySerializationEnabler"/>.
  /// </summary>
  public class ProxySerializationEnabler : IProxySerializationEnabler
  {
    private static readonly MethodInfo s_getObjectDataMetod =
        MemberInfoFromExpressionUtility.GetMethod ((ISerializable obj) => obj.GetObjectData (null, new StreamingContext()));
    private static readonly MethodInfo s_getValueMethod =
        MemberInfoFromExpressionUtility.GetMethod ((SerializationInfo obj) => obj.GetValue ("", null));
    private static readonly MethodInfo s_onDeserializationMethod =
        MemberInfoFromExpressionUtility.GetMethod ((IDeserializationCallback obj) => obj.OnDeserialization (null));

    private readonly ISerializableFieldFinder _serializableFieldFinder;

    public ProxySerializationEnabler (ISerializableFieldFinder serializableFieldFinder)
    {
      ArgumentUtility.CheckNotNull ("serializableFieldFinder", serializableFieldFinder);

      _serializableFieldFinder = serializableFieldFinder;
    }

    public void MakeSerializable (MutableType mutableType, MethodInfo initializationMethod)
    {
      ArgumentUtility.CheckNotNull ("mutableType", mutableType);
      // initializationMethod may be null

      // Existing fields are always serialized by the standard .NET serialization or by an implementation of ISerializable on the underlying type.
      // Added fields are also serialized by the standard .NET serialization, unless the mutable type implements ISerializable. In that case,
      // we need to extend the ISerializable implementation to include the added fields.
      
      var serializedFieldMapping = _serializableFieldFinder.GetSerializableFieldMapping (mutableType.AddedFields.Cast<FieldInfo> ()).ToArray ();
      var deserializationConstructor = GetDeserializationConstructor (mutableType);

      // If the underlying type implements ISerializable but has no deserialization constructor, we can't implement ISerializable correctly, so
      // we don't even try. (SerializationParticipant relies on this behavior.)
      var needsCustomFieldSerialization =
          serializedFieldMapping.Length != 0 && mutableType.IsAssignableTo (typeof (ISerializable)) && deserializationConstructor != null;

      if (needsCustomFieldSerialization)
      {
        OverrideGetObjectData (mutableType, serializedFieldMapping);
        AdaptDeserializationConstructor (mutableType, deserializationConstructor, serializedFieldMapping);
      }

      if (initializationMethod != null)
      {
        if (mutableType.IsAssignableTo (typeof (IDeserializationCallback)))
          OverrideOnDeserialization (mutableType, initializationMethod);
        else if (mutableType.IsSerializable)
          ExplicitlyImplementOnDeserialization (mutableType, initializationMethod);
      }
    }

    public bool IsDeserializationConstructor (ConstructorInfo constructor)
    {
      return constructor.GetParameters ().Select (x => x.ParameterType).SequenceEqual (new[] { typeof (SerializationInfo), typeof (StreamingContext) });
    }

    private void OverrideGetObjectData (MutableType mutableType, Tuple<string, FieldInfo>[] serializedFieldMapping)
    {
      try
      {
        mutableType
            .GetOrAddMutableMethod (s_getObjectDataMetod)
            .SetBody (
                ctx => Expression.Block (
                    typeof (void),
                    new[] { ctx.PreviousBody }.Concat (BuildFieldSerializationExpressions (ctx.This, ctx.Parameters[0], serializedFieldMapping))));
      }
      catch (NotSupportedException exception)
      {
        throw new NotSupportedException (
            "The underlying type implements ISerializable but GetObjectData cannot be overridden. "
            + "Make sure that GetObjectData is implemented implicitly (not explicitly) and virtual.",
            exception);
      }
    }

    private ConstructorInfo GetDeserializationConstructor (Type type)
    {
      return type.GetConstructor (
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
          null,
          new[] { typeof (SerializationInfo), typeof (StreamingContext) },
          null);
    }

    private void AdaptDeserializationConstructor (
        MutableType mutableType, ConstructorInfo constructor, Tuple<string, FieldInfo>[] serializedFieldMapping)
    {
      mutableType
          .GetMutableConstructor (constructor)
          .SetBody (
              ctx => Expression.Block (
                  typeof (void),
                  new[] { ctx.PreviousBody }.Concat (BuildFieldDeserializationExpressions (ctx.This, ctx.Parameters[0], serializedFieldMapping))));
    }

    private static void OverrideOnDeserialization (MutableType mutableType, MethodInfo initializationMethod)
    {
      try
      {
        mutableType.GetOrAddMutableMethod (s_onDeserializationMethod)
                   .SetBody (ctx => Expression.Block (typeof (void), ctx.PreviousBody, Expression.Call (ctx.This, initializationMethod)));
      }
      catch (NotSupportedException exception)
      {
        throw new NotSupportedException (
            "The underlying type implements IDeserializationCallback but OnDeserialization cannot be overridden. "
            + "Make sure that OnDeserialization is implemented implicitly (not explicitly) and virtual.",
            exception);
      }
    }

    private static void ExplicitlyImplementOnDeserialization (MutableType mutableType, MethodInfo initializationMethod)
    {
      mutableType.AddInterface (typeof (IDeserializationCallback));
      mutableType.AddExplicitOverride (s_onDeserializationMethod, ctx => Expression.Call (ctx.This, initializationMethod));
    }

    private IEnumerable<Expression> BuildFieldSerializationExpressions (
        Expression @this, Expression serializationInfo, IEnumerable<Tuple<string, FieldInfo>> fieldMapping)
    {
      return fieldMapping
          .Select (
              entry => (Expression) Expression.Call (
                  serializationInfo, "AddValue", Type.EmptyTypes, Expression.Constant (entry.Item1), Expression.Field (@this, entry.Item2)));
    }

    private IEnumerable<Expression> BuildFieldDeserializationExpressions (
        Expression @this, Expression serializationInfo, IEnumerable<Tuple<string, FieldInfo>> fieldMapping)
    {
      return fieldMapping
          .Select (
              entry => (Expression) Expression.Assign (
                  Expression.Field (@this, entry.Item2),
                  Expression.Convert (
                      Expression.Call (
                          serializationInfo, s_getValueMethod, Expression.Constant (entry.Item1), Expression.Constant (entry.Item2.FieldType)),
                      entry.Item2.FieldType)));
    }
  }
}