using System;
using System.Xml.Schema;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Schemas;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Schemas
{
  [TestFixture]
  public class SchemaLoaderTest
  {
    [Test]
    public void InitializeWithQueries ()
    {
      Assert.AreEqual (PrefixNamespace.QueryConfigurationNamespace.Uri, SchemaLoader.Queries.SchemaUri);
    }

    [Test]
    public void LoadSchemaSetWithQueries ()
    {
      XmlSchemaSet schemaSet = SchemaLoader.Queries.LoadSchemaSet ();

      Assert.IsNotNull (schemaSet);
      Assert.AreEqual (2, schemaSet.Count);
      Assert.IsTrue (schemaSet.Contains (PrefixNamespace.QueryConfigurationNamespace.Uri));
      Assert.IsTrue (schemaSet.Contains ("http://www.re-motion.org/Data/DomainObjects/Types"));
    }
  }
}
