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
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.Serialization;
using Remotion.Reflection.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  [Serializable]
  public class SerializationHelper : IObjectReference, ISerializable, IDeserializationCallback
  {
    // Always remember: the whole configuration must be serialized as one single, flat object (or SerializationInfo), we cannot rely on any
    // nested objects to be deserialized in the right order
    public static void GetObjectDataForGeneratedTypes (
        SerializationInfo info, 
        StreamingContext context, 
        object concreteObject,
        ClassContext classContext,
        object[] extensions, 
        bool serializeBaseMembers)
    {
      info.SetType (typeof (SerializationHelper));

      var classContextSerializer = new SerializationInfoClassContextSerializer (info, "__configuration.ConfigurationContext");
      classContext.Serialize (classContextSerializer);

      info.AddValue ("__extensions", extensions);

      object[] baseMemberValues;
      if (serializeBaseMembers)
      {
        MemberInfo[] baseMembers = FormatterServices.GetSerializableMembers (classContext.Type);
        baseMemberValues = FormatterServices.GetObjectData (concreteObject, baseMembers);
      }
      else
        baseMemberValues = null;

      info.AddValue ("__baseMemberValues", baseMemberValues);
    }

    private readonly IMixinTarget _deserializedObject;
    private readonly ClassContext _classContext;
    private readonly object[] _extensions;
    private readonly object[] _baseMemberValues;
    private readonly StreamingContext _context;

    public SerializationHelper (SerializationInfo info, StreamingContext context)
        : this (info, context, t => t)
    {
    }

    public SerializationHelper (SerializationInfo info, StreamingContext context, Func<Type, Type> typeTransformer)
    {
      ArgumentUtility.CheckNotNull ("typeTransformer", typeTransformer);
      ArgumentUtility.CheckNotNull ("info", info);

      _context = context;

      var classContextDeserializer = new SerializationInfoClassContextDeserializer (info, "__configuration.ConfigurationContext");
      _classContext = ClassContext.Deserialize (classContextDeserializer);

      Type untransformedConcreteType = ConcreteTypeBuilder.Current.GetConcreteType (_classContext);
      Type concreteType = typeTransformer (untransformedConcreteType);

      if (!_classContext.Type.IsAssignableFrom (concreteType))
      {
        string message = string.Format ("TypeTransformer returned type '{0}', which is not compatible with the serialized mixin configuration. The "
            + "configuration requires a type assignable to '{1}'.", concreteType, _classContext.Type);
        throw new InvalidOperationException (message);
      }
      else if (!typeof (IMixinTarget).IsAssignableFrom (concreteType))
      {
        string message = string.Format ("TypeTransformer returned type '{0}', which does not implement IMixinTarget.", concreteType);
        throw new InvalidOperationException (message);
      }

      _extensions = (object[]) info.GetValue ("__extensions", typeof (object[]));
      Assertion.IsNotNull (_extensions);

      _baseMemberValues = (object[]) info.GetValue ("__baseMemberValues", typeof (object[]));

      // Usually, instantiate a deserialized object using GetSafeUninitializedObject.
      // However, _baseMemberValues being null means that the object itself manages its member deserialization via ISerializable. In such a case, we
      // need to use the deserialization constructor to instantiate the object.
      if (_baseMemberValues != null)
        _deserializedObject = (IMixinTarget) FormatterServices.GetSafeUninitializedObject (concreteType);
      else
      {
        Assertion.IsTrue (typeof (ISerializable).IsAssignableFrom (concreteType));
        _deserializedObject = (IMixinTarget) Activator.CreateInstance (
            concreteType, 
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
            null, 
            new object[] {info, context}, 
            null);
      }

      SerializationImplementer.RaiseOnDeserializing (_deserializedObject, _context);
    }

    public object GetRealObject (StreamingContext context)
    {
      return _deserializedObject;
    }

    void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
    {
      throw new NotImplementedException ("This should never be called.");
    }

    // Here, we can rely on everything being deserialized as needed.
    public void OnDeserialization (object sender)
    {
      if (_baseMemberValues != null)
      {
        MemberInfo[] baseMembers = FormatterServices.GetSerializableMembers (_classContext.Type);
        FormatterServices.PopulateObjectMembers (_deserializedObject, baseMembers, _baseMemberValues);
      }

      ConcreteTypeBuilder.Current.Scope.InitializeDeserializedMixinTarget (_deserializedObject, _extensions);

      SerializationImplementer.RaiseOnDeserialized (_deserializedObject, _context);
      SerializationImplementer.RaiseOnDeserialization (_deserializedObject, sender);
    }
  }
}
