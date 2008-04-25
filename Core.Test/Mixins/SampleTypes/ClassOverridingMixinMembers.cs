using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Uses(typeof(MixinWithAbstractMembers))]
  [Serializable]
  public class ClassOverridingMixinMembers
  {
    [OverrideMixin]
    public virtual string AbstractMethod(int i)
    {
      return "ClassOverridingMixinMembers.AbstractMethod-" + i;
    }

    [OverrideMixin]
    public virtual string AbstractProperty
    {
      get { return "ClassOverridingMixinMembers.AbstractProperty"; }
    }

    [OverrideMixin]
    public virtual string RaiseEvent ()
    {
      return _abstractEvent ();
    }

    private Func<string> _abstractEvent;

    [OverrideMixin]
    public virtual event Func<string> AbstractEvent
    {
      add { _abstractEvent += value; }
      remove { _abstractEvent -= value; }
    }
  }
}
