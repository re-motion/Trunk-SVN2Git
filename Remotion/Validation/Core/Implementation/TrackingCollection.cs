// Decompiled with JetBrains decompiler
// Type: FluentValidation.Internal.TrackingCollection`1
// Assembly: FluentValidation, Version=5.0.0.1, Culture=neutral, PublicKeyToken=a82054b837897c66
// MVID: 30628A95-CE3F-41E4-BA2A-29882CBD79CE
// Assembly location: C:\Development\re-motion_svn2git\Remotion\ObjectBinding\Web.Development.WebTesting.TestSite\Bin\FluentValidation.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Remotion.Validation.Implementation
{
  public class TrackingCollection<T> : IEnumerable<T>
  {
    private readonly List<T> _innerCollection = new List<T>();

    public event Action<T> ItemAdded;

    public void Add (T item)
    {
      _innerCollection.Add (item);
      ItemAdded?.Invoke (item);
    }

    public IDisposable OnItemAdded (Action<T> onItemAdded)
    {
      ItemAdded += onItemAdded;
      return new EventDisposable (this, onItemAdded);
    }

    public IEnumerator<T> GetEnumerator ()
    {
      return _innerCollection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    private class EventDisposable : IDisposable
    {
      private readonly TrackingCollection<T> _parent;
      private readonly Action<T> _handler;

      public EventDisposable (TrackingCollection<T> parent, Action<T> handler)
      {
        _parent = parent;
        _handler = handler;
      }

      public void Dispose ()
      {
        _parent.ItemAdded -= _handler;
      }
    }
  }
}