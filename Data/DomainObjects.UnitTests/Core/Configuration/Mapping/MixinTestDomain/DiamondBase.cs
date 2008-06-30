using System;
using System.Collections.Generic;
using System.Text;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.MixinTestDomain
{
  public class DiamondBase : DomainObjectMixin<DomainObject>
  {
    public int PBase
    {
      get { return Properties[typeof (MixinBase), "PBase"].GetValue<int> (); }
    }
  }
}
