using System;
using Remotion.Mixins;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Samples
{
  public abstract class DisposableMixin : Mixin<object>, IDisposable
  {
    private bool _disposed = false;

    protected virtual void Dispose (bool disposing)
    {
      if (!_disposed)
      {
        if (disposing)
          CleanupManagedResources ();
        CleanupUnmanagedResources ();
        _disposed = true;
      }
    }

    protected abstract void CleanupManagedResources ();
    protected abstract void CleanupUnmanagedResources ();

    ~DisposableMixin ()
    {
      Dispose (false);
    }

    public void Dispose()
    {
      Dispose (true);
      GC.SuppressFinalize (this);
    }
  }
}
