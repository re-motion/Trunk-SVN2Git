using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [Instantiable]
  public abstract class DerivedClassWithoutStorageGroupWithMixedProperties : ClassWithoutStorageGroupWithMixedProperties
  {
    protected DerivedClassWithoutStorageGroupWithMixedProperties()
    {
    }

    public override int Int32
    {
      get { return 0; }
      set { }
    }

    public abstract string OtherString { get; set; }

    [DBColumn ("NewString")]
    public new abstract string String { get; set; }

    [DBColumn ("DerivedPrivateString")]
    private string PrivateString
    {
      get
      {
        return Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.DerivedClassWithoutStorageGroupWithMixedProperties.PrivateString"]
            .GetValue<string>();
      }
      set
      {
        Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.DerivedClassWithoutStorageGroupWithMixedProperties.PrivateString"]
            .SetValue (value);
      }
    }
  }
}