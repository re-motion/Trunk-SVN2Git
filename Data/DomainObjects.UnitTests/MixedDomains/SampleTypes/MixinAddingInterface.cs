using System;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.MixedDomains.SampleTypes
{
  [Serializable]
  public class MixinAddingInterface : Mixin<DomainObject>, IInterfaceAddedByMixin
  {
    public string GetGreetings ()
    {
      return "Hello, my ID is " + This.ID;
    }
  }
}