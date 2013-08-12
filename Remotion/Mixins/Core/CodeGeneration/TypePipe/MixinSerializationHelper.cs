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
using Remotion.Mixins.CodeGeneration.Serialization;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  [Serializable]
  public class MixinSerializationHelper : IObjectReference, ISerializable, IDeserializationCallback
  {
    // Always remember: the whole configuration must be serialized as one single, flat object (or SerializationInfo), we cannot rely on any
    // nested objects to be deserialized in the right order
    public static void GetObjectDataForGeneratedTypes (
        SerializationInfo info, 
        StreamingContext context, 
        object mixin, 
        ConcreteMixinTypeIdentifier identifier,
        bool serializeBaseMembers,
        string pipelineIdentifier)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      ArgumentUtility.CheckNotNull ("mixin", mixin);
      ArgumentUtility.CheckNotNull ("identifier", identifier);

      info.SetType (typeof (MixinSerializationHelper));

      var identifierSerializer = new SerializationInfoConcreteMixinTypeIdentifierSerializer (info, "__identifier");
      identifier.Serialize (identifierSerializer);
      
      object[] baseMemberValues;
      if (serializeBaseMembers)
      {
        var baseType = mixin.GetType ().BaseType;
        Assertion.IsNotNull (baseType, "Generated mixin types always have a base type.");
        MemberInfo[] baseMembers = FormatterServices.GetSerializableMembers (baseType);
        baseMemberValues = FormatterServices.GetObjectData (mixin, baseMembers);
      }
      else
        baseMemberValues = null;

      info.AddValue ("__baseMemberValues", baseMemberValues);
      info.AddValue ("__participantConfigurationID", pipelineIdentifier);
    }

    private readonly object[] _baseMemberValues;
    private readonly object _deserializedObject;
    private readonly StreamingContext _context;

    public MixinSerializationHelper (SerializationInfo info, StreamingContext context)
      : this (info, context, t => t)
    {
    }

    public MixinSerializationHelper (SerializationInfo info, StreamingContext context, Func<Type, Type> typeTransformer)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      ArgumentUtility.CheckNotNull ("typeTransformer", typeTransformer);

      _context = context;

      var identifierDeserializer = new SerializationInfoConcreteMixinTypeIdentifierDeserializer (info, "__identifier");
      var identifier = ConcreteMixinTypeIdentifier.Deserialize (identifierDeserializer);

      var pipelineIdentifier = info.GetString ("__participantConfigurationID");
      var pipeline = SafeServiceLocator.Current.GetInstance<IPipelineRegistry>().Get (pipelineIdentifier);

      Type untransformedConcreteType = pipeline.ReflectionService.GetAdditionalType (identifier);
      // TODO 5370: Is there a bug here? The typeTransformer parameter is always t => t because the ctor is always called by the Reflection engine?
      // This would mean that deserialization of a mixed mixin would not work correctly. Create an integration test. Then, remove typeTransformer and 
      // directly call pipeline.ReflectionService.GetAssembledType (untransformedConcreteType)?
      var concreteType = typeTransformer(untransformedConcreteType);

      if (!identifier.MixinType.IsAssignableFrom (concreteType))
      {
        string message = string.Format ("TypeTransformer returned type '{0}', which is not compatible with the serialized mixin configuration. The "
            + "configuration requires a type assignable to '{1}'.", concreteType, identifier.MixinType);
        throw new InvalidOperationException (message);
      }

      _baseMemberValues = (object[]) info.GetValue ("__baseMemberValues", typeof (object[]));

      // Usually, instantiate a deserialized object using GetSafeUninitializedObject.
      // However, _baseMemberValues being null means that the object itself manages its member deserialization via ISerializable. In such a case, we
      // need to use the deserialization constructor to instantiate the object.
      if (_baseMemberValues != null)
        _deserializedObject = FormatterServices.GetSafeUninitializedObject (concreteType);
      else
      {
        Assertion.IsTrue (typeof (ISerializable).IsAssignableFrom (concreteType));
        _deserializedObject = Activator.CreateInstance (
            concreteType, 
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
            null,
            new object[] { info, context },
            null);
      }
      Reflection.CodeGeneration.SerializationImplementer.RaiseOnDeserializing (_deserializedObject, _context);
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
        var baseType = _deserializedObject.GetType ().BaseType;
        Assertion.IsNotNull (baseType, "Mixed types always have a base type.");
        MemberInfo[] baseMembers = FormatterServices.GetSerializableMembers (baseType);
        FormatterServices.PopulateObjectMembers (_deserializedObject, baseMembers, _baseMemberValues);
      }

      Reflection.CodeGeneration.SerializationImplementer.RaiseOnDeserialized (_deserializedObject, _context);
      Reflection.CodeGeneration.SerializationImplementer.RaiseOnDeserialization (_deserializedObject, sender);

      // Note: This and Next properties are initialized from the target class via InitializeDeserializedMixinTarget
    }
  }
}
