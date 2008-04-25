using System;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public interface IExplicit
  {
    string Explicit();
  }
 
  public class MixinWithExplicitImplementation : IExplicit
  {
    string IExplicit.Explicit ()
    {
      return "XXX";
    }
  }
}
