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
using System.Runtime.Serialization;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration
{
  public class SerializationEventRaiser
  {
    private readonly InterlockedCache<Tuple<Type, Type>, List<MethodInfo>> _attributedMethodCache = new InterlockedCache<Tuple<Type, Type>, List<MethodInfo>>();

    public virtual void InvokeAttributedMethod (object deserializedObject, Type attributeType, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("deserializedObject", deserializedObject);
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      foreach (MethodInfo method in FindDeserializationMethodsWithCache (deserializedObject.GetType (), attributeType))
        method.Invoke (deserializedObject, new object[] { context });
    }

    protected virtual List<MethodInfo> FindDeserializationMethodsWithCache (Type type, Type attributeType)
    {
      return _attributedMethodCache.GetOrCreateValue (Tuple.Create (type, attributeType), delegate (Tuple<Type, Type> typeAndAttributeType) {
          return new List<MethodInfo> (FindDeserializationMethodsNoCache (typeAndAttributeType.A, typeAndAttributeType.B)); });
    }

    protected virtual IEnumerable<MethodInfo> FindDeserializationMethodsNoCache (Type type, Type attributeType)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      for (Type currentType = type; currentType != null; currentType = currentType.BaseType)
      {
        foreach (
            MethodInfo method in
                currentType.GetMethods (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
          if (method.IsDefined (attributeType, false))
            yield return method;
        }
      }
    }

    public virtual void RaiseDeserializationEvent (object deserializedObject, object sender)
    {
      ArgumentUtility.CheckNotNull ("deserializedObject", deserializedObject);

      IDeserializationCallback objectAsDeserializationCallback = deserializedObject as IDeserializationCallback;
      if (objectAsDeserializationCallback != null)
        objectAsDeserializationCallback.OnDeserialization (sender);
    }
  }
}
