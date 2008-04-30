using System;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Core.MixedDomains.SampleTypes
{
  public class MixinOverridingPropertiesAndMethods
      : Mixin<MixinOverridingPropertiesAndMethods.IBaseRequirements, MixinOverridingPropertiesAndMethods.IBaseRequirements>
  {

    [OverrideTarget]
    public virtual string Property
    {
      get { return Base.Property + "-MixinGetter"; }
      set { Base.Property = value + "-MixinSetter"; }
    }

    [OverrideTarget]
    public virtual string GetSomething ()
    {
      return Base.GetSomething () + "-MixinMethod";
    }

    public interface IBaseRequirements
    {
      string Property { get; set; }
      string GetSomething ();
    }
  }
}