using System;

namespace Remotion.Data.DomainObjects.UnitTests.Core.MixedDomains.SampleTypes
{
  public class DerivedMixinAddingPersistentProperties : MixinAddingPersistentProperties
  {
    public int AdditionalPersistentProperty
    {
      get { return Properties[typeof (DerivedMixinAddingPersistentProperties), "AdditionalPersistentProperty"].GetValue<int> (); }
      set { Properties[typeof (DerivedMixinAddingPersistentProperties), "AdditionalPersistentProperty"].SetValue (value); }
    }
  }
}