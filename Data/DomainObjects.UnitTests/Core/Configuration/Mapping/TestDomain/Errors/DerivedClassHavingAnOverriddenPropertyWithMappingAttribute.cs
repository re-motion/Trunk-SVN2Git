using System;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.TestDomain.Errors
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