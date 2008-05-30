using System;
using Remotion.Collections;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  public class OtherBusinessObjectImplementationProvider : IBusinessObjectProvider
  {
    public IBusinessObjectService GetService (Type serviceType)
    {
      throw new NotImplementedException();
    }

    public T GetService<T> () where T: IBusinessObjectService
    {
      throw new NotImplementedException();
    }

    public void AddService (Type serviceType, IBusinessObjectService service)
    {
      throw new NotImplementedException();
    }

    public void AddService<T> (T service) where T: IBusinessObjectService
    {
      throw new NotImplementedException();
    }

    public char GetPropertyPathSeparator ()
    {
      throw new NotImplementedException();
    }

    public IBusinessObjectPropertyPath CreatePropertyPath (IBusinessObjectProperty[] properties)
    {
      throw new NotImplementedException();
    }

    public string GetNotAccessiblePropertyStringPlaceHolder ()
    {
      throw new NotImplementedException();
    }
  }
}