using System;
using Remotion.Mixins;

namespace Remotion.UnitTests.Mixins.SampleTypes
{
  [Extends (typeof (BaseType1))]
  [Serializable]
  [AcceptsAlphabeticOrdering]
  public class BT1Mixin2
  {
    [OverrideTarget]
    public string VirtualMethod ()
    {
      return "Mixin2ForBT1.VirtualMethod";
    }

    [OverrideTarget]
    public string VirtualProperty
    {
      get { return "Mixin2ForBT1.VirtualProperty"; }
      // no setter
    }

    public EventHandler BackingEventField;

    [OverrideTarget]
    public virtual event EventHandler VirtualEvent
    {
      add { BackingEventField += value; }
      remove { BackingEventField -= value; }
    }
  }
}
