using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Remotion;
using Remotion.Utilities;

namespace Remotion.Mixins.Utilities.Singleton
{
  public class ThreadSafeSingleton<T>
      where T : class
  {
    private DoubleCheckedLockingContainer<T> _instanceHolder;

    public ThreadSafeSingleton (Func<T> creator)
    {
      ArgumentUtility.CheckNotNull ("creator", creator);

      _instanceHolder = new DoubleCheckedLockingContainer<T> (creator);
    }

    public bool HasCurrent
    {
      get { return _instanceHolder.HasValue; }
    }

    public T Current
    {
      get { return _instanceHolder.Value; }
    }

    public void SetCurrent (T value)
    {
      _instanceHolder.Value = value;
    }
  }
}
