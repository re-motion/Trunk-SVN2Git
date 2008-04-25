using System;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  public abstract class DerivedClassHavingAnOverriddenPropertyWithMappingAttribute: BaseClass
  {
    protected DerivedClassHavingAnOverriddenPropertyWithMappingAttribute ()
    {
    }

    [StorageClassNoneAttribute]
    public override int Int32
    {
      get { return 0; }
      set { }
    }
  }
}