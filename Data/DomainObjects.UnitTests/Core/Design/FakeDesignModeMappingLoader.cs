using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Design
{
  public class FakeDesignModeMappingLoader : IMappingLoader
  {
    private readonly IDesignerHost _designerHost;

    public FakeDesignModeMappingLoader (IDesignerHost site)
    {
      _designerHost = site;
    }

    public IDesignerHost DesignerHost
    {
      get { return _designerHost; }
    }

    public ClassDefinitionCollection GetClassDefinitions ()
    {
      ClassDefinitionCollection classDefinitionCollection = new ClassDefinitionCollection();
      classDefinitionCollection.Add (
          new ReflectionBasedClassDefinition ("Fake", "Fake", "Fake", typeof (Company), false, new List<Type>()));

      return classDefinitionCollection;
    }

    public RelationDefinitionCollection GetRelationDefinitions (ClassDefinitionCollection classDefinitions)
    {
      return new RelationDefinitionCollection();
    }

    bool IMappingLoader.ResolveTypes
    {
      get { return true; }
    }
  }
}