// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Mixins.Definitions;
using Remotion.Reflection.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  [Serializable]
  public class MixinSerializationHelper : IObjectReference, ISerializable, IDeserializationCallback
  {
    // Always remember: the whole configuration must be serialized as one single, flat object (or SerializationInfo), we cannot rely on any
    // nested objects to be deserialized in the right order
    public static void GetObjectDataForGeneratedTypes (SerializationInfo info, StreamingContext context, object mixin, IMixinTarget targetObject,
        bool serializeBaseMembers)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      ArgumentUtility.CheckNotNull ("mixin", mixin);
      ArgumentUtility.CheckNotNull ("targetObject", targetObject);

      info.SetType (typeof (MixinSerializationHelper));

      ClassContext targetClassContext = targetObject.Configuration.ConfigurationContext;
      int mixinIndex = Array.IndexOf (targetObject.Mixins, mixin);
      if (mixinIndex == -1)
        throw new ArgumentException ("The given mixin is not part of the given targetObject.", "targetObject");

      var classContextSerializer = new SerializationInfoClassContextSerializer (info, "__configuration.TargetClass.ConfigurationContext.");
      targetClassContext.Serialize (classContextSerializer);
      
      info.AddValue ("__configuration.MixinIndex", mixinIndex);

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

      var classContextDeserializer = new SerializationInfoClassContextDeserializer (info, "__configuration.TargetClass.ConfigurationContext.");
      var configurationContext = ClassContext.Deserialize (classContextDeserializer);
      TargetClassDefinition targetClassDefinition = TargetClassDefinitionCache.Current.GetTargetClassDefinition (configurationContext);

      int mixinIndex = info.GetInt32 ("__configuration.MixinIndex");
      _mixinDefinition = targetClassDefinition.Mixins[mixinIndex];

      Type concreteType = ConcreteTypeBuilder.Current.GetConcreteMixinType (_mixinDefinition).GeneratedType;
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
