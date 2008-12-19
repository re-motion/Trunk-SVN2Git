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
using System.Runtime.Serialization;
using System.Reflection;
using Remotion.Reflection;
using Remotion.Reflection.CodeGeneration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.Interception
{
  [Serializable]
  public class SerializationHelper : IObjectReference, ISerializable, IDeserializationCallback
  {
    private readonly DomainObject _realObject;
    private readonly IObjectReference _nestedObjectReference;
    private readonly object[] _baseMemberValues;
    private readonly Type _publicDomainObjectType;
    private ObjectID _idForCheck;
    private readonly StreamingContext _context;

    // Always remember: the whole configuration must be serialized as one single, flat object (or SerializationInfo), we cannot rely on any
    // nested objects to be deserialized in the right order
    public static void GetObjectDataForGeneratedTypes (SerializationInfo info, StreamingContext context, DomainObject concreteObject,
        bool serializeBaseMembers)
    {
      ArgumentUtility.CheckNotNull ("info", info);
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("concreteObject", concreteObject);

      info.SetType (typeof (SerializationHelper));

      Type publicDomainObjectType = concreteObject.GetPublicDomainObjectType();
      info.AddValue ("PublicDomainObjectType.AssemblyQualifiedName", publicDomainObjectType.AssemblyQualifiedName);
      
      object[] baseMemberValues = null;
      if (serializeBaseMembers)
      {
        MemberInfo[] baseMembers = FormatterServices.GetSerializableMembers (publicDomainObjectType);
        baseMemberValues = FormatterServices.GetObjectData (concreteObject, baseMembers);
      }
      else
      {
        // The ObjectID is actually stored and retrieved by ObjectID's BaseGetObjectData and deserialization constructor. However, to check
        // whether these methods are correctly invoked, we need to store another copy for ourselves.
        info.AddValue ("DomainObject._idForCheck", concreteObject.ID);
      }

      info.AddValue ("baseMemberValues", baseMemberValues);

      DomainObjectMixinCodeGenerationBridge.SerializeMetadata (concreteObject, info, context);
    }

    public SerializationHelper (SerializationInfo info, StreamingContext context)
    {
      ArgumentUtility.CheckNotNull ("info", info);

      _context = context;

      string publicDomainObjectTypeName = info.GetString ("PublicDomainObjectType.AssemblyQualifiedName");
      _baseMemberValues = (object[]) info.GetValue ("baseMemberValues", typeof (object[]));
      _publicDomainObjectType = ContextAwareTypeDiscoveryUtility.GetType (publicDomainObjectTypeName, true);

      IDomainObjectFactory factory = DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory;

      // Usually, instantiate a deserialized object using GetSafeUninitializedObject.
      // However, _baseMemberValues being null means that the object itself manages its member deserialization via ISerializable. In such a case, we
      // need to use the deserialization constructor to instantiate the object. Of course, it might also be a mixed type, so we better delegate
      // to the DomainObjectMixinCodeGenerationBridge.
      if (_baseMemberValues != null)
      {
        Type concreteDomainObjectType = factory.GetConcreteDomainObjectType (_publicDomainObjectType);
        _realObject = (DomainObject) FormatterServices.GetSafeUninitializedObject (concreteDomainObjectType);
        factory.PrepareUnconstructedInstance (_realObject);
      }
      else
      {
        _nestedObjectReference = DomainObjectMixinCodeGenerationBridge.BeginDeserialization (_publicDomainObjectType, info, context);
        _realObject = (DomainObject) _nestedObjectReference.GetRealObject (context);
        _idForCheck = (ObjectID) info.GetValue ("DomainObject._idForCheck", typeof (ObjectID));
      }

      DomainObjectMixinCodeGenerationBridge.RaiseOnDeserializing (_realObject, _context);
    }

    public object GetRealObject (StreamingContext context)
    {
      return _realObject;
    }

    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      throw new NotImplementedException ("This method should never be called.");
    }

    public void OnDeserialization (object sender)
    {
      if (_baseMemberValues != null)
      {
        MemberInfo[] baseMembers = FormatterServices.GetSerializableMembers (_publicDomainObjectType);
        FormatterServices.PopulateObjectMembers (_realObject, baseMembers, _baseMemberValues);
      }
      else
      {
        Assertion.IsNotNull (_nestedObjectReference);
        DomainObjectMixinCodeGenerationBridge.FinishDeserialization (_nestedObjectReference);

        if (!object.Equals (_realObject.ID, _idForCheck))
        {
          string message = string.Format (
              "The deserialization constructor on type {0} did not call DomainObject's base deserialization constructor.", _publicDomainObjectType.FullName);
          throw new SerializationException (message);
        }
      }

      DomainObjectMixinCodeGenerationBridge.RaiseOnDeserialized (_realObject, _context);
      DomainObjectMixinCodeGenerationBridge.RaiseOnDeserialization (_realObject, sender);
    }
  }
}
