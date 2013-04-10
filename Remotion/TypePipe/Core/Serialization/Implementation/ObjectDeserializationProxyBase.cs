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
using System.Runtime.Serialization;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.TypePipe.Serialization.Implementation
{
  /// <summary>
  /// A common base class for objects used as placeholders in the .NET deserialization process.
  /// </summary>
  /// <remarks>
  /// This class uses the metadata in the <see cref="SerializationInfo"/> that was added by the <see cref="SerializationParticipant"/> to 
  /// regenerate a suitable type for deserialization.
  /// </remarks>
  public abstract class ObjectDeserializationProxyBase : ISerializable, IObjectReference, IDeserializationCallback
  {
    private readonly IPipelineRegistry _registry = SafeServiceLocator.Current.GetInstance<IPipelineRegistry>();

    private readonly SerializationInfo _serializationInfo;

    private object _instance;

    // ReSharper disable UnusedParameter.Local
    protected ObjectDeserializationProxyBase (SerializationInfo serializationInfo, StreamingContext streamingContext)
    // ReSharper restore UnusedParameter.Local
    {
      ArgumentUtility.CheckNotNull ("serializationInfo", serializationInfo);

      _serializationInfo = serializationInfo;
    }

    public SerializationInfo SerializationInfo
    {
      get { return _serializationInfo; }
    }

    public void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      throw new NotSupportedException ("This method should not be called.");
    }

    public object GetRealObject (StreamingContext context)
    {
      if (_instance != null)
        return _instance;

      var requestedTypeName = (string) SerializationInfo.GetValue (SerializationParticipant.RequestedTypeKey, typeof (string));
      var participantConfigurationID = (string) SerializationInfo.GetValue (SerializationParticipant.ParticipantConfigurationID, typeof (string));

      var underlyingType = Type.GetType (requestedTypeName, throwOnError: true);
      var factory = _registry.Get (participantConfigurationID);
      var instance = CreateRealObject (factory, underlyingType, context);

      _instance = instance;

      return instance;
    }

    public void OnDeserialization (object sender)
    {
      var deserializationCallback = _instance as IDeserializationCallback;
      if (deserializationCallback != null)
        deserializationCallback.OnDeserialization (sender);
    }

    protected abstract object CreateRealObject (IPipeline pipeline, Type requestedType, StreamingContext context);
  }
}