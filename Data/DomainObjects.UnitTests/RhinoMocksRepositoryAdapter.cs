using System;
using Rhino.Mocks;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests
{
  public class RhinoMocksRepositoryAdapter : IMockRepository
  {
    private readonly MockRepository _mockRepository;

    public RhinoMocksRepositoryAdapter ()
        : this (new MockRepository())
    {
    }

    public RhinoMocksRepositoryAdapter (MockRepository mockRepository)
    {
      ArgumentUtility.CheckNotNull ("mockRepository", mockRepository);
      _mockRepository = mockRepository;
    }

    public T CreateMock<T> (params object[] argumentsForConstructor)
    {
      return _mockRepository.CreateMock<T> (argumentsForConstructor);
    }

    public void ReplayAll ()
    {
      _mockRepository.ReplayAll ();
    }

    public void VerifyAll ()
    {
      _mockRepository.VerifyAll ();
    }

    public IDisposable Ordered ()
    {
      return _mockRepository.Ordered ();
    }

    public void LastCall_IgnoreArguments ()
    {
      LastCall.IgnoreArguments ();
    }
  }
}