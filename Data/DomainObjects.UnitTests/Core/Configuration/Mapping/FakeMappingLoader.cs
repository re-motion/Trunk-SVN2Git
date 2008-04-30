using System;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Design;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Core.Design;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping
{
  [DesignModeMappingLoader(typeof (FakeDesignModeMappingLoader))]
  public class FakeMappingLoader: IMappingLoader
  {
    public FakeMappingLoader()
    {
    }

    public ClassDefinitionCollection GetClassDefinitions()
    {
      return new ClassDefinitionCollection();
    }

    public RelationDefinitionCollection GetRelationDefinitions (ClassDefinitionCollection classDefinitions)
    {
      return new RelationDefinitionCollection();
    }

    public bool ResolveTypes
    {
      get { return false; }
    }
  }
}