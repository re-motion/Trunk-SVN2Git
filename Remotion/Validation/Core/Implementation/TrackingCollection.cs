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
  public class TrackingCollection<T> : IEnumerable<T>, IEnumerable
  {
    private readonly List<T> innerCollection = new List<T>();

    public event Action<T> ItemAdded;

    public void Add (T item)
    {
      this.innerCollection.Add (item);
      if (this.ItemAdded == null)
        return;
      this.ItemAdded (item);
    }

    public IDisposable OnItemAdded (Action<T> onItemAdded)
    {
      this.ItemAdded += onItemAdded;
      return (IDisposable) new TrackingCollection<T>.EventDisposable (this, onItemAdded);
    }

    public IEnumerator<T> GetEnumerator ()
    {
      return (IEnumerator<T>) this.innerCollection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return (IEnumerator) this.GetEnumerator();
    }

    private class EventDisposable : IDisposable
    {
      private readonly TrackingCollection<T> parent;
      private readonly Action<T> handler;

      public EventDisposable (TrackingCollection<T> parent, Action<T> handler)
      {
        this.parent = parent;
        this.handler = handler;
      }

      public void Dispose ()
      {
        this.parent.ItemAdded -= this.handler;
      }
    }
  }
}