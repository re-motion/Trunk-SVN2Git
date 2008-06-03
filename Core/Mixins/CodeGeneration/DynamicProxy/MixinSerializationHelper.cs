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
using System.IO;
using System.Runtime.Serialization;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Remotion.Reflection.CodeGeneration;
using Remotion.Mixins;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Definitions.Building;
using Remotion.Mixins.Utilities;
using Remotion.Utilities;
using Remotion.Mixins.Context;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  [Serializable]
  public class MixinSerializationHelper : IObjectReference, ISerializable, IDeserializationCallback
  {
    // Always remember: the whole configuration must be serialized as one single, flat object (or SerializationInfo), we cannot rely on any
    // nested objects to be deserialized in the right order
    public static void GetObjectDataForGeneratedTypes (SerializationInfo info, StreamingContext context, object mixin, MixinDefinition configuration,
        bool serializeBaseMembers)
    {
      info.SetType (typeof (MixinSerializationHelper));

      ClassContext targetClassContext = configuration.TargetClass.ConfigurationContext;
      info.AddValue ("__configuration.TargetClass.ConfigurationContext", targetClassContext);
      info.AddValue ("__configuration.MixinIndex", configuration.MixinIndex);

      object[] baseMemberValues;
      if (serializeBaseMembers)
      {
        MemberInfo[] baseMembers = FormatterServices.GetSerializableMembers (mixin.GetType ().BaseType);
        baseMemberValues = FormatterServices.GetObjectData (mixin, baseMembers);
      }
      else
        baseMemberValues = null;

      info.AddValue ("__baseMemberValues", baseMemberValues);
    }

    private readonly MixinDefinition _mixinDefinition;
    private readonly object[] _baseMemberValues;
    private readonly object _deserializedObject;
    private readonly StreamingContext _context;

    public MixinSerializationHelper (SerializationInfo info, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      _context = context;

      ClassContext targetClassContext = (ClassContext) info.GetValue ("__configuration.TargetClass.ConfigurationContext", typeof (ClassContext));
      TargetClassDefinition targetClassDefinition = TargetClassDefinitionCache.Current.GetTargetClassDefinition (targetClassContext);

      int mixinIndex = info.GetInt32 ("__configuration.MixinIndex");
      _mixinDefinition = targetClassDefinition.Mixins[mixinIndex];

      Type concreteType = ConcreteTypeBuilder.Current.GetConcreteMixinType (_mixinDefinition);
      _baseMemberValues = (object[]) info.GetValue ("__baseMemberValues", typeof (object[]));

      // Usually, instantiate a deserialized object using GetSafeUninitializedObject.
      // However, _baseMemberValues being null means that the object itself manages its member deserialization via ISerializable. In such a case, we
      // need to use the deserialization constructor to instantiate the object.
      if (_baseMemberValues != null)
        _deserializedObject = FormatterServices.GetSafeUninitializedObject (concreteType);
      else
      {
        Assertion.IsTrue (typeof (ISerializable).IsAssignableFrom (concreteType));
        _deserializedObject = Activator.CreateInstance (concreteType, new object[] { info, context });
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
        MemberInfo[] baseMembers = FormatterServices.GetSerializableMembers (_deserializedObject.GetType ().BaseType);
        FormatterServices.PopulateObjectMembers (_deserializedObject, baseMembers, _baseMemberValues);
      }

      SerializationImplementer.RaiseOnDeserialized (_deserializedObject, _context);
      SerializationImplementer.RaiseOnDeserialization (_deserializedObject, sender);
    }
  }
}
