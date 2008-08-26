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
using System.Runtime.Serialization;
using Remotion.Development.UnitTesting;

namespace Remotion.Development.UnitTesting
{
  public class SerializationCallbackTester<T>
  {
    private readonly IMockRepository _mockRepository;
    private readonly Action<ISerializationEventReceiver> _receiverSetter;
    private readonly T _instance;

    public SerializationCallbackTester (IMockRepository mockRepository, T instance, Action<ISerializationEventReceiver> receiverSetter)
    {
      _mockRepository = mockRepository;
      _instance = instance;
      _receiverSetter = receiverSetter;

      _receiverSetter (null);
    }

    public void Test_SerializationCallbacks ()
    {
      _receiverSetter (null);
      try
      {
        ISerializationEventReceiver receiver = _mockRepository.StrictMock<ISerializationEventReceiver> ();

        _receiverSetter (receiver);
        CheckSerialization (receiver);
      }
      finally
      {
        _receiverSetter (null);
      }
    }

    public void Test_DeserializationCallbacks ()
    {
      _receiverSetter (null);
      try
      {
        byte[] bytes = Serializer.Serialize (_instance);
        ISerializationEventReceiver receiver = _mockRepository.StrictMock<ISerializationEventReceiver> ();
        _receiverSetter (receiver);

        CheckDeserialization (bytes, receiver);
      }
      finally
      {
        _receiverSetter (null);
      }
    }

    private void CheckSerialization (ISerializationEventReceiver receiver)
    {
      ExpectSerializationCallbacks (receiver);

      _mockRepository.ReplayAll();

      Serializer.Serialize (_instance);

      _mockRepository.VerifyAll();
    }

    private void CheckDeserialization (byte[] bytes, ISerializationEventReceiver receiver)
    {
      ExpectDeserializationCallbacks (receiver);

      _mockRepository.ReplayAll();

      Serializer.Deserialize (bytes);

      _mockRepository.VerifyAll();
    }

    private void ExpectSerializationCallbacks (ISerializationEventReceiver receiver)
    {
      using (_mockRepository.Ordered())
      {
        StreamingContext context = new StreamingContext();
        receiver.OnSerializing (context);
        _mockRepository.LastCall_IgnoreArguments();

        receiver.OnSerialized (context);
        _mockRepository.LastCall_IgnoreArguments ();
      }
    }

    private void ExpectDeserializationCallbacks (ISerializationEventReceiver receiver)
    {
      using (_mockRepository.Ordered())
      {
        StreamingContext context = new StreamingContext();
        receiver.OnDeserializing (context);
        _mockRepository.LastCall_IgnoreArguments ();

        receiver.OnDeserialized (context);
        _mockRepository.LastCall_IgnoreArguments ();

        receiver.OnDeserialization (null);
        _mockRepository.LastCall_IgnoreArguments ();
      }
    }
  }
}
