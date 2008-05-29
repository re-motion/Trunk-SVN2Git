using System;
using Remotion.Collections;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  public class StubBusinessObjectProvider : BusinessObjectProvider
  {
    private readonly InterlockedDataStore<Type, IBusinessObjectService> _serviceStore = new InterlockedDataStore<Type, IBusinessObjectService>();


    public StubBusinessObjectProvider ()
        : this (MockRepository.GenerateStub<IBusinessObjectServiceFactory>())
    {
    }

    public StubBusinessObjectProvider (IBusinessObjectServiceFactory serviceFactory)
        : base (serviceFactory)
    {
    }

    protected override IDataStore<Type, IBusinessObjectService> ServiceStore
    {
      get { return _serviceStore; }
    }
  }
}