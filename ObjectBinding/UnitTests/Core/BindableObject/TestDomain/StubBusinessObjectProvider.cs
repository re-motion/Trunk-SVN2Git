using System;
using Remotion.Collections;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain
{
  public class StubBusinessObjectProvider:BusinessObjectProvider
  {
    public class StringFormatterService : IBusinessObjectStringFormatterService
    {
      public string GetPropertyString (IBusinessObject businessObject, IBusinessObjectProperty property, string format)
      {
        throw new NotImplementedException ();
      }
    }

    private readonly InterlockedDataStore<Type, IBusinessObjectService> _serviceStore = new InterlockedDataStore<Type, IBusinessObjectService> ();

    protected override IDataStore<Type, IBusinessObjectService> ServiceStore
    {
      get { return _serviceStore; }
    }

    protected override void InitializeDefaultServices ()
    {
      _serviceStore.Add (typeof (IBusinessObjectStringFormatterService), new StringFormatterService ());
    }
  }
}