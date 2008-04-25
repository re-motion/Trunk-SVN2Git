using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  public class MixinWithProtectedOverrider : Mixin<MixinWithProtectedOverrider.IRequirements, MixinWithProtectedOverrider.IRequirements>
  {
    public interface IRequirements
    {
      string VirtualMethod ();
      string VirtualProperty { get; }
      event EventHandler VirtualEvent;
    }

    [OverrideTarget]
    protected string VirtualMethod ()
    {
      return "MixinWithProtectedOverrider.VirtualMethod-" + Base.VirtualMethod ();
    }

    [OverrideTarget]
    protected string VirtualProperty
    {
      get { return "MixinWithProtectedOverrider.VirtualProperty-" + Base.VirtualProperty; }
    }

    [OverrideTarget]
    protected event EventHandler VirtualEvent
    {
      add
      {
        Base.VirtualEvent += value;
        Base.VirtualEvent += ThisHandler;
      }
      remove
      {
        Base.VirtualEvent -= value;
        Base.VirtualEvent -= ThisHandler;
      }
    }

    private void ThisHandler (object sender, EventArgs e)
    {
      throw new NotImplementedException();
    }
  }
}
