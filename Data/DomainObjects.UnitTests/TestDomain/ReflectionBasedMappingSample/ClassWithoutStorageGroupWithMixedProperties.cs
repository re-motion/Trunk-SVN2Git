using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  //No StorageGroup
  [Instantiable]
  public abstract class ClassWithoutStorageGroupWithMixedProperties : DomainObject
  {
    protected ClassWithoutStorageGroupWithMixedProperties ()
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
        return Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithoutStorageGroupWithMixedProperties.String"]
            .GetValue<string> ();
      }
      set { Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithoutStorageGroupWithMixedProperties.String"]
            .SetValue (value); }
    }

    public abstract ClassWithOneSideRelationProperties UnidirectionalOneToOne { get; set; }

    private string PrivateString
    {
      get
      {
        return Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithoutStorageGroupWithMixedProperties.PrivateString"]
            .GetValue<string> ();
      }
      set
      {
        Properties["Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties.PrivateString"].SetValue (value);
      }
    }
  }
}
