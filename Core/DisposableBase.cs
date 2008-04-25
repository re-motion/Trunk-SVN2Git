using System;

namespace Remotion
{
  // TODO: Remove finalizer, check derivations whether they really need a finalizer (unmanaged resources only); document the change
  /// <summary>
  /// This class can be used as a base class for any class that needs to implement <see cref="IDisposable"/>, but does not want to 
  /// expose a public <c>Dispose</c> method.
  /// <see cref="DisposableBase"/>
  /// </summary>
  [Serializable]
  public abstract class DisposableExplicitBase : IDisposable
  {
    private bool _disposed = false;

    protected abstract void Dispose (bool disposing);

    void IDisposable.Dispose()
    {
      Dispose();
    }

    protected void Dispose()
    {
      if (! _disposed)
      {
        Dispose (true);
        GC.SuppressFinalize (this);
        _disposed = true;
      }
    }

    ~DisposableExplicitBase()
    {
      Dispose (false);
    }

    protected bool Disposed
    { 
      get { return _disposed; }
    }

    protected void AssertNotDisposed ()
    {
      if (_disposed)
        throw new InvalidOperationException ("Object disposed.");
    }

    protected void Resurrect ()
    {
      if (_disposed)
      {
        _disposed = false;
        GC.ReRegisterForFinalize (this);
      }
    }
  }

  /// <summary>
  /// This class can be used as a base class for any class that needs to implement <see cref="IDisposable"/>.
  /// <see cref="DisposableExplicitBase"/>
  /// </summary>
  [Serializable]
  public abstract class DisposableBase: DisposableExplicitBase
  {
    public new void Dispose()
    {
      base.Dispose();
    }
  }

}
