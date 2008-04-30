using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithMixedProperties: ClassWithMixedPropertiesNotInMapping
  {
    protected ClassWithMixedProperties ()
    {
    }

    [StorageClassNone]
    public object Unmanaged
    {
      get { return null; }
      set { }
    }

    public abstract int Int32 { get; set; }

    public virtual string String
    {
      get
      {
        return CurrentProperty.GetValue<string> (); // CurrentProperty used on purpose here - tests whether shadowed properties work correctly
      }
      set
      {
        CurrentProperty.SetValue (value); // CurrentProperty used on purpose here - tests whether shadowed properties work correctly
      }
    }

    public abstract ClassWithOneSideRelationProperties UnidirectionalOneToOne { get; set; }

    private string PrivateString
    {
      get
      {
        return Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties.PrivateString"]
            .GetValue<string> ();
      }
      set
      {
        Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties.PrivateString"]
            .SetValue (value);
      }
    }
  }
}
