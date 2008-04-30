using System;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithPropertiesHavingStorageClassAttribute : DomainObject
  {
    protected ClassWithPropertiesHavingStorageClassAttribute ()
    {
    }

    public abstract int NoAttribute { get; set; }

    [StorageClass (StorageClass.Persistent)]
    public abstract int Persistent { get; set; }

    //[StorageClassTransaction]
    //public abstract object Transaction { get; set; }

    [StorageClassNone]
    public object None 
    { get { return null; }
      set { }
    }
  }
}