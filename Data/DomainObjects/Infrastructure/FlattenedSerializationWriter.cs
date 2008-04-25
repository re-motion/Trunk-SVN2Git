using System;
using System.Collections.Generic;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  public class FlattenedSerializationWriter<T>
  {
    private readonly List<T> _data = new List<T> ();
    
    public T[] GetData ()
    {
      return _data.ToArray ();
    }

    public void AddSimpleValue (T value)
    {
      _data.Add (value);
    }
  }
}