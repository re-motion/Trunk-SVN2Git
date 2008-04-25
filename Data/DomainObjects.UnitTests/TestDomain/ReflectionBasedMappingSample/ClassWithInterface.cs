using System;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample
{
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class ClassWithInterface : DomainObject, IInterfaceWithProperties
  {
    protected ClassWithInterface ()
    {
    }

    public string Property
    {
      get { throw new Exception ("The method or operation is not implemented."); }
      set { throw new Exception ("The method or operation is not implemented."); }
    }

    public string ImplicitProperty
    {
      get { throw new Exception ("The method or operation is not implemented."); }
      set { throw new Exception ("The method or operation is not implemented."); }
    }

    string IInterfaceWithProperties.ExplicitProperty
    {
      get { throw new Exception ("The method or operation is not implemented."); }
      set { throw new Exception ("The method or operation is not implemented."); }
    }

    [StorageClass (StorageClass.Persistent)]
    string IInterfaceWithProperties.ExplicitManagedProperty
    {
      get { throw new Exception ("The method or operation is not implemented."); }
      set { throw new Exception ("The method or operation is not implemented."); }
    }
  }
}