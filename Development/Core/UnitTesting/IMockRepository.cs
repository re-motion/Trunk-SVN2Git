using System;

namespace Remotion.Development.UnitTesting
{
  public interface IMockRepository
  {
    T CreateMock<T> (params object[] argumentsForConstructor);
    void ReplayAll ();
    void VerifyAll ();
    IDisposable Ordered ();

    void LastCall_IgnoreArguments ();
  }
}