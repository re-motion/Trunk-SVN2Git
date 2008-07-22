using System;
using Remotion.Data.DomainObjects;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.MixinTestDomain
{
  public class DiamondBase : DomainObjectMixin<DomainObject>
  {
    public int PBase
    {
      get { return Properties[typeof (MixinBase), "PBase"].GetValue<int> (); }
    }
  }
}
