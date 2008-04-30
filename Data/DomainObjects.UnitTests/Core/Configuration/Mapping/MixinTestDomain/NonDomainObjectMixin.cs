using System;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.MixinTestDomain
{
  public class NonDomainObjectMixin : Mixin<DomainObject>
  {
    public int PNo
    {
      get { return 0; }
      set { Dev.Null = value; }
    }
  }
}