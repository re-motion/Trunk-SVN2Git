using System;
using System.ComponentModel.Design;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.Design;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Design
{
  [TestFixture]
  public class DesignModeMappingLoaderAttributeTest
  {
    [Test]
    public void Initialize()
    {
      DesignModeMappingLoaderAttribute attribute = new DesignModeMappingLoaderAttribute (typeof (FakeDesignModeMappingLoader));

      Assert.That (attribute.Type, Is.SameAs (typeof (FakeDesignModeMappingLoader)));
    }

    [Test]
    public void CreateInstance ()
    {
      MockRepository mockRepository = new MockRepository ();
      IDesignerHost stubDesignerHost = mockRepository.CreateMock<IDesignerHost> ();
      DesignModeMappingLoaderAttribute attribute = new DesignModeMappingLoaderAttribute (typeof (FakeDesignModeMappingLoader));

      IMappingLoader mappingLoader = attribute.CreateInstance (stubDesignerHost);
      Assert.That (mappingLoader, Is.InstanceOfType (typeof (FakeDesignModeMappingLoader)));
      Assert.That (((FakeDesignModeMappingLoader) mappingLoader).DesignerHost, Is.SameAs (stubDesignerHost));
    }
  }
}