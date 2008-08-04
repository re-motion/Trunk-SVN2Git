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
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.Serialization
{
  public class FlattenedDeserializationInfo
  {
    private readonly FlattenedSerializationReader<object> _objectReader;
    private readonly FlattenedSerializationReader<int> _intReader;
    private readonly FlattenedSerializationReader<bool> _boolReader;
    private readonly Dictionary<int, object> _handleMap = new Dictionary<int, object>();

    public FlattenedDeserializationInfo (object[] data)
    {
      ArgumentUtility.CheckNotNull ("data", data);
      object[] objects = ArgumentUtility.CheckType<object[]> ("data[0]", data[0]);
      int[] ints = ArgumentUtility.CheckType<int[]> ("data[1]", data[1]);
      bool[] bools = ArgumentUtility.CheckType<bool[]> ("data[2]", data[2]);

      _objectReader = new FlattenedSerializationReader<object> (objects);
      _intReader = new FlattenedSerializationReader<int> (ints);
      _boolReader = new FlattenedSerializationReader<bool> (bools);
    }

    public int GetIntValue ()
    {
      return GetValue (_intReader);
    }

    public bool GetBoolValue ()
    {
      return GetValue (_boolReader);
    }

    public T GetValue<T> ()
    {
      int originalPosition = _objectReader.ReadPosition;
      object o = GetValue (_objectReader);
      if (o is FlattenedSerializableMarker)
        return GetFlattenedSerializable<T> (originalPosition);
      else
        return CastValue<T> (o, originalPosition, "Object");
    }

    private T GetValue<T> (FlattenedSerializationReader<T> reader)
    {
      try
      {
        return reader.ReadValue ();
      }
      catch (InvalidOperationException ex)
      {
        throw new SerializationException (typeof (T).Name + " stream: " + ex.Message, ex);
      }
    }

    private T GetFlattenedSerializable<T> (int originalPosition)
    {
      Type type = GetValueForHandle<Type>();
      object instance = TypesafeActivator.CreateInstance (type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).With (this);
      return CastValue<T>(instance, originalPosition, "Object");
    }

    private T CastValue<T> (object uncastValue, int originalPosition, string streamName)
    {
      T value;
      try
      {
        value = (T) uncastValue;
      }
      catch (InvalidCastException ex)
      {
        string message = string.Format ("{0} stream: The serialization stream contains an object of type {1} at position {2}, but an object of "
            + "type {3} was expected.", streamName, uncastValue.GetType ().FullName, originalPosition, typeof (T).FullName);
        throw new SerializationException (message, ex);
      }
      catch (NullReferenceException ex)
      {
        string message = string.Format ("{0} stream: The serialization stream contains a null value at position {1}, but an object of type {2} was "
            + "expected.", streamName, originalPosition, typeof (T).FullName);
        throw new SerializationException (message, ex);
      }
      return value;
    }

    public T[] GetArray<T> ()
    {
      int length = GetIntValue ();
      T[] array = new T[length];
      for (int i = 0; i < length; ++i)
        array[i] = GetValue<T> ();
      return array;
    }

    public void FillCollection<T> (ICollection<T> targetCollection)
    {
      int length = GetIntValue ();
      for (int i = 0; i < length; ++i)
        targetCollection.Add (GetValue<T> ());
    }

    public T GetValueForHandle<T> ()
    {
      int handle = GetIntValue ();
      if (handle == -1)
        return (T) (object) null;
      else
      {
        object objectValue;
        if (!_handleMap.TryGetValue (handle, out objectValue))
        {
          T value = GetValue<T> ();
          _handleMap.Add (handle, value);
          return value;
        }
        else
          return CastValue<T> (objectValue, _objectReader.ReadPosition, "Object");
      }
    }
  }
}