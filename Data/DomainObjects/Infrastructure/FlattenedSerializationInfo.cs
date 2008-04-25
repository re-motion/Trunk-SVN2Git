using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  public class FlattenedSerializationInfo
  {
    private readonly FlattenedSerializationWriter<object> _objectWriter = new FlattenedSerializationWriter<object> ();
    private readonly FlattenedSerializationWriter<int> _intWriter = new FlattenedSerializationWriter<int> ();
    private readonly FlattenedSerializationWriter<bool> _boolWriter = new FlattenedSerializationWriter<bool> ();
    private readonly Dictionary<object, int> _handleMap = new Dictionary<object, int>();

    public FlattenedSerializationInfo ()
    {
    }

    public object[] GetData ()
    {
      return new object[] { _objectWriter.GetData (), _intWriter.GetData (), _boolWriter.GetData() };
    }

    public void AddIntValue (int value)
    {
      _intWriter.AddSimpleValue (value);
    }

    public void AddBoolValue (bool value)
    {
      _boolWriter.AddSimpleValue (value);
    }

    public void AddValue<T> (T value)
    {
      IFlattenedSerializable serializable = value as IFlattenedSerializable;
      if (serializable != null)
        AddFlattenedSerializable (serializable);
      else
        _objectWriter.AddSimpleValue (value);
    }

    private void AddFlattenedSerializable (IFlattenedSerializable serializable)
    {
      _objectWriter.AddSimpleValue (FlattenedSerializableMarker.Instance);
      AddHandle (serializable.GetType ());
      serializable.SerializeIntoFlatStructure (this);
    }

    public void AddArray<T> (T[] valueArray)
    {
      ArgumentUtility.CheckNotNull ("valueArray", valueArray);
      AddCollection (valueArray);
    }

    public void AddCollection<T> (ICollection<T> valueCollection)
    {
      ArgumentUtility.CheckNotNull ("valueCollection", valueCollection);
      AddIntValue (valueCollection.Count);
      foreach (T t in valueCollection)
        AddValue (t);
    }

    public void AddHandle<T> (T value)
    {
      if (value == null)
        AddIntValue (-1);
      else
      {
        int handle;
        if (!_handleMap.TryGetValue (value, out handle))
        {
          handle = _handleMap.Count;
          _handleMap.Add (value, handle);
          AddIntValue (handle);
          AddValue (value);
        }
        else
          AddIntValue (handle);
      }
    }
  }
}