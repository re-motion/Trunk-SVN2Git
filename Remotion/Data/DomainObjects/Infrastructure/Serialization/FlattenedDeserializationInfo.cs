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
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.Serialization
{
  public class FlattenedDeserializationInfo
  {
    public event EventHandler DeserializationFinished;

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
      if (typeof (IFlattenedSerializable).IsAssignableFrom (typeof (T)))
        return GetFlattenedSerializable<T> ();
      else
      {
        var originalPosition = _objectReader.ReadPosition;
        var o = GetValue (_objectReader);
        return CastValue<T> (o, originalPosition, "Object");
      }
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

    private T GetFlattenedSerializable<T> ()
    {
      Type type = GetValueForHandle<Type>();
      if (type == null)
        return default (T);
      else
      {
        object instance = TypesafeActivator.CreateInstance (type, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).With (this);
        var originalPosition = _objectReader.ReadPosition;
        return CastValue<T> (instance, originalPosition, "Object");
      }
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
          if (handle < _handleMap.Count)
            throw new NotSupportedException ("The serialized data contains a cycle, this is not supported.");

          T value = GetValue<T> ();
          _handleMap.Add (handle, value);
          return value;
        }
        else
          return CastValue<T> (objectValue, _objectReader.ReadPosition, "Object");
      }
    }

    public void SignalDeserializationFinished ()
    {
      if (!_intReader.EndReached)
        throw new InvalidOperationException ("Cannot signal DeserializationFinished when there is still integer data left to deserialize.");
      if (!_boolReader.EndReached)
        throw new InvalidOperationException ("Cannot signal DeserializationFinished when there is still boolean data left to deserialize.");
      if (!_objectReader.EndReached)
        throw new InvalidOperationException ("Cannot signal DeserializationFinished when there is still object data left to deserialize.");

      if (DeserializationFinished != null)
        DeserializationFinished (this, EventArgs.Empty);
    }
  }
}
