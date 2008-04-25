using System;
using Remotion.Collections;
using Remotion.ObjectBinding;

namespace Remotion.ObjectBinding.UnitTests.Core.BusinessObjectPropertyPathTests
{
  public class StubBusinessObjectProvider : BusinessObjectProvider
  {
    protected override IDataStore<Type, IBusinessObjectService> ServiceStore
    {
      get { throw new NotImplementedException(); }
    }
  }
}