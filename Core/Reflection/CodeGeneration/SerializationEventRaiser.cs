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
      return _attributedMethodCache.GetOrCreateValue (Tuple.NewTuple (type, attributeType), delegate (Tuple<Type, Type> typeAndAttributeType) {
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