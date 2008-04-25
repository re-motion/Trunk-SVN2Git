using System;
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.Utilities;
using Remotion.Reflection.CodeGeneration;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  [Serializable]
  public class SerializationHelper : IObjectReference, ISerializable, IDeserializationCallback
  {
    // Always remember: the whole configuration must be serialized as one single, flat object (or SerializationInfo), we cannot rely on any
    // nested objects to be deserialized in the right order
    public static void GetObjectDataForGeneratedTypes (SerializationInfo info, StreamingContext context, object concreteObject,
        TargetClassDefinition configuration, object[] extensions, bool serializeBaseMembers)
    {
      info.SetType (typeof (SerializationHelper));

      info.AddValue ("__configuration.ConfigurationContext", configuration.ConfigurationContext);
      info.AddValue ("__extensions", extensions);

      object[] baseMemberValues;
      if (serializeBaseMembers)
      {
        MemberInfo[] baseMembers = FormatterServices.GetSerializableMembers (configuration.Type);
        baseMemberValues = FormatterServices.GetObjectData (concreteObject, baseMembers);
      }
      else
        baseMemberValues = null;

      info.AddValue ("__baseMemberValues", baseMemberValues);
    }

    private readonly IMixinTarget _deserializedObject;
    private readonly object[] _extensions;
    private readonly object[] _baseMemberValues;
    private readonly TargetClassDefinition _targetClassDefinition;
    private readonly StreamingContext _context;

    public SerializationHelper (SerializationInfo info, StreamingContext context)
        : this (delegate (Type t) { return t; }, info, context)
    {
    }

    public SerializationHelper (Func<Type, Type> typeTransformer, SerializationInfo info, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("typeTransformer", typeTransformer);
      ArgumentUtility.CheckNotNull ("info", info);

      _context = context;

      ClassContext configurationContext = (ClassContext) info.GetValue ("__configuration.ConfigurationContext", typeof (ClassContext));
      _targetClassDefinition = TargetClassDefinitionCache.Current.GetTargetClassDefinition (configurationContext);

      Type concreteType = ConcreteTypeBuilder.Current.GetConcreteType (_targetClassDefinition);
      concreteType = typeTransformer (concreteType);

      if (!_targetClassDefinition.Type.IsAssignableFrom (concreteType))
      {
        string message = string.Format ("TypeTransformer returned type {0}, which is not compatible with the serialized mixin configuration. The "
            + "configuration requires a type assignable to {1}.", concreteType.FullName, _targetClassDefinition.Type.FullName);
        throw new ArgumentException (message, "typeTransformer");
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
        _deserializedObject = (IMixinTarget) Activator.CreateInstance (concreteType, new object[] {info, context});
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
        MemberInfo[] baseMembers = FormatterServices.GetSerializableMembers (_targetClassDefinition.Type);
        FormatterServices.PopulateObjectMembers (_deserializedObject, baseMembers, _baseMemberValues);
      }

      ConcreteTypeBuilder.Current.Scope.InitializeDeserializedMixinTarget (_deserializedObject, _extensions);

      SerializationImplementer.RaiseOnDeserialized (_deserializedObject, _context);
      SerializationImplementer.RaiseOnDeserialization (_deserializedObject, sender);
    }
  }
}
