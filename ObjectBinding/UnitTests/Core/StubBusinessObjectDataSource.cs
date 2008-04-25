using System;

namespace Remotion.ObjectBinding.UnitTests.Core
{
  public class StubBusinessObjectDataSource : BusinessObjectDataSource
  {
    private readonly IBusinessObjectClass _businessObjectClass;
    private IBusinessObject _BusinessObject;

    public StubBusinessObjectDataSource (IBusinessObjectClass businessObjectClass)
    {
      _businessObjectClass = businessObjectClass;
    }

    public override IBusinessObjectClass BusinessObjectClass
    {
      get { return _businessObjectClass; }
    }

    public override IBusinessObject BusinessObject
    {
      get { return _BusinessObject; }
      set { _BusinessObject = value; }
    }
  }
}